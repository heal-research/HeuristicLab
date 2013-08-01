#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("PathValue", "Represents a path.")]
  [StorableClass]
  public abstract class PathValue : Item {

    [Storable]
    private readonly StringValue stringValue = new StringValue();
    public StringValue StringValue {
      get { return stringValue; }
    }

    public string Value {
      get { return stringValue.Value; }
      set {
        if (value == null) value = string.Empty;
        value = value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        stringValue.Value = value;
      }
    }

    [StorableConstructor]
    protected PathValue(bool deserializing) : base(deserializing) { }
    protected PathValue(PathValue original, Cloner cloner)
      : base(original, cloner) {
      stringValue = cloner.Clone(original.stringValue);
    }

    protected PathValue() : base() { }

    public abstract bool Exists();
  }
}
