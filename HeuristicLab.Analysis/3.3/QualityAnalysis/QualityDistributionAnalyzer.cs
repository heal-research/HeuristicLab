#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis.QualityAnalysis {
  [Item("QualityDistributionAnalyzer", "Analyzes the distribution of the quality values in that it adds a Histogram of them into the result collection.")]
  [StorableClass]
  public class QualityDistributionAnalyzer : SingleSuccessorOperator, IAnalyzer, IIterationBasedOperator, ISingleObjectiveOperator {

    #region Parameter properties
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    private ValueParameter<StringValue> HistogramNameParameter {
      get { return (ValueParameter<StringValue>)Parameters["HistogramName"]; }
    }
    private ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    #endregion

    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public string HistogramName {
      get { return HistogramNameParameter.Value.Value; }
      set { HistogramNameParameter.Value.Value = value; }
    }

    public bool StoreHistory {
      get { return StoreHistoryParameter.Value.Value; }
      set { StoreHistoryParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected QualityDistributionAnalyzer(bool deserializing) : base(deserializing) { }
    protected QualityDistributionAnalyzer(QualityDistributionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public QualityDistributionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection where the analysis values should be stored."));
      Parameters.Add(new FixedValueParameter<StringValue>("HistogramName", "The name of the histogram that gets injected in to the results collection.", new StringValue("Quality Distribution")));
      Parameters.Add(new FixedValueParameter<BoolValue>("StoreHistory", "True if the history should be stored in addition to the current distribution", new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "Optional: A value indicating the current iteration."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "Unused", new IntValue(-1)));

      QualityParameter.Hidden = true;
      ResultsParameter.Hidden = true;
      IterationsParameter.Hidden = true;
      MaximumIterationsParameter.Hidden = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityDistributionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      DataTable qualityDistribution = null;
      ResultCollection results = ResultsParameter.ActualValue;
      string description = "Shows the quality distributions in the current population.";
      if (results.ContainsKey(HistogramName)) {
        qualityDistribution = results[HistogramName].Value as DataTable;
      } else {
        qualityDistribution = new DataTable("Population Quality Distribution", description);
        qualityDistribution.VisualProperties.XAxisTitle = QualityParameter.ActualName;
        qualityDistribution.VisualProperties.YAxisTitle = "Frequency";
        results.Add(new Result(HistogramName, description, qualityDistribution));
      }
      DataRow row;
      if (!qualityDistribution.Rows.TryGetValue("QualityDistribution", out row)) {
        row = new DataRow("QualityDistribution");
        row.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Histogram;
        qualityDistribution.Rows.Add(row);
      }
      var qualities = QualityParameter.ActualValue;
      row.Values.Replace(qualities.Select(x => x.Value));

      if (StoreHistory) {
        string historyResultName = HistogramName + " History";
        DataTableHistory qdHistory = null;
        if (results.ContainsKey(historyResultName)) {
          qdHistory = results[historyResultName].Value as DataTableHistory;
        } else {
          qdHistory = new DataTableHistory();
          results.Add(new Result(historyResultName, qdHistory));
        }
        DataTable table = (DataTable)qualityDistribution.Clone();
        IntValue iteration = IterationsParameter.ActualValue;
        if (iteration != null) {
          string iterationName = IterationsParameter.ActualName;
          if (iterationName.EndsWith("s")) iterationName = iterationName.Remove(iterationName.Length - 1);
          string appendix = " at " + iterationName + " " + iteration.Value.ToString();
          table.Name += appendix;
          table.Rows["QualityDistribution"].VisualProperties.DisplayName += appendix;
        }
        qdHistory.Add(table);
      }
      return base.Apply();
    }
  }
}
