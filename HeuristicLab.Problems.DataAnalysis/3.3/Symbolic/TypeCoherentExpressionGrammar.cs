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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("TypeCoherentExpressionGrammar", "Represents a grammar for functional expressions in which special syntactic constraints are enforced so that boolean and real-valued expressions are not mixed.")]
  public class TypeCoherentExpressionGrammar : DefaultSymbolicExpressionGrammar {

    [StorableConstructor]
    protected TypeCoherentExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected TypeCoherentExpressionGrammar(TypeCoherentExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public TypeCoherentExpressionGrammar()
      : base() {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TypeCoherentExpressionGrammar(this, cloner);
    }

    private void Initialize() {
      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var mean = new Average();
      var sin = new Sine();
      var cos = new Cosine();
      var tan = new Tangent();
      var log = new Logarithm();
      var pow = new Power();
      pow.InitialFrequency = 0.0;
      var root = new Root();
      root.InitialFrequency = 0.0;
      var exp = new Exponential();
      var @if = new IfThenElse();
      var gt = new GreaterThan();
      var lt = new LessThan();
      var and = new And();
      var or = new Or();
      var not = new Not();

      var timeLag = new TimeLag();
      timeLag.InitialFrequency = 0.0;
      var integral = new Integral();
      integral.InitialFrequency = 0.0;
      var derivative = new Derivative();
      derivative.InitialFrequency = 0.0;
      var variableCondition = new VariableCondition();
      variableCondition.InitialFrequency = 0.0;

      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable();
      var laggedVariable = new LaggedVariable();

      laggedVariable.InitialFrequency = 0.0;
      mean.InitialFrequency = 0.0;

      /*
       * Start = RealValueExpression
       * 
       * RealValueExpression = 
       *   "Variable"  |
       *   "Constant" | 
       *   BinaryOperator RealValueExpression RealValueExpression |
       *   UnaryOperator RealValueExpression | 
       *   "IF" BooleanExpression RealValueExpression RealValueExpression | 
       *   "VariableCondition" RealValueExpression RealValueExpression
       * 
       * BinaryOperator = 
       *   "+" | "-" | "*" | "/" | "Power"
       * 
       * UnaryOperator = 
       *   "Sin" | "Cos" | "Tan" | "Log" | "Exp"
       * 
       * BooleanExpression = 
       *   "AND" BooleanExpression BooleanExpression |
       *   "OR" BooleanExpression BooleanExpression |
       *   "NOT" BooleanExpression |
       *   ">" RealValueExpression RealValueExpression |
       *   "<" RealValueExpression RealValueExpression
       */

      var allSymbols = new List<Symbol>() { add, sub, mul, div, mean, sin, cos, tan, log, pow, root, exp, @if, gt, lt, and, or, not, timeLag, integral, derivative, constant, variableSymbol, laggedVariable, variableCondition };

      var unaryFunctionSymbols = new List<Symbol>() { sin, cos, tan, log, exp, timeLag, integral, derivative };
      var binaryFunctionSymbols = new List<Symbol>() { add, sub, mul, div, mean, pow, root, variableCondition };

      var unaryBooleanFunctionSymbols = new List<Symbol>() { not };
      var binaryBooleanFunctionSymbols = new List<Symbol>() { or, and };
      var relationalFunctionSymbols = new List<Symbol>() { gt, lt };
      var terminalSymbols = new List<Symbol>() { variableSymbol, constant, laggedVariable };
      var realValuedSymbols = unaryFunctionSymbols.Concat(binaryFunctionSymbols).Concat(terminalSymbols).Concat(new List<Symbol>() { @if });
      var booleanSymbols = unaryBooleanFunctionSymbols.Concat(binaryBooleanFunctionSymbols).Concat(relationalFunctionSymbols);

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      foreach (var unaryFun in unaryFunctionSymbols.Concat(unaryBooleanFunctionSymbols)) {
        SetMinSubtreeCount(unaryFun, 1);
        SetMaxSubtreeCount(unaryFun, 1);
      }
      foreach (var binaryFun in binaryFunctionSymbols.Concat(binaryBooleanFunctionSymbols).Concat(relationalFunctionSymbols)) {
        SetMinSubtreeCount(binaryFun, 2);
        SetMaxSubtreeCount(binaryFun, 2);
      }

      foreach (var terminalSymbol in terminalSymbols) {
        SetMinSubtreeCount(terminalSymbol, 0);
        SetMaxSubtreeCount(terminalSymbol, 0);
      }

      SetMinSubtreeCount(@if, 3);
      SetMaxSubtreeCount(@if, 3);


      // allow only real-valued expressions as child of the start symbol
      foreach (var symb in realValuedSymbols) {
        SetAllowedChild(StartSymbol, symb, 0);
      }

      foreach (var symb in unaryFunctionSymbols) {
        foreach (var childSymb in realValuedSymbols) {
          SetAllowedChild(symb, childSymb, 0);
        }
      }

      foreach (var symb in binaryFunctionSymbols) {
        foreach (var childSymb in realValuedSymbols) {
          SetAllowedChild(symb, childSymb, 0);
          SetAllowedChild(symb, childSymb, 1);
        }
      }

      foreach (var childSymb in booleanSymbols) {
        SetAllowedChild(@if, childSymb, 0);
      }
      foreach (var childSymb in realValuedSymbols) {
        SetAllowedChild(@if, childSymb, 1);
        SetAllowedChild(@if, childSymb, 2);
      }

      foreach (var symb in relationalFunctionSymbols) {
        foreach (var childSymb in realValuedSymbols) {
          SetAllowedChild(symb, childSymb, 0);
          SetAllowedChild(symb, childSymb, 1);
        }
      }
      foreach (var symb in binaryBooleanFunctionSymbols) {
        foreach (var childSymb in booleanSymbols) {
          SetAllowedChild(symb, childSymb, 0);
          SetAllowedChild(symb, childSymb, 1);
        }
      }
      foreach (var symb in unaryBooleanFunctionSymbols) {
        foreach (var childSymb in booleanSymbols) {
          SetAllowedChild(symb, childSymb, 0);
        }
      }
    }
  }
}
