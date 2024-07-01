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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("BoolConverter", "Converts a ValueTypeValue<bool>, ValueTypeArray<bool>, or ValueTypeMatrix<bool> and adds it to the SolutionMessage's BoolVars or BoolArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableType("8341D6D1-33C1-4CB7-AC4F-9A5447948022")]
  public class BoolConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<bool>), typeof(ValueTypeArray<bool>), typeof(ValueTypeMatrix<bool>) };
    [StorableConstructor]
    protected BoolConverter(StorableConstructorFlag _) : base(_) { }
    protected BoolConverter(BoolConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoolConverter(this, cloner);
    }
    public BoolConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage builder) {
      ValueTypeValue<bool> value = (item as ValueTypeValue<bool>);
      if (value != null) {
        SolutionMessage.Types.BoolVariable var = new SolutionMessage.Types.BoolVariable();
        var.Name = name;
        var.Data = value.Value;
        builder.BoolVars.Add(var);
      } else {
        ValueTypeArray<bool> array = (item as ValueTypeArray<bool>);
        if (array != null) {
          SolutionMessage.Types.BoolArrayVariable var = new SolutionMessage.Types.BoolArrayVariable();
          var.Name = name;
          var.Data.AddRange(array);
          var.Length = array.Length;
          builder.BoolArrayVars.Add(var);
        } else {
          ValueTypeMatrix<bool> matrix = (item as ValueTypeMatrix<bool>);
          if (matrix != null) {
            SolutionMessage.Types.BoolArrayVariable var = new SolutionMessage.Types.BoolArrayVariable();
            var.Name = name;
            var.Data.AddRange(matrix.AsEnumerable());
            var.Length = matrix.Columns;
            builder.BoolArrayVars.Add(var);
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
