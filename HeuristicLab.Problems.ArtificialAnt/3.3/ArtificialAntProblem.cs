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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.ArtificialAnt.Analyzers;

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("Artificial Ant Problem", "Represents the Artificial Ant problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class ArtificialAntProblem : ParameterizedNamedItem, ISingleObjectiveHeuristicOptimizationProblem, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region constant for default world (Santa Fe)
    private readonly bool[,] santaFeAntTrail = new bool[,] {
      {false, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, false}, 
      {false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, true, false, false}, 
      {false, false, false, true, true, true, true, false, true, true, true, true, true, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, true, true, true, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false}, 
      {false, false, false, true, true, false, false, true, true, true, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, false, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}, 
      {false, false, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
      {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
    };
    #endregion

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<SymbolicExpressionTreeCreator> SolutionCreatorParameter {
      get { return (ValueParameter<SymbolicExpressionTreeCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<Evaluator> EvaluatorParameter {
      get { return (ValueParameter<Evaluator>)Parameters["Evaluator"]; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public ValueParameter<ISymbolicExpressionGrammar> ArtificialAntExpressionGrammarParameter {
      get { return (ValueParameter<ISymbolicExpressionGrammar>)Parameters["ArtificialAntExpressionGrammar"]; }
    }
    public ValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionLength"]; }
    }
    public ValueParameter<IntValue> MaxExpressionDepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionDepth"]; }
    }
    public ValueParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxFunctionDefinitions"]; }
    }
    public ValueParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxFunctionArguments"]; }
    }
    public ValueParameter<BoolMatrix> WorldParameter {
      get { return (ValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public ValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxTimeSteps"]; }
    }

    public ValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public BoolMatrix World {
      get { return WorldParameter.Value; }
      set { WorldParameter.Value = value; }
    }
    public IntValue MaxTimeSteps {
      get { return MaxTimeStepsParameter.Value; }
      set { MaxTimeStepsParameter.Value = value; }
    }
    public IntValue MaxExpressionLength {
      get { return MaxExpressionLengthParameter.Value; }
      set { MaxExpressionLengthParameter.Value = value; }
    }
    public IntValue MaxExpressionDepth {
      get { return MaxExpressionDepthParameter.Value; }
      set { MaxExpressionDepthParameter.Value = value; }
    }
    public IntValue MaxFunctionDefinitions {
      get { return MaxFunctionDefinitionsParameter.Value; }
      set { MaxFunctionDefinitionsParameter.Value = value; }
    }
    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.Value; }
      set { MaxFunctionArgumentsParameter.Value = value; }
    }
    public SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public Evaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public GlobalSymbolicExpressionGrammar ArtificialAntExpressionGrammar {
      get { return (GlobalSymbolicExpressionGrammar)ArtificialAntExpressionGrammarParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }

    public IEnumerable<IAntTrailAnalyzer> AntTrailAnalyzers {
      get { return operators.OfType<IAntTrailAnalyzer>(); }
    }
    #endregion

    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    private ArtificialAntProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (operators == null) InitializeOperators();
      #endregion
      AttachEventHandlers();
    }

    private ArtificialAntProblem(ArtificialAntProblem original, Cloner cloner)
      : base(original, cloner) {
      operators = original.operators.Select(x => cloner.Clone(x)).ToList();
      AttachEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArtificialAntProblem(this, cloner);
    }
    public ArtificialAntProblem()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      Evaluator evaluator = new Evaluator();
      BoolMatrix world = new BoolMatrix(santaFeAntTrail);
      ISymbolicExpressionGrammar grammar = new GlobalSymbolicExpressionGrammar(new ArtificialAntExpressionGrammar());
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the Artificial Ant Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new artificial ant solutions.", creator));
      Parameters.Add(new ValueParameter<Evaluator>("Evaluator", "The operator which should be used to evaluate artificial ant solutions.", evaluator));
      Parameters.Add(new ValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this artificial ant instance.", new DoubleValue(89)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the expression to control the artificial ant.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the expression to control the artificial ant.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionDefinitions", "Maximal number of automatically defined functions in the expression to control the artificial ant.", new IntValue(3)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionArguments", "Maximal number of arguments of automatically defined functions in the expression to control the artificial ant.", new IntValue(3)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("ArtificialAntExpressionGrammar", "The grammar that should be used for artificial ant expressions.", grammar));
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", world));
      Parameters.Add(new ValueParameter<IntValue>("MaxTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));

      creator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolution";
      evaluator.QualityParameter.ActualName = "FoodEaten";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      InitializeOperators();
      AttachEventHandlers();
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

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      OnEvaluatorChanged();
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
    }

    #endregion

    #region Helpers
    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(MaxFunctionArgumentsParameter_ValueChanged);
      MaxFunctionArguments.ValueChanged += new EventHandler(MaxFunctionArgumentsParameter_ValueChanged);
      MaxFunctionDefinitionsParameter.ValueChanged += new EventHandler(MaxFunctionDefinitionsParameter_ValueChanged);
      MaxFunctionDefinitions.ValueChanged += new EventHandler(MaxFunctionDefinitionsParameter_ValueChanged);
    }

    void MaxFunctionDefinitionsParameter_ValueChanged(object sender, EventArgs e) {
      ArtificialAntExpressionGrammar.MaxFunctionDefinitions = MaxFunctionDefinitions.Value;
    }

    void MaxFunctionArgumentsParameter_ValueChanged(object sender, EventArgs e) {
      ArtificialAntExpressionGrammar.MaxFunctionArguments = MaxFunctionArguments.Value;
    }

    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>().OfType<IOperator>());
      operators.Add(new BestAntTrailAnalyzer());
      operators.Add(new MinAverageMaxSymbolicExpressionTreeSizeAnalyzer());
      operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.SymbolicExpressionGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
      SolutionCreator.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
      SolutionCreator.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
      Evaluator.WorldParameter.ActualName = WorldParameter.Name;
    }
    private void ParameterizeAnalyzers() {
      foreach (IAntTrailAnalyzer analyzer in AntTrailAnalyzers) {
        analyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        analyzer.WorldParameter.ActualName = WorldParameter.Name;
        analyzer.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
      }
      foreach (ISymbolicExpressionTreeAnalyzer analyzer in Operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
    }

    private void ParameterizeOperators() {
      foreach (ISymbolicExpressionTreeOperator op in Operators.OfType<ISymbolicExpressionTreeOperator>()) {
        op.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
        op.SymbolicExpressionGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
      }
      foreach (Evaluator op in Operators.OfType<Evaluator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
        op.WorldParameter.ActualName = WorldParameter.Name;
      }
      foreach (ISymbolicExpressionTreeCrossover op in Operators.OfType<ISymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeManipulator op in Operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeArchitectureManipulator op in Operators.OfType<ISymbolicExpressionTreeArchitectureManipulator>()) {
        op.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefinitionsParameter.Name;
        op.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      }
      foreach (SymbolicExpressionTreeCreator op in Operators.OfType<SymbolicExpressionTreeCreator>()) {
        op.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
        op.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefinitionsParameter.Name;
      }
    }


    #endregion
  }
}
