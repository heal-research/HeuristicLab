#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization {
  [Item("MultiEncoding Manipulator", "Applies different manipulators to change a multi-encoding.")]
  [StorableType("574D0530-47E8-4FD9-8AC8-B8EA2DE3C203")]
  public sealed class MultiEncodingManipulator : MultiEncodingOperator<IManipulator>, IManipulator {
    [StorableConstructor]
    private MultiEncodingManipulator(StorableConstructorFlag _) : base(_) { }
    private MultiEncodingManipulator(MultiEncodingManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiEncodingManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) { return new MultiEncodingManipulator(this, cloner); }

  }
}
