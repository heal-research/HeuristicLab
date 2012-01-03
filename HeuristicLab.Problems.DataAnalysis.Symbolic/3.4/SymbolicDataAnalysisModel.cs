#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for symbolic data analysis models
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisModel : NamedItem, ISymbolicDataAnalysisModel {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    #region properties

    [Storable]
    private ISymbolicExpressionTree symbolicExpressionTree;
    public ISymbolicExpressionTree SymbolicExpressionTree {
      get { return symbolicExpressionTree; }
    }

    [Storable]
    private ISymbolicDataAnalysisExpressionTreeInterpreter interpreter;
    public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter {
      get { return interpreter; }
    }

    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisModel(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisModel(SymbolicDataAnalysisModel original, Cloner cloner)
      : base(original, cloner) {
      this.symbolicExpressionTree = cloner.Clone(original.symbolicExpressionTree);
      this.interpreter = cloner.Clone(original.interpreter);
    }
    public SymbolicDataAnalysisModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.symbolicExpressionTree = tree;
      this.interpreter = interpreter;
    }
  }
}
