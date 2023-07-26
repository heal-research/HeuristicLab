﻿#region License Information
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
  [Item("IntegerConverter", "Converts a ValueTypeValue<int>, ValueTypeArray<int>, or ValueTypeMatrix<int> and adds it to the SolutionMessage's IntegerVars or IntegerArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableType("540B4803-05EC-4AB8-AFAC-A8E35A98F4D7")]
  public class IntegerConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<int>), typeof(ValueTypeArray<int>), typeof(ValueTypeMatrix<int>) };

    [StorableConstructor]
    protected IntegerConverter(StorableConstructorFlag _) : base(_) { }
    protected IntegerConverter(IntegerConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerConverter(this, cloner);
    }
    public IntegerConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage builder) {
      ValueTypeValue<int> value = (item as ValueTypeValue<int>);
      if (value != null) {
        SolutionMessage.Types.IntegerVariable var = new SolutionMessage.Types.IntegerVariable();
        var.Name = name;
        var.Data = value.Value;
        builder.IntegerVars.Add(var);
      } else {
        ValueTypeArray<int> array = (item as ValueTypeArray<int>);
        if (array != null) {
          SolutionMessage.Types.IntegerArrayVariable var = new SolutionMessage.Types.IntegerArrayVariable();
          var.Name = name;
          var.Data.AddRange(array);
          var.Length = array.Length;
          builder.IntegerArrayVars.Add(var);
        } else {
          ValueTypeMatrix<int> matrix = (item as ValueTypeMatrix<int>);
          if (matrix != null) {
            SolutionMessage.Types.IntegerArrayVariable var = new SolutionMessage.Types.IntegerArrayVariable();
            var.Name = name;
            var.Data.AddRange(matrix.AsEnumerable());
            var.Length = matrix.Columns;
            builder.IntegerArrayVars.Add(var);
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
