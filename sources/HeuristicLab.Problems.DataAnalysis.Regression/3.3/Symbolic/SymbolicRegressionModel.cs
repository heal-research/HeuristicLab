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
  public class SymbolicRegressionModel : NamedItem, IDataAnalysisModel {
    private SymbolicRegressionModel() : base() { } // for cloning
    [StorableConstructor]
    protected SymbolicRegressionModel(bool deserializing)
      : base(deserializing) {
    }
    public SymbolicRegressionModel(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree tree)
      : base() {
      this.tree = tree;
      this.interpreter = interpreter;
      this.inputVariables = tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(var => var.VariableName).Distinct().ToList();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
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
      return interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, Enumerable.Range(start, end - start));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      var clone = (SymbolicRegressionModel)base.Clone(cloner);
      clone.tree = (SymbolicExpressionTree)cloner.Clone(tree);
      clone.interpreter = (ISymbolicExpressionTreeInterpreter)cloner.Clone(interpreter);
      clone.inputVariables = new List<string>(inputVariables);
      return clone;
    }
  }
}
