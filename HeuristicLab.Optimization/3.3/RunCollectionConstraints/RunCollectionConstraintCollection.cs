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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("39066245-2B02-4B41-BEA3-158A4E21FCC1")]
  [Item("RunCollectionConstraintCollection", "Represents a collection of constraints.")]
  public class RunCollectionConstraintCollection : ItemCollection<IRunCollectionConstraint> {
    public RunCollectionConstraintCollection() : base() { }
    public RunCollectionConstraintCollection(int capacity) : base(capacity) { }
    public RunCollectionConstraintCollection(IEnumerable<IRunCollectionConstraint> collection) : base(collection) { }
    [StorableConstructor]
    protected RunCollectionConstraintCollection(StorableConstructorFlag _) : base(_) { }
    protected RunCollectionConstraintCollection(RunCollectionConstraintCollection original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionConstraintCollection(this, cloner);
    }
  }
}
