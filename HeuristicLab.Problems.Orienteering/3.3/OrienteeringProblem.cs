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
using System.IO;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.Types;

namespace HeuristicLab.Problems.Orienteering {
  [Item("Orienteering Problem (OP)", "Represents a single-objective Orienteering Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 115)]
  [StorableType("0B8DB4A4-F183-4368-86C6-C51289B183D2")]
  public sealed class OrienteeringProblem
    : SingleObjectiveHeuristicOptimizationProblem<IOrienteeringEvaluator, IOrienteeringSolutionCreator>,
    IStorableContent, IProblemInstanceConsumer<OPData>, IProblemInstanceConsumer<TSPData>, IProblemInstanceConsumer<CVRPData> {

    public string Filename { get; set; }

    #region Parameter Properties
    public OptionalValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return (OptionalValueParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public IValueParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (IValueParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }

    public IFixedValueParameter<IntValue> StartingPointParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["StartingPoint"]; }
    }
    public IFixedValueParameter<IntValue> TerminalPointParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["TerminalPoint"]; }
    }
    public IFixedValueParameter<DoubleValue> MaximumDistanceParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["MaximumDistance"]; }
    }
    public IValueParameter<DoubleArray> ScoresParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Scores"]; }
    }
    public IFixedValueParameter<DoubleValue> PointVisitingCostsParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["PointVisitingCosts"]; }
    }

    public OptionalValueParameter<IntegerVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<IntegerVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public DoubleMatrix Coordinates {
      get { return CoordinatesParameter.Value; }
      set { CoordinatesParameter.Value = value; }
    }
    public DistanceMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.Value; }
      set { DistanceMatrixParameter.Value = value; }
    }
    public int StartingPoint {
      get { return StartingPointParameter.Value.Value; }
      set { StartingPointParameter.Value.Value = value; }
    }
    public int TerminalPoint {
      get { return TerminalPointParameter.Value.Value; }
      set { TerminalPointParameter.Value.Value = value; }
    }
    public double MaximumDistance {
      get { return MaximumDistanceParameter.Value.Value; }
      set { MaximumDistanceParameter.Value.Value = value; }
    }
    public DoubleArray Scores {
      get { return ScoresParameter.Value; }
      set { ScoresParameter.Value = value; }
    }
    public double PointVisitingCosts {
      get { return PointVisitingCostsParameter.Value.Value; }
      set { PointVisitingCostsParameter.Value.Value = value; }
    }
    public IntegerVector BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestOrienteeringSolutionAnalyzer BestOrienteeringSolutionAnalyser {
      get { return Operators.OfType<BestOrienteeringSolutionAnalyzer>().SingleOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private OrienteeringProblem(StorableConstructorFlag _) : base(_) {
    }
    private OrienteeringProblem(OrienteeringProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringProblem(this, cloner);
    }
    public OrienteeringProblem()
      : base(new OrienteeringEvaluator(), new GreedyOrienteeringTourCreator()) {
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the points."));
      Parameters.Add(new ValueParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the points."));
      Parameters.Add(new FixedValueParameter<IntValue>("StartingPoint", "Index of the starting point.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<IntValue>("TerminalPoint", "Index of the ending point.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("MaximumDistance", "The maximum distance constraint for a Orienteering solution."));
      Parameters.Add(new ValueParameter<DoubleArray>("Scores", "The scores of the points."));
      Parameters.Add(new FixedValueParameter<DoubleValue>("PointVisitingCosts", "The costs for visiting a point."));
      Parameters.Add(new OptionalValueParameter<IntegerVector>("BestKnownSolution", "The best known solution of this Orienteering instance."));

      Maximization.Value = true;
      MaximizationParameter.Hidden = true;

      SolutionCreator.IntegerVectorParameter.ActualName = "OrienteeringSolution";

      InitializeInitialOrienteeringInstance();

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      RegisterEventHandlers();
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.IntegerVectorParameter.ActualNameChanged += SolutionCreator_IntegerVectorParameter_ActualNameChanged;
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
    }
    private void SolutionCreator_IntegerVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      if (Coordinates != null) {
        Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(CoordinatesValue_ItemChanged);
        Coordinates.Reset += new EventHandler(CoordinatesValue_Reset);
      }
      ParameterizeSolutionCreator();
      UpdateDistanceMatrix();
      CheckStartingIndex();
      CheckTerminalIndex();
    }
    private void CoordinatesValue_ItemChanged(object sender, EventArgs<int, int> e) {
      UpdateDistanceMatrix();
      CheckStartingIndex();
      CheckTerminalIndex();
    }
    private void CoordinatesValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      UpdateDistanceMatrix();
      CheckStartingIndex();
      CheckTerminalIndex();
    }
    private void StartingPointParameterValue_ValueChanged(object sender, EventArgs e) {
      CheckStartingIndex();
    }

    private void TerminalPointParameterValue_ValueChanged(object sender, EventArgs e) {
      CheckTerminalIndex();
    }
    private void MaximumDistanceParameterValue_ValueChanged(object sender, EventArgs e) { }
    private void ScoresParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeSolutionCreator();

      ScoresParameter.Value.Reset += new EventHandler(ScoresValue_Reset);
    }
    private void ScoresValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    private void PointVisitingCostsParameterValue_ValueChanged(object sender, EventArgs e) { }

    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      if (BestKnownSolution == null)
        BestKnownQuality = null;
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      SolutionCreator.IntegerVectorParameter.ActualNameChanged += SolutionCreator_IntegerVectorParameter_ActualNameChanged;

      CoordinatesParameter.ValueChanged += CoordinatesParameter_ValueChanged;
      if (CoordinatesParameter.Value != null) {
        CoordinatesParameter.Value.ItemChanged += CoordinatesValue_ItemChanged;
        CoordinatesParameter.Value.Reset += CoordinatesValue_Reset;
      }

      StartingPointParameter.Value.ValueChanged += StartingPointParameterValue_ValueChanged;
      TerminalPointParameter.Value.ValueChanged += TerminalPointParameterValue_ValueChanged;
      MaximumDistanceParameter.Value.ValueChanged += MaximumDistanceParameterValue_ValueChanged;
      PointVisitingCostsParameter.Value.ValueChanged += PointVisitingCostsParameterValue_ValueChanged;

      ScoresParameter.ValueChanged += ScoresParameter_ValueChanged;
      ScoresParameter.Value.Reset += ScoresValue_Reset;

      BestKnownSolutionParameter.ValueChanged += BestKnownSolutionParameter_ValueChanged;
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
      SolutionCreator.ScoresParameter.ActualName = ScoresParameter.Name;
      SolutionCreator.MaximumDistanceParameter.ActualName = MaximumDistanceParameter.Name;
      SolutionCreator.StartingPointParameter.ActualName = StartingPointParameter.Name;
      SolutionCreator.TerminalPointParameter.ActualName = TerminalPointParameter.Name;
      SolutionCreator.PointVisitingCostsParameter.ActualName = PointVisitingCostsParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
      Evaluator.ScoresParameter.ActualName = ScoresParameter.Name;
      Evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
      Evaluator.MaximumDistanceParameter.ActualName = MaximumDistanceParameter.Name;
      Evaluator.PointVisitingCostsParameter.ActualName = PointVisitingCostsParameter.Name;
    }
    private void ParameterizeAnalyzer() {
      if (BestOrienteeringSolutionAnalyser != null) {
        BestOrienteeringSolutionAnalyser.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;

        BestOrienteeringSolutionAnalyser.IntegerVector.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        BestOrienteeringSolutionAnalyser.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        BestOrienteeringSolutionAnalyser.ScoresParameter.ActualName = ScoresParameter.Name;

        BestOrienteeringSolutionAnalyser.ResultsParameter.ActualName = "Results";
        BestOrienteeringSolutionAnalyser.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestOrienteeringSolutionAnalyser.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      }
    }
    private void InitializeOperators() {
      Operators.Add(new BestOrienteeringSolutionAnalyzer());
      ParameterizeAnalyzer();

      Operators.Add(new OrienteeringLocalImprovementOperator());
      Operators.Add(new OrienteeringShakingOperator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      ParameterizeOperators();
    }
    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<OrienteeringLocalImprovementOperator>()) {
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.ScoresParameter.ActualName = ScoresParameter.Name;
        op.MaximumDistanceParameter.ActualName = MaximumDistanceParameter.Name;
        op.StartingPointParameter.ActualName = StartingPointParameter.Name;
        op.TerminalPointParameter.ActualName = TerminalPointParameter.Name;
        op.PointVisitingCostsParameter.ActualName = PointVisitingCostsParameter.Name;
      }
      foreach (var op in Operators.OfType<OrienteeringShakingOperator>()) {
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.ScoresParameter.ActualName = ScoresParameter.Name;
        op.MaximumDistanceParameter.ActualName = MaximumDistanceParameter.Name;
        op.StartingPointParameter.ActualName = StartingPointParameter.Name;
        op.TerminalPointParameter.ActualName = TerminalPointParameter.Name;
        op.PointVisitingCostsParameter.ActualName = PointVisitingCostsParameter.Name;
      }
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = SolutionCreator.IntegerVectorParameter.ActualName;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }
    #endregion

    private DistanceMatrix CalculateDistanceMatrix(double[,] coordinates) {
      var distances = DistanceHelper.GetDistanceMatrix(DistanceMeasure.Euclidean, coordinates, null, coordinates.GetLength(0));

      return new DistanceMatrix(distances);
    }
    private void UpdateDistanceMatrix() {
      if (Coordinates == null) {
        DistanceMatrix = new DistanceMatrix(0, 0);
        return;
      }

      var coordinates = Coordinates;
      int dimension = coordinates.Rows;
      var distances = new double[dimension, dimension];
      for (int i = 0; i < dimension - 1; i++) {
        for (int j = i + 1; j < dimension; j++) {
          double x1 = coordinates[i, 0];
          double y1 = coordinates[i, 1];
          double x2 = coordinates[j, 0];
          double y2 = coordinates[j, 1];
          distances[i, j] = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
          distances[j, i] = distances[i, j];
        }
      }
      DistanceMatrix = new DistanceMatrix(distances);
    }
    private void CheckStartingIndex() {
      if (StartingPoint < 0) StartingPoint = 0;
      if (StartingPoint >= DistanceMatrix.Rows) StartingPoint = DistanceMatrix.Rows - 1;
    }
    private void CheckTerminalIndex() {
      if (TerminalPoint < 0) TerminalPoint = 0;
      if (TerminalPoint >= DistanceMatrix.Rows) TerminalPoint = DistanceMatrix.Rows - 1;
    }

    private void InitializeInitialOrienteeringInstance() {
      var coordinates = new double[21, 2] {
        {  4.60,  7.10 }, {  5.70, 11.40 }, {  4.40, 12.30 }, {  2.80, 14.30 }, {  3.20, 10.30 },
        {  3.50,  9.80 }, {  4.40,  8.40 }, {  7.80, 11.00 }, {  8.80,  9.80 }, {  7.70,  8.20 },
        {  6.30,  7.90 }, {  5.40,  8.20 }, {  5.80,  6.80 }, {  6.70,  5.80 }, { 13.80, 13.10 },
        { 14.10, 14.20 }, { 11.20, 13.60 }, {  9.70, 16.40 }, {  9.50, 18.80 }, {  4.70, 16.80 },
        {  5.00,  5.60 }
      };
      Coordinates = new DoubleMatrix(coordinates);
      DistanceMatrix = CalculateDistanceMatrix(coordinates);

      StartingPoint = 0;
      TerminalPoint = 20;
      MaximumDistance = 30;

      Scores = new DoubleArray(new double[21] { 0, 20, 20, 30, 15, 15, 10, 20, 20, 20, 15, 10, 10, 25, 40, 40, 30, 30, 50, 30, 0 });
    }

    #region Instance consuming
    public void Load(OPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;

      Coordinates = data.Coordinates != null ? new DoubleMatrix(data.Coordinates) : null;
      if (data.Distances != null)
        DistanceMatrix = new DistanceMatrix(data.Distances);
      else
        DistanceMatrix = new DistanceMatrix(data.GetDistanceMatrix());

      StartingPoint = data.StartingPoint;
      TerminalPoint = data.TerminalPoint;

      PointVisitingCosts = data.PointVisitingCosts;
      MaximumDistance = data.MaximumDistance;
      Scores = new DoubleArray(data.Scores);
    }

    public void Load(TSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;

      Coordinates = data.Coordinates != null ? new DoubleMatrix(data.Coordinates) : null;
      if (data.Distances != null)
        DistanceMatrix = new DistanceMatrix(data.Distances);
      else
        DistanceMatrix = new DistanceMatrix(data.GetDistanceMatrix());

      StartingPoint = 0; // First city is interpreted as start point
      TerminalPoint = data.Dimension - 1; // Last city is interpreted als end point

      PointVisitingCosts = 0;
      MaximumDistance = DistanceMatrix.Average() * 5.0; // distance from start to end first to last city is interpreted as maximum distance
      Scores = new DoubleArray(Enumerable.Repeat(1.0, data.Dimension).ToArray()); // all scores are 1
    }

    public void Load(CVRPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;

      Coordinates = data.Coordinates != null ? new DoubleMatrix(data.Coordinates) : null;
      DistanceMatrix = data.Distances != null
        ? new DistanceMatrix(data.Distances)
        : CalculateDistanceMatrix(data.Coordinates);

      StartingPoint = 0; // Depot is interpreted as start point
      TerminalPoint = 0; // Depot is interpreted als end point

      PointVisitingCosts = 0;
      MaximumDistance = data.Capacity * 2; // capacity is interpreted as max distance
      Scores = new DoubleArray(data.Demands); // demands are interpreted as scores
    }
    #endregion
  }
}