#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("A1B9F4C8-5E29-493C-A483-2AC68453BC63")]
  public abstract class SymbolicExpressionTreeProblem : SingleObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {
    [Storable] protected ReferenceParameter<IntValue> TreeLengthRefParameter { get; private set; }
    [Storable] protected ReferenceParameter<IntValue> TreeDepthRefParameter { get; private set; }
    [Storable] protected ReferenceParameter<ISymbolicExpressionGrammar> GrammarRefParameter { get; private set; }

    public int TreeLength {
      get => TreeLengthRefParameter.Value.Value;
      set => TreeLengthRefParameter.Value.Value = value;
    }

    public int TreeDepth {
      get => TreeDepthRefParameter.Value.Value;
      set => TreeDepthRefParameter.Value.Value = value;
    }

    public ISymbolicExpressionGrammar Grammar {
      get => GrammarRefParameter.Value;
      set => GrammarRefParameter.Value = value;
    }

    // persistence
    [StorableConstructor]
    protected SymbolicExpressionTreeProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    // cloning
    protected SymbolicExpressionTreeProblem(SymbolicExpressionTreeProblem original, Cloner cloner)
      : base(original, cloner) {
      TreeLengthRefParameter = cloner.Clone(original.TreeLengthRefParameter);
      TreeDepthRefParameter = cloner.Clone(original.TreeDepthRefParameter);
      GrammarRefParameter = cloner.Clone(original.GrammarRefParameter);
      RegisterEventHandlers();
    }

    protected SymbolicExpressionTreeProblem() : this(new SymbolicExpressionTreeEncoding()) { }
    protected SymbolicExpressionTreeProblem(SymbolicExpressionTreeEncoding encoding)
      : base(encoding) {
      EncodingParameter.ReadOnly = true;
      EvaluatorParameter.ReadOnly = true;
      Parameters.Add(TreeLengthRefParameter = new ReferenceParameter<IntValue>("TreeLength", "The maximum amount of nodes.", Encoding.TreeLengthParameter));
      Parameters.Add(TreeDepthRefParameter = new ReferenceParameter<IntValue>("TreeDepth", "The maximum depth of the tree.", Encoding.TreeDepthParameter));
      Parameters.Add(GrammarRefParameter = new ReferenceParameter<ISymbolicExpressionGrammar>("Grammar", "The grammar that describes a valid tree.", Encoding.GrammarParameter));

      // TODO: These should be added in the SingleObjectiveProblem base class (if they were accessible from there)
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(ISingleObjectiveSolutionContext<ISymbolicExpressionTree>[] solutionContexts, IRandom random) {
      //TODO reimplement code below using results directly

      //if (!results.ContainsKey("Best Solution Quality")) {
      //  results.Add(new Result("Best Solution Quality", typeof(DoubleValue)));
      //}
      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof(ISymbolicExpressionTree)));
      //}

      //var bestQuality = Maximization ? qualities.Max() : qualities.Min();

      //if (results["Best Solution Quality"].Value == null ||
      //    IsBetter(bestQuality, ((DoubleValue)results["Best Solution Quality"].Value).Value)) {
      //  var bestIdx = Array.IndexOf(qualities, bestQuality);
      //  var bestClone = (IItem)trees[bestIdx].Clone();

      //  results["Best Solution"].Value = bestClone;
      //  results["Best Solution Quality"].Value = new DoubleValue(bestQuality);
      //}
    }

    protected sealed override void OnEvaluatorChanged() {
      throw new InvalidOperationException("Evaluator may not change!");
    }

    protected sealed override void OnEncodingChanged() {
      throw new InvalidOperationException("Encoding may not change!");
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      // TODO: this is done in base class as well (but operators are added at this level of the hierarchy)
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }
    private void RegisterEventHandlers() {
      IntValueParameterChangeHandler.Create(TreeLengthRefParameter, TreeLengthOnChanged);
      IntValueParameterChangeHandler.Create(TreeDepthRefParameter, TreeDepthOnChanged);
      ParameterChangeHandler<ISymbolicExpressionGrammar>.Create(GrammarRefParameter, GrammarOnChanged);
    }

    protected virtual void TreeLengthOnChanged() { }
    protected virtual void TreeDepthOnChanged() { }
    protected virtual void GrammarOnChanged() { }
  }
}
