#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions.
  /// </summary>
  [Item("TSPMoveEvaluator", "A base class for operators which evaluate TSP moves.")]
  [StorableClass]
  public abstract class TSPPathMoveEvaluator : TSPMoveEvaluator, ITSPPathMoveEvaluator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public ILookupParameter<EnumValue<TSPDistanceFunction>> DistanceFunctionParameter {
      get { return (ILookupParameter<EnumValue<TSPDistanceFunction>>)Parameters["DistanceFunction"]; }
    }

    [StorableConstructor]
    protected TSPPathMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected TSPPathMoveEvaluator(TSPPathMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected TSPPathMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The city's coordinates."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated (if it does not exist already) and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<EnumValue<TSPDistanceFunction>>("DistanceFunction", "The distance function to use when the distance matrix is not being used."));
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      var coordinates = CoordinatesParameter.ActualValue;
      var distanceFunction = DistanceFunctionParameter.ActualValue.Value;
      double relativeQualityDifference = 0;
      if (UseDistanceMatrixParameter.ActualValue.Value) {
        DistanceMatrix distanceMatrix = DistanceMatrixParameter.ActualValue;
        if (distanceMatrix == null) {
          throw new InvalidOperationException("Distance matrix is not given.");
        }
        relativeQualityDifference = EvaluateByDistanceMatrix(permutation, distanceMatrix);
      } else {
        if (coordinates == null) throw new InvalidOperationException("No coordinates were given.");
        relativeQualityDifference = EvaluateByCoordinates(permutation, coordinates, distanceFunction);
      }
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(QualityParameter.ActualValue.Value + relativeQualityDifference);
      else moveQuality.Value = QualityParameter.ActualValue.Value + relativeQualityDifference;
      return base.Apply();
    }
    protected abstract double EvaluateByDistanceMatrix(Permutation permutation, DistanceMatrix distanceMatrix);
    protected abstract double EvaluateByCoordinates(Permutation permutation, DoubleMatrix coordinates, TSPDistanceFunction distanceFunction);
  }
}
