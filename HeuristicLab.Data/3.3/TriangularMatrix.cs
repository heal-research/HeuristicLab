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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("TriangularMatrix", "Represents a lower triangular matrix.")]
  [StorableClass]
  public class TriangularMatrix<T> : ValueTypeMatrix<T>, IStringConvertibleMatrix where T : struct {
    [Storable]
    private readonly T[] storage;

    private readonly int dimension;
    private readonly long capacity;

    private TriangularMatrix() { }

    public TriangularMatrix(int dimension) {
      this.dimension = dimension;
      capacity = (long)dimension * (dimension + 1) / 2;
      storage = new T[capacity];

      readOnly = true;
    }

    [StorableConstructor]
    protected TriangularMatrix(bool deserializing) : base(deserializing) { }

    protected TriangularMatrix(TriangularMatrix<T> original, Cloner cloner) : base(original, cloner) {
      capacity = original.capacity;
      dimension = original.dimension;
      storage = (T[])original.storage.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TriangularMatrix<T>(this, cloner);
    }

    public override int Rows {
      get { return dimension; }
      protected set { throw new NotSupportedException(); }
    }

    public override int Columns {
      get { return dimension; }
      protected set { throw new NotSupportedException(); }
    }

    public int Length { get { return storage.Length; } }

    // the indexing rule for the (lower-)triangular matrix is that always i <= j, otherwise an IndexOutOfBounds exception will occur 
    public override T this[int rowIndex, int columnIndex] {
      get {
        // provide symmetry of returned values 
        if (columnIndex > rowIndex) return this[columnIndex, rowIndex];
        return storage[rowIndex * (rowIndex + 1) / 2 + columnIndex];
      }
      set {
        if (columnIndex > rowIndex) this[columnIndex, rowIndex] = value;
        else storage[rowIndex * (rowIndex + 1) / 2 + columnIndex] = value;
      }
    }

    public T this[int index] {
      get { return storage[index]; }
      set { storage[index] = value; }
    }

    protected virtual string GetValue(int rowIndex, int columnIndex) {
      return this[rowIndex, columnIndex].ToString(); // see above indexing rule
    }

    protected virtual bool SetValue(string value, int rowIndex, int columnIndex) {
      T val;
      if (!TryParse(value, out val))
        return false;
      this[rowIndex, columnIndex] = val;
      return true;
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      T val;
      errorMessage = "";
      if (!TryParse(value, out val)) {
        errorMessage = string.Format("Could not parse string \"{0}\" as {1}.", value, typeof(T));
        return false;
      }
      return true;
    }

    public override IEnumerator<T> GetEnumerator() {
      return storage.Cast<T>().GetEnumerator();
    }

    private static bool TryParse(string value, out T val) {
      if (typeof(T) == typeof(sbyte)) {
        sbyte v;
        if (sbyte.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(byte)) {
        byte v;
        if (byte.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(char)) {
        char v;
        if (char.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(short)) {
        short v;
        if (short.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(ushort)) {
        ushort v;
        if (ushort.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(int)) {
        int v;
        if (int.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(uint)) {
        uint v;
        if (uint.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(long)) {
        long v;
        if (long.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      } else if (typeof(T) == typeof(ulong)) {
        ulong v;
        if (ulong.TryParse(value, out v)) {
          val = (T)(object)v;
          return true;
        }
      }
      val = default(T);
      return false;
    }

    public void GetMatrixCoordinates(int index, out int row, out int col) {
      var root = TriangularRoot(index);
      row = (int)Math.Floor(root);
      col = index - row * (row + 1) / 2;
    }

    private static double TriangularRoot(double x) {
      return (Math.Sqrt(8 * x + 1) - 1) / 2;
    }

    #region IStringConvertibleMatrix Members
    int IStringConvertibleMatrix.Rows {
      get { return dimension; }
      set { throw new NotSupportedException("The triangular matrix does not support changing the number of rows."); }
    }

    int IStringConvertibleMatrix.Columns {
      get { return dimension; }
      set { throw new NotSupportedException("The triangular matrix does not support changing the number of columns."); }
    }

    string IStringConvertibleMatrix.GetValue(int rowIndex, int columnIndex) {
      return GetValue(rowIndex, columnIndex);
    }

    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      return SetValue(value, rowIndex, columnIndex);
    }

    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    #endregion
  }
}
