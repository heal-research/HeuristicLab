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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("f4819c68-b6fc-469f-bcb5-cb5b2a9d8aff")]
  public abstract class SymbolicExpressionTreeMultiObjectiveProblem : MultiObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {
    [Storable] private ReferenceParameter<IntValue> TreeLengthRefParameter { get; set; }
    [Storable] private ReferenceParameter<IntValue> TreeDepthRefParameter { get; set; }
    [Storable] private ReferenceParameter<ISymbolicExpressionGrammar> GrammarRefParameter { get; set; }

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
    protected SymbolicExpressionTreeMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }


    // cloning
    protected SymbolicExpressionTreeMultiObjectiveProblem(SymbolicExpressionTreeMultiObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      TreeLengthRefParameter = cloner.Clone(original.TreeLengthRefParameter);
      TreeDepthRefParameter = cloner.Clone(original.TreeDepthRefParameter);
      GrammarRefParameter = cloner.Clone(original.GrammarRefParameter);
      RegisterEventHandlers();
    }

    protected SymbolicExpressionTreeMultiObjectiveProblem(SymbolicExpressionTreeEncoding encoding)
      : base(encoding) {
      EncodingParameter.ReadOnly = true;
      Parameters.Add(TreeLengthRefParameter = new ReferenceParameter<IntValue>("TreeLength", "The maximum amount of nodes.", Encoding.TreeLengthParameter));
      Parameters.Add(TreeDepthRefParameter = new ReferenceParameter<IntValue>("TreeDepth", "The maximum depth of the tree.", Encoding.TreeDepthParameter));
      Parameters.Add(GrammarRefParameter = new ReferenceParameter<ISymbolicExpressionGrammar>("Grammar", "The grammar that describes a valid tree.", Encoding.GrammarParameter));

      Parameterize();
      RegisterEventHandlers();
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[][] qualities, ResultCollection results,
      IRandom random) {
      base.Analyze(trees, qualities, results, random);

      var fronts = DominationCalculator.CalculateAllParetoFrontsIndices(trees, qualities, Maximization);
      var plot = new ParetoFrontScatterPlot<ISymbolicExpressionTree>(fronts, trees, qualities, Objectives, BestKnownFront);
      results.AddOrUpdateResult("Pareto Front Scatter Plot", plot);
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualitiesParameter.ActualName;
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
