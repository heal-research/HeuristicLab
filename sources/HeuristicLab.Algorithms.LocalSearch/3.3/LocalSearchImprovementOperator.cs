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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Operators;
using HeuristicLab.Common;
using HeuristicLab.Parameters;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Analysis;

namespace HeuristicLab.Algorithms.LocalSearch {
  /// <summary>
  /// A local search improvement operator.
  /// </summary>
  [Item("LocalSearchImprovementOperator", "A local search improvement operator.")]
  [StorableClass]
  public class LocalSearchImprovementOperator: SingleSuccessorOperator, ILocalImprovementOperator {
    [Storable]
    private LocalSearchMainLoop loop;

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    
    private ConstrainedValueParameter<IMoveGenerator> MoveGeneratorParameter {
      get { return (ConstrainedValueParameter<IMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    private ConstrainedValueParameter<IMoveMaker> MoveMakerParameter {
      get { return (ConstrainedValueParameter<IMoveMaker>)Parameters["MoveMaker"]; }
    }
    private ConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter {
      get { return (ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters["MoveEvaluator"]; }
    }
    private ValueParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<IntValue> SampleSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }

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

    [StorableConstructor]
    protected LocalSearchImprovementOperator(bool deserializing) : base(deserializing) {}
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    protected LocalSearchImprovementOperator(LocalSearchImprovementOperator original, Cloner cloner)
      : base(original, cloner) {
        this.loop = cloner.Clone(original.loop);
        this.qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
        Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalSearchImprovementOperator(this, cloner);
    }
    public LocalSearchImprovementOperator()
      : base() {
        loop = new LocalSearchMainLoop();
        
        ResultsCollector rc = (loop.OperatorGraph.InitialOperator as SingleSuccessorOperator).Successor as ResultsCollector;
        rc.CollectedValues.Remove("BestLocalQuality");

        qualityAnalyzer = new BestAverageWorstQualityAnalyzer();

        Parameters.Add(new ConstrainedValueParameter<IMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
        Parameters.Add(new ConstrainedValueParameter<IMoveMaker>("MoveMaker", "The operator used to perform a move."));
        Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>("MoveEvaluator", "The operator used to evaluate a move."));
        Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(150)));
        Parameters.Add(new ValueParameter<IntValue>("SampleSize", "Number of moves that MultiMoveGenerators should create. This is ignored for Exhaustive- and SingleMoveGenerators.", new IntValue(1500)));
        Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated moves."));
        Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the solution.", new MultiAnalyzer()));

        Initialize();
    }

    private void Initialize() {
      MoveGeneratorParameter.ValueChanged += new EventHandler(MoveGeneratorParameter_ValueChanged);
    }

    public void OnProblemChanged(IProblem problem) {
      UpdateMoveOperators(problem);
      ChooseMoveOperators();

      ParameterizeMoveGenerators(problem as ISingleObjectiveProblem);
      ParameterizeMoveEvaluators(problem as ISingleObjectiveProblem);
      ParameterizeMoveMakers(problem as ISingleObjectiveProblem);

      ParameterizeAnalyzers(problem as ISingleObjectiveProblem);
      UpdateAnalyzers(problem as ISingleObjectiveProblem);
    }

    void ParameterizeAnalyzers(ISingleObjectiveProblem problem) {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      if (problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = problem.MaximizationParameter.Name;
        if (MoveEvaluator != null)
          qualityAnalyzer.QualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = problem.BestKnownQualityParameter.Name;
      }
    }

    void UpdateAnalyzers(ISingleObjectiveProblem problem) {
      Analyzer.Operators.Clear();
      if (problem != null) {
        foreach (IAnalyzer analyzer in problem.Operators.OfType<IAnalyzer>()) {
          IAnalyzer clone = analyzer.Clone() as IAnalyzer;
          foreach (IScopeTreeLookupParameter param in clone.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 0;
          Analyzer.Operators.Add(clone);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer);
    }

    void MoveGeneratorParameter_ValueChanged(object sender, EventArgs e) {
      ChooseMoveOperators();
    }

    private void UpdateMoveOperators(IProblem problem) {
      IMoveGenerator oldMoveGenerator = MoveGenerator;
      IMoveMaker oldMoveMaker = MoveMaker;
      ISingleObjectiveMoveEvaluator oldMoveEvaluator = MoveEvaluator;

      ClearMoveParameters();

      if (problem != null) {
        foreach (IMoveGenerator generator in problem.Operators.OfType<IMoveGenerator>().OrderBy(x => x.Name))
          MoveGeneratorParameter.ValidValues.Add(generator);

        foreach (IMoveMaker maker in problem.Operators.OfType<IMoveMaker>().OrderBy(x => x.Name))
          MoveMakerParameter.ValidValues.Add(maker);

        foreach (ISingleObjectiveMoveEvaluator evaluator in problem.Operators.OfType<ISingleObjectiveMoveEvaluator>().OrderBy(x => x.Name))
          MoveEvaluatorParameter.ValidValues.Add(evaluator);
      }

      if (oldMoveGenerator != null) {
        IMoveGenerator newMoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
        if (newMoveGenerator != null) MoveGenerator = newMoveGenerator;
      }
      if (MoveGenerator == null) {
        ClearMoveParameters();
      }

      if (oldMoveMaker != null) {
        IMoveMaker mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
        if (mm != null) MoveMaker = mm;
      }

      if (oldMoveEvaluator != null) {
        ISingleObjectiveMoveEvaluator me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
        if (me != null) MoveEvaluator = me;
      }
    }

    private void ChooseMoveOperators() {
      IMoveMaker oldMoveMaker = MoveMaker;
      ISingleObjectiveMoveEvaluator oldMoveEvaluator = MoveEvaluator;

      if (MoveGenerator != null) {
        List<Type> moveTypes = MoveGenerator.GetType().GetInterfaces().Where(x => typeof(IMoveOperator).IsAssignableFrom(x)).ToList();
        foreach (Type type in moveTypes.ToList()) {
          if (moveTypes.Any(t => t != type && type.IsAssignableFrom(t)))
            moveTypes.Remove(type);
        }
        List<IMoveMaker> validMoveMakers = new List<IMoveMaker>();
        List<ISingleObjectiveMoveEvaluator> validMoveEvaluators = new List<ISingleObjectiveMoveEvaluator>();

        foreach (Type type in moveTypes) {
          var moveMakers = MoveMakerParameter.ValidValues.Where(x => type.IsAssignableFrom(x.GetType())).OrderBy(x => x.Name);
          foreach (IMoveMaker moveMaker in moveMakers)
            validMoveMakers.Add(moveMaker);

          var moveEvaluators = MoveEvaluatorParameter.ValidValues.Where(x => type.IsAssignableFrom(x.GetType())).OrderBy(x => x.Name);
          foreach (ISingleObjectiveMoveEvaluator moveEvaluator in moveEvaluators)
            validMoveEvaluators.Add(moveEvaluator);
        }
        if (oldMoveMaker != null) {
          IMoveMaker mm = validMoveMakers.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
          if (mm != null) MoveMaker = mm;
          else MoveMaker = validMoveMakers.FirstOrDefault();
        } 

        if (oldMoveEvaluator != null) {
          ISingleObjectiveMoveEvaluator me = validMoveEvaluators.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
          if (me != null) MoveEvaluator = me;
          else MoveEvaluator = validMoveEvaluators.FirstOrDefault();
        } 
      }
    }

    private void ClearMoveParameters() {
      MoveGeneratorParameter.ValidValues.Clear();
      MoveMakerParameter.ValidValues.Clear();
      MoveEvaluatorParameter.ValidValues.Clear();
    }

    private void ParameterizeMoveGenerators(ISingleObjectiveProblem problem) {
      if (problem != null) {
        foreach (IMultiMoveGenerator generator in problem.Operators.OfType<IMultiMoveGenerator>())
          generator.SampleSizeParameter.ActualName = SampleSizeParameter.Name;
      }
    }
    private void ParameterizeMoveEvaluators(ISingleObjectiveProblem problem) {
      foreach (ISingleObjectiveMoveEvaluator op in problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private void ParameterizeMoveMakers(ISingleObjectiveProblem problem) {
      foreach (IMoveMaker op in problem.Operators.OfType<IMoveMaker>()) {
        op.QualityParameter.ActualName = problem.Evaluator.QualityParameter.ActualName;
        if (MoveEvaluator != null)
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      }
    }

    public override IOperation Apply() {
      Scope subScope = new Scope();
      Scope individual = new Scope();

      foreach (Variable var in ExecutionContext.Scope.Variables) {
        individual.Variables.Add(var);
      }
      subScope.SubScopes.Add(individual);

      ExecutionContext.Scope.SubScopes.Add(subScope);
      int index = subScope.Parent.SubScopes.IndexOf(subScope);

      SubScopesProcessor processor = new SubScopesProcessor();
      SubScopesRemover remover = new SubScopesRemover();

      remover.RemoveAllSubScopes = false;
      remover.SubScopeIndexParameter.Value = new IntValue(index);

      for (int i = 0; i < index; i++) {
        processor.Operators.Add(new EmptyOperator());
      }

      VariableCreator variableCreator = new VariableCreator();
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("LocalIterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestLocalQuality", new DoubleValue(0)));

      variableCreator.Successor = loop;

      loop.EvaluatedMovesParameter.ActualName = EvaluatedSolutionsParameter.ActualName;

      processor.Operators.Add(variableCreator);
      processor.Successor = remover;

      IOperation next = base.Apply();
      if (next as ExecutionContext != null) {
        remover.Successor = (next as ExecutionContext).Operator;
      }

      return ExecutionContext.CreateChildOperation(processor);
    }
  }
}
