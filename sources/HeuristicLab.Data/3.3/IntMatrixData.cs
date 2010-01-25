#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [EmptyStorableClass]
  [Item("IntMatrixData", "Represents a matrix of integer values.")]
  [Creatable("Test")]
  public sealed class IntMatrixData : ValueTypeMatrixData<int>, IStringConvertibleMatrixData {
    public IntMatrixData() : base() { }
    public IntMatrixData(int rows, int columns) : base(rows, columns) { }
    public IntMatrixData(int[,] elements) : base(elements) { }
    private IntMatrixData(IntMatrixData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      IntMatrixData clone = new IntMatrixData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleMatrixData Members
    StringConvertibleArrayDataDimensions IStringConvertibleMatrixData.Dimensions {
      get { return StringConvertibleArrayDataDimensions.Both; }
    }
    int IStringConvertibleMatrixData.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return Columns; }
      set { Columns = value; }
    }

    bool IStringConvertibleMatrixData.Validate(string value) {
      int i;
      return int.TryParse(value, out i);
    }
    string IStringConvertibleMatrixData.GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex].ToString();
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      int i;
      if (int.TryParse(value, out i)) {
        this[rowIndex, columnIndex] = i;
        return true;
      } else {
        return false;
      }
    }
    event EventHandler<EventArgs<int, int>> IStringConvertibleMatrixData.ItemChanged {
      add { base.ItemChanged += value; }
      remove { base.ItemChanged -= value; }
    }
    event EventHandler IStringConvertibleMatrixData.Reset {
      add { base.Reset += value; }
      remove { base.Reset -= value; }
    }
    #endregion
  }
}
