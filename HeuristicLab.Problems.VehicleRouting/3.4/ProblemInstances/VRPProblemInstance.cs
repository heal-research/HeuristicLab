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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("VRPProblemInstance", "Represents a VRP instance.")]
  [StorableClass]
  public abstract class VRPProblemInstance : ParameterizedNamedItem, IVRPProblemInstance, IStatefulItem {
    IVRPEvaluator moveEvaluator;

    private object locker = new object();

    public IVRPEvaluator MoveEvaluator {
      get {
        lock (locker) {
          if (evaluator == null)
            return null;
          else {
            if (moveEvaluator == null) {
              moveEvaluator = evaluator.Clone() as IVRPEvaluator;

              foreach (IParameter parameter in moveEvaluator.Parameters) {
                if (parameter is ILookupParameter
                  && parameter != moveEvaluator.ProblemInstanceParameter
                  && parameter != moveEvaluator.VRPToursParameter) {
                  (parameter as ILookupParameter).ActualName =
                    VRPMoveEvaluator.MovePrefix +
                    (parameter as ILookupParameter).ActualName;
                }
              }
            }

            return moveEvaluator;
          }
        }
      }
    }

    protected abstract IEnumerable<IOperator> GetOperators();
    protected abstract IEnumerable<IOperator> GetAnalyzers();

    public IEnumerable<IOperator> Operators {
      get {
        return GetOperators().Union(GetAnalyzers());
      }
    }

    protected ValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ValueParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    protected OptionalValueParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (OptionalValueParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }
    protected ValueParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    protected ValueParameter<IntValue> VehiclesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Vehicles"]; }
    }
    protected ValueParameter<DoubleArray> DemandParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["Demand"]; }
    }

    protected IValueParameter<DoubleValue> FleetUsageFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalFleetUsageFactor"]; }
    }
    protected IValueParameter<DoubleValue> DistanceFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalDistanceFactor"]; }
    }

    public DoubleMatrix Coordinates {
      get { return CoordinatesParameter.Value; }
      set { CoordinatesParameter.Value = value; }
    }

    public DoubleMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.Value; }
      set { DistanceMatrixParameter.Value = value; }
    }
    public BoolValue UseDistanceMatrix {
      get { return UseDistanceMatrixParameter.Value; }
      set { UseDistanceMatrixParameter.Value = value; }
    }
    public IntValue Vehicles {
      get { return VehiclesParameter.Value; }
      set { VehiclesParameter.Value = value; }
    }
    public DoubleArray Demand {
      get { return DemandParameter.Value; }
      set { DemandParameter.Value = value; }
    }
    public virtual IntValue Cities {
      get { return new IntValue(Demand.Length); }
    }

    public DoubleValue FleetUsageFactor {
      get { return FleetUsageFactorParameter.Value; }
      set { FleetUsageFactorParameter.Value = value; }
    }
    public DoubleValue DistanceFactor {
      get { return DistanceFactorParameter.Value; }
      set { DistanceFactorParameter.Value = value; }
    }

    protected virtual double CalculateDistance(int start, int end) {
      double distance = 0.0;

      distance =
          Math.Sqrt(
            Math.Pow(Coordinates[start, 0] - Coordinates[end, 0], 2) +
            Math.Pow(Coordinates[start, 1] - Coordinates[end, 1], 2));

      return distance;
    }

    private DoubleMatrix CreateDistanceMatrix() {
      DoubleMatrix distanceMatrix = new DoubleMatrix(Coordinates.Rows, Coordinates.Rows);

      for (int i = 0; i < distanceMatrix.Rows; i++) {
        for (int j = 0; j < distanceMatrix.Columns; j++) {
          double distance = CalculateDistance(i, j);

          distanceMatrix[i, j] = distance;
        }
      }

      return distanceMatrix;
    }

    public virtual double[] GetCoordinates(int city) {
      double[] coordinates = new double[Coordinates.Columns];

      for (int i = 0; i < Coordinates.Columns; i++) {
        coordinates[i] = Coordinates[city, i];
      }

      return coordinates;
    }

    public virtual double GetDemand(int city) {
      return Demand[city];
    }

    //cache for performance improvement
    private DoubleMatrix distanceMatrix = null;
    private IVRPEvaluator evaluator = null;

    public IVRPEvaluator SolutionEvaluator {
      get {
        return evaluator;
      }

      set {
        lock (locker) {
          moveEvaluator = null;
          evaluator = value;
          EvalBestKnownSolution();
        }
      }
    }

    public virtual double GetDistance(int start, int end, IVRPEncoding solution) {
      if (distanceMatrix == null && UseDistanceMatrix.Value) {
        distanceMatrix = DistanceMatrix ?? CreateDistanceMatrix();
      }

      if (distanceMatrix != null) return distanceMatrix[start, end];
      return CalculateDistance(start, end);
    }

    public virtual double GetInsertionDistance(int start, int customer, int end, IVRPEncoding solution,
      out double startDistance, out double endDistance) {
      double distance = GetDistance(start, end, solution);

      startDistance = GetDistance(start, customer, solution);
      endDistance = GetDistance(customer, end, solution);

      double newDistance = startDistance + endDistance;

      return newDistance - distance;
    }

    public bool Feasible(IVRPEncoding solution) {
      return evaluator.Feasible(
        evaluator.Evaluate(
          this, solution));
    }

    public bool TourFeasible(Tour tour, IVRPEncoding solution) {
      return evaluator.Feasible(
        evaluator.EvaluateTour(
        this, tour, solution));
    }

    public VRPEvaluation Evaluate(IVRPEncoding solution) {
      return evaluator.Evaluate(this, solution);
    }

    public VRPEvaluation EvaluateTour(Tour tour, IVRPEncoding solution) {
      return evaluator.EvaluateTour(this, tour, solution);
    }

    public bool Feasible(VRPEvaluation eval) {
      return evaluator.Feasible(eval);
    }

    public double GetInsertionCosts(VRPEvaluation eval, IVRPEncoding solution, int customer, int tour, int index, out bool feasible) {
      return evaluator.GetInsertionCosts(this, solution, eval, customer, tour, index, out feasible);
    }


    public event EventHandler EvaluationChanged;

    protected void EvalBestKnownSolution() {
      EventHandler tmp = EvaluationChanged;
      if (tmp != null)
        tmp(this, null);
    }

    protected abstract IVRPEvaluator Evaluator { get; }
    protected abstract IVRPCreator Creator { get; }

    [StorableConstructor]
    protected VRPProblemInstance(bool deserializing) : base(deserializing) { }

    public VRPProblemInstance()
      : base() {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities.", new DoubleMatrix()));
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("Vehicles", "The number of vehicles.", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleArray>("Demand", "The demand of each customer.", new DoubleArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalFleetUsageFactor", "The fleet usage factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalDistanceFactor", "The distance factor considered in the evaluation.", new DoubleValue(1)));

      evaluator = Evaluator;
      AttachEventHandlers();
    }

    protected VRPProblemInstance(VRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      evaluator = Evaluator;
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      evaluator = Evaluator;
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      DistanceFactorParameter.ValueChanged += new EventHandler(DistanceFactorParameter_ValueChanged);
      DistanceFactorParameter.Value.ValueChanged += new EventHandler(DistanceFactor_ValueChanged);
      FleetUsageFactorParameter.ValueChanged += new EventHandler(FleetUsageFactorParameter_ValueChanged);
      FleetUsageFactorParameter.Value.ValueChanged += new EventHandler(FleetUsageFactor_ValueChanged);
      DistanceMatrixParameter.ValueChanged += new EventHandler(DistanceMatrixParameter_ValueChanged);
      if (DistanceMatrix != null) {
        DistanceMatrix.ItemChanged += new EventHandler<EventArgs<int, int>>(DistanceMatrix_ItemChanged);
        DistanceMatrix.Reset += new EventHandler(DistanceMatrix_Reset);
      }
      UseDistanceMatrixParameter.ValueChanged += new EventHandler(UseDistanceMatrixParameter_ValueChanged);
      UseDistanceMatrix.ValueChanged += new EventHandler(UseDistanceMatrix_ValueChanged);
    }

    public virtual void InitializeState() {
    }

    public virtual void ClearState() {
    }

    #region Event handlers
    void DistanceFactorParameter_ValueChanged(object sender, EventArgs e) {
      DistanceFactorParameter.Value.ValueChanged += new EventHandler(DistanceFactor_ValueChanged);
      EvalBestKnownSolution();
    }
    void DistanceFactor_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void FleetUsageFactorParameter_ValueChanged(object sender, EventArgs e) {
      FleetUsageFactorParameter.Value.ValueChanged += new EventHandler(FleetUsageFactor_ValueChanged);
      EvalBestKnownSolution();
    }
    void FleetUsageFactor_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void DistanceMatrixParameter_ValueChanged(object sender, EventArgs e) {
      if (DistanceMatrix != null) {
        DistanceMatrix.ItemChanged += new EventHandler<EventArgs<int, int>>(DistanceMatrix_ItemChanged);
        DistanceMatrix.Reset += new EventHandler(DistanceMatrix_Reset);
      }
      distanceMatrix = DistanceMatrix;
      EvalBestKnownSolution();
    }
    void DistanceMatrix_Reset(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void DistanceMatrix_ItemChanged(object sender, EventArgs<int, int> e) {
      distanceMatrix = DistanceMatrix;
      EvalBestKnownSolution();
    }
    void UseDistanceMatrixParameter_ValueChanged(object sender, EventArgs e) {
      UseDistanceMatrix.ValueChanged += new EventHandler(UseDistanceMatrix_ValueChanged);
      if (!UseDistanceMatrix.Value)
        distanceMatrix = null;
      EvalBestKnownSolution();
    }
    void UseDistanceMatrix_ValueChanged(object sender, EventArgs e) {
      if (!UseDistanceMatrix.Value)
        distanceMatrix = null;
      EvalBestKnownSolution();
    }
    #endregion
  }
}