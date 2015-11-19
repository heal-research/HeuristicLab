#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.ArtificialAnt.Analyzers;

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("Artificial Ant Problem", "Represents the Artificial Ant problem.")]
  [StorableClass]
  public sealed class ArtificialAntProblem : SingleObjectiveHeuristicOptimizationProblem<Evaluator, ISymbolicExpressionTreeCreator>, IStorableContent {
    public string Filename { get; set; }

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
    public IValueParameter<ISymbolicExpressionGrammar> ArtificialAntExpressionGrammarParameter {
      get { return (IValueParameter<ISymbolicExpressionGrammar>)Parameters["ArtificialAntExpressionGrammar"]; }
    }
    public IValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumExpressionLength"]; }
    }
    public IValueParameter<IntValue> MaxExpressionDepthParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumExpressionDepth"]; }
    }
    public IValueParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumFunctionDefinitions"]; }
    }
    public IValueParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumFunctionArguments"]; }
    }
    public IValueParameter<BoolMatrix> WorldParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["World"]; }
    }
    public IValueParameter<IntValue> MaxTimeStepsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumTimeSteps"]; }
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
    public ArtificialAntExpressionGrammar ArtificialAntExpressionGrammar {
      get { return (ArtificialAntExpressionGrammar)ArtificialAntExpressionGrammarParameter.Value; }
    }
    public IEnumerable<IAntTrailAnalyzer> AntTrailAnalyzers {
      get { return Operators.OfType<IAntTrailAnalyzer>(); }
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
    private ArtificialAntProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private ArtificialAntProblem(ArtificialAntProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArtificialAntProblem(this, cloner);
    }
    public ArtificialAntProblem()
      : base(new Evaluator(), new ProbabilisticTreeCreator()) {
      BoolMatrix world = new BoolMatrix(santaFeAntTrail);
      Parameters.Add(new ValueParameter<IntValue>("MaximumExpressionLength", "Maximal length of the expression to control the artificial ant.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumExpressionDepth", "Maximal depth of the expression to control the artificial ant.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumFunctionDefinitions", "Maximal number of automatically defined functions in the expression to control the artificial ant.", new IntValue(3)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumFunctionArguments", "Maximal number of arguments of automatically defined functions in the expression to control the artificial ant.", new IntValue(3)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("ArtificialAntExpressionGrammar", "The grammar that should be used for artificial ant expressions.", new ArtificialAntExpressionGrammar()));
      Parameters.Add(new ValueParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items.", world));
      Parameters.Add(new ValueParameter<IntValue>("MaximumTimeSteps", "The number of time steps the artificial ant has available to collect all food items.", new IntValue(600)));

      Maximization.Value = true;
      MaximizationParameter.Hidden = true;
      BestKnownQuality = new DoubleValue(89);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolution";
      ((ProbabilisticTreeCreator)SolutionCreator).SymbolicExpressionTreeGrammarParameter.ActualName = "ArtificialAntExpressionGrammar";
      Evaluator.QualityParameter.ActualName = "FoodEaten";
      InitializeOperators();
      RegisterEventHandlers();

      ArtificialAntExpressionGrammar.MaximumFunctionDefinitions = MaxFunctionDefinitions.Value;
      ArtificialAntExpressionGrammar.MaximumFunctionArguments = MaxFunctionArguments.Value;
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolution";
      var grammarBased = SolutionCreator as ISymbolicExpressionTreeGrammarBasedOperator;
      if (grammarBased != null)
        grammarBased.SymbolicExpressionTreeGrammarParameter.ActualName = "ArtificialAntExpressionGrammar";

      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualName = "FoodEaten";
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    #endregion

    #region Helpers
    private void RegisterEventHandlers() {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(MaxFunctionArgumentsParameter_ValueChanged);
      MaxFunctionArguments.ValueChanged += new EventHandler(MaxFunctionArgumentsParameter_ValueChanged);
      MaxFunctionDefinitionsParameter.ValueChanged += new EventHandler(MaxFunctionDefinitionsParameter_ValueChanged);
      MaxFunctionDefinitions.ValueChanged += new EventHandler(MaxFunctionDefinitionsParameter_ValueChanged);
    }

    private void MaxFunctionDefinitionsParameter_ValueChanged(object sender, EventArgs e) {
      ArtificialAntExpressionGrammar.MaximumFunctionDefinitions = MaxFunctionDefinitions.Value;
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }
    private void MaxFunctionArgumentsParameter_ValueChanged(object sender, EventArgs e) {
      ArtificialAntExpressionGrammar.MaximumFunctionArguments = MaxFunctionArguments.Value;
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>().OfType<IOperator>());
      Operators.Add(new BestAntTrailAnalyzer());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      ParameterizeAnalyzers();
      ParameterizeOperators();
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
      var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);

      foreach (ISymbolicExpressionTreeGrammarBasedOperator op in operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>()) {
        op.SymbolicExpressionTreeGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
      }
      foreach (ISymbolicExpressionTreeSizeConstraintOperator op in operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        op.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaxExpressionLengthParameter.Name;

      }
      foreach (Evaluator op in operators.OfType<Evaluator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
        op.WorldParameter.ActualName = WorldParameter.Name;
      }
      foreach (ISymbolicExpressionTreeCrossover op in operators.OfType<ISymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeManipulator op in operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeArchitectureAlteringOperator op in operators.OfType<ISymbolicExpressionTreeArchitectureAlteringOperator>()) {
        op.MaximumFunctionDefinitionsParameter.ActualName = MaxFunctionDefinitionsParameter.Name;
        op.MaximumFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      }
    }
    #endregion
  }
}
