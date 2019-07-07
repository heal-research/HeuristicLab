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

using System.IO;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Data {
  [Item("DirectoryValue", "Represents the path to a directory.")]
  [StorableType("AFB5A3C6-7FC8-45C6-A3B9-F3B3414A4CAB")]
  public class DirectoryValue : PathValue {
    [StorableConstructor]
    protected DirectoryValue(StorableConstructorFlag _) : base(_) { }
    protected DirectoryValue(DirectoryValue original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectoryValue(this, cloner);
    }

    public DirectoryValue() : base() { }

    public override bool Exists() {
      if (!Path.IsPathRooted(Value)) return false;
      return Directory.Exists(Value);
    }
  }
}
