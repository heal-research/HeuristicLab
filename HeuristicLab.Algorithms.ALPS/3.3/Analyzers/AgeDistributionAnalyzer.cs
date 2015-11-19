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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ALPS {
  // Based on QualityDistributionAnalyzer
  [Item("AgeDistributionAnalyzer", "Analyzes the distribution of the ages in that it adds a Histogram of them into the result collection.")]
  [StorableClass]
  public sealed class AgeDistributionAnalyzer : SingleSuccessorOperator, IAnalyzer, IIterationBasedOperator {

    #region Parameter Properties
    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    private IValueParameter<StringValue> HistogramNameParameter {
      get { return (IValueParameter<StringValue>)Parameters["HistogramName"]; }
    }
    private IValueParameter<BoolValue> StoreHistoryParameter {
      get { return (IValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    #endregion

    public bool EnabledByDefault { get { return false; } }

    public string HistogramName {
      get { return HistogramNameParameter.Value.Value; }
      set { HistogramNameParameter.Value.Value = value; }
    }
    public bool StoreHistory {
      get { return StoreHistoryParameter.Value.Value; }
      set { StoreHistoryParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private AgeDistributionAnalyzer(bool deserializing)
      : base(deserializing) { }
    private AgeDistributionAnalyzer(AgeDistributionAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AgeDistributionAnalyzer(this, cloner);
    }
    public AgeDistributionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The value which represents the age of a solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection where the analysis values should be stored."));
      Parameters.Add(new FixedValueParameter<StringValue>("HistogramName", "The name of the histogram that gets injected in to the results collection.", new StringValue("Age Distribution")));
      Parameters.Add(new FixedValueParameter<BoolValue>("StoreHistory", "True if the history should be stored in addition to the current distribution", new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "Optional: A value indicating the current iteration."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "Unused", new IntValue(-1)));

      AgeParameter.Hidden = true;
      ResultsParameter.Hidden = true;
      IterationsParameter.Hidden = true;
      MaximumIterationsParameter.Hidden = true;
    }

    public override IOperation Apply() {
      DataTable ageDistribution = null;
      ResultCollection results = ResultsParameter.ActualValue;
      string description = "Shows the age distributions in the current population.";
      if (results.ContainsKey(HistogramName)) {
        ageDistribution = results[HistogramName].Value as DataTable;
      } else {
        ageDistribution = new DataTable("Population Age Distribution", description);
        ageDistribution.VisualProperties.XAxisTitle = AgeParameter.ActualName;
        ageDistribution.VisualProperties.YAxisTitle = "Frequency";
        results.Add(new Result(HistogramName, description, ageDistribution));
      }
      DataRow row;
      if (!ageDistribution.Rows.TryGetValue("AgeDistribution", out row)) {
        row = new DataRow("AgeDistribution");
        row.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Histogram;
        ageDistribution.Rows.Add(row);
      }
      var ages = AgeParameter.ActualValue;
      row.Values.Replace(ages.Select(x => x.Value));

      if (StoreHistory) {
        string historyResultName = HistogramName + " History";
        DataTableHistory adHistory = null;
        if (results.ContainsKey(historyResultName)) {
          adHistory = results[historyResultName].Value as DataTableHistory;
        } else {
          adHistory = new DataTableHistory();
          results.Add(new Result(historyResultName, adHistory));
        }
        DataTable table = (DataTable)ageDistribution.Clone();
        IntValue iteration = IterationsParameter.ActualValue;
        if (iteration != null) {
          string iterationName = IterationsParameter.ActualName;
          if (iterationName.EndsWith("s")) iterationName = iterationName.Remove(iterationName.Length - 1);
          string appendix = " at " + iterationName + " " + iteration.Value.ToString();
          table.Name += appendix;
          table.Rows["AgeDistribution"].VisualProperties.DisplayName += appendix;
        }
        adHistory.Add(table);
      }
      return base.Apply();
    }
  }
}