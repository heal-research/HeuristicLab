#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate inversion moves (2-opt).
  /// </summary>
  [Item("TSPInversionMoveDistanceMatrixEvaluator", "Evaluates an inversion move (2-opt) by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableType("1EA2C5B2-96E0-4C64-9089-F08F6D2B1CAE")]
  public class TSPInversionMoveDistanceMatrixEvaluator : TSPMoveEvaluator, IPermutationInversionMoveOperator {
    public override Type EvaluatorType {
      get { return typeof(TSPDistanceMatrixEvaluator); }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected TSPInversionMoveDistanceMatrixEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPInversionMoveDistanceMatrixEvaluator(TSPInversionMoveDistanceMatrixEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPInversionMoveDistanceMatrixEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPInversionMoveDistanceMatrixEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      double relativeQualityDifference = 0;
      var distanceMatrix = DistanceMatrixParameter.ActualValue;
      if (distanceMatrix == null) throw new InvalidOperationException("The distance matrix is not given.");
      var move = InversionMoveParameter.ActualValue;
      relativeQualityDifference = TSPInversionMovePathEvaluator.EvaluateByDistanceMatrix(permutation, move, distanceMatrix);

      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(QualityParameter.ActualValue.Value + relativeQualityDifference);
      else moveQuality.Value = QualityParameter.ActualValue.Value + relativeQualityDifference;
      return base.Apply();
    }
  }
}
