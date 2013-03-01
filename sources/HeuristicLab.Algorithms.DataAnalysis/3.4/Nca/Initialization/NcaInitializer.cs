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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NcaInitializer", "Base class for initializers for NCA.")]
  [StorableClass]
  public abstract class NcaInitializer : SingleSuccessorOperator, INcaInitializer {

    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters["ProblemData"]; }
    }
    public ILookupParameter<IntValue> DimensionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Dimensions"]; }
    }
    public ILookupParameter<RealVector> NcaMatrixParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NcaMatrix"]; }
    }
    public ILookupParameter<Scaling> ScalingParameter {
      get { return (ILookupParameter<Scaling>)Parameters["Scaling"]; }
    }

    [StorableConstructor]
    protected NcaInitializer(bool deserializing) : base(deserializing) { }
    protected NcaInitializer(NcaInitializer original, Cloner cloner) : base(original, cloner) { }
    public NcaInitializer() {
      Parameters.Add(new LookupParameter<IClassificationProblemData>("ProblemData", "The classification problem data."));
      Parameters.Add(new LookupParameter<IntValue>("Dimensions", "The number of dimensions to which the features should be pruned."));
      Parameters.Add(new LookupParameter<RealVector>("NcaMatrix", "The coefficients of the matrix that need to be optimized. Note that the matrix is flattened."));
      Parameters.Add(new LookupParameter<Scaling>("Scaling", "Each dataset is scaled and the information is stored in a scaling object."));
    }

    public override IOperation Apply() {
      var problemData = ProblemDataParameter.ActualValue;
      var scaling = new Scaling(problemData.Dataset, problemData.AllowedInputVariables, problemData.TrainingIndices);

      var dimensions = DimensionsParameter.ActualValue.Value;
      var matrix = Initialize(problemData, scaling, dimensions);
      var attributes = matrix.GetLength(0);

      var result = new double[attributes * dimensions];
      for (int i = 0; i < attributes; i++)
        for (int j = 0; j < dimensions; j++)
          result[i * dimensions + j] = matrix[i, j];

      ScalingParameter.ActualValue = scaling;
      NcaMatrixParameter.ActualValue = new RealVector(result);
      return base.Apply();
    }

    public abstract double[,] Initialize(IClassificationProblemData data, Scaling scaling, int dimensions);
  }
}
