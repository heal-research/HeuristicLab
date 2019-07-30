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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("36A22322-0627-4E25-A468-F2A788AF6D46")]
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
    private const string SpecialFunctionsName = "Special Functions";
    private const string TimeSeriesSymbolsName = "Time Series Symbols";

    [StorableConstructor]
    protected TypeCoherentExpressionGrammar(StorableConstructorFlag _) : base(_) { }
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
      var square = new Square();
      var root = new Root();
      var sqrt = new SquareRoot();
      var cube = new Cube();
      var cubeRoot = new CubeRoot();
      var exp = new Exponential();
      var abs = new Absolute();

      var airyA = new AiryA();
      var airyB = new AiryB();
      var bessel = new Bessel();
      var cosineIntegral = new CosineIntegral();
      var dawson = new Dawson();
      var erf = new Erf();
      var expIntegralEi = new ExponentialIntegralEi();
      var fresnelCosineIntegral = new FresnelCosineIntegral();
      var fresnelSineIntegral = new FresnelSineIntegral();
      var gamma = new Gamma();
      var hypCosineIntegral = new HyperbolicCosineIntegral();
      var tanh = new HyperbolicTangent();
      var hypSineIntegral = new HyperbolicSineIntegral();
      var norm = new Norm();
      var psi = new Psi();
      var sineIntegral = new SineIntegral();
      var analyticalQuotient = new AnalyticQuotient();

      var @if = new IfThenElse();
      var gt = new GreaterThan();
      var lt = new LessThan();
      var and = new And();
      var or = new Or();
      var not = new Not();
      var xor = new Xor();
      var variableCondition = new VariableCondition();

      var timeLag = new TimeLag();
      var integral = new Integral();
      var derivative = new Derivative();

      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new Variable();
      var binFactorVariable = new BinaryFactorVariable();
      var factorVariable = new FactorVariable();
      var laggedVariable = new LaggedVariable();
      var autoregressiveVariable = new AutoregressiveTargetVariable();
      #endregion

      #region group symbol declaration
      var arithmeticSymbols = new GroupSymbol(ArithmeticFunctionsName, new List<ISymbol>() { add, sub, mul, div, mean });
      var trigonometricSymbols = new GroupSymbol(TrigonometricFunctionsName, new List<ISymbol>() { sin, cos, tan, tanh});
      var exponentialAndLogarithmicSymbols = new GroupSymbol(ExponentialFunctionsName, new List<ISymbol> { exp, log});
      var specialFunctions = new GroupSymbol(SpecialFunctionsName, new List<ISymbol> { abs, airyA, airyB, bessel, cosineIntegral, dawson, erf, expIntegralEi,
        fresnelCosineIntegral,fresnelSineIntegral,gamma,hypCosineIntegral,hypSineIntegral,norm, psi, sineIntegral, analyticalQuotient});
      var terminalSymbols = new GroupSymbol(TerminalsName, new List<ISymbol> { constant, variableSymbol, binFactorVariable, factorVariable });
      var realValuedSymbols = new GroupSymbol(RealValuedSymbolsName, new List<ISymbol>() { arithmeticSymbols, trigonometricSymbols, exponentialAndLogarithmicSymbols, specialFunctions, terminalSymbols });

      var powerSymbols = new GroupSymbol(PowerFunctionsName, new List<ISymbol> { square, pow, sqrt, root, cube, cubeRoot });

      var conditionSymbols = new GroupSymbol(ConditionsName, new List<ISymbol> { @if, variableCondition });
      var comparisonSymbols = new GroupSymbol(ComparisonsName, new List<ISymbol> { gt, lt });
      var booleanOperationSymbols = new GroupSymbol(BooleanOperatorsName, new List<ISymbol> { and, or, not, xor });
      var conditionalSymbols = new GroupSymbol(ConditionalSymbolsName, new List<ISymbol> { conditionSymbols, comparisonSymbols, booleanOperationSymbols });

      var timeSeriesSymbols = new GroupSymbol(TimeSeriesSymbolsName, new List<ISymbol> { timeLag, integral, derivative, laggedVariable, autoregressiveVariable });
      #endregion

      AddSymbol(realValuedSymbols);
      AddSymbol(powerSymbols);
      AddSymbol(conditionalSymbols);
      AddSymbol(timeSeriesSymbols);

      #region subtree count configuration
      SetSubtreeCount(arithmeticSymbols, 2, 2);
      SetSubtreeCount(trigonometricSymbols, 1, 1);
      SetSubtreeCount(pow, 2, 2);
      SetSubtreeCount(root, 2, 2);
      SetSubtreeCount(square, 1, 1);
      SetSubtreeCount(cube, 1, 1);
      SetSubtreeCount(sqrt, 1, 1);
      SetSubtreeCount(cubeRoot, 1, 1);
      SetSubtreeCount(exponentialAndLogarithmicSymbols, 1, 1);
      foreach(var sy in specialFunctions.Symbols.Except(new[] { analyticalQuotient})) {
        SetSubtreeCount(sy, 1, 1);
      }
      SetSubtreeCount(analyticalQuotient, 2, 2);

      SetSubtreeCount(terminalSymbols, 0, 0);

      SetSubtreeCount(@if, 3, 3);
      SetSubtreeCount(variableCondition, 2, 2);
      SetSubtreeCount(comparisonSymbols, 2, 2);
      SetSubtreeCount(and, 2, 2);
      SetSubtreeCount(or, 2, 2);
      SetSubtreeCount(not, 1, 1);
      SetSubtreeCount(xor, 2, 2);

      SetSubtreeCount(timeLag, 1, 1);
      SetSubtreeCount(integral, 1, 1);
      SetSubtreeCount(derivative, 1, 1);
      SetSubtreeCount(laggedVariable, 0, 0);
      SetSubtreeCount(autoregressiveVariable, 0, 0);
      #endregion

      #region allowed child symbols configuration
      AddAllowedChildSymbol(StartSymbol, realValuedSymbols);
      AddAllowedChildSymbol(StartSymbol, powerSymbols);
      AddAllowedChildSymbol(StartSymbol, conditionSymbols);
      AddAllowedChildSymbol(StartSymbol, timeSeriesSymbols);
      AddAllowedChildSymbol(StartSymbol, specialFunctions);

      AddAllowedChildSymbol(DefunSymbol, realValuedSymbols);
      AddAllowedChildSymbol(DefunSymbol, powerSymbols);
      AddAllowedChildSymbol(DefunSymbol, conditionSymbols);
      AddAllowedChildSymbol(DefunSymbol, timeSeriesSymbols);
      AddAllowedChildSymbol(DefunSymbol, specialFunctions);

      AddAllowedChildSymbol(realValuedSymbols, realValuedSymbols);
      AddAllowedChildSymbol(realValuedSymbols, powerSymbols);
      AddAllowedChildSymbol(realValuedSymbols, conditionSymbols);
      AddAllowedChildSymbol(realValuedSymbols, timeSeriesSymbols);
      AddAllowedChildSymbol(realValuedSymbols, specialFunctions);

      AddAllowedChildSymbol(powerSymbols, variableSymbol, 0);
      AddAllowedChildSymbol(powerSymbols, laggedVariable, 0);
      AddAllowedChildSymbol(powerSymbols, autoregressiveVariable, 0);
      AddAllowedChildSymbol(powerSymbols, constant, 1);

      AddAllowedChildSymbol(square, realValuedSymbols, 0);
      AddAllowedChildSymbol(square, conditionSymbols, 0);
      AddAllowedChildSymbol(square, timeSeriesSymbols, 0);

      AddAllowedChildSymbol(sqrt, realValuedSymbols, 0);
      AddAllowedChildSymbol(sqrt, conditionSymbols, 0);
      AddAllowedChildSymbol(sqrt, timeSeriesSymbols, 0);

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
      Symbols.First(s => s is Average).Enabled = false;
      Symbols.First(s => s is Absolute).Enabled = false;
      Symbols.First(s => s is HyperbolicTangent).Enabled = false;
      Symbols.First(s => s.Name == TrigonometricFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == PowerFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == SpecialFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == ConditionalSymbolsName).Enabled = false;
      Symbols.First(s => s.Name == TimeSeriesSymbolsName).Enabled = false;
    }

    public void ConfigureAsDefaultClassificationGrammar() {
      Symbols.First(s => s is Average).Enabled = false;
      Symbols.First(s => s is VariableCondition).Enabled = false;
      Symbols.First(s => s is Xor).Enabled = false;
      Symbols.First(s => s is Absolute).Enabled = false;
      Symbols.First(s => s is HyperbolicTangent).Enabled = false;
      Symbols.First(s => s.Name == TrigonometricFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == ExponentialFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == SpecialFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == PowerFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == TimeSeriesSymbolsName).Enabled = false;
    }

    public void ConfigureAsDefaultTimeSeriesPrognosisGrammar() {
      Symbols.First(s => s is Average).Enabled = false;
      Symbols.First(s => s is Absolute).Enabled = false;
      Symbols.First(s => s is HyperbolicTangent).Enabled = false;
      Symbols.First(s => s.Name == TrigonometricFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == PowerFunctionsName).Enabled = false;
      Symbols.First(s => s.Name == ConditionalSymbolsName).Enabled = false;
      Symbols.First(s => s.Name == SpecialFunctionsName).Enabled = false;

      Symbols.Single(s => s.Name == "Variable").Enabled = false;
      Symbols.First(s => s.Name == TimeSeriesSymbolsName).Enabled = true;
      Symbols.First(s => s is Derivative).Enabled = false;
      Symbols.First(s => s is Integral).Enabled = false;
      Symbols.First(s => s is TimeLag).Enabled = false;
    }
  }
}
