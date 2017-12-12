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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  public enum ReductionOperations {
    Sum,
    Product,
    Count,
    Min,
    Max,
    Avg,
    Assign,
    All,
    Any
  }

  [Item("ReductionOperation", "Represents a certain type of reduction operation.")]
  [StorableClass]
  public class ReductionOperation : ValueTypeValue<ReductionOperations> {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Enum; }
    }

    [StorableConstructor]
    protected ReductionOperation(bool deserializing) : base(deserializing) { }
    protected ReductionOperation(ReductionOperation original, Cloner cloner) : base(original, cloner) { }
    public ReductionOperation() : base() { }
    public ReductionOperation(ReductionOperations type) : base(type) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReductionOperation(this, cloner);
    }
  }
}