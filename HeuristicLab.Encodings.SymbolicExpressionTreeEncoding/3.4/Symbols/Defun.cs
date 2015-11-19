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
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Symbol for function defining branches
  /// </summary>
  [StorableClass]
  [Item(Defun.DefunName, Defun.DefunDescription)]
  public sealed class Defun : Symbol, IReadOnlySymbol {
    public const string DefunName = "Defun";
    public const string DefunDescription = "Symbol that represents a function defining node.";
    private const int minimumArity = 1;
    private const int maximumArity = 1;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private Defun(bool deserializing) : base(deserializing) { }
    private Defun(Defun original, Cloner cloner) : base(original, cloner) { }
    public Defun() : base(Defun.DefunName, Defun.DefunDescription) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Defun(this, cloner);
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new DefunTreeNode(this);
    }
  }
}
