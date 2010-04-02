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


using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  public class ArithmeticExpressionGrammar : Item, ISymbolicExpressionGrammar {

    public ArithmeticExpressionGrammar()
      : base() {
    }
    #region ISymbolicExpressionGrammar Members
    [Storable]
    private StartSymbol startSymbol = new StartSymbol();
    public Symbol StartSymbol {
      get { return startSymbol; }
    }

    [Storable]
    private static List<Symbol> allSymbols = new List<Symbol>() {
      new Addition(),
      new Subtraction(),
      new Multiplication(),
      new Division(),
      new Constant(),
      new HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable()
    };
    [Storable]
    private Dictionary<Type, Dictionary<int, IEnumerable<Symbol>>> allowedSymbols = new Dictionary<Type, Dictionary<int, IEnumerable<Symbol>>>() {
      {
        typeof(StartSymbol),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
        }
      },      {
        typeof(Addition),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
      {
        typeof(Subtraction),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
      {
        typeof(Multiplication),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
      {
        typeof(Division),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
    };
    public IEnumerable<Symbol> AllowedSymbols(Symbol parent, int argumentIndex) {
      return allowedSymbols[parent.GetType()][argumentIndex];
    }

    [Storable]
    private Dictionary<Type, int> minLength = new Dictionary<Type, int>() {
      {typeof(StartSymbol), 1}, 
      {typeof(Addition), 3},
      {typeof(Subtraction), 3},
      {typeof(Multiplication), 4},
      {typeof(Division), 4},
      {typeof(Constant), 1},
      {typeof(HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable), 1},
    };
    public int MinimalExpressionLength(Symbol start) {
      return minLength[start.GetType()];
    }

    [Storable]
    private Dictionary<Type, int> maxLength = new Dictionary<Type, int>() {
      {typeof(StartSymbol), int.MaxValue}, 
      {typeof(Addition), int.MaxValue},
      {typeof(Subtraction), int.MaxValue},
      {typeof(Multiplication), int.MaxValue},
      {typeof(Division), int.MaxValue},
      {typeof(HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable), 1},
      {typeof(Constant), 1},
    };
    public int MaximalExpressionLength(Symbol start) {
      return maxLength[start.GetType()];
    }

    [Storable]
    private Dictionary<Type, int> minDepth = new Dictionary<Type, int>() {
      {typeof(StartSymbol), 1}, 
      {typeof(Addition), 1},
      {typeof(Subtraction), 1},
      {typeof(Multiplication), 1},
      {typeof(Division), 1},
      {typeof(Constant), 0},
      {typeof(HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable), 0}
    };
    public int MinimalExpressionDepth(Symbol start) {
      return minDepth[start.GetType()];
    }

    [Storable]
    private Dictionary<Type, int> subTrees = new Dictionary<Type, int>() {
      {typeof(StartSymbol), 1}, 
      {typeof(Addition), 2},
      {typeof(Subtraction), 2},
      {typeof(Multiplication), 2},
      {typeof(Division), 2},
      {typeof(HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable), 0},
      {typeof(Constant), 0},
    };
    public int MinSubTrees(Symbol start) {
      return subTrees[start.GetType()];
    }
    public int MaxSubTrees(Symbol start) {
      return subTrees[start.GetType()];
    }

    #endregion

    #region ISymbolicExpressionGrammar Members


    public bool IsValidExpression(SymbolicExpressionTree expression) {
      if (expression.Root.Symbol != StartSymbol) return false;
      return IsValidExpression(expression.Root);
    }

    #endregion

    private bool IsValidExpression(SymbolicExpressionTreeNode root) {
      if (root.SubTrees.Count < MinSubTrees(root.Symbol)) return false;
      if (root.SubTrees.Count > MaxSubTrees(root.Symbol)) return false;
      for (int i = 0; i < root.SubTrees.Count; i++) {
        if (!AllowedSymbols(root.Symbol, i).Contains(root.SubTrees[i].Symbol)) return false;
        if (!IsValidExpression(root.SubTrees[i])) return false;
      }
      return true;
    }
  }
}
