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
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("TypeCoherentExpressionGrammar", "Represents a grammar for functional expressions in which special syntactic constraints are enforced so that boolean and real-valued expressions are not mixed.")]
  public class TypeCoherentExpressionGrammar : SymbolicExpressionGrammar, ISymbolicDataAnalysisGrammar {
    private const string ArithmeticFunctionsName = "Arithmetic Functions";
    private const string TrigonometricFunctionsName = "Trigonometric Functions";
    private const string ExponentialFunctionsName = "Exponential and Logarithmic Functions";
    private const string RealValuedSymbolsName = "Real Valued Symbols";
    private const string TerminalsName = "Terminals";
    private const string PowerFunctionsName = "Power Functions";
    private const string ConditionsName = "Conditions";
    private const string ComparisonsName = "Comparisons";
    private const string BooleanOperatorsName = "Boolean Operators";
    private const string ConditionalSymbolsName = "ConditionalSymbols";
    private const string TimeSeriesSymbolsName = "Time Series Symbols";

    [StorableConstructor]
    protected TypeCoherentExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected TypeCoherentExpressionGrammar(TypeCoherentExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public TypeCoherentExpressionGrammar()
      : base(ItemAttribute.GetName(typeof(TypeCoherentExpressionGrammar)), ItemAttribute.GetDescription(typeof(TypeCoherentExpressionGrammar))) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TypeCoherentExpressionGrammar(this, cloner);
    }

    private void Initialize() {
      #region symbol declaration
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
      var root = new Root();
      var exp = new Exponential();
      var @if = new IfThenElse();
      var gt = new GreaterThan();
      var lt = new LessThan();
      var and = new And();
      var or = new Or();
      var not = new Not();
      var variableCondition = new VariableCondition();

      var timeLag = new TimeLag();
      var integral = new Integral();
      var derivative = new Derivative();

      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new Variable();
      var laggedVariable = new LaggedVariable();
      #endregion

      #region group symbol declaration
      var arithmeticSymbols = new GroupSymbol(ArithmeticFunctionsName, new List<ISymbol>() { add, sub, mul, div, mean });
      var trigonometricSymbols = new GroupSymbol(TrigonometricFunctionsName, new List<ISymbol>() { sin, cos, tan });
      var exponentialAndLogarithmicSymbols = new GroupSymbol(ExponentialFunctionsName, new List<ISymbol> { exp, log });
      var terminalSymbols = new GroupSymbol(TerminalsName, new List<ISymbol> { constant, variableSymbol });
      var realValuedSymbols = new GroupSymbol(RealValuedSymbolsName, new List<ISymbol>() { arithmeticSymbols, trigonometricSymbols, exponentialAndLogarithmicSymbols, terminalSymbols });

      var powerSymbols = new GroupSymbol(PowerFunctionsName, new List<ISymbol> { pow, root });

      var conditionSymbols = new GroupSymbol(ConditionsName, new List<ISymbol> { @if, variableCondition });
      var comparisonSymbols = new GroupSymbol(ComparisonsName, new List<ISymbol> { gt, lt });
      var booleanOperationSymbols = new GroupSymbol(BooleanOperatorsName, new List<ISymbol> { and, or, not });
      var conditionalSymbols = new GroupSymbol(ConditionalSymbolsName, new List<ISymbol> { conditionSymbols, comparisonSymbols, booleanOperationSymbols });

      var timeSeriesSymbols = new GroupSymbol(TimeSeriesSymbolsName, new List<ISymbol> { timeLag, integral, derivative, laggedVariable });
      #endregion

      AddSymbol(realValuedSymbols);
      AddSymbol(powerSymbols);
      AddSymbol(conditionalSymbols);
      AddSymbol(timeSeriesSymbols);

      #region subtree count configuration
      SetSubtreeCount(arithmeticSymbols, 2, 2);
      SetSubtreeCount(trigonometricSymbols, 1, 1);
      SetSubtreeCount(powerSymbols, 2, 2);
      SetSubtreeCount(exponentialAndLogarithmicSymbols, 1, 1);
      SetSubtreeCount(terminalSymbols, 0, 0);

      SetSubtreeCount(@if, 3, 3);
      SetSubtreeCount(variableCondition, 2, 2);
      SetSubtreeCount(comparisonSymbols, 2, 2);
      SetSubtreeCount(and, 2, 2);
      SetSubtreeCount(or, 2, 2);
      SetSubtreeCount(not, 1, 1);

      SetSubtreeCount(timeLag, 1, 1);
      SetSubtreeCount(integral, 1, 1);
      SetSubtreeCount(derivative, 1, 1);
      SetSubtreeCount(laggedVariable, 0, 0);
      #endregion

      #region allowed child symbols configuration
      AddAllowedChildSymbol(StartSymbol, realValuedSymbols);
      AddAllowedChildSymbol(DefunSymbol, realValuedSymbols);

      AddAllowedChildSymbol(realValuedSymbols, realValuedSymbols);
      AddAllowedChildSymbol(realValuedSymbols, powerSymbols);
      AddAllowedChildSymbol(realValuedSymbols, conditionSymbols);
      AddAllowedChildSymbol(realValuedSymbols, timeSeriesSymbols);

      AddAllowedChildSymbol(powerSymbols, variableSymbol, 0);
      AddAllowedChildSymbol(powerSymbols, laggedVariable, 0);
      AddAllowedChildSymbol(powerSymbols, constant, 1);

      AddAllowedChildSymbol(@if, comparisonSymbols, 0);
      AddAllowedChildSymbol(@if, booleanOperationSymbols, 0);
      AddAllowedChildSymbol(@if, conditionSymbols, 1);
      AddAllowedChildSymbol(@if, realValuedSymbols, 1);
      AddAllowedChildSymbol(@if, powerSymbols, 1);
      AddAllowedChildSymbol(@if, timeSeriesSymbols, 1);
      AddAllowedChildSymbol(@if, conditionSymbols, 2);
      AddAllowedChildSymbol(@if, realValuedSymbols, 2);
      AddAllowedChildSymbol(@if, powerSymbols, 2);
      AddAllowedChildSymbol(@if, timeSeriesSymbols, 2);

      AddAllowedChildSymbol(booleanOperationSymbols, comparisonSymbols);
      AddAllowedChildSymbol(comparisonSymbols, realValuedSymbols);
      AddAllowedChildSymbol(comparisonSymbols, powerSymbols);
      AddAllowedChildSymbol(comparisonSymbols, conditionSymbols);
      AddAllowedChildSymbol(comparisonSymbols, timeSeriesSymbols);

      AddAllowedChildSymbol(variableCondition, realValuedSymbols);
      AddAllowedChildSymbol(variableCondition, powerSymbols);
      AddAllowedChildSymbol(variableCondition, conditionSymbols);
      AddAllowedChildSymbol(variableCondition, timeSeriesSymbols);


      AddAllowedChildSymbol(timeLag, realValuedSymbols);
      AddAllowedChildSymbol(timeLag, powerSymbols);
      AddAllowedChildSymbol(timeLag, conditionSymbols);

      AddAllowedChildSymbol(integral, realValuedSymbols);
      AddAllowedChildSymbol(integral, powerSymbols);
      AddAllowedChildSymbol(integral, conditionSymbols);

      AddAllowedChildSymbol(derivative, realValuedSymbols);
      AddAllowedChildSymbol(derivative, powerSymbols);
      AddAllowedChildSymbol(derivative, conditionSymbols);
      #endregion
    }

    public void ConfigureAsDefaultRegressionGrammar() {
      Symbols.Where(s => s is Average).First().Enabled = false;
      Symbols.Where(s => s.Name == TrigonometricFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == PowerFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == ConditionalSymbolsName).First().Enabled = false;
      Symbols.Where(s => s.Name == TimeSeriesSymbolsName).First().Enabled = false;
    }

    public void ConfigureAsDefaultClassificationGrammar() {
      Symbols.Where(s => s is Average).First().Enabled = false;
      Symbols.Where(s => s is VariableCondition).First().Enabled = false;
      Symbols.Where(s => s.Name == TrigonometricFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == ExponentialFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == PowerFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == TimeSeriesSymbolsName).First().Enabled = false;
    }

    public void ConfigureAsDefaultTimeSeriesPrognosisGrammar() {
      Symbols.Where(s => s is Variable).First().Enabled = false;
      Symbols.Where(s => s.Name == TrigonometricFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == PowerFunctionsName).First().Enabled = false;
      Symbols.Where(s => s.Name == ConditionalSymbolsName).First().Enabled = false;
    }
  }
}
