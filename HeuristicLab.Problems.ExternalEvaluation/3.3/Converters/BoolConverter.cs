#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("BoolConverter", "Converts a ValueTypeValue<bool>, ValueTypeArray<bool>, or ValueTypeMatrix<bool> and adds it to the SolutionMessage's BoolVars or BoolArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableClass]
  public class BoolConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<bool>), typeof(ValueTypeArray<bool>), typeof(ValueTypeMatrix<bool>) };
    [StorableConstructor]
    protected BoolConverter(bool deserializing) : base(deserializing) { }
    protected BoolConverter(BoolConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoolConverter(this, cloner);
    }
    public BoolConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      ValueTypeValue<bool> value = (item as ValueTypeValue<bool>);
      if (value != null) {
        SolutionMessage.Types.BoolVariable.Builder var = SolutionMessage.Types.BoolVariable.CreateBuilder();
        var.SetName(name).SetData(value.Value);
        builder.AddBoolVars(var.Build());
      } else {
        ValueTypeArray<bool> array = (item as ValueTypeArray<bool>);
        if (array != null) {
          SolutionMessage.Types.BoolArrayVariable.Builder var = SolutionMessage.Types.BoolArrayVariable.CreateBuilder();
          var.SetName(name).AddRangeData(array).SetLength(array.Length);
          builder.AddBoolArrayVars(var.Build());
        } else {
          ValueTypeMatrix<bool> matrix = (item as ValueTypeMatrix<bool>);
          if (matrix != null) {
            SolutionMessage.Types.BoolArrayVariable.Builder var = SolutionMessage.Types.BoolArrayVariable.CreateBuilder();
            var.SetName(name).AddRangeData(matrix.AsEnumerable()).SetLength(matrix.Columns);
            builder.AddBoolArrayVars(var.Build());
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
