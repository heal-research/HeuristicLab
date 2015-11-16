#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator to evaluate inversion moves (2-opt).
  /// </summary>
  [Item("TSPTranslocationMoveDistanceMatrixEvaluator", "Evaluates a translocation or insertion move (3-opt).")]
  [StorableClass]
  public class TSPTranslocationMoveDistanceMatrixEvaluator : TSPMoveEvaluator, IPermutationTranslocationMoveOperator {
    public override Type EvaluatorType {
      get { return typeof(TSPDistanceMatrixEvaluator); }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected TSPTranslocationMoveDistanceMatrixEvaluator(bool deserializing) : base(deserializing) { }
    protected TSPTranslocationMoveDistanceMatrixEvaluator(TSPTranslocationMoveDistanceMatrixEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPTranslocationMoveDistanceMatrixEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPTranslocationMoveDistanceMatrixEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      double relativeQualityDifference = 0;
      var distanceMatrix = DistanceMatrixParameter.ActualValue;
      if (distanceMatrix == null) throw new InvalidOperationException("The distance matrix is not given.");
      var move = TranslocationMoveParameter.ActualValue;
      relativeQualityDifference = TSPTranslocationMovePathEvaluator.EvaluateByDistanceMatrix(permutation, move, distanceMatrix);

      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(QualityParameter.ActualValue.Value + relativeQualityDifference);
      else moveQuality.Value = QualityParameter.ActualValue.Value + relativeQualityDifference;
      return base.Apply();
    }
  }
}
