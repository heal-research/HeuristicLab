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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("71CF8C14-65CB-4393-9D46-2673A69C0729")]
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
    private SimpleSymbol(StorableConstructorFlag _) : base(_) { }

    private SimpleSymbol(SimpleSymbol original, Cloner cloner)
      : base(original, cloner) {
      minimumArity = original.minimumArity;
      maximumArity = original.maximumArity;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleSymbol(this, cloner);
    }

    public SimpleSymbol(string name, int arity)
      : this(name, string.Empty, arity, arity) {
    }

    public SimpleSymbol(string name, string description, int minimumArity, int maximumArity)
      : base(name, description) {
      this.minimumArity = minimumArity;
      this.maximumArity = maximumArity;
    }

  }
}
