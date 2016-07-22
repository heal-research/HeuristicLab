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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Analysis {
  // use HeuristicLab.Analysis.PopulationSimilarityAnalyzer instead
  // BackwardsCompatibility3.3
  #region Backwards compatible code, remove with 3.4
  /// <summary>
  /// An operator for analyzing the solution diversity in a population.
  /// </summary>
  [Obsolete]
  [NonDiscoverableType]
  [Item("SingleObjectivePopulationDiversityAnalyzer", "An operator for analyzing the solution diversity in a population.")]
  [StorableClass]
  public class SingleObjectivePopulationDiversityAnalyzer : PopulationSimilarityAnalyzer {
    [StorableConstructor]
    protected SingleObjectivePopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    protected SingleObjectivePopulationDiversityAnalyzer(SingleObjectivePopulationDiversityAnalyzer original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePopulationDiversityAnalyzer(this, cloner);
    }
  }
  #endregion
}
