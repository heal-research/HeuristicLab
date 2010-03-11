#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.Permutation;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// An operator to evaluate 2-opt moves.
  /// </summary>
  [Item("TwoOptMoveTSPEvaluator", "Evaluates a 2-opt move (rounded euclidean distances) by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableClass(StorableClassType.Empty)]
  public class TwoOptMoveTSPEvaluator : SingleSuccessorOperator {
    public ILookupParameter<DoubleData> QualityParameter {
      get { return (ILookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    public LookupParameter<TwoOptMove> MoveParameter {
      get { return (LookupParameter<TwoOptMove>)Parameters["Move"]; }
    }
    public LookupParameter<DoubleData> MoveQualityParameter {
      get { return (LookupParameter<DoubleData>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }

    public TwoOptMoveTSPEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleData>("Quality", "The evaluated quality of the TSP solution."));
      Parameters.Add(new LookupParameter<TwoOptMove>("Move", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleData>("MoveQuality", "Where to store the move quality."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DoubleMatrixData>("Coordinates", "The city's coordinates."));
    }

    public override IOperation Apply() {
      TwoOptMove move = MoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleMatrixData coordinates = CoordinatesParameter.ActualValue;
      double moveQuality = QualityParameter.ActualValue.Value;
      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      // remove two edges
      moveQuality -= CalculateDistance(coordinates[edge1source ,0], coordinates[edge1source, 1],
        coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality -= CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      // add two edges
      moveQuality += CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge2source, 0], coordinates[edge2source, 1]);
      moveQuality += CalculateDistance(coordinates[edge1target, 0], coordinates[edge1target, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      MoveQualityParameter.ActualValue = new DoubleData(moveQuality);
      return base.Apply();
    }

    private double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
  }
}
