#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;
using FCE = HeuristicLab.Problems.DataAnalysis.FeatureCorrelationEnums;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [NonDiscoverableType]
  internal class FeatureCorrelationTimeframeCache : Object {
    private Dictionary<FCE.CorrelationCalculators, Dictionary<FCE.Partitions, Dictionary<string, double[,]>>> timeFrameCorrelationsCache;

    public FeatureCorrelationTimeframeCache()
      : base() {
      InitializeCaches();
    }

    private void InitializeCaches() {
      timeFrameCorrelationsCache = new Dictionary<FCE.CorrelationCalculators, Dictionary<FCE.Partitions, Dictionary<string, double[,]>>>();
      foreach (var calc in FCE.EnumToList<FCE.CorrelationCalculators>()) {
        timeFrameCorrelationsCache.Add(calc, new Dictionary<FCE.Partitions, Dictionary<string, double[,]>>());
        foreach (var part in FCE.EnumToList<FCE.Partitions>()) {
          timeFrameCorrelationsCache[calc].Add(part, new Dictionary<string, double[,]>());
        }
      }
    }

    public void Reset() {
      InitializeCaches();
    }

    public double[,] GetTimeframeCorrelation(FCE.CorrelationCalculators calc, FCE.Partitions partition, string variable) {
      double[,] corr;
      timeFrameCorrelationsCache[calc][partition].TryGetValue(variable, out corr);
      return corr;
    }

    public void SetTimeframeCorrelation(FCE.CorrelationCalculators calc, FCE.Partitions partition, string variable, double[,] correlation) {
      timeFrameCorrelationsCache[calc][partition][variable] = correlation;
    }
  }
}
