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
  [Item("DoubleConverter", "Converts a ValueTypeValue<double>, ValueTypeArray<double>, or ValueTypeMatrix<double> and adds it to the SolutionMessage's DoubleVars or DoubleArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableType("30B9F1EE-5D41-4EE3-97F5-39500BBC5172")]
  public class DoubleConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<double>), typeof(ValueTypeArray<double>), typeof(ValueTypeMatrix<double>) };

    [StorableConstructor]
    protected DoubleConverter(StorableConstructorFlag _) : base(_) { }
    protected DoubleConverter(DoubleConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleConverter(this, cloner);
    }
    public DoubleConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage builder) {
      ValueTypeValue<double> value = (item as ValueTypeValue<double>);
      if (value != null) {
        SolutionMessage.Types.DoubleVariable var = new SolutionMessage.Types.DoubleVariable();
        var.Name = name;
        var.Data = value.Value;
        builder.DoubleVars.Add(var);
      } else {
        ValueTypeArray<double> array = (item as ValueTypeArray<double>);
        if (array != null) {
          SolutionMessage.Types.DoubleArrayVariable var = new SolutionMessage.Types.DoubleArrayVariable();
          var.Name = name;
          var.Data.AddRange(array.AsEnumerable());
          var.Length = array.Length;
          builder.DoubleArrayVars.Add(var);
        } else {
          ValueTypeMatrix<double> matrix = (item as ValueTypeMatrix<double>);
          if (matrix != null) {
            SolutionMessage.Types.DoubleArrayVariable var = new SolutionMessage.Types.DoubleArrayVariable();
            var.Name = name;
            var.Data.AddRange(matrix.AsEnumerable());
            var.Length = matrix.Columns;
            builder.DoubleArrayVars.Add(var);
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
