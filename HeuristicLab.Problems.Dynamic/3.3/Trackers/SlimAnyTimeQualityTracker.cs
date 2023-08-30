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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Slim Any Time Quality Tracker", "")]
  [StorableType("20617025-09E8-49DC-905E-8C4581201568")]
  public class SlimAnyTimeQualityTracker : ParameterizedNamedItem, ISingleObjectiveDynamicProblemTracker<IItem, object> {
    public const string PlotResultName = "Slim Any Time Performance";
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
    public SlimAnyTimeQualityTracker() {
      Qualities = new List<Tuple<long, long, double>>();
      EpochChanges = new List<Tuple<long, long>>();
      LastResolvedEpochChange = Tuple.Create(long.MinValue, long.MinValue);
      LastMaximumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MinValue);
      LastMinimumResolvedQuality = Tuple.Create(long.MinValue, long.MinValue, double.MaxValue);
    }

    [StorableConstructor]
    protected SlimAnyTimeQualityTracker(StorableConstructorFlag _) : base(_) {
    }

    protected SlimAnyTimeQualityTracker(SlimAnyTimeQualityTracker original, Cloner cloner) : base(original, cloner) {
      Qualities = original.Qualities.ToList();
      EpochChanges = original.EpochChanges.ToList();
      LastResolvedEpochChange = original.LastResolvedEpochChange;
      LastMaximumResolvedQuality = original.LastMaximumResolvedQuality;
      LastMinimumResolvedQuality = original.LastMinimumResolvedQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SlimAnyTimeQualityTracker(this, cloner);
    }
    #endregion

    #region tracker functions
    public void OnEvaluation(object _, IItem solution, double quality, long version, long time) {
      lock (locker) Qualities.Add(Tuple.Create(time, version, quality));
    }

    public void OnEpochChange(object _, long version, long time) {
      lock (locker) EpochChanges.Add(Tuple.Create(time, version));
    }

    public void OnAnalyze(object _, ResultCollection results) {
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
      ScatterPlotDataRow maxRow, minRow, epochRow, avgRow, bestRow;
      ScatterPlot plot;
      if (!results.TryGetValue(PlotResultName, out plotResult)) {
        plot = new ScatterPlot("AnyTimePerformance", "");
        var hidden = new HiddenResults();
        hidden.Scope.Variables.Add(new Variable(PlotResultName, plot));
        results.Add(plotResult = new Result(PlotResultName, hidden));
      } else {
        plot = (ScatterPlot)((HiddenResults)plotResult.Value).Scope.Variables[PlotResultName].Value;
      }
      
      if (!plot.Rows.TryGetValue(MaximumQualitiesRowName, out maxRow))
        plot.Rows.Add(maxRow = new ScatterPlotDataRow(MaximumQualitiesRowName, "", new List<Point2D<double>>()));
      if (!plot.Rows.TryGetValue(MinimumQualitiesRowName, out minRow))
        plot.Rows.Add(minRow = new ScatterPlotDataRow(MinimumQualitiesRowName, "", new List<Point2D<double>>()));
      if (!plot.Rows.TryGetValue(EpochChangesRowName, out epochRow))
        plot.Rows.Add(epochRow = new ScatterPlotDataRow(EpochChangesRowName, "", new List<Point2D<double>>()));
      if (!plot.Rows.TryGetValue(AverageQualitiesRowName, out avgRow))
        plot.Rows.Add(avgRow = new ScatterPlotDataRow(AverageQualitiesRowName, "", new List<Point2D<double>>()));
      if (!plot.Rows.TryGetValue(BestQualitiesRowName, out bestRow))
        plot.Rows.Add(bestRow = new ScatterPlotDataRow(BestQualitiesRowName, "", new List<Point2D<double>>()));
      //fill rows with data (always use AddRange)
      if (ecs.Count != 0) {
        epochRow.Points.AddRange(ecs.Select(x => new Point2D<double>(x.Item1, x.Item2)));
        LastResolvedEpochChange = ecs.Last();
      }

      if (qs.Count == 0) return;
      //calculate cumulative Minimum
      var points2 = new List<Point2D<double>>();
      foreach (var tuple in qs) {
        if (tuple.Item2 == LastMinimumResolvedQuality.Item2 &&
            !(tuple.Item3 < LastMinimumResolvedQuality.Item3)) continue;
        LastMinimumResolvedQuality = tuple;
        points2.Add(new Point2D<double>(LastMinimumResolvedQuality.Item1, LastMinimumResolvedQuality.Item3));
      }
      points2.Add(new Point2D<double>(qs.Last().Item1, LastMinimumResolvedQuality.Item3));
      minRow.Points.AddRange(points2);

      //calculate cumulative Maximum
      var points3 = new List<Point2D<double>>();
      foreach (var tuple in qs) {
        if (tuple.Item2 == LastMaximumResolvedQuality.Item2 &&
            !(tuple.Item3 > LastMaximumResolvedQuality.Item3)) continue;
        LastMaximumResolvedQuality = tuple;
        points3.Add(new Point2D<double>(LastMaximumResolvedQuality.Item1, LastMaximumResolvedQuality.Item3));
      }

      points3.Add(new Point2D<double>(qs.Last().Item1, LastMaximumResolvedQuality.Item3));
      maxRow.Points.AddRange(points3);
      avgRow.Points.AddRange(qs
        .GroupBy(x => x.Item2)
        .Select(g => new Point2D<double>(
          g.Max(x => x.Item1),
          g.Average(x => x.Item3))));
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
  [StorableType("927CC3C4-8782-435A-A193-5FBD1AAC6D75")]
  public class HiddenResults : NamedItem {
    [Storable] public IScope Scope { get; set; } = new Scope();

    [StorableConstructor]
    public HiddenResults(StorableConstructorFlag _) : base(_) { }

    public HiddenResults() { }

    public HiddenResults(HiddenResults original, Cloner cloner) : base(original, cloner) {
      Scope = cloner.Clone(original.Scope);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HiddenResults(this, cloner);
    }
  }
}
