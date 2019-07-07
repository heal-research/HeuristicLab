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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [Item("EstimatedPTSPMoveEvaluator", "A base class for operators which evaluate PTSP moves.")]
  [StorableType("F2F7F857-F2CD-4AB2-8656-5158BD04EFDD")]
  public abstract class EstimatedPTSPMoveEvaluator : SingleSuccessorOperator, IEstimatedPTSPMoveEvaluator {

    public override bool CanChangeName {
      get { return false; }
    }

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
    public ILookupParameter<ItemList<BoolArray>> RealizationsParameter {
      get { return (ILookupParameter<ItemList<BoolArray>>)Parameters["Realizations"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<DistanceCalculator> DistanceCalculatorParameter {
      get { return (ILookupParameter<DistanceCalculator>)Parameters["DistanceCalculator"]; }
    }

    [StorableConstructor]
    protected EstimatedPTSPMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected EstimatedPTSPMoveEvaluator(EstimatedPTSPMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected EstimatedPTSPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The city's coordinates."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated (if it does not exist already) and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<ItemList<BoolArray>>("Realizations", "The list of samples drawn from all possible stochastic instances."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a TSP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a TSP solution."));
      Parameters.Add(new LookupParameter<DistanceCalculator>("DistanceCalculator", "The class that can compute distances between coordinates."));
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      var coordinates = CoordinatesParameter.ActualValue;
      var realizations = RealizationsParameter.ActualValue;
      Func<int, int, double> distance = null;
      if (UseDistanceMatrixParameter.ActualValue.Value) {
        var distanceMatrix = DistanceMatrixParameter.ActualValue;
        if (distanceMatrix == null) throw new InvalidOperationException("The distance matrix has not been calculated.");
        distance = (a, b) => distanceMatrix[a, b];
      } else {
        if (coordinates == null) throw new InvalidOperationException("No coordinates were given.");
        var distanceCalculator = DistanceCalculatorParameter.ActualValue;
        if (distanceCalculator == null) throw new InvalidOperationException("Distance calculator is null!");
        distance = (a, b) => distanceCalculator.Calculate(a, b, coordinates);
      }
      var relativeQualityDifference = EvaluateMove(permutation, distance, realizations);
      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(QualityParameter.ActualValue.Value + relativeQualityDifference);
      else moveQuality.Value = QualityParameter.ActualValue.Value + relativeQualityDifference;
      return base.Apply();
    }

    protected abstract double EvaluateMove(Permutation permutation, Func<int, int, double> distance, ItemList<BoolArray> realizations);
  }
}
