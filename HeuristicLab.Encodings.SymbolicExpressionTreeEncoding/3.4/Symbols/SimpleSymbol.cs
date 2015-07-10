#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("SimpleSymbol", "Represents a symbol that is identified by its name.")]
  [NonDiscoverableType]
  public sealed class SimpleSymbol : Symbol {
    [Storable]
    private readonly int minimumArity;
    public override int MinimumArity {
      get { return minimumArity; }
    }

    [Storable]
    private readonly int maximumArity = byte.MaxValue;
    public override int MaximumArity {
      get { return maximumArity; }
    }

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private SimpleSymbol(bool deserializing) : base(deserializing) { }

    private SimpleSymbol(SimpleSymbol original, Cloner cloner)
      : base(original, cloner) {
      minimumArity = original.minimumArity;
      maximumArity = original.maximumArity;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleSymbol(this, cloner);
    }

    public SimpleSymbol(string name, string description, int minimumArity, int maximumArity)
      : base(name, description) {
      this.minimumArity = minimumArity;
      this.maximumArity = maximumArity;
    }

  }
}
