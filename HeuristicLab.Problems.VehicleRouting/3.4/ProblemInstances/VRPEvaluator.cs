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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("VRPEvaluator", "Represents a VRP evaluator.")]
  [StorableClass]
  public abstract class VRPEvaluator : VRPOperator, IVRPEvaluator {
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    #region ISingleObjectiveEvaluator Members
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    #endregion

    public ILookupParameter<DoubleValue> DistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ILookupParameter<DoubleValue> VehcilesUtilizedParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }

    public ILookupParameter<DoubleValue> PenaltyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Penalty"]; }
    }

    [StorableConstructor]
    protected VRPEvaluator(bool deserializing) : base(deserializing) { }

    public VRPEvaluator() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));

      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("VehiclesUtilized", "The number of vehicles utilized."));

      Parameters.Add(new LookupParameter<DoubleValue>("Penalty", "The applied penalty."));
    }

    protected VRPEvaluator(VRPEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected virtual VRPEvaluation CreateTourEvaluation() {
      return new VRPEvaluation();
    }

    protected abstract void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution);

    protected virtual void InitResultParameters() {
      QualityParameter.ActualValue = new DoubleValue(0);
      VehcilesUtilizedParameter.ActualValue = new DoubleValue(0);
      DistanceParameter.ActualValue = new DoubleValue(0);
      PenaltyParameter.ActualValue = new DoubleValue(0);
    }

    protected virtual void SetResultParameters(VRPEvaluation tourEvaluation) {
      QualityParameter.ActualValue.Value = tourEvaluation.Quality;
      VehcilesUtilizedParameter.ActualValue.Value = tourEvaluation.VehicleUtilization;
      DistanceParameter.ActualValue.Value = tourEvaluation.Distance;
      PenaltyParameter.ActualValue.Value = tourEvaluation.Penalty;
    }

    #region IVRPEvaluator Members

    public bool Feasible(VRPEvaluation evaluation) {
      return evaluation.Penalty < double.Epsilon;
    }

    protected abstract double GetTourInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, TourInsertionInfo tourInsertionInfo, int index, int customer, out bool feasible);

    public double GetInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, VRPEvaluation eval, int customer, int tour, int index, out bool feasible) {
      bool tourFeasible;
      double costs = GetTourInsertionCosts(
        instance,
        solution,
        eval.InsertionInfo.GetTourInsertionInfo(tour),
        index,
        customer, out tourFeasible);

      feasible = tourFeasible;

      return costs;
    }

    public VRPEvaluation EvaluateTour(IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
      VRPEvaluation evaluation = CreateTourEvaluation();
      EvaluateTour(evaluation, instance, tour, solution);
      return evaluation;
    }

    public virtual VRPEvaluation Evaluate(IVRPProblemInstance instance, IVRPEncoding solution) {
      VRPEvaluation evaluation = CreateTourEvaluation();

      foreach (Tour tour in solution.GetTours()) {
        EvaluateTour(evaluation, instance, tour, solution);
      }

      return evaluation;
    }

    public override IOperation Apply() {
      InitResultParameters();

      VRPEvaluation evaluation = CreateTourEvaluation();
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      foreach (Tour tour in solution.GetTours()) {
        EvaluateTour(evaluation, ProblemInstance, tour, solution);
      }
      SetResultParameters(evaluation);

      QualityParameter.ActualValue = new DoubleValue(evaluation.Quality);

      return base.Apply();
    }

    #endregion
  }
}