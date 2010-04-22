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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  [Item("SymbolicRegressionModel", "A symbolic regression model represents an entity that provides estimated values based on input values.")]
  public class SymbolicRegressionModel : Item {
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

    public SymbolicRegressionModel() : base() { } // for cloning

    public SymbolicRegressionModel(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree tree, IEnumerable<string> inputVariables)
      : base() {
      this.tree = tree;
      this.interpreter = interpreter;
      this.inputVariables = inputVariables.ToList();
    }

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, int start, int end) {
      return interpreter.GetSymbolicExpressionTreeValues(tree, dataset, Enumerable.Range(start, end - start));
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
