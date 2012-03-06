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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("Traveling Salesman Problem", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class TravelingSalesmanProblem : SingleObjectiveHeuristicOptimizationProblem<ITSPEvaluator, IPermutationCreator>, IStorableContent,
    IProblemInstanceConsumer<TSPData> {
    private static readonly int DistanceMatrixSizeLimit = 1000;
    public string Filename { get; set; }

    #region Parameter Properties
    public ValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ValueParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public OptionalValueParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (OptionalValueParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ValueParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public OptionalValueParameter<Permutation> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<Permutation>)Parameters["BestKnownSolution"]; }
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
    public BoolValue UseDistanceMatrix {
      get { return UseDistanceMatrixParameter.Value; }
      set { UseDistanceMatrixParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestTSPSolutionAnalyzer BestTSPSolutionAnalyzer {
      get { return Operators.OfType<BestTSPSolutionAnalyzer>().FirstOrDefault(); }
    }
    private TSPAlleleFrequencyAnalyzer TSPAlleleFrequencyAnalyzer {
      get { return Operators.OfType<TSPAlleleFrequencyAnalyzer>().FirstOrDefault(); }
    }
    private TSPPopulationDiversityAnalyzer TSPPopulationDiversityAnalyzer {
      get { return Operators.OfType<TSPPopulationDiversityAnalyzer>().FirstOrDefault(); }
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
    private TravelingSalesmanProblem(bool deserializing) : base(deserializing) { }
    private TravelingSalesmanProblem(TravelingSalesmanProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TravelingSalesmanProblem(this, cloner);
    }
    public TravelingSalesmanProblem()
      : base(new TSPRoundedEuclideanPathEvaluator(), new RandomPermutationCreator()) {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new OptionalValueParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false.", new BoolValue(true)));
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution of this TSP instance."));

      Maximization.Value = false;
      MaximizationParameter.Hidden = true;
      DistanceMatrixParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;

      Coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
      });

      SolutionCreator.PermutationParameter.ActualName = "TSPTour";
      Evaluator.QualityParameter.ActualName = "TSPTourLength";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      RegisterEventHandlers();
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      UpdateMoveEvaluators();
      ParameterizeAnalyzers();
      ClearDistanceMatrix();
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
    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      OptionalValueParameter<DoubleMatrix> oldDistanceMatrixParameter = Parameters["DistanceMatrix"] as OptionalValueParameter<DoubleMatrix>;
      if (oldDistanceMatrixParameter != null) {
        Parameters.Remove(oldDistanceMatrixParameter);
        Parameters.Add(new OptionalValueParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
        DistanceMatrixParameter.GetsCollected = oldDistanceMatrixParameter.GetsCollected;
        DistanceMatrixParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;
        if (oldDistanceMatrixParameter.Value != null) {
          DoubleMatrix oldDM = oldDistanceMatrixParameter.Value;
          DistanceMatrix newDM = new DistanceMatrix(oldDM.Rows, oldDM.Columns, oldDM.ColumnNames, oldDM.RowNames);
          newDM.SortableView = oldDM.SortableView;
          for (int i = 0; i < newDM.Rows; i++)
            for (int j = 0; j < newDM.Columns; j++)
              newDM[i, j] = oldDM[i, j];
          DistanceMatrixParameter.Value = (DistanceMatrix)newDM.AsReadOnly();
        }
      }

      if (Operators.Count == 0) InitializeOperators();
      #endregion
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    private void InitializeOperators() {
      Operators.Add(new BestTSPSolutionAnalyzer());
      Operators.Add(new TSPAlleleFrequencyAnalyzer());
      Operators.Add(new TSPPopulationDiversityAnalyzer());
      ParameterizeAnalyzers();
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>().Cast<IOperator>());
      ParameterizeOperators();
      UpdateMoveEvaluators();
    }
    private void UpdateMoveEvaluators() {
      Operators.RemoveAll(x => x is ISingleObjectiveMoveEvaluator);
      foreach (ITSPPathMoveEvaluator op in ApplicationManager.Manager.GetInstances<ITSPPathMoveEvaluator>())
        if (op.EvaluatorType == Evaluator.GetType()) {
          Operators.Add(op);
        }
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntValue(Coordinates.Rows);
      SolutionCreator.LengthParameter.Hidden = true;
      SolutionCreator.PermutationTypeParameter.Value = new PermutationType(PermutationTypes.RelativeUndirected);
      SolutionCreator.PermutationTypeParameter.Hidden = true;
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is ITSPPathEvaluator) {
        ITSPPathEvaluator evaluator = (ITSPPathEvaluator)Evaluator;
        evaluator.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        evaluator.PermutationParameter.Hidden = true;
      }
      if (Evaluator is ITSPCoordinatesPathEvaluator) {
        ITSPCoordinatesPathEvaluator evaluator = (ITSPCoordinatesPathEvaluator)Evaluator;
        evaluator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        evaluator.CoordinatesParameter.Hidden = true;
        evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        evaluator.DistanceMatrixParameter.Hidden = true;
        evaluator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        evaluator.UseDistanceMatrixParameter.Hidden = true;
      }
      if (Evaluator is ITSPDistanceMatrixEvaluator) {
        var evaluator = (ITSPDistanceMatrixEvaluator)Evaluator;
        evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        evaluator.DistanceMatrixParameter.Hidden = true;
      }
    }
    private void ParameterizeAnalyzers() {
      if (BestTSPSolutionAnalyzer != null) {
        BestTSPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestTSPSolutionAnalyzer.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        BestTSPSolutionAnalyzer.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        BestTSPSolutionAnalyzer.ResultsParameter.ActualName = "Results";
        BestTSPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestTSPSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        BestTSPSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      }

      if (TSPAlleleFrequencyAnalyzer != null) {
        TSPAlleleFrequencyAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        TSPAlleleFrequencyAnalyzer.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        TSPAlleleFrequencyAnalyzer.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        TSPAlleleFrequencyAnalyzer.SolutionParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        TSPAlleleFrequencyAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        TSPAlleleFrequencyAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        TSPAlleleFrequencyAnalyzer.ResultsParameter.ActualName = "Results";
      }

      if (TSPPopulationDiversityAnalyzer != null) {
        TSPPopulationDiversityAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        TSPPopulationDiversityAnalyzer.SolutionParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        TSPPopulationDiversityAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        TSPPopulationDiversityAnalyzer.ResultsParameter.ActualName = "Results";
      }
    }
    private void ParameterizeOperators() {
      foreach (IPermutationCrossover op in Operators.OfType<IPermutationCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ParentsParameter.Hidden = true;
        op.ChildParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ChildParameter.Hidden = true;
      }
      foreach (IPermutationManipulator op in Operators.OfType<IPermutationManipulator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }
      foreach (IPermutationMoveOperator op in Operators.OfType<IPermutationMoveOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }
      foreach (ITSPPathMoveEvaluator op in Operators.OfType<ITSPPathMoveEvaluator>()) {
        op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        op.CoordinatesParameter.Hidden = true;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.DistanceMatrixParameter.Hidden = true;
        op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        op.UseDistanceMatrixParameter.Hidden = true;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }
      foreach (IPermutationMultiNeighborhoodShakingOperator op in Operators.OfType<IPermutationMultiNeighborhoodShakingOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }
    }

    private void ClearDistanceMatrix() {
      if (!(Evaluator is ITSPDistanceMatrixEvaluator))
        DistanceMatrixParameter.Value = null;
    }
    #endregion

    public void Load(TSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new System.IO.InvalidDataException("The given instance does not specify neither coordinates or distances!");
      if (data.Dimension > DistanceMatrixSizeLimit && (data.DistanceMeasure == TSPDistanceMeasure.Att
        || data.DistanceMeasure == TSPDistanceMeasure.Manhattan
        || data.DistanceMeasure == TSPDistanceMeasure.Maximum
        || data.DistanceMeasure == TSPDistanceMeasure.UpperEuclidean))
        throw new System.IO.InvalidDataException("The given instance uses an unsupported distance measure and is too large for using a distance matrix.");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new System.IO.InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      Name = data.Name;
      Description = data.Description;

      if (data.Coordinates != null && data.Coordinates.GetLength(0) > 0)
        Coordinates = new DoubleMatrix(data.Coordinates);

      TSPEvaluator evaluator;
      if (data.DistanceMeasure == TSPDistanceMeasure.Att
        || data.DistanceMeasure == TSPDistanceMeasure.Manhattan
        || data.DistanceMeasure == TSPDistanceMeasure.Maximum
        || data.DistanceMeasure == TSPDistanceMeasure.UpperEuclidean) {
        evaluator = new TSPDistanceMatrixEvaluator();
        UseDistanceMatrix = new BoolValue(true);
        DistanceMatrix = new DistanceMatrix(data.GetDistanceMatrix());
      } else if (data.DistanceMeasure == TSPDistanceMeasure.Direct && data.Distances != null) {
        evaluator = new TSPDistanceMatrixEvaluator();
        UseDistanceMatrix = new BoolValue(true);
        DistanceMatrix = new DistanceMatrix(data.Distances);
      } else {
        DistanceMatrix = null;
        UseDistanceMatrix = new BoolValue(data.Dimension <= DistanceMatrixSizeLimit);
        switch (data.DistanceMeasure) {
          case TSPDistanceMeasure.Euclidean:
            evaluator = new TSPEuclideanPathEvaluator();
            break;
          case TSPDistanceMeasure.RoundedEuclidean:
            evaluator = new TSPRoundedEuclideanPathEvaluator();
            break;
          case TSPDistanceMeasure.Geo:
            evaluator = new TSPGeoPathEvaluator();
            break;
          default:
            throw new InvalidDataException("An unknown distance measure is given in the instance!");
        }
      }
      evaluator.QualityParameter.ActualName = "TSPTourLength";
      Evaluator = evaluator;

      BestKnownSolution = null;
      BestKnownQuality = null;

      if (data.BestKnownTour != null) {
        try {
          EvaluateAndLoadTour(data.BestKnownTour);
        } catch (InvalidOperationException) {
          if (data.BestKnownQuality.HasValue)
            BestKnownQuality = new DoubleValue(data.BestKnownQuality.Value);
        }
      } else if (data.BestKnownQuality.HasValue) {
        BestKnownQuality = new DoubleValue(data.BestKnownQuality.Value);
      }
      OnReset();
    }

    public void EvaluateAndLoadTour(int[] tour) {
      var route = new Permutation(PermutationTypes.RelativeUndirected, tour);
      BestKnownSolution = route;

      double quality;
      if (Evaluator is TSPDistanceMatrixEvaluator) {
        quality = TSPDistanceMatrixEvaluator.Apply(DistanceMatrix, route);
      } else if (Evaluator is TSPCoordinatesPathEvaluator) {
        quality = TSPCoordinatesPathEvaluator.Apply((TSPCoordinatesPathEvaluator)Evaluator, Coordinates, route);
      } else {
        throw new InvalidOperationException("Cannot calculate solution quality, evaluator type is unknown.");
      }
      BestKnownQuality = new DoubleValue(quality);
    }
  }
}
