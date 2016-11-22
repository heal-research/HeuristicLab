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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Orienteering {
  [Item("OrienteeringEvaluator", "Operator to evaluate a solution to the orienteering problem.")]
  [StorableClass]
  public class OrienteeringEvaluator : InstrumentedOperator, IOrienteeringEvaluator {

    #region ParameterProperties
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> PenaltyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public ILookupParameter<DoubleValue> DistancePenaltyFactorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["DistancePenaltyFactor"]; }
    }
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    public ILookupParameter<DoubleArray> ScoresParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Scores"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<DoubleValue> MaximumDistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MaximumDistance"]; }
    }
    public ILookupParameter<DoubleValue> PointVisitingCostsParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PointVisitingCosts"]; }
    }
    #endregion

    [StorableConstructor]
    protected OrienteeringEvaluator(bool deserializing)
      : base(deserializing) {
    }
    protected OrienteeringEvaluator(OrienteeringEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringEvaluator(this, cloner);
    }
    public OrienteeringEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the Orienteering solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("Penalty", "The applied penalty"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("DistancePenaltyFactor", "The penalty applied per distance violation.", new DoubleValue(0)));

      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The Orienteering Solution given in path representation."));
      Parameters.Add(new LookupParameter<DoubleArray>("Scores", "The scores of the points."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the points."));
      Parameters.Add(new LookupParameter<DoubleValue>("MaximumDistance", "The maximum distance constraint for a Orienteering solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("PointVisitingCosts", "The costs for visiting a point."));
    }

    public static OrienteeringEvaluationResult Apply(IntegerVector solution, DoubleArray scores,
      DistanceMatrix distances, double maximumDistance, double pointVisitingCosts, double distancePenaltyFactor) {

      double score = solution.Sum(t => scores[t]);
      double distance = distances.CalculateTourLength(solution.ToList(), pointVisitingCosts);

      double distanceViolation = distance - maximumDistance;

      double penalty = 0.0;
      penalty += distanceViolation > 0 ? distanceViolation * distancePenaltyFactor : 0;

      double quality = score - penalty;

      return new OrienteeringEvaluationResult {
        Quality = new DoubleValue(quality),
        Penalty = new DoubleValue(penalty),
        Distance = new DoubleValue(distance)
      };
    }

    public override IOperation InstrumentedApply() {
      var evaluation = Apply(IntegerVectorParameter.ActualValue, ScoresParameter.ActualValue,
        DistanceMatrixParameter.ActualValue, MaximumDistanceParameter.ActualValue.Value,
        PointVisitingCostsParameter.ActualValue.Value, DistancePenaltyFactorParameter.ActualValue.Value);

      QualityParameter.ActualValue = evaluation.Quality;
      PenaltyParameter.ActualValue = evaluation.Penalty;

      return base.InstrumentedApply();
    }

    public OrienteeringEvaluationResult Evaluate(IntegerVector solution, DoubleArray scores,
      DistanceMatrix distances, double maximumDistance, double pointVisitingCosts) {
      return Apply(solution, scores, distances, maximumDistance, pointVisitingCosts,
        ((IValueParameter<DoubleValue>)DistancePenaltyFactorParameter).Value.Value);
    }
  }
}