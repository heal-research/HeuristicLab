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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LinearAssignment {
  /// <summary>
  /// A genetic algorithm.
  /// </summary>
  [Item("Hungarian Algorithm", "The Hungarian algorithm can be used to solve the linear assignment problem in O(n^3). It is also known as the Kuhn–Munkres algorithm or Munkres assignment algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class HungarianAlgorithm : EngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(LinearAssignmentProblem); }
    }
    public new LinearAssignmentProblem Problem {
      get { return (LinearAssignmentProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    #endregion

    #region Properties
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    private LinearAssignmentProblemSolver Solver {
      get { return (LinearAssignmentProblemSolver)OperatorGraph.InitialOperator; }
    }
    #endregion

    [StorableConstructor]
    private HungarianAlgorithm(bool deserializing) : base(deserializing) { }
    private HungarianAlgorithm(HungarianAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public HungarianAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the result.", new MultiAnalyzer()));

      var solver = new LinearAssignmentProblemSolver();
      OperatorGraph.InitialOperator = solver;

      var placeholder = new Placeholder();
      placeholder.Name = "(Analyzer)";
      placeholder.OperatorParameter.ActualName = AnalyzerParameter.Name;
      solver.Successor = placeholder;

      UpdateAnalyzers();
      RegisterEventHandlers();

      Problem = new LinearAssignmentProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HungarianAlgorithm(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      Problem.SolutionCreatorChanged += new EventHandler(Problem_SolutionCreatorChanged);
      Problem.EvaluatorChanged += new EventHandler(Problem_EvaluatorChanged);
      UpdateAnalyzers();
      Parameterize();
      base.OnProblemChanged();
    }

    private void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      Parameterize();
    }

    private void Problem_EvaluatorChanged(object sender, EventArgs e) {
      Parameterize();
    }

    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      if (Problem != null) {
        Problem.SolutionCreatorChanged += new EventHandler(Problem_SolutionCreatorChanged);
        Problem.EvaluatorChanged += new EventHandler(Problem_EvaluatorChanged);
      }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (var analyzer in Problem.OperatorsParameter.Value.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 0;
          Analyzer.Operators.Add(analyzer);
        }
      }
    }
    private void Parameterize() {
      if (Problem != null) {
        Solver.AssignmentParameter.ActualName = Problem.SolutionCreator.PermutationParameter.ActualName;
        Solver.CostsParameter.ActualName = Problem.CostsParameter.Name;
        Solver.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    #endregion
  }
}
