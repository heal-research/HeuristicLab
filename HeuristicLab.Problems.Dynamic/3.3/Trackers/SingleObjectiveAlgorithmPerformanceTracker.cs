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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Algorithm Performance Tracker (single objective)", "")]
  [StorableType("17350422-B37A-4C1D-8DFD-D0457435DFCF")]
  public class SingleObjectiveAlgorithmPerformanceTracker : ParameterizedNamedItem, ISingleObjectiveDynamicProblemTracker<IItem, object> {
    public const string MinResultName = "OfflineMinimum";
    public const string MaxResultName = "OfflineMaximum";
    public const string MinOnlineResultName = "OnlineMinimum";
    public const string MaxOnlineResultName = "OfflineMaximum";

    [Storable] private double minSum;
    [Storable] private double maxSum;
    [Storable] private double minCur;
    [Storable] private double maxCur;
    [Storable] private double minLast;
    [Storable] private double maxLast;
    [Storable] private long epCount;
    [Storable] private long epCur;

    #region Constructors and cloning
    public SingleObjectiveAlgorithmPerformanceTracker() {
      Reset();
    }

    [StorableConstructor]
    protected SingleObjectiveAlgorithmPerformanceTracker(StorableConstructorFlag _) : base(_) {
    }

    protected SingleObjectiveAlgorithmPerformanceTracker(SingleObjectiveAlgorithmPerformanceTracker original, Cloner cloner) : base(original, cloner) {
      minSum = original.minSum;
      maxSum = original.maxSum;
      minCur = original.minCur;
      maxCur = original.maxCur;
      epCount = original.epCount;
      epCur = original.epCur;
      minLast = original.minLast;
      maxLast = original.maxLast;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveAlgorithmPerformanceTracker(this, cloner);
    }
    #endregion

    #region tracker functions
    public void OnEvaluation(object _, IItem solution, double quality, long version, long time) {
      if (quality < minCur) minCur = quality;
      if (quality > maxCur) maxCur = quality;

    }

    public void OnEpochChange(object _, long version, long time) {
      if (minCur != double.MaxValue) {
        minSum += minCur;
        maxSum += maxCur;
        epCount++;
        epCur = version;
        minLast = minCur;
        maxLast = maxCur;
      }
      maxCur = double.MinValue;
      minCur = double.MaxValue;
      
    }

    public void OnAnalyze(object _, ResultCollection results) {
      var a1 = 0;
      var aMin = 0.0;
      var aMax = 0.0;
      if (minCur != double.MaxValue) {
        a1 = 1;
        aMax = maxCur;
        aMin = minCur;
      }

      var eps = epCount + a1;
      if (eps == 0) return;
      var mine = (minSum + aMin) / eps;
      var mane = (maxSum + aMax) / eps;
      results.AddOrUpdateResult(MinResultName,new DoubleValue(mine));
      results.AddOrUpdateResult(MaxResultName,new DoubleValue(mane));
      if(epCount<=1) return;
      results.AddOrUpdateResult(MinOnlineResultName,new DoubleValue(minLast));
      results.AddOrUpdateResult(MaxOnlineResultName,new DoubleValue(maxLast));
    }

    public void Reset() {
      epCount = 0;
      epCur = long.MinValue;
      minCur = double.MaxValue;
      maxCur = double.MinValue;
      minLast = double.MaxValue;
      maxLast = double.MinValue;
      minSum = 0;
      maxSum = 0;
    }
    #endregion
  }
}
