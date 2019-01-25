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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.LocalSearch {
  /// <summary>
  /// A local search improvement operator.
  /// </summary>
  [Item("LocalSearchImprovementOperator", "A local search improvement operator.")]
  [StorableType("40424217-DBA8-4787-A38A-2AE7130CB69D")]
  public sealed class LocalSearchImprovementOperator : SingleSuccessorOperator, ILocalImprovementAlgorithmOperator, IStochasticOperator, ISingleObjectiveOperator {
    #region IGenericLocalImprovementOperator Properties
    public Type ProblemType { get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); } }
    public IProblem Problem {
      get { return problem; }
      set {
        if (problem != value) {
          if (value != null && !(value is ISingleObjectiveHeuristicOptimizationProblem))
            throw new ArgumentException("Only problems of type " + ProblemType.ToString() + " can be assigned.");
          if (problem != null) DeregisterProblemEventHandlers();
          problem = (ISingleObjectiveHeuristicOptimizationProblem)value;
          if (problem != null) RegisterProblemEventHandlers();
          UpdateProblem();
        }
      }
    }
    #endregion

    [Storable]
    private ISingleObjectiveHeuristicOptimizationProblem problem;
    [Storable]
    private LocalSearchMainLoop loop;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    #region Parameter Properties
    public IConstrainedValueParameter<IMoveGenerator> MoveGeneratorParameter {
      get { return (IConstrainedValueParameter<IMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    public IConstrainedValueParameter<IMoveMaker> MoveMakerParameter {
      get { return (IConstrainedValueParameter<IMoveMaker>)Parameters["MoveMaker"]; }
    }
    public IConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter {
      get { return (IConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters["MoveEvaluator"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }
    public ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    #region ILocalImprovementOperator Parameters
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    #endregion
    #endregion

    #region Properties
    public IMoveGenerator MoveGenerator {
      get { return MoveGeneratorParameter.Value; }
      set { MoveGeneratorParameter.Value = value; }
    }
    public IMoveMaker MoveMaker {
      get { return MoveMakerParameter.Value; }
      set { MoveMakerParameter.Value = value; }
    }
    public ISingleObjectiveMoveEvaluator MoveEvaluator {
      get { return MoveEvaluatorParameter.Value; }
      set { MoveEvaluatorParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private LocalSearchImprovementOperator(StorableConstructorFlag _) : base(_) { }
    private LocalSearchImprovementOperator(LocalSearchImprovementOperator original, Cloner cloner)
      : base(original, cloner) {
      this.loop = cloner.Clone(original.loop);
      this.qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      this.problem = cloner.Clone(original.problem);
      RegisterEventHandlers();
    }
    public LocalSearchImprovementOperator()
      : base() {
      Parameters.Add(new ConstrainedValueParameter<IMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
      Parameters.Add(new ConstrainedValueParameter<IMoveMaker>("MoveMaker", "The operator used to perform a move."));
      Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>("MoveEvaluator", "The operator used to evaluate a move."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(150)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "Number of moves that MultiMoveGenerators should create. This is ignored for Exhaustive- and SingleMoveGenerators.", new IntValue(300)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated moves."));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the solution.", new MultiAnalyzer()));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The name of the collection where the results are stored."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality/fitness value of a solution."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));

      loop = new LocalSearchMainLoop();
      ((ResultsCollector)((SingleSuccessorOperator)loop.OperatorGraph.InitialOperator).Successor).CollectedValues.Remove(loop.BestLocalQualityParameter.Name);
      ParameterizeLSMainLoop();

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      Analyzer.Operators.Add(qualityAnalyzer);

      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalSearchImprovementOperator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    #region Event Handler Registration
    private void RegisterEventHandlers() {
      MoveGeneratorParameter.ValueChanged += new EventHandler(MoveGeneratorParameter_ValueChanged);
      if (problem != null)
        RegisterProblemEventHandlers();
    }

    private void RegisterProblemEventHandlers() {
      problem.Reset += new EventHandler(problem_Reset);
      problem.OperatorsChanged += new EventHandler(problem_OperatorsChanged);
    }

    private void DeregisterProblemEventHandlers() {
      problem.Reset -= new EventHandler(problem_Reset);
      problem.OperatorsChanged -= new EventHandler(problem_OperatorsChanged);
    }
    #endregion

    #region Event Handlers
    private void MoveGeneratorParameter_ValueChanged(object sender, EventArgs e) {
      ChooseMoveOperators();
      ParameterizeLSMainLoop();
    }

    private void problem_Reset(object sender, EventArgs e) {
      UpdateProblem();
    }

    private void problem_OperatorsChanged(object sender, EventArgs e) {
      UpdateProblem();
    }
    #endregion

    #region Parameterize and Update Methods
    private void UpdateProblem() {
      UpdateMoveOperators();
      ChooseMoveOperators();

      ParameterizeMoveGenerators();

      ParameterizeLSMainLoop();
      ParameterizeAnalyzers();
      UpdateAnalyzers();
    }

    private void ParameterizeLSMainLoop() {
      loop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      loop.BestLocalQualityParameter.ActualName = QualityParameter.Name;
      loop.EvaluatedMovesParameter.ActualName = EvaluatedSolutionsParameter.Name;
      loop.IterationsParameter.ActualName = "LocalIterations";
      loop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      loop.MoveEvaluatorParameter.ActualName = MoveEvaluatorParameter.Name;
      loop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
      loop.MoveMakerParameter.ActualName = MoveMakerParameter.Name;
      loop.QualityParameter.ActualName = QualityParameter.Name;
      loop.RandomParameter.ActualName = RandomParameter.Name;
      loop.ResultsParameter.ActualName = ResultsParameter.Name;

      if (problem != null) {
        loop.BestKnownQualityParameter.ActualName = problem.BestKnownQualityParameter.Name;
        loop.MaximizationParameter.ActualName = problem.MaximizationParameter.Name;
      }
      if (MoveEvaluator != null) {
        loop.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      }
    }

    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      if (problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = problem.MaximizationParameter.Name;
        if (MoveEvaluator != null)
          qualityAnalyzer.QualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = problem.BestKnownQualityParameter.Name;
      }
    }

    private bool IsSubclassOfGeneric(Type generic, Type toCheck) {
      while (toCheck != typeof(object)) {
        var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        if (generic == cur) {
          return true;
        }
        toCheck = toCheck.BaseType;
      }
      return false;
    }

    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (problem != null) {
        foreach (IAnalyzer analyzer in problem.Operators.OfType<IAnalyzer>()) {
          if (!IsSubclassOfGeneric(typeof(AlleleFrequencyAnalyzer<>), analyzer.GetType()) &&
              !(analyzer is PopulationSimilarityAnalyzer)) {
            IAnalyzer clone = analyzer.Clone() as IAnalyzer;
            foreach (IScopeTreeLookupParameter param in clone.Parameters.OfType<IScopeTreeLookupParameter>())
              param.Depth = 0;
            Analyzer.Operators.Add(clone, false);
          }
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer, false);
    }

    private void UpdateMoveOperators() {
      IMoveGenerator oldMoveGenerator = MoveGenerator;
      IMoveMaker oldMoveMaker = MoveMaker;
      ISingleObjectiveMoveEvaluator oldMoveEvaluator = MoveEvaluator;

      ClearMoveParameters();

      if (problem != null) {
        foreach (IMultiMoveGenerator generator in problem.Operators.OfType<IMultiMoveGenerator>().OrderBy(x => x.Name))
          MoveGeneratorParameter.ValidValues.Add(generator);
        foreach (IExhaustiveMoveGenerator generator in problem.Operators.OfType<IExhaustiveMoveGenerator>().OrderBy(x => x.Name))
          MoveGeneratorParameter.ValidValues.Add(generator);

        if (oldMoveGenerator != null) {
          IMoveGenerator newMoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
          if (newMoveGenerator != null) MoveGenerator = newMoveGenerator;
        }

        ChooseMoveOperators(oldMoveMaker, oldMoveEvaluator);
      }
    }

    private void ChooseMoveOperators(IMoveMaker oldMoveMaker = null, ISingleObjectiveMoveEvaluator oldMoveEvaluator = null) {
      if (oldMoveMaker == null) oldMoveMaker = MoveMaker;
      if (oldMoveEvaluator == null) oldMoveEvaluator = MoveEvaluator;
      MoveMakerParameter.ValidValues.Clear();
      MoveEvaluatorParameter.ValidValues.Clear();

      if (MoveGenerator != null && Problem != null) {
        IMoveGenerator generator = MoveGeneratorParameter.Value;
        foreach (IMoveMaker moveMaker in MoveHelper.GetCompatibleMoveMakers(generator, Problem.Operators.OfType<IOperator>()).OrderBy(x => x.Name))
          MoveMakerParameter.ValidValues.Add(moveMaker);
        foreach (ISingleObjectiveMoveEvaluator moveEvaluator in MoveHelper.GetCompatibleSingleObjectiveMoveEvaluators(generator, Problem.Operators.OfType<IOperator>()).OrderBy(x => x.Name))
          MoveEvaluatorParameter.ValidValues.Add(moveEvaluator);

        if (oldMoveMaker != null) {
          IMoveMaker mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
          if (mm != null) MoveMaker = mm;
        }
        if (oldMoveEvaluator != null) {
          ISingleObjectiveMoveEvaluator me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
          if (me != null) MoveEvaluator = me;
        }
      }
    }

    private void ClearMoveParameters() {
      MoveGeneratorParameter.ValidValues.Clear();
      MoveMakerParameter.ValidValues.Clear();
      MoveEvaluatorParameter.ValidValues.Clear();
    }

    private void ParameterizeMoveGenerators() {
      if (problem != null) {
        foreach (IMultiMoveGenerator generator in problem.Operators.OfType<IMultiMoveGenerator>())
          generator.SampleSizeParameter.ActualName = SampleSizeParameter.Name;
      }
    }
    #endregion

    public override IOperation Apply() {
      IScope currentScope = ExecutionContext.Scope;

      Scope localScope = new Scope();
      Scope individual = new Scope();

      foreach (IVariable var in currentScope.Variables)
        individual.Variables.Add(var); // add reference to variable otherwise the analyzer fails (it's looking down the tree)

      localScope.SubScopes.Add(individual);
      currentScope.SubScopes.Add(localScope);
      int index = currentScope.SubScopes.Count - 1;

      SubScopesProcessor processor = new SubScopesProcessor();
      SubScopesRemover remover = new SubScopesRemover();

      remover.RemoveAllSubScopes = false;
      remover.SubScopeIndexParameter.Value = new IntValue(index);

      if (index > 0) {
        EmptyOperator eo = new EmptyOperator();
        for (int i = 0; i < index - 1; i++) {
          processor.Operators.Add(eo);
        }
      }

      VariableCreator variableCreator = new VariableCreator();
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>(loop.IterationsParameter.ActualName, new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>(loop.BestLocalQualityParameter.ActualName, new DoubleValue(0)));

      variableCreator.Successor = loop;

      processor.Operators.Add(variableCreator);
      processor.Successor = remover;

      OperationCollection next = new OperationCollection(base.Apply());
      next.Insert(0, ExecutionContext.CreateChildOperation(processor));

      return next;
    }
  }
}
