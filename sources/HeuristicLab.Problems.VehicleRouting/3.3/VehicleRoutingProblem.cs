#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Encodings.Prins;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("Vehicle Routing Problem", "Represents a Vehicle Routing Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class VehicleRoutingProblem : SingleObjectiveHeuristicOptimizationProblem<IVRPEvaluator, IVRPCreator>, IStorableContent {
    public string Filename { get; set; }

    #region Parameter Properties
    public ValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ValueParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public OptionalValueParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (OptionalValueParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ValueParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public ValueParameter<IntValue> VehiclesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Vehicles"]; }
    }
    public ValueParameter<DoubleValue> CapacityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Capacity"]; }
    }
    public ValueParameter<DoubleArray> DemandParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["Demand"]; }
    }
    public ValueParameter<DoubleArray> ReadyTimeParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    public ValueParameter<DoubleArray> DueTimeParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    public ValueParameter<DoubleArray> ServiceTimeParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["ServiceTime"]; }
    }
    public IValueParameter<DoubleValue> FleetUsageFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalFleetUsageFactor"]; }
    }
    public IValueParameter<DoubleValue> TimeFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTimeFactor"]; }
    }
    public IValueParameter<DoubleValue> DistanceFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalDistanceFactor"]; }
    }
    public IValueParameter<DoubleValue> OverloadPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalOverloadPenalty"]; }
    }
    public IValueParameter<DoubleValue> TardinessPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTardinessPenalty"]; }
    }
    public OptionalValueParameter<IVRPEncoding> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<IVRPEncoding>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
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
    public DoubleValue Capacity {
      get { return CapacityParameter.Value; }
      set { CapacityParameter.Value = value; }
    }
    public DoubleArray Demand {
      get { return DemandParameter.Value; }
      set { DemandParameter.Value = value; }
    }
    public DoubleArray ReadyTime {
      get { return ReadyTimeParameter.Value; }
      set { ReadyTimeParameter.Value = value; }
    }
    public DoubleArray DueTime {
      get { return DueTimeParameter.Value; }
      set { DueTimeParameter.Value = value; }
    }
    public DoubleArray ServiceTime {
      get { return ServiceTimeParameter.Value; }
      set { ServiceTimeParameter.Value = value; }
    }
    public IVRPEncoding BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestVRPSolutionAnalyzer BestVRPSolutionAnalyzer {
      get { return Operators.OfType<BestVRPSolutionAnalyzer>().FirstOrDefault(); }
    }
    private BestAverageWorstVRPToursAnalyzer BestAverageWorstVRPToursAnalyzer {
      get { return Operators.OfType<BestAverageWorstVRPToursAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [Obsolete]
    [Storable(Name = "operators")]
    private IEnumerable<IOperator> oldOperators {
      get { return null; }
      set {
        if (value != null && value.Any())
          Operators.AddRange(value);
      }
    }
    #endregion

    [StorableConstructor]
    private VehicleRoutingProblem(bool deserializing) : base(deserializing) { }
    private VehicleRoutingProblem(VehicleRoutingProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public VehicleRoutingProblem()
      : base(new VRPEvaluator(), new RandomCreator()) {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities.", new DoubleMatrix()));
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("Vehicles", "The number of vehicles.", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("Capacity", "The capacity of each vehicle.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleArray>("Demand", "The demand of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ReadyTime", "The ready time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("DueTime", "The due time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ServiceTime", "The service time of each customer.", new DoubleArray()));
      Parameters.Add(new OptionalValueParameter<IVRPEncoding>("BestKnownSolution", "The best known solution of this VRP instance."));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalFleetUsageFactor", "The fleet usage factor considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalDistanceFactor", "The distance factor considered in the evaluation.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation.", new DoubleValue(100)));

      Maximization.Value = false;
      MaximizationParameter.Hidden = true;

      SolutionCreator.VRPToursParameter.ActualName = "VRPTours";
      Evaluator.QualityParameter.ActualName = "VRPQuality";

      InitializeRandomVRPInstance();

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VehicleRoutingProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      UpdateMoveEvaluators();
      ParameterizeAnalyzer();
    }
    private void VehiclesValue_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      ParameterizeSolutionCreator();
      ClearDistanceMatrix();

      BestKnownSolution = null;
    }
    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      ClearDistanceMatrix();

      BestKnownSolution = null;
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      ClearDistanceMatrix();

      BestKnownSolution = null;
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzer();
    }

    void DistanceFactor_ValueChanged(object sender, EventArgs e) {
      DistanceFactorParameter.Value.ValueChanged += new EventHandler(DistanceFactorValue_ValueChanged);
      EvalBestKnownSolution();
    }
    void DistanceFactorValue_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void FleetUsageFactor_ValueChanged(object sender, EventArgs e) {
      FleetUsageFactorParameter.Value.ValueChanged += new EventHandler(FleetUsageFactorValue_ValueChanged);
      EvalBestKnownSolution();
    }
    void FleetUsageFactorValue_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void OverloadPenalty_ValueChanged(object sender, EventArgs e) {
      OverloadPenaltyParameter.Value.ValueChanged += new EventHandler(OverloadPenaltyValue_ValueChanged);
      EvalBestKnownSolution();
    }
    void OverloadPenaltyValue_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void TardinessPenalty_ValueChanged(object sender, EventArgs e) {
      TardinessPenaltyParameter.Value.ValueChanged += new EventHandler(TardinessPenaltyValue_ValueChanged);
      EvalBestKnownSolution();
    }
    void TardinessPenaltyValue_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void TimeFactor_ValueChanged(object sender, EventArgs e) {
      TimeFactorParameter.Value.ValueChanged += new EventHandler(TimeFactorValue_ValueChanged);
      EvalBestKnownSolution();
    }
    void TimeFactorValue_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void DistanceMatrixParameter_ValueChanged(object sender, EventArgs e) {
      if (DistanceMatrix != null) {
        DistanceMatrix.ItemChanged += new EventHandler<EventArgs<int, int>>(DistanceMatrix_ItemChanged);
        DistanceMatrix.Reset += new EventHandler(DistanceMatrix_Reset);
      }
      EvalBestKnownSolution();
    }
    void DistanceMatrix_Reset(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void DistanceMatrix_ItemChanged(object sender, EventArgs<int, int> e) {
      EvalBestKnownSolution();
    }
    void UseDistanceMatrixParameter_ValueChanged(object sender, EventArgs e) {
      UseDistanceMatrix.ValueChanged += new EventHandler(UseDistanceMatrix_ValueChanged);
      EvalBestKnownSolution();
    }
    void UseDistanceMatrix_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void CapacityParameter_ValueChanged(object sender, EventArgs e) {
      Capacity.ValueChanged += new EventHandler(Capacity_ValueChanged);
      BestKnownSolution = null;
    }
    void Capacity_ValueChanged(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void DemandParameter_ValueChanged(object sender, EventArgs e) {
      Demand.ItemChanged += new EventHandler<EventArgs<int>>(Demand_ItemChanged);
      Demand.Reset += new EventHandler(Demand_Reset);
      BestKnownSolution = null;
    }
    void Demand_Reset(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void Demand_ItemChanged(object sender, EventArgs<int> e) {
      BestKnownSolution = null;
    }
    void DueTimeParameter_ValueChanged(object sender, EventArgs e) {
      DueTime.ItemChanged += new EventHandler<EventArgs<int>>(DueTime_ItemChanged);
      DueTime.Reset += new EventHandler(DueTime_Reset);
      BestKnownSolution = null;
    }
    void DueTime_Reset(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void DueTime_ItemChanged(object sender, EventArgs<int> e) {
      BestKnownSolution = null;
    }
    void ReadyTimeParameter_ValueChanged(object sender, EventArgs e) {
      ReadyTime.ItemChanged += new EventHandler<EventArgs<int>>(ReadyTime_ItemChanged);
      ReadyTime.Reset += new EventHandler(ReadyTime_Reset);
      BestKnownSolution = null;
    }
    void ReadyTime_Reset(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void ReadyTime_ItemChanged(object sender, EventArgs<int> e) {
      BestKnownSolution = null;
    }
    void ServiceTimeParameter_ValueChanged(object sender, EventArgs e) {
      ServiceTime.ItemChanged += new EventHandler<EventArgs<int>>(ServiceTime_ItemChanged);
      ServiceTime.Reset += new EventHandler(ServiceTime_Reset);
      BestKnownSolution = null;
    }
    void ServiceTime_Reset(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void ServiceTime_ItemChanged(object sender, EventArgs<int> e) {
      BestKnownSolution = null;
    }
    void VehiclesParameter_ValueChanged(object sender, EventArgs e) {
      Vehicles.ValueChanged += new EventHandler(Vehicles_ValueChanged);
      BestKnownSolution = null;
    }
    void Vehicles_ValueChanged(object sender, EventArgs e) {
      BestKnownSolution = null;
    }
    void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      #region Backwards Compatibility
      if (!Parameters.ContainsKey("BestKnownSolution")) {
        Parameters.Add(new OptionalValueParameter<IVRPEncoding>("BestKnownSolution", "The best known solution of this TSP instance."));
      }
      #endregion

      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);

      Vehicles.ValueChanged += new EventHandler(VehiclesValue_ValueChanged);

      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);

      DistanceFactorParameter.ValueChanged += new EventHandler(DistanceFactor_ValueChanged);
      DistanceFactorParameter.Value.ValueChanged += new EventHandler(DistanceFactorValue_ValueChanged);
      FleetUsageFactorParameter.ValueChanged += new EventHandler(FleetUsageFactor_ValueChanged);
      FleetUsageFactorParameter.Value.ValueChanged += new EventHandler(FleetUsageFactorValue_ValueChanged);
      OverloadPenaltyParameter.ValueChanged += new EventHandler(OverloadPenalty_ValueChanged);
      OverloadPenaltyParameter.Value.ValueChanged += new EventHandler(OverloadPenaltyValue_ValueChanged);
      TardinessPenaltyParameter.ValueChanged += new EventHandler(TardinessPenalty_ValueChanged);
      TardinessPenaltyParameter.Value.ValueChanged += new EventHandler(TardinessPenaltyValue_ValueChanged);
      TimeFactorParameter.ValueChanged += new EventHandler(TimeFactor_ValueChanged);
      TimeFactorParameter.Value.ValueChanged += new EventHandler(TimeFactorValue_ValueChanged);

      DistanceMatrixParameter.ValueChanged += new EventHandler(DistanceMatrixParameter_ValueChanged);
      UseDistanceMatrixParameter.ValueChanged += new EventHandler(UseDistanceMatrixParameter_ValueChanged);
      UseDistanceMatrix.ValueChanged += new EventHandler(UseDistanceMatrix_ValueChanged);

      CapacityParameter.ValueChanged += new EventHandler(CapacityParameter_ValueChanged);
      Capacity.ValueChanged += new EventHandler(Capacity_ValueChanged);
      DemandParameter.ValueChanged += new EventHandler(DemandParameter_ValueChanged);
      Demand.ItemChanged += new EventHandler<EventArgs<int>>(Demand_ItemChanged);
      Demand.Reset += new EventHandler(Demand_Reset);
      DueTimeParameter.ValueChanged += new EventHandler(DueTimeParameter_ValueChanged);
      DueTime.ItemChanged += new EventHandler<EventArgs<int>>(DueTime_ItemChanged);
      DueTime.Reset += new EventHandler(DueTime_Reset);
      ReadyTimeParameter.ValueChanged += new EventHandler(ReadyTimeParameter_ValueChanged);
      ReadyTime.ItemChanged += new EventHandler<EventArgs<int>>(ReadyTime_ItemChanged);
      ReadyTime.Reset += new EventHandler(ReadyTime_Reset);
      ServiceTimeParameter.ValueChanged += new EventHandler(ServiceTimeParameter_ValueChanged);
      ServiceTime.ItemChanged += new EventHandler<EventArgs<int>>(ServiceTime_ItemChanged);
      ServiceTime.Reset += new EventHandler(ServiceTime_Reset);
      VehiclesParameter.ValueChanged += new EventHandler(VehiclesParameter_ValueChanged);
      Vehicles.ValueChanged += new EventHandler(Vehicles_ValueChanged);

      BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
    }

    private void InitializeOperators() {
      Operators.Add(new BestVRPSolutionAnalyzer());
      Operators.Add(new BestAverageWorstVRPToursAnalyzer());
      ParameterizeAnalyzer();
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IVRPOperator>().Cast<IOperator>().OrderBy(op => op.Name));
      ParameterizeOperators();
      UpdateMoveEvaluators();
    }
    private void UpdateMoveEvaluators() {
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.VehiclesParameter.ActualName = VehiclesParameter.Name;
      SolutionCreator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      Evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
      Evaluator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
      SolutionCreator.CapacityParameter.ActualName = CapacityParameter.Name;
      SolutionCreator.DemandParameter.ActualName = DemandParameter.Name;
      SolutionCreator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
      SolutionCreator.DueTimeParameter.ActualName = DueTimeParameter.Name;
      SolutionCreator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      Evaluator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      Evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
      Evaluator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
      Evaluator.VehiclesParameter.ActualName = VehiclesParameter.Name;
      Evaluator.CapacityParameter.ActualName = CapacityParameter.Name;
      Evaluator.DemandParameter.ActualName = DemandParameter.Name;
      Evaluator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
      Evaluator.DueTimeParameter.ActualName = DueTimeParameter.Name;
      Evaluator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      Evaluator.FleetUsageFactor.ActualName = FleetUsageFactorParameter.Name;
      Evaluator.TimeFactor.ActualName = TimeFactorParameter.Name;
      Evaluator.DistanceFactor.ActualName = DistanceFactorParameter.Name;
      Evaluator.OverloadPenalty.ActualName = OverloadPenaltyParameter.Name;
      Evaluator.TardinessPenalty.ActualName = TardinessPenaltyParameter.Name;
    }
    private void ParameterizeAnalyzer() {
      BestVRPSolutionAnalyzer.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      BestVRPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      BestVRPSolutionAnalyzer.DistanceParameter.ActualName = Evaluator.DistanceParameter.ActualName;
      BestVRPSolutionAnalyzer.OverloadParameter.ActualName = Evaluator.OverloadParameter.ActualName;
      BestVRPSolutionAnalyzer.TardinessParameter.ActualName = Evaluator.TardinessParameter.ActualName;
      BestVRPSolutionAnalyzer.TravelTimeParameter.ActualName = Evaluator.TravelTimeParameter.ActualName;
      BestVRPSolutionAnalyzer.VehiclesUtilizedParameter.ActualName = Evaluator.VehcilesUtilizedParameter.ActualName;
      BestVRPSolutionAnalyzer.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      BestVRPSolutionAnalyzer.ResultsParameter.ActualName = "Results";

      BestAverageWorstVRPToursAnalyzer.DistanceParameter.ActualName = Evaluator.DistanceParameter.ActualName;
      BestAverageWorstVRPToursAnalyzer.OverloadParameter.ActualName = Evaluator.OverloadParameter.ActualName;
      BestAverageWorstVRPToursAnalyzer.TardinessParameter.ActualName = Evaluator.TardinessParameter.ActualName;
      BestAverageWorstVRPToursAnalyzer.TravelTimeParameter.ActualName = Evaluator.TravelTimeParameter.ActualName;
      BestAverageWorstVRPToursAnalyzer.VehiclesUtilizedParameter.ActualName = Evaluator.VehcilesUtilizedParameter.ActualName;
      BestAverageWorstVRPToursAnalyzer.ResultsParameter.ActualName = "Results";
    }
    private void ParameterizeOperators() {
      foreach (IVRPOperator op in Operators.OfType<IVRPOperator>()) {
        if (op.CoordinatesParameter != null) op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        if (op.DistanceMatrixParameter != null) op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        if (op.UseDistanceMatrixParameter != null) op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        if (op.VehiclesParameter != null) op.VehiclesParameter.ActualName = VehiclesParameter.Name;
        if (op.CapacityParameter != null) op.CapacityParameter.ActualName = CapacityParameter.Name;
        if (op.DemandParameter != null) op.DemandParameter.ActualName = DemandParameter.Name;
        if (op.ReadyTimeParameter != null) op.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        if (op.DueTimeParameter != null) op.DueTimeParameter.ActualName = DueTimeParameter.Name;
        if (op.ServiceTimeParameter != null) op.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      }

      foreach (IVRPMoveOperator op in Operators.OfType<IVRPMoveOperator>()) {
        op.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      }

      foreach (IPrinsOperator op in Operators.OfType<IPrinsOperator>()) {
        op.FleetUsageFactor.ActualName = FleetUsageFactorParameter.Name;
        op.TimeFactor.ActualName = TimeFactorParameter.Name;
        op.DistanceFactor.ActualName = DistanceFactorParameter.Name;
        op.OverloadPenalty.ActualName = OverloadPenaltyParameter.Name;
        op.TardinessPenalty.ActualName = TardinessPenaltyParameter.Name;
      }

      foreach (IVRPMoveEvaluator op in Operators.OfType<IVRPMoveEvaluator>()) {
        op.FleetUsageFactor.ActualName = FleetUsageFactorParameter.Name;
        op.TimeFactor.ActualName = TimeFactorParameter.Name;
        op.DistanceFactor.ActualName = DistanceFactorParameter.Name;
        op.OverloadPenalty.ActualName = OverloadPenaltyParameter.Name;
        op.TardinessPenalty.ActualName = TardinessPenaltyParameter.Name;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      }
      string translocationMove = Operators.OfType<IMoveGenerator>().OfType<IAlbaTranslocationMoveOperator>().First().TranslocationMoveParameter.ActualName;
      foreach (IAlbaTranslocationMoveOperator op in Operators.OfType<IAlbaTranslocationMoveOperator>())
        op.TranslocationMoveParameter.ActualName = translocationMove;

      foreach (IVRPCrossover op in Operators.OfType<IVRPCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      }

      foreach (IVRPManipulator op in Operators.OfType<IVRPManipulator>()) {
        op.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      }

      foreach (var op in Operators.OfType<IVRPMultiNeighborhoodShakingOperator>()) {
        op.VRPToursParameter.ActualName = SolutionCreator.VRPToursParameter.ActualName;
      }
    }
    private void ClearDistanceMatrix() {
      DistanceMatrixParameter.Value = null;
    }
    #endregion

    public void ImportFromSolomon(string solomonFileName) {
      SolomonParser parser = new SolomonParser(solomonFileName);
      parser.Parse();

      this.Name = parser.ProblemName;

      BestKnownSolution = null;
      Coordinates = new DoubleMatrix(parser.Coordinates);
      Vehicles.Value = parser.Vehicles;
      Capacity.Value = parser.Capacity;
      Demand = new DoubleArray(parser.Demands);
      ReadyTime = new DoubleArray(parser.Readytimes);
      DueTime = new DoubleArray(parser.Duetimes);
      ServiceTime = new DoubleArray(parser.Servicetimes);

      OnReset();
    }

    public void ImportFromTSPLib(string tspFileName) {
      TSPLIBParser parser = new TSPLIBParser(tspFileName);
      parser.Parse();

      this.Name = parser.Name;
      int problemSize = parser.Demands.Length;

      BestKnownSolution = null;
      Coordinates = new DoubleMatrix(parser.Vertices);
      if (parser.Vehicles != -1)
        Vehicles.Value = parser.Vehicles;
      else
        Vehicles.Value = problemSize - 1;
      Capacity.Value = parser.Capacity;
      Demand = new DoubleArray(parser.Demands);
      ReadyTime = new DoubleArray(problemSize);
      DueTime = new DoubleArray(problemSize);
      ServiceTime = new DoubleArray(problemSize);

      for (int i = 0; i < problemSize; i++) {
        ReadyTime[i] = 0;
        DueTime[i] = int.MaxValue;
        ServiceTime[i] = 0;
      }

      if (parser.Distance != -1) {
        DueTime[0] = parser.Distance;
      }

      if (parser.Depot != 1)
        ErrorHandling.ShowErrorDialog(new Exception("Invalid depot specification"));

      if (parser.WeightType != TSPLIBParser.TSPLIBEdgeWeightType.EUC_2D)
        ErrorHandling.ShowErrorDialog(new Exception("Invalid weight type"));

      OnReset();
    }

    private void EvalBestKnownSolution() {
      if (BestKnownSolution != null) {
        //call evaluator
        IValueParameter<DoubleMatrix> distMatrix = new ValueLookupParameter<DoubleMatrix>("DistMatrix",
          DistanceMatrix);

        TourEvaluation eval = VRPEvaluator.Evaluate(
          BestKnownSolution,
          Vehicles,
          DueTime,
          ServiceTime,
          ReadyTime,
          Demand,
          Capacity,
          FleetUsageFactorParameter.Value,
          TimeFactorParameter.Value,
          DistanceFactorParameter.Value,
          OverloadPenaltyParameter.Value,
          TardinessPenaltyParameter.Value,
          Coordinates,
          distMatrix,
          UseDistanceMatrix);

        DistanceMatrix = distMatrix.Value;

        BestKnownQuality = new DoubleValue(eval.Quality);
      } else {
        BestKnownQuality = null;
      }
    }

    public void ImportSolution(string solutionFileName) {
      SolutionParser parser = new SolutionParser(solutionFileName);
      parser.Parse();

      HeuristicLab.Problems.VehicleRouting.Encodings.Potvin.PotvinEncoding encoding = new Encodings.Potvin.PotvinEncoding();

      int cities = 0;
      foreach (List<int> route in parser.Routes) {
        Encodings.Tour tour = new Encodings.Tour();
        tour.Cities.AddRange(route);
        cities += tour.Cities.Count;

        encoding.Tours.Add(tour);
      }

      if (cities != Coordinates.Rows - 1)
        ErrorHandling.ShowErrorDialog(new Exception("The optimal solution does not seem to correspond  with the problem data."));
      else
        BestKnownSolutionParameter.Value = encoding;
    }

    public void ImportFromORLib(string orFileName) {
      ORLIBParser parser = new ORLIBParser(orFileName);
      parser.Parse();

      this.Name = parser.Name;
      int problemSize = parser.Demands.Length;

      BestKnownSolution = null;
      Coordinates = new DoubleMatrix(parser.Vertices);
      Vehicles.Value = problemSize - 1;
      Capacity.Value = parser.Capacity;
      Demand = new DoubleArray(parser.Demands);
      ReadyTime = new DoubleArray(problemSize);
      DueTime = new DoubleArray(problemSize);
      ServiceTime = new DoubleArray(problemSize);

      ReadyTime[0] = 0;
      DueTime[0] = parser.MaxRouteTime;
      ServiceTime[0] = 0;

      for (int i = 1; i < problemSize; i++) {
        ReadyTime[i] = 0;
        DueTime[i] = int.MaxValue;
        ServiceTime[i] = parser.ServiceTime;
      }

      OnReset();
    }

    private void InitializeRandomVRPInstance() {
      System.Random rand = new System.Random();

      int cities = 100;

      Coordinates = new DoubleMatrix(cities + 1, 2);
      Demand = new DoubleArray(cities + 1);
      DueTime = new DoubleArray(cities + 1);
      ReadyTime = new DoubleArray(cities + 1);
      ServiceTime = new DoubleArray(cities + 1);

      Vehicles.Value = 100;
      Capacity.Value = 200;

      for (int i = 0; i <= cities; i++) {
        Coordinates[i, 0] = rand.Next(0, 100);
        Coordinates[i, 1] = rand.Next(0, 100);

        if (i == 0) {
          Demand[i] = 0;
          DueTime[i] = Int16.MaxValue;
          ReadyTime[i] = 0;
          ServiceTime[i] = 0;
        } else {
          Demand[i] = rand.Next(10, 50);
          DueTime[i] = rand.Next((int)Math.Ceiling(VRPUtilities.CalculateDistance(0, i, Coordinates)), 1200);
          ReadyTime[i] = DueTime[i] - rand.Next(0, 100);
          ServiceTime[i] = 90;
        }
      }
    }
  }
}
