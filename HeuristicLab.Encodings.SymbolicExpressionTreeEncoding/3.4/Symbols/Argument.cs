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
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Symbol for function arguments
  /// </summary>
  [StorableType("B0D02BED-6A67-469E-9A7C-8651C3805329")]
  [Item(Argument.ArgumentName, Argument.ArgumentDescription)]
  public sealed class Argument : Symbol, IReadOnlySymbol {
    public const string ArgumentName = "Argument";
    public const string ArgumentDescription = "Symbol that represents a function argument.";
    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [Storable]
    private int argumentIndex;
    public int ArgumentIndex {
      get { return argumentIndex; }
    }

    [StorableConstructor]
    private Argument(StorableConstructorFlag _) : base(_) { }
    private Argument(Argument original, Cloner cloner)
      : base(original, cloner) {
      argumentIndex = original.argumentIndex;
      name = "ARG" + original.argumentIndex;
    }
    public Argument(int argumentIndex)
      : base("ARG" + argumentIndex, Argument.ArgumentDescription) {
      this.argumentIndex = argumentIndex;
      this.name = "ARG" + argumentIndex;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new ArgumentTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Argument(this, cloner);
    }
  }
}
