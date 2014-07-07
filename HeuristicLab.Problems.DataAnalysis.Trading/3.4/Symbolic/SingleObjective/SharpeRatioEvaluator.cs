#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  [Item("Sharpe Ratio Evaluator", "")]
  [StorableClass]
  public class SharpeRatioEvaluator : SingleObjectiveEvaluator {
    [StorableConstructor]
    protected SharpeRatioEvaluator(bool deserializing) : base(deserializing) { }
    protected SharpeRatioEvaluator(SharpeRatioEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SharpeRatioEvaluator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SharpeRatioEvaluator(this, cloner);
    }

    public override bool Maximization { get { return true; } }

    public override IOperation InstrumentedApply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue, rows);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, IProblemData problemData, IEnumerable<int> rows) {
      IEnumerable<double> signals = GetSignals(interpreter, solution, problemData.Dataset, rows);
      IEnumerable<double> returns = problemData.Dataset.GetDoubleValues(problemData.PriceChangeVariable, rows);
      OnlineCalculatorError errorState;
      double sharpRatio = OnlineSharpeRatioCalculator.Calculate(returns, signals, problemData.TransactionCosts, out errorState);
      if (errorState != OnlineCalculatorError.None) return 0.0;
      else return sharpRatio;
    }

    private static IEnumerable<double> GetSignals(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, Dataset dataset, IEnumerable<int> rows) {
      return Model.GetSignals(interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows));
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      double sharpRatio = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, problemData, rows);
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      return sharpRatio;
    }
  }
}
