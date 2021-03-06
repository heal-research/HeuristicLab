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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NCA Classification Solution", "")]
  [StorableType("3FE3A37F-2926-43EB-AC77-3C80654D93AA")]
  public class NcaClassificationSolution : ClassificationSolution, INcaClassificationSolution {

    public new INcaModel Model {
      get { return (INcaModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    protected NcaClassificationSolution(StorableConstructorFlag _) : base(_) { }
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
