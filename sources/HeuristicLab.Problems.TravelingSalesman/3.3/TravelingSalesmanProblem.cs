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
using System.Drawing;
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

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("Traveling Salesman Problem", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class TravelingSalesmanProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
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
    public ValueParameter<IPermutationCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IPermutationCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<ITSPEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ITSPEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
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
    public DoubleMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.Value; }
      set { DistanceMatrixParameter.Value = value; }
    }
    public BoolValue UseDistanceMatrix {
      get { return UseDistanceMatrixParameter.Value; }
      set { UseDistanceMatrixParameter.Value = value; }
    }
    public IPermutationCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ITSPEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private List<IOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    private BestTSPSolutionAnalyzer BestTSPSolutionAnalyzer {
      get { return operators.OfType<BestTSPSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    public TravelingSalesmanProblem()
      : base() {
      RandomPermutationCreator creator = new RandomPermutationCreator();
      TSPRoundedEuclideanPathEvaluator evaluator = new TSPRoundedEuclideanPathEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the Traveling Salesman Problem is a minimization problem.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IPermutationCreator>("SolutionCreator", "The operator which should be used to create new TSP solutions.", creator));
      Parameters.Add(new ValueParameter<ITSPEvaluator>("Evaluator", "The operator which should be used to evaluate TSP solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this TSP instance."));
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution of this TSP instance."));

      Coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
      });

      creator.PermutationParameter.ActualName = "TSPTour";
      evaluator.QualityParameter.ActualName = "TSPTourLength";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }
    [StorableConstructor]
    private TravelingSalesmanProblem(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TravelingSalesmanProblem clone = (TravelingSalesmanProblem)base.Clone(cloner);
      clone.DistanceMatrixParameter.Value = DistanceMatrixParameter.Value;
      clone.Initialize();
      return clone;
    }

    public void ImportFromTSPLIB(string tspFileName, string optimalTourFileName) {
      TSPLIBParser tspParser = new TSPLIBParser(tspFileName);
      tspParser.Parse();
      Name = tspParser.Name + " TSP (imported from TSPLIB)";
      if (!string.IsNullOrEmpty(tspParser.Comment)) Description = tspParser.Comment;
      Coordinates = new DoubleMatrix(tspParser.Vertices);
      if (tspParser.WeightType == TSPLIBParser.TSPLIBEdgeWeightType.EUC_2D) {
        TSPRoundedEuclideanPathEvaluator evaluator = new TSPRoundedEuclideanPathEvaluator();
        evaluator.QualityParameter.ActualName = "TSPTourLength";
        Evaluator = evaluator;
      } else if (tspParser.WeightType == TSPLIBParser.TSPLIBEdgeWeightType.GEO) {
        TSPGeoPathEvaluator evaluator = new TSPGeoPathEvaluator();
        evaluator.QualityParameter.ActualName = "TSPTourLength";
        Evaluator = evaluator;
      }
      BestKnownQuality = null;
      BestKnownSolution = null;

      if (!string.IsNullOrEmpty(optimalTourFileName)) {
        TSPLIBTourParser tourParser = new TSPLIBTourParser(optimalTourFileName);
        tourParser.Parse();
        if (tourParser.Tour.Length != Coordinates.Rows) throw new InvalidDataException("Length of optimal tour is not equal to number of cities.");
        BestKnownSolution = new Permutation(PermutationTypes.RelativeUndirected, tourParser.Tour);
      }
      OnReset();
    }
    public void ImportFromTSPLIB(string tspFileName, string optimalTourFileName, double bestKnownQuality) {
      ImportFromTSPLIB(tspFileName, optimalTourFileName);
      BestKnownQuality = new DoubleValue(bestKnownQuality);
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
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
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
      ClearDistanceMatrix();
      OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzer();
    }
    private void MoveGenerator_InversionMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<InversionMove>)sender).ActualName;
      foreach (IPermutationInversionMoveOperator op in Operators.OfType<IPermutationInversionMoveOperator>()) {
        op.InversionMoveParameter.ActualName = name;
      }
    }
    private void MoveGenerator_TranslocationMoveParameter_ActualNameChanged(object sender, EventArgs e) {
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
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.Add(new BestTSPSolutionAnalyzer());
      ParameterizeAnalyzer();
      operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>().Cast<IOperator>());
      ParameterizeOperators();
      UpdateMoveEvaluators();
      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (IPermutationInversionMoveOperator op in Operators.OfType<IPermutationInversionMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.InversionMoveParameter.ActualNameChanged += new EventHandler(MoveGenerator_InversionMoveParameter_ActualNameChanged);
        }
      }
      foreach (IPermutationTranslocationMoveOperator op in Operators.OfType<IPermutationTranslocationMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.TranslocationMoveParameter.ActualNameChanged += new EventHandler(MoveGenerator_TranslocationMoveParameter_ActualNameChanged);
        }
      }
    }
    private void UpdateMoveEvaluators() {
      foreach (ITSPPathMoveEvaluator op in Operators.OfType<ITSPPathMoveEvaluator>().ToList())
        operators.Remove(op);
      foreach (ITSPPathMoveEvaluator op in ApplicationManager.Manager.GetInstances<ITSPPathMoveEvaluator>())
        if (op.EvaluatorType == Evaluator.GetType()) {
          operators.Add(op);
        }
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntValue(Coordinates.Rows);
      SolutionCreator.PermutationTypeParameter.Value = new PermutationType(PermutationTypes.RelativeUndirected);
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is ITSPPathEvaluator)
        ((ITSPPathEvaluator)Evaluator).PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      if (Evaluator is ITSPCoordinatesPathEvaluator) {
        ITSPCoordinatesPathEvaluator evaluator = (ITSPCoordinatesPathEvaluator)Evaluator;
        evaluator.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        evaluator.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        evaluator.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
      }
    }
    private void ParameterizeAnalyzer() {
      BestTSPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      BestTSPSolutionAnalyzer.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
      BestTSPSolutionAnalyzer.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      BestTSPSolutionAnalyzer.ResultsParameter.ActualName = "Results";
    }
    private void ParameterizeOperators() {
      foreach (IPermutationCrossover op in Operators.OfType<IPermutationCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (IPermutationManipulator op in Operators.OfType<IPermutationManipulator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (IPermutationMoveOperator op in Operators.OfType<IPermutationMoveOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (ITSPPathMoveEvaluator op in Operators.OfType<ITSPPathMoveEvaluator>()) {
        op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      string inversionMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationInversionMoveOperator>().First().InversionMoveParameter.ActualName;
      foreach (IPermutationInversionMoveOperator op in Operators.OfType<IPermutationInversionMoveOperator>())
        op.InversionMoveParameter.ActualName = inversionMove;
      string translocationMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationTranslocationMoveOperator>().First().TranslocationMoveParameter.ActualName;
      foreach (IPermutationTranslocationMoveOperator op in Operators.OfType<IPermutationTranslocationMoveOperator>())
        op.TranslocationMoveParameter.ActualName = translocationMove;
    }

    private void ClearDistanceMatrix() {
      DistanceMatrixParameter.Value = null;
    }
    #endregion
  }
}
