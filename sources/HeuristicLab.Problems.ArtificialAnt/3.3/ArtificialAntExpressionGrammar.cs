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
namespace HeuristicLab.Problems.ArtificialAnt {
  [StorableClass]
  public class ArtificialAntExpressionGrammar : Item, ISymbolicExpressionGrammar {

    private class EmptySymbol : Symbol { }

    public ArtificialAntExpressionGrammar()
      : base() {
    }
    #region ISymbolicExpressionGrammar Members

    [Storable]
    private EmptySymbol startSymbol = new EmptySymbol();
    public Symbol StartSymbol {
      get { return startSymbol; }
    }

    [Storable]
    private static List<Symbol> allSymbols = new List<Symbol>() {
      new IfFoodAhead(),
      new Prog2(),
      new Prog3(),
      new Move(),
      new Left(),
      new Right()
    };
    [Storable]
    private Dictionary<Type, Dictionary<int, IEnumerable<Symbol>>> allowedSymbols = new Dictionary<Type, Dictionary<int, IEnumerable<Symbol>>>() {
      {
        typeof(EmptySymbol),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
        }
      },      {
        typeof(IfFoodAhead),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
      {
        typeof(Prog2),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols}
        }
      },
      {
        typeof(Prog3),
        new Dictionary<int, IEnumerable<Symbol>>() 
        {
          { 0, allSymbols},
          { 1, allSymbols},
          { 2, allSymbols}
        }
      },
    };
    public IEnumerable<Symbol> AllowedSymbols(Symbol parent, int argumentIndex) {
      return allowedSymbols[parent.GetType()][argumentIndex];
    }

    [Storable]
    private Dictionary<Type, int> minLength = new Dictionary<Type, int>() {
      {typeof(EmptySymbol), 1}, 
      {typeof(IfFoodAhead), 3},
      {typeof(Prog2), 3},
      {typeof(Prog3), 4},
      {typeof(Move), 1},
      {typeof(Left), 1},
      {typeof(Right), 1}
    };
    public int MinimalExpressionLength(Symbol start) {
      return minLength[start.GetType()];
    }

    [Storable]
    private Dictionary<Type, int> maxLength = new Dictionary<Type, int>() {
      {typeof(EmptySymbol), int.MaxValue}, 
      {typeof(IfFoodAhead), int.MaxValue},
      {typeof(Prog2), int.MaxValue},
      {typeof(Prog3), int.MaxValue},
      {typeof(Move), 1},
      {typeof(Left), 1},
      {typeof(Right), 1}
    };
    public int MaximalExpressionLength(Symbol start) {
      return maxLength[start.GetType()];
    }

    [Storable]
    private Dictionary<Type, int> minDepth = new Dictionary<Type, int>() {
      {typeof(EmptySymbol), 1}, 
      {typeof(IfFoodAhead), 1},
      {typeof(Prog2), 1},
      {typeof(Prog3), 1},
      {typeof(Move), 0},
      {typeof(Left), 0},
      {typeof(Right), 0}
    };
    public int MinimalExpressionDepth(Symbol start) {
      return minDepth[start.GetType()];
    }


    [Storable]
    private Dictionary<Type, int> subTrees = new Dictionary<Type, int>() {
      {typeof(EmptySymbol), 1}, 
      {typeof(IfFoodAhead), 2},
      {typeof(Prog2), 2},
      {typeof(Prog3), 3},
      {typeof(Move), 0},
      {typeof(Left), 0},
      {typeof(Right), 0}
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
