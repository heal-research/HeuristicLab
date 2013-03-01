#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NcaGradientCalculator", "Calculates the quality and gradient of a certain NCA matrix.")]
  [StorableClass]
  public class NcaGradientCalculator : SingleSuccessorOperator {

    #region Parameter Properties
    public ILookupParameter<IntValue> DimensionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Dimensions"]; }
    }

    public ILookupParameter<IntValue> NeighborSamplesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NeighborSamples"]; }
    }

    public ILookupParameter<DoubleValue> RegularizationParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Regularization"]; }
    }

    public ILookupParameter<RealVector> NcaMatrixParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NcaMatrix"]; }
    }

    public ILookupParameter<RealVector> NcaMatrixGradientsParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NcaMatrixGradients"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters["ProblemData"]; }
    }

    public ILookupParameter<Scaling> ScalingParameter {
      get { return (ILookupParameter<Scaling>)Parameters["Scaling"]; }
    }
    #endregion

    [StorableConstructor]
    protected NcaGradientCalculator(bool deserializing) : base(deserializing) { }
    protected NcaGradientCalculator(NcaGradientCalculator original, Cloner cloner) : base(original, cloner) { }
    public NcaGradientCalculator()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("Dimensions", "The dimensions to which the feature space should be reduced to."));
      Parameters.Add(new LookupParameter<IntValue>("NeighborSamples", "The number of neighbors that should be taken into account at maximum."));
      Parameters.Add(new LookupParameter<DoubleValue>("Regularization", "The regularization term that constrains the expansion of the projected space."));
      Parameters.Add(new LookupParameter<RealVector>("NcaMatrix", "The optimized matrix."));
      Parameters.Add(new LookupParameter<RealVector>("NcaMatrixGradients", "The gradients from the matrix that is being optimized."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the current matrix."));
      Parameters.Add(new LookupParameter<IClassificationProblemData>("ProblemData", "The classification problem data."));
      Parameters.Add(new LookupParameter<Scaling>("Scaling", "The scaling of the dataset."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaGradientCalculator(this, cloner);
    }

    public override IOperation Apply() {
      var problemData = ProblemDataParameter.ActualValue;
      var scaling = ScalingParameter.ActualValue;
      var dimensions = DimensionsParameter.ActualValue.Value;
      var neighborSamples = NeighborSamplesParameter.ActualValue.Value;
      var regularization = RegularizationParameter.ActualValue.Value;

      var vector = NcaMatrixParameter.ActualValue;
      var gradients = NcaMatrixGradientsParameter.ActualValue;
      if (gradients == null) {
        gradients = new RealVector(vector.Length);
        NcaMatrixGradientsParameter.ActualValue = gradients;
      }

      var data = AlglibUtil.PrepareAndScaleInputMatrix(problemData.Dataset, problemData.AllowedInputVariables,
                                                       problemData.TrainingIndices, scaling);
      var classes = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).ToArray();

      var quality = Gradient(vector, gradients, data, classes, dimensions, neighborSamples, regularization);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.Apply();
    }

    private static double Gradient(RealVector A, RealVector grad, double[,] data, double[] classes, int dimensions, int neighborSamples, double regularization) {
      var instances = data.GetLength(0);
      var attributes = data.GetLength(1);

      var AMatrix = new Matrix(A, A.Length / dimensions, dimensions);

      alglib.sparsematrix probabilities;
      alglib.sparsecreate(instances, instances, out probabilities);
      var transformedDistances = new Dictionary<int, double>(instances);
      for (int i = 0; i < instances; i++) {
        var iVector = new Matrix(GetRow(data, i), data.GetLength(1));
        for (int k = 0; k < instances; k++) {
          if (k == i) {
            transformedDistances.Remove(k);
            continue;
          }
          var kVector = new Matrix(GetRow(data, k));
          transformedDistances[k] = Math.Exp(-iVector.Multiply(AMatrix).Subtract(kVector.Multiply(AMatrix)).SumOfSquares());
        }
        var normalization = transformedDistances.Sum(x => x.Value);
        if (normalization <= 0) continue;
        foreach (var s in transformedDistances.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Take(neighborSamples)) {
          alglib.sparseset(probabilities, i, s.Key, s.Value / normalization);
        }
      }
      alglib.sparseconverttocrs(probabilities); // needed to enumerate in order (top-down and left-right)

      int t0 = 0, t1 = 0, r, c;
      double val;
      var pi = new double[instances];
      while (alglib.sparseenumerate(probabilities, ref t0, ref t1, out r, out c, out val)) {
        if (classes[r].IsAlmost(classes[c])) {
          pi[r] += val;
        }
      }

      var innerSum = new double[attributes, attributes];
      while (alglib.sparseenumerate(probabilities, ref t0, ref t1, out r, out c, out val)) {
        var vector = new Matrix(GetRow(data, r)).Subtract(new Matrix(GetRow(data, c)));
        vector.OuterProduct(vector).Multiply(val * pi[r]).AddTo(innerSum);

        if (classes[r].IsAlmost(classes[c])) {
          vector.OuterProduct(vector).Multiply(-val).AddTo(innerSum);
        }
      }

      var func = -pi.Sum() + regularization * AMatrix.SumOfSquares();

      r = 0;
      var newGrad = AMatrix.Multiply(-2.0).Transpose().Multiply(new Matrix(innerSum)).Transpose();
      foreach (var g in newGrad) {
        grad[r] = g + regularization * 2 * A[r];
        r++;
      }

      return func;
    }

    private static IEnumerable<double> GetRow(double[,] data, int row) {
      for (int i = 0; i < data.GetLength(1); i++)
        yield return data[row, i];
    }
  }
}