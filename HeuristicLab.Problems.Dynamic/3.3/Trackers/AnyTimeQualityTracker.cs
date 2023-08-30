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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Any Time Quality Tracker", "")]
  [StorableType("EDAC98B7-C40A-4995-A63C-9EE12E0BB14B")]
  public class AnyTimeQualityTracker<TSolution, TState>
    : ParameterizedNamedItem, ISingleObjectiveDynamicProblemTracker<TSolution, TState>
    where TSolution : IItem
  {
    protected virtual string PlotResultName { get => "Any Time Performance"; }
    public const string MinimumQualitiesRowName = "Minimum Qualities";
    public const string AverageQualitiesRowName = "Average Qualities";
    public const string MaximumQualitiesRowName = "Maximum Qualities";
    public const string BestQualitiesRowName = "Best Qualities";
    public const string EpochChangesRowName = "Epoch Changes";

    [Storable] public List<Tuple<long, long, double>> Qualities;
    [Storable] public List<Tuple<long, long>> EpochChanges;
    [Storable] public Tuple<long, long, double> LastMinimumResolvedQuality;
    [Storable] public Tuple<long, long, double> LastMaximumResolvedQuality;
    [Storable] public Tuple<long, long> LastResolvedEpochChange;
    private readonly object locker = new object();

    #region Constructors and cloning
    public AnyTimeQualityTracker() {
      Qualities = new List<Tuple<long, long, double>>();
      EpochChanges = new List<Tuple<long, long>>();
      LastResolvedEpochChange = Tuple.Create(long.MinValue, long.MinValue);
      LastMaximumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MinValue);
      LastMinimumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MaxValue);
    }

    [StorableConstructor]
    protected AnyTimeQualityTracker(StorableConstructorFlag _) : base(_) {
    }

    protected AnyTimeQualityTracker(AnyTimeQualityTracker<TSolution, TState> original, Cloner cloner) : base(original, cloner) {
      Qualities = original.Qualities.ToList();
      EpochChanges = original.EpochChanges.ToList();
      LastResolvedEpochChange = original.LastResolvedEpochChange;
      LastMaximumResolvedQuality = original.LastMaximumResolvedQuality;
      LastMinimumResolvedQuality = original.LastMinimumResolvedQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AnyTimeQualityTracker<TSolution, TState>(this, cloner);
    }
    #endregion

    #region tracker functions
    public virtual void OnEvaluation(TState state, TSolution solution, double quality, long version, long time) {
      lock (locker) Qualities.Add(Tuple.Create(time, version, quality));
    }

    public virtual void OnEpochChange(TState state, long version, long time) {
      lock (locker) EpochChanges.Add(Tuple.Create(time, version));
    }

    public virtual void OnAnalyze(TState state, ResultCollection results) {
      //fetch data
      List<Tuple<long, long, double>> qs;
      List<Tuple<long, long>> ecs;
      lock (locker) {
        qs = Qualities.ToList();
        Qualities.Clear();
        ecs = EpochChanges.ToList();
        EpochChanges.Clear();
      }

      //get or generate rows
      IResult plotResult;
      IndexedDataRow<double> maxRow, minRow, epochRow, avgRow, bestRow;
      if (!results.TryGetValue(PlotResultName, out plotResult))
        results.Add(plotResult = new Result(PlotResultName, new IndexedDataTable<double>("AnyTimePerformance", "")));
      var plot = (IndexedDataTable<double>)plotResult.Value;
      if (!plot.Rows.TryGetValue(MaximumQualitiesRowName, out maxRow))
        plot.Rows.Add(maxRow = new IndexedDataRow<double>(MaximumQualitiesRowName, "", new List<Tuple<double, double>>()));
      if (!plot.Rows.TryGetValue(AverageQualitiesRowName, out avgRow))
        plot.Rows.Add(avgRow = new IndexedDataRow<double>(AverageQualitiesRowName, "", new List<Tuple<double, double>>()));
      if (!plot.Rows.TryGetValue(MinimumQualitiesRowName, out minRow))
        plot.Rows.Add(minRow = new IndexedDataRow<double>(MinimumQualitiesRowName, "", new List<Tuple<double, double>>()));
      if (!plot.Rows.TryGetValue(BestQualitiesRowName, out bestRow))
        plot.Rows.Add(bestRow = new IndexedDataRow<double>(BestQualitiesRowName, "", new List<Tuple<double, double>>()));
      if (!plot.Rows.TryGetValue(EpochChangesRowName, out epochRow))
        plot.Rows.Add(epochRow = new IndexedDataRow<double>(EpochChangesRowName, "", new List<Tuple<double, double>>()) {
          VisualProperties = new DataRowVisualProperties() { SecondYAxis = true, ChartType = DataRowVisualProperties.DataRowChartType.StepLine }
        });
      
      //fill rows with data (always use AddRange)
      if (ecs.Count != 0) {
        epochRow.Values.AddRange(ecs.Select(x => new Tuple<double, double>(x.Item1, x.Item2)));
        LastResolvedEpochChange = ecs.Last();
      }

      if (qs.Count == 0) return;
      
      //calculate cumulative Minimum
      var newMinPoints = new List<Tuple<double, double>>();
      foreach (var tuple in qs) {
        if (tuple.Item2 == LastMinimumResolvedQuality.Item2 &&
            !(tuple.Item3 < LastMinimumResolvedQuality.Item3)) continue;
        LastMinimumResolvedQuality = tuple;
        newMinPoints.Add(new Tuple<double, double>(LastMinimumResolvedQuality.Item1, LastMinimumResolvedQuality.Item3));
      }
      newMinPoints.Add(new Tuple<double, double>(qs.Last().Item1, LastMinimumResolvedQuality.Item3));
      minRow.Values.AddRange(newMinPoints);

      //calculate cumulative Maximum
      var newMaxPoints = new List<Tuple<double, double>>();
      foreach (var tuple in qs) {
        if (tuple.Item2 == LastMaximumResolvedQuality.Item2 &&
            !(tuple.Item3 > LastMaximumResolvedQuality.Item3)) continue;
        LastMaximumResolvedQuality = tuple;
        newMaxPoints.Add(new Tuple<double, double>(LastMaximumResolvedQuality.Item1, LastMaximumResolvedQuality.Item3));
      }
      newMaxPoints.Add(new Tuple<double, double>(qs.Last().Item1, LastMaximumResolvedQuality.Item3));
      maxRow.Values.AddRange(newMaxPoints);
      
      avgRow.Values.AddRange(qs.GroupBy(x => x.Item2).Select(g => new Tuple<double, double>(g.Max(x => x.Item1), g.Average(x => x.Item3))));
    }

    public void Reset() {
      Qualities = new List<Tuple<long, long, double>>();
      EpochChanges = new List<Tuple<long, long>>();
      LastResolvedEpochChange = Tuple.Create(long.MinValue, long.MinValue);
      LastMaximumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MinValue);
      LastMinimumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MaxValue);
    }

    #endregion
  }
}
