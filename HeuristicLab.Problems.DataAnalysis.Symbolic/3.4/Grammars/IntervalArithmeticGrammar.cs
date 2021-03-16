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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("5A2300A0-D0FC-4F2D-B910-F86384FE9052")]
  [Item("IntervalArithmeticGrammar", "Represents a grammar for interval arithmetic which includes linear scaling parts implicitly.")]
  public class IntervalArithmeticGrammar : DataAnalysisGrammar, ISymbolicDataAnalysisGrammar {
    public IntervalArithmeticGrammar() : base(ItemAttribute.GetName(typeof(IntervalArithmeticGrammar)),
      ItemAttribute.GetDescription(typeof(IntervalArithmeticGrammar))) {
      Initialize();
    }

    [StorableConstructor]
    public IntervalArithmeticGrammar(StorableConstructorFlag _) : base(_) { }

    protected IntervalArithmeticGrammar(IntervalArithmeticGrammar original, Cloner cloner) : base(original, cloner) { }
    public IntervalArithmeticGrammar(string name, string description) : base(name, description) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntervalArithmeticGrammar(this, cloner);
    }

    private void Initialize() {
      #region Symbols

      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var sin = new Sine();
      var cos = new Cosine();
      var tan = new Tangent();
      var tanh = new HyperbolicTangent();
      var log = new Logarithm();
      var exp = new Exponential();
      var square = new Square();
      var sqrt = new SquareRoot();
      var cube = new Cube();
      var cbrt = new CubeRoot();
      var abs = new Absolute();
      var aq = new AnalyticQuotient();
      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new Variable();

      #endregion

      //Special symbols
      var offset = new Addition { Name = "Offset" };
      var scaling = new Multiplication { Name = "Scaling" };
      //all other symbols
      var allSymbols = new List<Symbol> {
        add, sub, mul, div, constant, variableSymbol, sin, cos, tan, log, square, sqrt, cube, cbrt, exp,
        tanh, aq, abs
      };

      var bivariateFuncs = new List<Symbol> { add, sub, mul, div, aq };
      var univariateFuncs = new List<Symbol> { sin, cos, tan, tanh, exp, log, abs, square, cube, sqrt, cbrt };
      var realValueSymbols = new List<Symbol> {
         add, sub, mul, div, sin, cos, tan, tanh, exp, log, aq, abs, square, cube, sqrt, cbrt,
         variableSymbol, constant,
        };


      //Add special symbols
      AddSymbol(offset);
      AddSymbol(scaling);
      //Add all other symbols
      foreach (var symb in allSymbols) AddSymbol(symb);

      #region define subtree count for special symbols

      foreach (var symb in bivariateFuncs) SetSubtreeCount(symb, 2, 2);

      foreach (var symb in univariateFuncs) SetSubtreeCount(symb, 1, 1);

      SetSubtreeCount(offset, 2, 2);
      SetSubtreeCount(scaling, 2, 2);

      #endregion

      #region child symbols config

      AddAllowedChildSymbol(StartSymbol, offset);

      //Define childs for offset
      AddAllowedChildSymbol(offset, scaling, 0);
      AddAllowedChildSymbol(offset, constant, 1);

      //Define childs for scaling
      foreach (var symb in allSymbols) AddAllowedChildSymbol(scaling, symb, 0);
      AddAllowedChildSymbol(scaling, constant, 1);

      //Define childs for realvalue symbols
      foreach (var symb in realValueSymbols) {
        foreach (var c in realValueSymbols) AddAllowedChildSymbol(symb, c);
      }

      #endregion

      Symbols.First(s => s is Cube).Enabled = false;
      Symbols.First(s => s is CubeRoot).Enabled = false;
      Symbols.First(s => s is Absolute).Enabled = false;
    }
  }
}