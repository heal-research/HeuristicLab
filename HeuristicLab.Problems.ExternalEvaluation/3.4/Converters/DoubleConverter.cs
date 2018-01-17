#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("DoubleConverter", "Converts a ValueTypeValue<double>, ValueTypeArray<double>, or ValueTypeMatrix<double> and adds it to the SolutionMessage's DoubleVars or DoubleArrayVars. A matrix is encoded as array by concatenating all rows and setting length as the length of a row.")]
  [StorableClass]
  public class DoubleConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(ValueTypeValue<double>), typeof(ValueTypeArray<double>), typeof(ValueTypeMatrix<double>) };

    [StorableConstructor]
    protected DoubleConverter(bool deserializing) : base(deserializing) { }
    protected DoubleConverter(DoubleConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleConverter(this, cloner);
    }
    public DoubleConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      ValueTypeValue<double> value = (item as ValueTypeValue<double>);
      if (value != null) {
        SolutionMessage.Types.DoubleVariable.Builder var = SolutionMessage.Types.DoubleVariable.CreateBuilder();
        var.SetName(name).SetData(value.Value);
        builder.AddDoubleVars(var.Build());
      } else {
        ValueTypeArray<double> array = (item as ValueTypeArray<double>);
        if (array != null) {
          SolutionMessage.Types.DoubleArrayVariable.Builder var = SolutionMessage.Types.DoubleArrayVariable.CreateBuilder();
          var.SetName(name).AddRangeData(array).SetLength(array.Length);
          builder.AddDoubleArrayVars(var.Build());
        } else {
          ValueTypeMatrix<double> matrix = (item as ValueTypeMatrix<double>);
          if (matrix != null) {
            SolutionMessage.Types.DoubleArrayVariable.Builder var = SolutionMessage.Types.DoubleArrayVariable.CreateBuilder();
            var.SetName(name).AddRangeData(matrix.AsEnumerable()).SetLength(matrix.Columns);
            builder.AddDoubleArrayVars(var.Build());
          } else {
            throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
          }
        }
      }
    }

    #endregion
  }
}
