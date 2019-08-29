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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("StringConverter", "Converts a StringValue, StringArray, StringMatrix, IStringConvertibleValue, IStringConvertibleArray, or IStringConvertibleMatrix and adds it to the SolutionMessage's StringVars or StringArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableType("FD48F37D-DA07-442D-85B9-EC7D5D1A6740")]
  public class StringConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(StringValue), typeof(StringArray), typeof(StringMatrix), typeof(IStringConvertibleValue), typeof(IStringConvertibleArray), typeof(IStringConvertibleMatrix) };

    [StorableConstructor]
    protected StringConverter(StorableConstructorFlag _) : base(_) { }
    protected StringConverter(StringConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new StringConverter(this, cloner);
    }
    public StringConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      IStringConvertibleValue value = (item as IStringConvertibleValue);
      if (value != null) {
        SolutionMessage.Types.StringVariable.Builder var = SolutionMessage.Types.StringVariable.CreateBuilder();
        var.SetName(name).SetData(value.GetValue());
        builder.AddStringVars(var.Build());
      } else {
        IStringConvertibleArray array = (item as IStringConvertibleArray);
        if (array != null) {
          SolutionMessage.Types.StringArrayVariable.Builder var = SolutionMessage.Types.StringArrayVariable.CreateBuilder();
          var.SetName(name).SetLength(array.Length);
          for (int i = 0; i < array.Length; i++)
            var.AddData(array.GetValue(i));
          builder.AddStringArrayVars(var.Build());
        } else {
          IStringConvertibleMatrix matrix = (item as IStringConvertibleMatrix);
          if (matrix != null) {
            SolutionMessage.Types.StringArrayVariable.Builder var = SolutionMessage.Types.StringArrayVariable.CreateBuilder();
            var.SetName(name).SetLength(matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
              for (int j = 0; j < matrix.Columns; j++)
                var.AddData(matrix.GetValue(i, j));
            builder.AddStringArrayVars(var.Build());
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
