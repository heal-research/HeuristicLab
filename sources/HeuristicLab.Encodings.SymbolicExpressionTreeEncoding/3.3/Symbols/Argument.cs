#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols {
  /// <summary>
  /// Symbol for function arguments
  /// </summary>
  [StorableClass]
  [Item(Argument.ArgumentName, Argument.ArgumentDescription)]
  public sealed class Argument : ReadOnlySymbol {
    public const string ArgumentName = "Argument";
    public const string ArgumentDescription = "Symbol that represents a function argument.";
    [Storable]
    private int argumentIndex;
    public int ArgumentIndex {
      get { return argumentIndex; }
    }

    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private Argument() : base() { }

    public Argument(int argumentIndex)
      : base("ARG" + argumentIndex, Argument.ArgumentDescription) {
      this.argumentIndex = argumentIndex;
      this.name = "ARG" + argumentIndex;
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new ArgumentTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Argument clone = (Argument)base.Clone(cloner);
      clone.argumentIndex = argumentIndex;
      clone.name = "ARG" + argumentIndex;
      return clone;
    }
  }
}
