#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [NonDiscoverableType]
  internal class FeatureCorrelationCache : Object {
    private Dictionary<FeatureCorrelationEnums.CorrelationCalculators, Dictionary<FeatureCorrelationEnums.Partitions, double[,]>> correlationsCache;

    public FeatureCorrelationCache()
      : base() {
      InitializeCaches();
    }

    private void InitializeCaches() {
      correlationsCache = new Dictionary<FeatureCorrelationEnums.CorrelationCalculators, Dictionary<FeatureCorrelationEnums.Partitions, double[,]>>();
      foreach (var calc in FeatureCorrelationEnums.EnumToList<FeatureCorrelationEnums.CorrelationCalculators>()) {
        correlationsCache.Add(calc, new Dictionary<FeatureCorrelationEnums.Partitions, double[,]>());
      }
    }

    public void Reset() {
      InitializeCaches();
    }

    public double[,] GetCorrelation(FeatureCorrelationEnums.CorrelationCalculators calc, FeatureCorrelationEnums.Partitions partition) {
      double[,] corr;
      correlationsCache[calc].TryGetValue(partition, out corr);
      return corr;
    }

    public void SetCorrelation(FeatureCorrelationEnums.CorrelationCalculators calc, FeatureCorrelationEnums.Partitions partition, double[,] correlation) {
      correlationsCache[calc][partition] = correlation;
    }
  }
}
