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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  [Item("SymbolicRegressionModel", "A symbolic regression model represents an entity that provides estimated values based on input values.")]
  public sealed class SymbolicRegressionModel : NamedItem, IDataAnalysisModel {
    [StorableConstructor]
    private SymbolicRegressionModel(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionModel(SymbolicRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      tree = (SymbolicExpressionTree)cloner.Clone(original.tree);
      interpreter = (ISymbolicExpressionTreeInterpreter)cloner.Clone(original.interpreter);
      inputVariables = new List<string>(original.inputVariables);
    }

    public SymbolicRegressionModel(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree tree)
      : base() {
      this.tree = tree;
      this.interpreter = interpreter;
      this.inputVariables = tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(var => var.VariableName).Distinct().ToList();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionModel(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (inputVariables == null)
        this.inputVariables = tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(var => var.VariableName).Distinct().ToList();
    }

    [Storable]
    private SymbolicExpressionTree tree;
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return tree; }
    }
    [Storable]
    private ISymbolicExpressionTreeInterpreter interpreter;
    public ISymbolicExpressionTreeInterpreter Interpreter {
      get { return interpreter; }
    }
    [Storable]
    private List<string> inputVariables;
    public IEnumerable<string> InputVariables {
      get { return inputVariables.AsEnumerable(); }
    }

    public IEnumerable<double> GetEstimatedValues(DataAnalysisProblemData problemData, int start, int end) {
      return GetEstimatedValues(problemData, Enumerable.Range(start, end - start));
    }
    public IEnumerable<double> GetEstimatedValues(DataAnalysisProblemData problemData, IEnumerable<int> rows) {
      return interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
    }
  }
}
