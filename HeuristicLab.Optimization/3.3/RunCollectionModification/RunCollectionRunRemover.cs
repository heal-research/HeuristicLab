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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("RunCollection Run Remover", "Removes all currently visible runs. Use the filtering tab to selectively remove runs.")]
  [StorableClass]
  public class RunCollectionRunRemover : ParameterizedNamedItem, IRunCollectionModifier {

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionRunRemover(bool deserializing) : base(deserializing) { }
    protected RunCollectionRunRemover(RunCollectionRunRemover original, Cloner cloner) : base(original, cloner) {}
    public RunCollectionRunRemover() {}
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionRunRemover(this, cloner);
    }
    #endregion

    #region IRunCollectionModifier Members
    public void Modify(List<IRun> runs) {
      runs.Clear();
    }
    #endregion

  }
}
