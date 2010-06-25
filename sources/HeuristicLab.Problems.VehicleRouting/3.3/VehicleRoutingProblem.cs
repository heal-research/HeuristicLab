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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Common;
using System.Drawing;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("Vehicle Routing Problem", "Represents a Vehicle Routing Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class VehicleRoutingProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
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
    ValueParameter<IVRPCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IVRPCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    ValueParameter<IVRPEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IVRPEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public IValueParameter<DoubleValue> FleetUsageFactor {
      get { return (IValueParameter<DoubleValue>)Parameters["FleetUsageFactor"]; }
    }
    public IValueParameter<DoubleValue> TimeFactor {
      get { return (IValueParameter<DoubleValue>)Parameters["TimeFactor"]; }
    }
    public IValueParameter<DoubleValue> DistanceFactor {
      get { return (IValueParameter<DoubleValue>)Parameters["DistanceFactor"]; }
    }
    public IValueParameter<DoubleValue> OverloadPenalty {
      get { return (IValueParameter<DoubleValue>)Parameters["OverloadPenalty"]; }
    }
    public IValueParameter<DoubleValue> TardinessPenalty {
      get { return (IValueParameter<DoubleValue>)Parameters["TardinessPenalty"]; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
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
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    IVRPCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    IVRPEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    private List<IOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    private BestVRPSolutionAnalyzer BestVRPSolutionAnalyzer {
      get { return operators.OfType<BestVRPSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    public VehicleRoutingProblem()
      : base() {
      IVRPCreator creator = new AlbaPermutationCreator();
      IVRPEvaluator evaluator = new VRPEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the Vehicle Routing Problem is a minimization problem.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities.", new DoubleMatrix()));
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("Vehicles", "The number of vehicles.", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("Capacity", "The capacity of each vehicle.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleArray>("Demand", "The demand of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ReadyTime", "The ready time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("DueTime", "The due time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ServiceTime", "The service time of each customer.", new DoubleArray()));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this VRP instance."));
      Parameters.Add(new ValueParameter<DoubleValue>("FleetUsageFactor", "The fleet usage factor considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new ValueParameter<DoubleValue>("TimeFactor", "The time factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("DistanceFactor", "The distance factor considered in the evaluation.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<DoubleValue>("OverloadPenalty", "The overload penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new ValueParameter<DoubleValue>("TardinessPenalty", "The tardiness penalty considered in the evaluation.", new DoubleValue(100)));

      Parameters.Add(new ValueParameter<IVRPCreator>("SolutionCreator", "The operator which should be used to create new VRP solutions.", creator));
      Parameters.Add(new ValueParameter<IVRPEvaluator>("Evaluator", "The operator which should be used to evaluate VRP solutions.", evaluator));

      creator.VRPSolutionParameter.ActualName = "VRPSolution";
      evaluator.QualityParameter.ActualName = "VRPQuality";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }
    [StorableConstructor]
    private VehicleRoutingProblem(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      VehicleRoutingProblem clone = (VehicleRoutingProblem)base.Clone(cloner);
      clone.DistanceMatrixParameter.Value = DistanceMatrixParameter.Value;
      clone.Initialize();
      return clone;
    }

    public void ImportFromSolomon(string solomonFileName) {
      SolomonParser parser = new SolomonParser(solomonFileName);
      parser.Parse();

      this.Name = parser.ProblemName;

      Coordinates = new DoubleMatrix(parser.Coordinates);
      Vehicles.Value = parser.Vehicles;
      Capacity.Value = parser.Capacity;
      Demand = new DoubleArray(parser.Demands);
      ReadyTime = new DoubleArray(parser.Readytimes);
      DueTime = new DoubleArray(parser.Duetimes);
      ServiceTime = new DoubleArray(parser.Servicetimes);

      OnReset();
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    private void OnReset() {
      EventHandler handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    void VehiclesValue_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      ParameterizeSolutionCreator();
      ClearDistanceMatrix();
    }
    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      ClearDistanceMatrix();
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      ClearDistanceMatrix();
    }
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      UpdateMoveEvaluators();
      ParameterizeAnalyzer();
      //UpdateDistanceMatrix();
      OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzer();
    }
    void TranslocationMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<TranslocationMove>)sender).ActualName;
      foreach (IPermutationTranslocationMoveOperator op in Operators.OfType<IPermutationTranslocationMoveOperator>()) {
        op.TranslocationMoveParameter.ActualName = name;
      }
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeOperators();
      CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Vehicles.ValueChanged += new EventHandler(VehiclesValue_ValueChanged);
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.Add(new BestVRPSolutionAnalyzer());
      ParameterizeAnalyzer();
      operators.AddRange(ApplicationManager.Manager.GetInstances<IVRPOperator>().Cast<IOperator>());
      ParameterizeOperators();
      UpdateMoveEvaluators();
      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (AlbaTranslocationMoveGenerator op in Operators.OfType<AlbaTranslocationMoveGenerator>()) {
        if (op is IMoveGenerator) {
          op.TranslocationMoveParameter.ActualNameChanged += new EventHandler(TranslocationMoveParameter_ActualNameChanged);
        }
      }

    }
    private void UpdateMoveEvaluators() {
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.CitiesParameter.Value = new IntValue(Coordinates.Rows - 1);
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
      Evaluator.VRPSolutionParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
      Evaluator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      Evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
      Evaluator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
      Evaluator.VehiclesParameter.ActualName = VehiclesParameter.Name;
      Evaluator.CapacityParameter.ActualName = CapacityParameter.Name;
      Evaluator.DemandParameter.ActualName = DemandParameter.Name;
      Evaluator.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
      Evaluator.DueTimeParameter.ActualName = DueTimeParameter.Name;
      Evaluator.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
      Evaluator.FleetUsageFactor.ActualName = FleetUsageFactor.Name;
      Evaluator.TimeFactor.ActualName = TimeFactor.Name;
      Evaluator.DistanceFactor.ActualName = DistanceFactor.Name;
      Evaluator.OverloadPenalty.ActualName = OverloadPenalty.Name;
      Evaluator.TardinessPenalty.ActualName = TardinessPenalty.Name;
    }
    private void ParameterizeAnalyzer() {
      BestVRPSolutionAnalyzer.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      BestVRPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      BestVRPSolutionAnalyzer.DistanceParameter.ActualName = Evaluator.DistanceParameter.ActualName;
      BestVRPSolutionAnalyzer.OverloadParameter.ActualName = Evaluator.OverloadParameter.ActualName;
      BestVRPSolutionAnalyzer.TardinessParameter.ActualName = Evaluator.TardinessParameter.ActualName;
      BestVRPSolutionAnalyzer.VRPSolutionParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
      BestVRPSolutionAnalyzer.ResultsParameter.ActualName = "Results";
    }
    private void ParameterizeOperators() {
      foreach (IVRPMoveOperator op in Operators.OfType<IVRPMoveOperator>()) {
        op.VRPSolutionParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
      }
      foreach (IVRPMoveEvaluator op in Operators.OfType<IVRPMoveEvaluator>()) {
        op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.VRPSolutionParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
        op.VehiclesParameter.ActualName = VehiclesParameter.Name;
        op.CapacityParameter.ActualName = CapacityParameter.Name;
        op.DemandParameter.ActualName = DemandParameter.Name;
        op.ReadyTimeParameter.ActualName = ReadyTimeParameter.Name;
        op.DueTimeParameter.ActualName = DueTimeParameter.Name;
        op.ServiceTimeParameter.ActualName = ServiceTimeParameter.Name;
        op.FleetUsageFactor.ActualName = FleetUsageFactor.Name;
        op.TimeFactor.ActualName = TimeFactor.Name;
        op.DistanceFactor.ActualName = DistanceFactor.Name;
        op.OverloadPenalty.ActualName = OverloadPenalty.Name;
        op.TardinessPenalty.ActualName = TardinessPenalty.Name;
      }
      string translocationMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationTranslocationMoveOperator>().First().TranslocationMoveParameter.ActualName;
      foreach (IPermutationTranslocationMoveOperator op in Operators.OfType<IPermutationTranslocationMoveOperator>())
        op.TranslocationMoveParameter.ActualName = translocationMove;

      foreach (IVRPCrossover op in Operators.OfType<IVRPCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
      }
      foreach (IVRPManipulator op in Operators.OfType<IVRPManipulator>()) {
        op.VRPSolutionParameter.ActualName = SolutionCreator.VRPSolutionParameter.ActualName;
      }
    }
    private void ClearDistanceMatrix() {
      DistanceMatrixParameter.Value = null;
    }
    #endregion
  }
}
