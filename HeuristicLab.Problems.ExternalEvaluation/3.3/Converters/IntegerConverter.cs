#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("IntegerConverter", "Converts a ValueTypeValue<int>, ValueTypeArray<int>, or ValueTypeMatrix<int> and adds it to the SolutionMessage's IntegerVars or IntegerArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableClass]
  public class IntegerConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<int>), typeof(ValueTypeArray<int>), typeof(ValueTypeMatrix<int>) };

    [StorableConstructor]
    protected IntegerConverter(bool deserializing) : base(deserializing) { }
    protected IntegerConverter(IntegerConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerConverter(this, cloner);
    }
    public IntegerConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      ValueTypeValue<int> value = (item as ValueTypeValue<int>);
      if (value != null) {
        SolutionMessage.Types.IntegerVariable.Builder var = SolutionMessage.Types.IntegerVariable.CreateBuilder();
        var.SetName(name).SetData(value.Value);
        builder.AddIntegerVars(var.Build());
      } else {
        ValueTypeArray<int> array = (item as ValueTypeArray<int>);
        if (array != null) {
          SolutionMessage.Types.IntegerArrayVariable.Builder var = SolutionMessage.Types.IntegerArrayVariable.CreateBuilder();
          var.SetName(name).AddRangeData(array).SetLength(array.Length);
          builder.AddIntegerArrayVars(var.Build());
        } else {
          ValueTypeMatrix<int> matrix = (item as ValueTypeMatrix<int>);
          if (matrix != null) {
            SolutionMessage.Types.IntegerArrayVariable.Builder var = SolutionMessage.Types.IntegerArrayVariable.CreateBuilder();
            var.SetName(name).AddRangeData(matrix.AsEnumerable()).SetLength(matrix.Columns);
            builder.AddIntegerArrayVars(var.Build());
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
