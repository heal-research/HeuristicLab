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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [Item("CrowdingIndicator", "Selection of Offspring based on CrowdingDistance")]
  [StorableType("FEC5F17A-C720-4411-8AD6-42BA0F392AE9")]
  internal class CrowdingIndicator : Item, IIndicator {
    #region Constructors and Cloning
    [StorableConstructor]
    protected CrowdingIndicator(StorableConstructorFlag _) : base(_) { }
    protected CrowdingIndicator(CrowdingIndicator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdingIndicator(this, cloner);
    }
    public CrowdingIndicator() { }
    #endregion

    public int LeastContributer(IReadOnlyList<Individual> front, IMultiObjectiveProblemDefinition problem) {
      var extracted = front.Select(x => x.PenalizedFitness).ToArray();
      if (extracted.Length <= 2) return 0;
      var pointsums = CrowdingCalculator.CalculateCrowdingDistances(extracted);
      return pointsums.Select((value, index) => new {value, index}).OrderBy(x => x.value).First().index;
    }
  }
}