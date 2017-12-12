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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NCA Classification Solution", "")]
  [StorableClass]
  public class NcaClassificationSolution : ClassificationSolution, INcaClassificationSolution {

    public new INcaModel Model {
      get { return (INcaModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    protected NcaClassificationSolution(bool deserializing) : base(deserializing) { }
    protected NcaClassificationSolution(NcaClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public NcaClassificationSolution(INcaModel ncaModel, IClassificationProblemData problemData)
      : base(ncaModel, problemData) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaClassificationSolution(this, cloner);
    }
  }
}
