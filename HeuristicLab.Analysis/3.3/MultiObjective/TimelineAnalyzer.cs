#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  [StorableType("48fc7ff7-a8ab-4ef9-8c01-cc6fbbe3da0b")]
  [Item("TimelineAnalyzer", "Collects the specified double values from the results and displays them as a timeline")]
  public class TimelineAnalyzer : SingleSuccessorOperator, IAnalyzer, IMultiObjectiveOperator {
    public bool EnabledByDefault {
      get { return true; }
    }
    public string ResultName {
      get { return "Timeline"; }
    }

    public IFixedValueParameter<CheckedItemList<StringValue>> CollectedSuccessMeasuresParameter {
      get { return (IFixedValueParameter<CheckedItemList<StringValue>>)Parameters["CollectedSuccessMeasures"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ResultParameter<DataTable> ResultParameter {
      get { return (ResultParameter<DataTable>)Parameters[ResultName]; }
    }

    public CheckedItemList<StringValue> CollectedSuccessMeasures {
      get { return CollectedSuccessMeasuresParameter.Value; }
    }

    [StorableConstructor]
    protected TimelineAnalyzer(StorableConstructorFlag _) : base(_) { }


    protected TimelineAnalyzer(TimelineAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimelineAnalyzer(this, cloner);
    }

    public TimelineAnalyzer() {
      var names = new List<string> {
        new CrowdingAnalyzer().ResultName,
        new GenerationalDistanceAnalyzer().ResultName,
        new InvertedGenerationalDistanceAnalyzer().ResultName,
        new HypervolumeAnalyzer().ResultName,
        new SpacingAnalyzer().ResultName
      };
      var analyzers = new CheckedItemList<StringValue>(names.Select(n => new StringValue(n)));
      Parameters.Add(new FixedValueParameter<CheckedItemList<StringValue>>("CollectedSuccessMeasures", analyzers));
      Parameters.Add(new LookupParameter<ResultCollection>("Results"));
      Parameters.Add(new ResultParameter<DataTable>("Timeline", "The development of the success measures over the generations", "Results", new DataTable("Timeline", "")));
    }

    public override IOperation Apply() {
      if (ResultParameter.ActualValue == null) ResultParameter.ActualValue = new DataTable(ResultName, "");
      var plot = ResultParameter.ActualValue;
      var resultCollection = ResultsParameter.ActualValue;
      foreach (var resultName in CollectedSuccessMeasures.CheckedItems.Select(i => i.Value.Value)) {
        if (!resultCollection.ContainsKey(resultName)) continue;
        var res = resultCollection[resultName].Value as DoubleValue;
        if (res == null) continue;
        if (!plot.Rows.ContainsKey(resultName)) plot.Rows.Add(new DataRow(resultName));
        plot.Rows[resultName].Values.Add(res.Value);
      }
      return base.Apply();
    }
  }
}