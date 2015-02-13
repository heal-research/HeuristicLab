#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  /// <summary>
  /// Abstract base class for trading data analysis solutions
  /// </summary>
  [StorableClass]
  public abstract class Solution : DataAnalysisSolution, ISolution {
    private const string TrainingSharpeRatioResultName = "Sharpe ratio (training)";
    private const string TestSharpeRatioResultName = "Sharpe ratio (test)";
    private const string TrainingProfitResultName = "Profit (training)";
    private const string TestProfitResultName = "Profit (test)";

    public new IModel Model {
      get { return (IModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IProblemData ProblemData {
      get { return (IProblemData)base.ProblemData; }
      protected set { base.ProblemData = value; }
    }

    public double TrainingSharpeRatio {
      get { return ((DoubleValue)this[TrainingSharpeRatioResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingSharpeRatioResultName].Value).Value = value; }
    }

    public double TestSharpeRatio {
      get { return ((DoubleValue)this[TestSharpeRatioResultName].Value).Value; }
      private set { ((DoubleValue)this[TestSharpeRatioResultName].Value).Value = value; }
    }
    public double TrainingProfit {
      get { return ((DoubleValue)this[TrainingProfitResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingProfitResultName].Value).Value = value; }
    }

    public double TestProfit {
      get { return ((DoubleValue)this[TestProfitResultName].Value).Value; }
      private set { ((DoubleValue)this[TestProfitResultName].Value).Value = value; }
    }

    [StorableConstructor]
    protected Solution(bool deserializing) : base(deserializing) { }
    protected Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
    }
    public Solution(IModel model, IProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingSharpeRatioResultName, "Share ratio of the signals of the model on the training partition", new DoubleValue()));
      Add(new Result(TestSharpeRatioResultName, "Sharpe ratio of the signals of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingProfitResultName, "Profit of the model on the training partition", new DoubleValue()));
      Add(new Result(TestProfitResultName, "Profit of the model on the test partition", new DoubleValue()));
    }

    protected override void RecalculateResults() {
      CalculateTradingResults();
    }

    protected void CalculateTradingResults() {
      double[] trainingSignals = TrainingSignals.ToArray(); // cache values
      IEnumerable<double> trainingReturns = ProblemData.Dataset.GetDoubleValues(ProblemData.PriceChangeVariable, ProblemData.TrainingIndices);
      double[] testSignals = TestSignals.ToArray(); // cache values
      IEnumerable<double> testReturns = ProblemData.Dataset.GetDoubleValues(ProblemData.PriceChangeVariable, ProblemData.TestIndices);

      OnlineCalculatorError errorState;
      double trainingSharpeRatio = OnlineSharpeRatioCalculator.Calculate(trainingReturns, trainingSignals, ProblemData.TransactionCosts, out errorState);
      TrainingSharpeRatio = errorState == OnlineCalculatorError.None ? trainingSharpeRatio : double.NaN;
      double testSharpeRatio = OnlineSharpeRatioCalculator.Calculate(testReturns, testSignals, ProblemData.TransactionCosts, out errorState);
      TestSharpeRatio = errorState == OnlineCalculatorError.None ? testSharpeRatio : double.NaN;

      double trainingProfit = OnlineProfitCalculator.Calculate(trainingReturns, trainingSignals, ProblemData.TransactionCosts, out errorState);
      TrainingProfit = errorState == OnlineCalculatorError.None ? trainingProfit : double.NaN;
      double testProfit = OnlineProfitCalculator.Calculate(testReturns, testSignals, ProblemData.TransactionCosts, out errorState);
      TestProfit = errorState == OnlineCalculatorError.None ? testProfit : double.NaN;

    }

    public virtual IEnumerable<double> Signals {
      get { return GetSignals(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }
    public virtual IEnumerable<double> TrainingSignals {
      get { return GetSignals(ProblemData.TrainingIndices); }
    }
    public virtual IEnumerable<double> TestSignals {
      get { return GetSignals(ProblemData.TestIndices); }
    }
    public virtual IEnumerable<double> GetSignals(IEnumerable<int> rows) {
      return Model.GetSignals(ProblemData.Dataset, rows);
    }
  }
}
