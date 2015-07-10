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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  [Creatable("Problems")]
  [Item("Lawn Mower Problem", "The lawn mower demo problem for genetic programming.")]
  public class Problem : SingleObjectiveHeuristicOptimizationProblem<Evaluator, ISymbolicExpressionTreeCreator> {
    private const string LawnWidthParameterName = "LawnWidth";
    private const string LawnLengthParameterName = "LawnLength";
    private const string LawnMowerProgramParameterName = "Program";
    private const string MaxLawnMowerProgramLengthParameterName = "MaxProgramLength";
    private const string MaxLawnMowerProgramDepthParameterName = "MaxProgramDepth";
    private const string LawnMowerGrammarParameterName = "Grammar";
    private const string MaxFunctionDefinitionsParameterName = "MaxFunctionDefinitions";
    private const string MaxArgumentDefinitionsParameterName = "MaxArgumentDefinitions";

    public IFixedValueParameter<IntValue> LawnWidthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[LawnWidthParameterName]; }
    }
    public IFixedValueParameter<IntValue> LawnLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[LawnLengthParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxLawnMowerProgramLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxLawnMowerProgramLengthParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxLawnMowerProgramDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxLawnMowerProgramDepthParameterName]; }
    }
    public IValueParameter<Grammar> GrammarParameter {
      get { return (IValueParameter<Grammar>)Parameters[LawnMowerGrammarParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxFunctionDefinitionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxFunctionDefinitionsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxArgumentDefinitionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxArgumentDefinitionsParameterName]; }
    }

    [StorableConstructor]
    protected Problem(bool deserializing)
      : base(deserializing) {
    }
    protected Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public Problem()
      : base(new Evaluator(), new RampedHalfAndHalfTreeCreator()) {
      Parameters.Add(new FixedValueParameter<IntValue>(LawnWidthParameterName, "Width of the lawn.", new IntValue(8)));
      Parameters.Add(new FixedValueParameter<IntValue>(LawnLengthParameterName, "Length of the lawn.", new IntValue(8)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxLawnMowerProgramDepthParameterName, "Maximal depth of the lawn mower program.", new IntValue(13)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxLawnMowerProgramLengthParameterName, "Maximal length of the lawn mower program.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxFunctionDefinitionsParameterName, "Maximal number of automatically defined functions (ADF).", new IntValue(3)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxArgumentDefinitionsParameterName, "Maximal number of automatically defined arguments.", new IntValue(3)));
      Parameters.Add(new ValueParameter<Grammar>(LawnMowerGrammarParameterName, "Grammar for the lawn mower program.",
                     new Grammar()));
      Maximization.Value = true;

      GrammarParameter.Value.MaximumFunctionDefinitions = MaxFunctionDefinitionsParameter.Value.Value;
      GrammarParameter.Value.MaximumFunctionArguments = MaxArgumentDefinitionsParameter.Value.Value;

      InitializeOperators();      
      RegisterEventHandlers();
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Problem(this, cloner);
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      Operators.Add(new BestSolutionAnalyzer());
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }


    private void RegisterEventHandlers() {
      Evaluator.QualityParameter.ActualNameChanged += QualityParameterOnActualNameChanged;
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged +=
        SymbolicExpressionTreeParameterOnActualNameChanged;
      MaxArgumentDefinitionsParameter.ValueChanged += ParameterizeGrammar;
      MaxFunctionDefinitionsParameter.ValueChanged += ParameterizeGrammar;
    }

    protected override void OnEvaluatorChanged() {
      Evaluator.LawnMowerProgramParameter.ActualName = LawnMowerProgramParameterName;
      Evaluator.LawnLengthParameter.ActualName = LawnLengthParameterName;
      Evaluator.LawnWidthParameter.ActualName = LawnWidthParameterName;
      Evaluator.QualityParameter.ActualNameChanged += QualityParameterOnActualNameChanged;
      ParameterizeAnalyzers();
      ParameterizeOperators();
      base.OnEvaluatorChanged();
    }

    protected override void OnSolutionCreatorChanged() {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualName = LawnMowerProgramParameterName;
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += SymbolicExpressionTreeParameterOnActualNameChanged;
      ParameterizeAnalyzers();
      ParameterizeOperators();
      base.OnSolutionCreatorChanged();
    }

    private void SymbolicExpressionTreeParameterOnActualNameChanged(object sender, EventArgs eventArgs) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void QualityParameterOnActualNameChanged(object sender, EventArgs eventArgs) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void ParameterizeGrammar(object sender, EventArgs eventArgs) {
      GrammarParameter.Value.MaximumFunctionArguments = MaxArgumentDefinitionsParameter.Value.Value;
      GrammarParameter.Value.MaximumFunctionDefinitions = MaxFunctionDefinitionsParameter.Value.Value;
    }

    private void ParameterizeAnalyzers() {
      var analyzers = Operators.OfType<IAnalyzer>();
      foreach (var o in analyzers.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        o.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var o in analyzers.OfType<BestSolutionAnalyzer>()) {
        o.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      }
    }

    private void ParameterizeOperators() {
      var operators = Parameters
        .OfType<IValueParameter>()
        .Select(p => p.Value)
        .OfType<IOperator>()
        .Union(Operators);
      foreach (var o in operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>()) {
        o.SymbolicExpressionTreeGrammarParameter.ActualName = LawnMowerGrammarParameterName;
      }
      foreach (var o in operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        o.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaxLawnMowerProgramDepthParameterName;
        o.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaxLawnMowerProgramLengthParameterName;
      }
      foreach (var op in operators.OfType<Evaluator>()) {
        op.LawnMowerProgramParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.LawnLengthParameter.ActualName = LawnLengthParameterName;
        op.LawnWidthParameter.ActualName = LawnWidthParameterName;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeCreator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeArchitectureAlteringOperator op in operators.OfType<ISymbolicExpressionTreeArchitectureAlteringOperator>()) {
        op.MaximumFunctionDefinitionsParameter.ActualName = MaxFunctionDefinitionsParameter.Name;
        op.MaximumFunctionArgumentsParameter.ActualName = MaxArgumentDefinitionsParameter.Name;
      }
    }
  }
}
