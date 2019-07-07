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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [Item("SymbolicExpressionTreeStringConverter", "Abstract base class for symbolic expression tree converters.")]
  [StorableType("AA6D2726-06C7-4E12-92EC-24BFADAE7402")]
  public abstract class SymbolicExpressionTreeConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(SymbolicExpressionTree) };

    [StorableConstructor]
    protected SymbolicExpressionTreeConverter(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeConverter(SymbolicExpressionTreeConverter original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicExpressionTreeConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      SymbolicExpressionTree tree = (item as SymbolicExpressionTree);
      if (tree != null) {
        ConvertSymbolicExpressionTree(tree, name, builder);
      }
    }

    protected abstract void ConvertSymbolicExpressionTree(SymbolicExpressionTree tree, string name, SolutionMessage.Builder builder);

    #endregion
  }
}
