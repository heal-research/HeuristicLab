using HeuristicLab.Common;
using HeuristicLab.Optimization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HeuristicLab.Analysis {
  public static class ExpectedRuntimeHelper {
    public static ErtCalculationResult CalculateErt(IEnumerable<IEnumerable<Tuple<double, double>>> convGraphs, double target, bool maximization) {
      var successful = new List<double>();
      var unsuccessful = new List<double>();
      foreach (var graph in convGraphs) {
        var targetAchieved = false;
        var lastEffort = double.MaxValue;
        foreach (var v in graph) {
          if (maximization && v.Item2 >= target || !maximization && v.Item2 <= target) {
            successful.Add(v.Item1);
            targetAchieved = true;
            break;
          }
          lastEffort = v.Item1;
        }
        if (!targetAchieved) unsuccessful.Add(lastEffort);
      }

      var ert = double.PositiveInfinity;

      var nRuns = successful.Count + unsuccessful.Count;
      if (successful.Count > 0) {
        var succAvg = successful.Average();
        var succDev = successful.StandardDeviation() + 1e-7;
        successful.RemoveAll(x => x < succAvg - 2 * succDev);
        unsuccessful.RemoveAll(x => x < succAvg - 2 * succDev);
        nRuns = successful.Count + unsuccessful.Count;

        ert = successful.Average() / (successful.Count / (double)nRuns);
      }
      return new ErtCalculationResult(successful.Count, nRuns, ert);
    }

    public static ErtCalculationResult CalculateErt(List<IRun> runs, string indexedDataTableName, double target, bool maximization) {
      return CalculateErt(runs.Select(r => ((IndexedDataTable<double>)r.Results[indexedDataTableName]).Rows.First().Values), target, maximization);
    }
  }

  public struct ErtCalculationResult {
    public readonly int SuccessfulRuns;
    public readonly int TotalRuns;
    public readonly double ExpectedRuntime;

    public ErtCalculationResult(int successful, int total, double ert) {
      SuccessfulRuns = successful;
      TotalRuns = total;
      ExpectedRuntime = ert;
    }

    public override string ToString() {
      return ExpectedRuntime.ToString("##,0.0", CultureInfo.CurrentCulture.NumberFormat);
    }
  }
}
