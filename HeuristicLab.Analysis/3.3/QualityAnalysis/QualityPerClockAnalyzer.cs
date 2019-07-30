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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  [Item("QualityPerClockAnalyzer", @"Creates a plot of the solution quality with respect to the elapsed wall-clock time.")]
  [StorableType("23410F61-AEE0-44BD-B721-2C4B33A1F4FE")]
  public class QualityPerClockAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {
    public virtual bool EnabledByDefault {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }

    public ILookupParameter<DateTimeValue> LastUpdateTimeParameter {
      get { return (ILookupParameter<DateTimeValue>)Parameters["LastUpdateTime"]; }
    }

    public IResultParameter<IndexedDataTable<double>> QualityPerClockParameter {
      get { return (IResultParameter<IndexedDataTable<double>>)Parameters["QualityPerClock"]; }
    }

    [StorableConstructor]
    protected QualityPerClockAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected QualityPerClockAnalyzer(QualityPerClockAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public QualityPerClockAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "The quality value that should be compared."));
      Parameters.Add(new LookupParameter<DateTimeValue>("LastUpdateTime", "The time the analyzer was last run."));
      Parameters.Add(new ResultParameter<IndexedDataTable<double>>("QualityPerClock", "Data table containing the first hitting graph with elapsed wall clock time (in seconds) as the x-axis."));
      QualityPerClockParameter.DefaultValue = new IndexedDataTable<double>("Quality per Clock") {
        VisualProperties = {
          XAxisTitle = "Elapsed time [s]",
          YAxisTitle = "Quality"
        },
        Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
          ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
          LineWidth = 2
        } } }
      };
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityPerClockAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var bestQuality = BestQualityParameter.ActualValue.Value;
      var dataTable = QualityPerClockParameter.ActualValue;
      var values = dataTable.Rows["First-hit Graph"].Values;

      var lastUpdateTime = LastUpdateTimeParameter.ActualValue;
      if (lastUpdateTime == null) {
        lastUpdateTime = new DateTimeValue(DateTime.UtcNow.AddMilliseconds(-1));
        LastUpdateTimeParameter.ActualValue = lastUpdateTime;
      }

      var now = DateTime.UtcNow;
      var runtimeSoFar = (now - lastUpdateTime.Value).TotalSeconds + (values.Count > 0 ? values.Last().Item1 : 0);
      lastUpdateTime.Value = now;
      var newEntry = Tuple.Create(runtimeSoFar, bestQuality);

      if (values.Count == 0) {
        values.Add(newEntry); // record the first data
        values.Add(Tuple.Create(runtimeSoFar, bestQuality)); // last entry records max number of evaluations
        return base.Apply();
      }

      var improvement = values.Last().Item2 != bestQuality;
      if (improvement) {
        values[values.Count - 1] = newEntry; // record the improvement
        values.Add(Tuple.Create(runtimeSoFar, bestQuality)); // last entry records max number of evaluations
      } else {
        values[values.Count - 1] = Tuple.Create(runtimeSoFar, bestQuality);
      }
      return base.Apply();
    }
  }
}
