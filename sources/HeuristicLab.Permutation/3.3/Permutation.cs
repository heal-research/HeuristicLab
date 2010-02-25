#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  [EmptyStorableClass]
  [Item("Permutation", "Represents a permutation of integer values.")]
  [Creatable("Test")]
  public sealed class Permutation : ValueTypeArrayData<int>, IStringConvertibleMatrixData {
    public Permutation() : base() { }
    public Permutation(int length)
      : base(length) {
      for (int i = 0; i < length; i++)
        this[i] = i;
    }
    public Permutation(int length, IRandom random)
      : this(length) {
      Randomize(random);
    }
    public Permutation(int[] elements)
      : base(elements) {
    }
    private Permutation(Permutation elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      Permutation clone = new Permutation(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public bool Validate() {
      bool[] values = new bool[Length];
      int value;

      for (int i = 0; i < values.Length; i++)
        values[i] = false;
      for (int i = 0; i < Length; i++) {
        value = this[i];
        if ((value < 0) || (value >= values.Length)) return false;
        if (values[value]) return false;
        values[value] = true;
      }
      return true;
    }

    public void Randomize(IRandom random, int startIndex, int length) {  // Knuth shuffle
      int index1, index2;
      int val;
      for (int i = length - 1; i > 0; i--) {
        index1 = startIndex + i;
        index2 = startIndex + random.Next(i + 1);
        if (index1 != index2) {
          val = this[index1];
          this[index1] = this[index2];
          this[index2] = val;
        }
      }
    }
    public void Randomize(IRandom random) {
      Randomize(random, 0, Length);
    }

    #region IStringConvertibleMatrixData Members
    StringConvertibleArrayDataDimensions IStringConvertibleMatrixData.Dimensions {
      get { return StringConvertibleArrayDataDimensions.Rows; }
    }
    int IStringConvertibleMatrixData.Rows {
      get { return Length; }
      set { Length = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return 1; }
      set { throw new NotSupportedException("The number of columns cannot be changed."); }
    }

    bool IStringConvertibleMatrixData.Validate(string value, out string errorMessage) {
      int val;
      bool valid = int.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetIntFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    string IStringConvertibleMatrixData.GetValue(int rowIndex, int columIndex) {
      return this[rowIndex].ToString();
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      int val;
      if (int.TryParse(value, out val)) {
        this[rowIndex] = val;
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
