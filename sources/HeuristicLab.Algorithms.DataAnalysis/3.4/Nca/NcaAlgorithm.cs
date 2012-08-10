#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  internal delegate void Reporter(double quality, double[] coefficients);
  /// <summary>
  /// Neighborhood Components Analysis
  /// </summary>
  [Item("Neighborhood Components Analysis (NCA)", "Implementation of Neighborhood Components Analysis based on the description of J. Goldberger, S. Roweis, G. Hinton, R. Salakhutdinov. 2005. Neighbourhood Component Analysis. Advances in Neural Information Processing Systems, 17. pp. 513-520.")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class NcaAlgorithm : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    #region Parameter Properties
    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["K"]; }
    }
    public IFixedValueParameter<IntValue> DimensionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Dimensions"]; }
    }
    public IConstrainedValueParameter<INCAInitializer> InitializationParameter {
      get { return (IConstrainedValueParameter<INCAInitializer>)Parameters["Initialization"]; }
    }
    public IFixedValueParameter<IntValue> NeighborSamplesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NeighborSamples"]; }
    }
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Iterations"]; }
    }
    #endregion

    #region Properties
    private int K {
      get { return KParameter.Value.Value; }
      set { KParameter.Value.Value = value; }
    }
    private int Dimensions {
      get { return DimensionsParameter.Value.Value; }
      set { DimensionsParameter.Value.Value = value; }
    }
    private int NeighborSamples {
      get { return NeighborSamplesParameter.Value.Value; }
      set { NeighborSamplesParameter.Value.Value = value; }
    }
    private int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private NcaAlgorithm(bool deserializing) : base(deserializing) { }
    private NcaAlgorithm(NcaAlgorithm original, Cloner cloner) : base(original, cloner) { }
    public NcaAlgorithm()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("K", "The K for the nearest neighbor.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<IntValue>("Dimensions", "The number of dimensions that NCA should reduce the data to.", new IntValue(2)));
      Parameters.Add(new ConstrainedValueParameter<INCAInitializer>("Initialization", "Which method should be used to initialize the matrix. Typically LDA (linear discriminant analysis) should provide a good estimate."));
      Parameters.Add(new FixedValueParameter<IntValue>("NeighborSamples", "How many of the neighbors should be sampled in order to speed up the calculation. This should be at least the value of k and at most the number of training instances minus one.", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<IntValue>("Iterations", "How many iterations the conjugate gradient (CG) method should be allowed to perform. The method might still terminate earlier if a local optima has already been reached.", new IntValue(20)));

      INCAInitializer defaultInitializer = null;
      foreach (var initializer in ApplicationManager.Manager.GetInstances<INCAInitializer>().OrderBy(x => x.ItemName)) {
        if (initializer is LDAInitializer) defaultInitializer = initializer;
        InitializationParameter.ValidValues.Add(initializer);
      }
      if (defaultInitializer != null) InitializationParameter.Value = defaultInitializer;

      Problem = new ClassificationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaAlgorithm(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    protected override void Run() {
      var initializer = InitializationParameter.Value;

      var clonedProblem = (IClassificationProblemData)Problem.ProblemData.Clone();
      var model = Train(clonedProblem, K, Dimensions, NeighborSamples, Iterations, initializer.Initialize(clonedProblem, Dimensions), ReportQuality, CancellationToken.None);
      Results.Add(new Result("ClassificationSolution", "The classification solution.", model.CreateClassificationSolution(clonedProblem)));
    }

    public static INcaClassificationSolution CreateClassificationSolution(IClassificationProblemData data, int k, int dimensions, int neighborSamples, int iterations, INCAInitializer initializer) {
      var clonedProblem = (IClassificationProblemData)data.Clone();
      var model = Train(clonedProblem, k, dimensions, neighborSamples, iterations, initializer);
      return model.CreateClassificationSolution(clonedProblem);
    }

    public static INcaModel Train(IClassificationProblemData problemData, int k, int dimensions, int neighborSamples, int iterations, INCAInitializer initializer) {
      return Train(problemData, k, dimensions, neighborSamples, iterations, initializer.Initialize(problemData, dimensions), null, CancellationToken.None);
    }

    public static INcaModel Train(IClassificationProblemData problemData, int k, int neighborSamples, int iterations, double[,] initalMatrix) {
      var matrix = new double[initalMatrix.Length];
      for (int i = 0; i < initalMatrix.GetLength(0); i++)
        for (int j = 0; j < initalMatrix.GetLength(1); j++)
          matrix[i * initalMatrix.GetLength(1) + j] = initalMatrix[i, j];
      return Train(problemData, k, initalMatrix.GetLength(1), neighborSamples, iterations, matrix, null, CancellationToken.None);
    }

    private static INcaModel Train(IClassificationProblemData data, int k, int dimensions, int neighborSamples, int iterations, double[] matrix, Reporter reporter, CancellationToken cancellation) {
      var scaling = new Scaling(data.Dataset, data.AllowedInputVariables, data.TrainingIndices);
      var scaledData = AlglibUtil.PrepareAndScaleInputMatrix(data.Dataset, data.AllowedInputVariables, data.TrainingIndices, scaling);
      var classes = data.Dataset.GetDoubleValues(data.TargetVariable, data.TrainingIndices).ToArray();
      var attributes = scaledData.GetLength(1);

      alglib.mincgstate state;
      alglib.mincgreport rep;
      alglib.mincgcreate(matrix, out state);
      alglib.mincgsetcond(state, 0, 0, 0, iterations);
      alglib.mincgsetxrep(state, true);
      int neighborSampleSize = neighborSamples;
      Optimize(state, scaledData, classes, dimensions, neighborSampleSize, cancellation, reporter);
      alglib.mincgresults(state, out matrix, out rep);

      var transformationMatrix = new double[attributes, dimensions];
      var counter = 0;
      for (var i = 0; i < attributes; i++)
        for (var j = 0; j < dimensions; j++)
          transformationMatrix[i, j] = matrix[counter++];

      return new NcaModel(k, transformationMatrix, data.Dataset, data.TrainingIndices, data.TargetVariable, data.AllowedInputVariables, scaling, data.ClassValues.ToArray());
    }

    private static void Optimize(alglib.mincgstate state, double[,] data, double[] classes, int dimensions, int neighborSampleSize, CancellationToken cancellation, Reporter reporter) {
      while (alglib.mincgiteration(state)) {
        if (cancellation.IsCancellationRequested) break;
        if (state.needfg) {
          Gradient(state.x, ref state.innerobj.f, state.innerobj.g, data, classes, dimensions, neighborSampleSize);
          continue;
        }
        if (state.innerobj.xupdated) {
          if (reporter != null)
            reporter(state.innerobj.f, state.innerobj.x);
          continue;
        }
        throw new InvalidOperationException("Neighborhood Components Analysis: Error in Optimize() (some derivatives were not provided?)");
      }
    }

    private static void Gradient(double[] A, ref double func, double[] grad, double[,] data, double[] classes, int dimensions, int neighborSampleSize) {
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
          transformedDistances[k] = Math.Exp(-iVector.Multiply(AMatrix).Subtract(kVector.Multiply(AMatrix)).SquaredVectorLength());
        }
        var sample = transformedDistances.OrderByDescending(x => x.Value).Take(neighborSampleSize).ToArray();
        var normalization = sample.Sum(x => x.Value);
        if (normalization > 0) {
          foreach (var s in sample) {
            if (s.Value <= 0) break;
            alglib.sparseset(probabilities, i, s.Key, s.Value / normalization);
          }
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

      func = -pi.Sum();

      r = 0;
      var newGrad = AMatrix.Multiply(-2.0).Transpose().Multiply(new Matrix(innerSum)).Transpose();
      foreach (var g in newGrad) {
        grad[r++] = g;
      }
    }

    private void ReportQuality(double func, double[] coefficients) {
      var instances = Problem.ProblemData.TrainingIndices.Count();
      DataTable qualities;
      if (!Results.ContainsKey("Optimization")) {
        qualities = new DataTable("Optimization");
        qualities.Rows.Add(new DataRow("Quality", string.Empty));
        Results.Add(new Result("Optimization", qualities));
      } else qualities = (DataTable)Results["Optimization"].Value;
      qualities.Rows["Quality"].Values.Add(-func / instances);

      if (!Results.ContainsKey("Quality")) {
        Results.Add(new Result("Quality", new DoubleValue(-func / instances)));
      } else ((DoubleValue)Results["Quality"].Value).Value = -func / instances;
    }

    private static IEnumerable<double> GetRow(double[,] data, int row) {
      for (int i = 0; i < data.GetLength(1); i++)
        yield return data[row, i];
    }
  }
}
