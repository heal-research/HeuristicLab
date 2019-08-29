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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
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
  [StorableType("71612344-657C-4270-8A44-5D521C0118A8")]
  public class SingleObjectivePopulationDiversityAnalyzer : PopulationSimilarityAnalyzer {
    [StorableConstructor]
    protected SingleObjectivePopulationDiversityAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectivePopulationDiversityAnalyzer(SingleObjectivePopulationDiversityAnalyzer original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePopulationDiversityAnalyzer(this, cloner);
    }
  }
  #endregion
}
