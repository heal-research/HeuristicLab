#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NcaModelCreator", "Creates an NCA model with a given matrix.")]
  [StorableType("30D2840C-1FE3-4A45-97FF-294C93D33D8C")]
  public class NcaModelCreator : SingleSuccessorOperator, INcaModelCreator {

    public ILookupParameter<IntValue> KParameter {
      get { return (ILookupParameter<IntValue>)Parameters["K"]; }
    }

    public ILookupParameter<IntValue> DimensionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Dimensions"]; }
    }

    public ILookupParameter<RealVector> NcaMatrixParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NcaMatrix"]; }
    }

    public ILookupParameter<RealVector> NcaMatrixGradientsParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NcaMatrixGradients"]; }
    }

    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters["ProblemData"]; }
    }

    public ILookupParameter<INcaModel> NcaModelParameter {
      get { return (ILookupParameter<INcaModel>)Parameters["NcaModel"]; }
    }

    [StorableConstructor]
    protected NcaModelCreator(StorableConstructorFlag _) : base(_) { }
    protected NcaModelCreator(NcaModelCreator original, Cloner cloner) : base(original, cloner) { }
    public NcaModelCreator() {
      Parameters.Add(new LookupParameter<IntValue>("K", "How many neighbors should be considered for classification."));
      Parameters.Add(new LookupParameter<IntValue>("Dimensions", "The dimensions to which the feature space should be reduced to."));
      Parameters.Add(new LookupParameter<RealVector>("NcaMatrix", "The optimized matrix."));
      Parameters.Add(new LookupParameter<RealVector>("NcaMatrixGradients", "The gradients from the matrix that is being optimized."));
      Parameters.Add(new LookupParameter<IClassificationProblemData>("ProblemData", "The classification problem data."));
      Parameters.Add(new LookupParameter<INcaModel>("NcaModel", "The NCA model that should be created."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaModelCreator(this, cloner);
    }

    public override IOperation Apply() {
      var k = KParameter.ActualValue.Value;
      var dim = DimensionsParameter.ActualValue.Value;
      var vector = NcaMatrixParameter.ActualValue;
      var matrix = new double[vector.Length / dim, dim];

      for (int i = 0; i < matrix.GetLength(0); i++)
        for (int j = 0; j < dim; j++) {
          matrix[i, j] = vector[i * dim + j];
        }

      var problemData = ProblemDataParameter.ActualValue;
      NcaModelParameter.ActualValue = new NcaModel(k, matrix, problemData.Dataset, problemData.TrainingIndices, problemData.TargetVariable, problemData.AllowedInputVariables, problemData.ClassValues.ToArray());
      return base.Apply();
    }
  }
}
