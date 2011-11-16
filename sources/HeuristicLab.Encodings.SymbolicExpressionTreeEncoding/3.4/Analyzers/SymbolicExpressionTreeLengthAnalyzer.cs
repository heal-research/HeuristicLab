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

using System;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// An operator that tracks tree lengths of Symbolic Expression Trees
  /// </summary>
  [Item("SymbolicExpressionTreeLengthAnalyzer", "An operator that tracks tree lengths of Symbolic Expression Trees")]
  [StorableClass]
  public sealed class SymbolicExpressionTreeLengthAnalyzer : SingleSuccessorOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string SymbolicExpressionTreeLengthsParameterName = "SymbolicExpressionTreeLengthsTable";
    private const string SymbolicExpressionTreeLengthsHistoryParameterName = "SymbolicExpressionTreeLengthsHistoryTable";
    private const string ResultsParameterName = "Results";
    private const string StoreHistoryParameterName = "StoreHistory";
    private const string UpdateIntervalParameterName = "UpdateInterval";
    private const string UpdateCounterParameterName = "UpdateCounter";

    #region Parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ValueLookupParameter<DataTable> SymbolicExpressionTreeLengthsParameter {
      get {
        return (ValueLookupParameter<DataTable>)Parameters[SymbolicExpressionTreeLengthsParameterName];
      }
    }
    public ValueLookupParameter<DataTableHistory> SymbolicExpressionTreeLengthsHistoryParameter {
      get {
        return (ValueLookupParameter<DataTableHistory>)Parameters[SymbolicExpressionTreeLengthsHistoryParameterName];
      }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    // history
    public ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters[StoreHistoryParameterName]; }
    }
    public ValueParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters[UpdateIntervalParameterName]; }
    }
    public LookupParameter<IntValue> UpdateCounterParameter {
      get { return (LookupParameter<IntValue>)Parameters[UpdateCounterParameterName]; }
    }
    #endregion
    [StorableConstructor]
    private SymbolicExpressionTreeLengthAnalyzer(bool deserializing) : base() { }
    private SymbolicExpressionTreeLengthAnalyzer(SymbolicExpressionTreeLengthAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      AfterDeserialization();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeLengthAnalyzer(this, cloner);
    }
    public SymbolicExpressionTreeLengthAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose length should be calculated."));
      Parameters.Add(new LookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximum allowed symbolic expression tree length"));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolicExpressionTreeLengthsParameterName, "The data table to store the symbolic expression tree lengths."));
      Parameters.Add(new ValueLookupParameter<DataTableHistory>(SymbolicExpressionTreeLengthsHistoryParameterName, "The data table to store the symbolic expression tree lengths history."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));

      #region Add parameters for history
      Parameters.Add(new ValueParameter<BoolValue>(StoreHistoryParameterName, "True if the tree lengths history of the population should be stored.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called since the last update", "MinMaxAverageSymbolicExpressionTreeLengthAnalyzerUpdateCounter"));
      #endregion

      UpdateCounterParameter.Hidden = true;

      AfterDeserialization();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // check if all the parameters are present and accounted for 
      if (!Parameters.ContainsKey(StoreHistoryParameterName)) {
        Parameters.Add(new ValueParameter<BoolValue>(StoreHistoryParameterName, "True if the tree lengths history of the population should be stored.", new BoolValue(false)));
      }
      if (!Parameters.ContainsKey(UpdateIntervalParameterName)) {
        Parameters.Add(new ValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      }
      if (!Parameters.ContainsKey(UpdateCounterParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called since the last update", "MinMaxAverageSymbolicExpressionTreeLengthAnalyzerUpdateCounter"));
      }
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;
      // if the counter doesn't exist yet, we initialize it here with the current update interval
      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      } else updateCounter.Value++;

      // the analyzer runs periodically, every 'updateInterval' times
      if (updateCounter.Value == updateInterval) {
        updateCounter.Value = 0; // reset counter

        // compute all tree lengths and store them in the lengthsTable
        var solutions = SymbolicExpressionTreeParameter.ActualValue;

        var treeLengthsTable = SymbolicExpressionTreeLengthsParameter.ActualValue;
        // if the table was not created yet, we create it here
        if (treeLengthsTable == null) {
          treeLengthsTable = new DataTable("Tree Length Values");
          SymbolicExpressionTreeLengthsParameter.ActualValue = treeLengthsTable;
        }

        // data table which stores tree length values
        DataRow dataRow;

        var treeLengths = solutions.Select(s => (double)s.Length);

        if (!treeLengthsTable.Rows.ContainsKey("Tree Lengths")) {
          dataRow = new DataRow("Tree Lengths", "", treeLengths);
          treeLengthsTable.Rows.Add(dataRow);
        } else {
          dataRow = treeLengthsTable.Rows["Tree Lengths"];
          dataRow.Values.Replace(treeLengths);
        }

        double maximumAllowedTreeLength = ((LookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]).ActualValue.Value;

        dataRow.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Histogram;
        dataRow.VisualProperties.ExactBins = false;

        // the following trick should result in an intervalWidth of 1,2,4,... (an int value)
        dataRow.VisualProperties.Bins = solutions.Max(s => s.Length) - solutions.Min(s => s.Length);

        int maxLength = solutions.Max(s => s.Length);
        if (maxLength <= 25)      // [0,25]
          dataRow.VisualProperties.ScaleFactor = 1.0;
        else if (maxLength <= 100) // [26,100])
          dataRow.VisualProperties.ScaleFactor = 1.0 / 2.0;
        else if (maxLength <= 250) // [100,250]
          dataRow.VisualProperties.ScaleFactor = 1.0 / 5.0;
        else if (maxLength <= 500) // [251,500]
          dataRow.VisualProperties.ScaleFactor = 1.0 / 10.0;
        else
          dataRow.VisualProperties.ScaleFactor = 1.0 / 20.0;   // [501,inf]

        // visual properties for the X-axis
        treeLengthsTable.VisualProperties.XAxisMinimumAuto = false;
        treeLengthsTable.VisualProperties.XAxisMaximumAuto = false;
        treeLengthsTable.VisualProperties.XAxisMinimumFixedValue = 0.0;
        if (maxLength > maximumAllowedTreeLength + 1)
          treeLengthsTable.VisualProperties.XAxisMaximumFixedValue = solutions.Max(s => s.Length) + 1; // +1 so the histogram column for the maximum length won't get trimmed
        else
          treeLengthsTable.VisualProperties.XAxisMaximumFixedValue = maximumAllowedTreeLength + 1;
        // visual properties for the Y-axis
        treeLengthsTable.VisualProperties.YAxisMinimumAuto = false;
        treeLengthsTable.VisualProperties.YAxisMaximumAuto = false;
        treeLengthsTable.VisualProperties.YAxisMinimumFixedValue = 0.0;
        treeLengthsTable.VisualProperties.YAxisMaximumFixedValue = Math.Ceiling(solutions.Length / 2.0);

        var results = ResultsParameter.ActualValue;

        if (!results.ContainsKey("Tree Lengths")) {
          results.Add(new Result("Tree Lengths", treeLengthsTable));
        } else {
          results["Tree Lengths"].Value = treeLengthsTable;
        }

        bool storeHistory = StoreHistoryParameter.Value.Value;

        if (storeHistory) {
          // store tree lengths for each generation
          var historyDataRow = new DataRow("Tree Lengths " + results["Generations"].Value, "", dataRow.Values);
          historyDataRow.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Histogram;
          historyDataRow.VisualProperties.ExactBins = false;
          historyDataRow.VisualProperties.Bins = solutions.Max(s => s.Length) - solutions.Min(s => s.Length);
          historyDataRow.VisualProperties.ScaleFactor = dataRow.VisualProperties.ScaleFactor;
          var historyTable = new DataTable("Lengths Table " + results["Generations"]);
          historyTable.Rows.Add(historyDataRow);
          // visual properties for the X-axis
          historyTable.VisualProperties.XAxisMinimumAuto = false;
          historyTable.VisualProperties.XAxisMaximumAuto = false;
          historyTable.VisualProperties.XAxisMinimumFixedValue = 0.0;
          if (solutions.Max(s => s.Length) > maximumAllowedTreeLength + 1)
            historyTable.VisualProperties.XAxisMaximumFixedValue = maxLength + 1; // +1 so the histogram column for the maximum length won't get trimmed
          else
            historyTable.VisualProperties.XAxisMaximumFixedValue = maximumAllowedTreeLength + 1;
          // visual properties for the Y-axis
          historyTable.VisualProperties.YAxisMinimumAuto = false;
          historyTable.VisualProperties.YAxisMaximumAuto = false;
          historyTable.VisualProperties.YAxisMinimumFixedValue = 0.0;
          historyTable.VisualProperties.YAxisMaximumFixedValue = Math.Ceiling(solutions.Length / 2.0);

          var treeLengthsHistory = SymbolicExpressionTreeLengthsHistoryParameter.ActualValue;
          if (treeLengthsHistory == null) {
            treeLengthsHistory = new DataTableHistory();
            SymbolicExpressionTreeLengthsHistoryParameter.ActualValue = treeLengthsHistory;
          }

          treeLengthsHistory.Add(historyTable);

          if (!results.ContainsKey("Tree Lengths History")) {
            results.Add(new Result("Tree Lengths History", treeLengthsHistory));
          } else {
            results["Tree Lengths History"].Value = treeLengthsHistory;
          }
        }
      }

      return base.Apply();
    }
  }
}
