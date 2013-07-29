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

using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Trading {
  [Item("Sharpe Ratio Evaluator", "")]
  [StorableClass]
  public class SymbolicTradingSingleObjectiveSharpeRatioEvaluator : SymbolicTradingSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicTradingSingleObjectiveSharpeRatioEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicTradingSingleObjectiveSharpeRatioEvaluator(SymbolicTradingSingleObjectiveSharpeRatioEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicTradingSingleObjectiveSharpeRatioEvaluator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTradingSingleObjectiveSharpeRatioEvaluator(this, cloner);
    }

    public override bool Maximization { get { return true; } }

    public override IOperation Apply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue, rows);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.Apply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, ITradingProblemData problemData, IEnumerable<int> rows) {
      IEnumerable<double> signals = GetSignals(interpreter, solution, problemData.Dataset, rows);
      IEnumerable<double> returns = problemData.Dataset.GetEnumeratedVariableValues(problemData.PriceVariable, rows);
      OnlineCalculatorError errorState;
      double sharpRatio = OnlineSharpeRatioCalculator.Calculate(returns, signals, problemData.TransactionCosts, out errorState);
      if (errorState != OnlineCalculatorError.None) return 0.0;
      else return sharpRatio;
    }

    private static IEnumerable<double> GetSignals(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, Dataset dataset, IEnumerable<int> rows) {
      double signal = 0;
      double prevSignal = 0;
      foreach (var x in interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows)) {
        if (prevSignal == 1) {
          signal = x > -1 ? 1 : x > -2 ? 0 : -1; // 0, -1, -2
        } else if (prevSignal == 0) {
          signal = x > 1 ? 1 : x < -1 ? -1 : 0; // 1, 0 , -1
        } else if (prevSignal == -1) {
          signal = x < 1 ? -1 : x < 2 ? 0 : 1; // 2, 1, 0
        }
        yield return signal;
        prevSignal = signal;
      }
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, ITradingProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;

      double sharpRatio = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;

      return sharpRatio;
    }
  }
}
