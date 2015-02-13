#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [NonDiscoverableType]
  internal class Matrix : IEnumerable<double>, IDeepCloneable {
    // this type is immutable
    private readonly IEnumerable<double> values;
    public readonly int Rows;
    public readonly int Columns;

    protected Matrix(Matrix original, Cloner cloner) {
      this.values = original.values.ToArray();
      this.Rows = original.Rows;
      this.Columns = original.Columns;
      cloner.RegisterClonedObject(original, this);
    }
    public Matrix(IEnumerable<double> vector) {
      this.values = vector;
      Rows = 1;
      Columns = vector.Count();
    }
    public Matrix(IEnumerable<double> vector, int length) {
      this.values = vector;
      Rows = 1;
      Columns = length;
    }
    public Matrix(double[,] matrix) {
      this.values = GetOnlineValues(matrix);
      Rows = matrix.GetLength(0);
      Columns = matrix.GetLength(1);
    }
    public Matrix(IEnumerable<double> matrix, int rows, int columns) {
      this.values = matrix;
      Rows = rows;
      Columns = columns;
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public IDeepCloneable Clone(Cloner cloner) {
      return new Matrix(this, cloner);
    }

    public Matrix Transpose() {
      return new Matrix(Transpose(values, Columns, Rows), Columns, Rows);
    }

    private IEnumerable<double> Transpose(IEnumerable<double> values, int rows, int columns) {
      // vectors don't need to be transposed
      if (rows == 1 || columns == 1) {
        foreach (var v in values) yield return v;
        yield break;
      }

      int skip = 0;
      var iter = values.GetEnumerator();
      if (!iter.MoveNext()) yield break;
      while (skip < rows) {
        for (int i = 0; i < skip; i++) iter.MoveNext();
        yield return iter.Current;
        for (int j = 0; j < columns - 1; j++) {
          for (int i = 0; i < rows; i++) iter.MoveNext();
          yield return iter.Current;
        }
        skip++;
        if (skip < rows) {
          iter = values.GetEnumerator();
          iter.MoveNext();
        }
      }
    }

    public Matrix Add(Matrix other) {
      return new Matrix(AddOnline(other), Rows, Columns);
    }

    public void AddTo(double[,] matrix) {
      if (Rows != matrix.GetLength(0) || Columns != matrix.GetLength(1)) throw new ArgumentException("unequal size", "matrix");
      var iter = values.GetEnumerator();
      for (int i = 0; i < Rows; i++)
        for (int j = 0; j < Columns; j++) {
          iter.MoveNext();
          matrix[i, j] += iter.Current;
        }
    }

    public Matrix Subtract(Matrix other) {
      return new Matrix(SubtractOnline(other), Rows, Columns);
    }

    public Matrix Multiply(Matrix other) {
      return new Matrix(MultiplyOnline(other), Rows, other.Columns);
    }

    public Matrix Multiply(double value) {
      return new Matrix(values.Select(x => x * value), Rows, Columns);
    }

    public double EuclideanNorm() {
      return Math.Sqrt(SumOfSquares());
    }

    public double SumOfSquares() {
      return values.Sum(x => x * x);
    }

    public Matrix OuterProduct(Matrix other) {
      if (Rows != 1 || other.Rows != 1) throw new ArgumentException("OuterProduct can only be applied to vectors.");
      return Transpose().Multiply(other);
    }

    public IEnumerable<double> ColumnSums() {
      return Transpose().RowSums();
    }

    public IEnumerable<double> RowSums() {
      var sum = 0.0;
      int counter = 0;
      foreach (var v in values) {
        sum += v;
        counter++;
        if (counter == Rows) {
          yield return sum;
          sum = 0.0;
          counter = 0;
        }
      }
    }

    public Matrix Negate() {
      return new Matrix(values.Select(x => -x), Rows, Columns);
    }

    public Matrix Apply() {
      return new Matrix(values.ToArray(), Rows, Columns);
    }

    public IEnumerator<double> GetEnumerator() { return values.GetEnumerator(); }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


    private IEnumerable<double> AddOnline(Matrix other) {
      if (Rows != other.Rows || Columns != other.Columns) throw new ArgumentException("Number of rows and columns are not equal.");
      var meIter = values.GetEnumerator();
      var otherIter = other.GetEnumerator();
      if (!meIter.MoveNext()) yield break;
      if (!otherIter.MoveNext()) yield break;
      for (int i = 0; i < Rows * Columns; i++) {
        yield return meIter.Current + otherIter.Current;
        meIter.MoveNext();
        otherIter.MoveNext();
      }
    }

    private IEnumerable<double> SubtractOnline(Matrix other) {
      if (Rows != other.Rows || Columns != other.Columns) throw new ArgumentException("Number of rows and columns are not equal.");
      var meIter = values.GetEnumerator();
      var otherIter = other.GetEnumerator();
      if (!meIter.MoveNext()) yield break;
      if (!otherIter.MoveNext()) yield break;
      for (int i = 0; i < Rows * Columns; i++) {
        yield return meIter.Current - otherIter.Current;
        meIter.MoveNext();
        otherIter.MoveNext();
      }
    }

    private IEnumerable<double> MultiplyOnline(Matrix other) {
      if (Columns != other.Rows) throw new ArgumentException("Number of rows and columns are not equal.");
      var meIter = values.GetEnumerator();
      var otherByColumn = other.Transpose();
      var otherIter = otherByColumn.GetEnumerator();
      if (!meIter.MoveNext()) yield break;
      if (!otherIter.MoveNext()) yield break;
      for (int r = 0; r < Rows; r++) {
        var row = new double[Columns];
        for (int x = 0; x < Columns; x++) {
          row[x] = meIter.Current;
          meIter.MoveNext();
        }
        for (int c = 0; c < other.Columns; c++) {
          var sum = 0.0;
          for (int y = 0; y < other.Rows; y++) {
            sum += row[y] * otherIter.Current;
            otherIter.MoveNext();
          }
          yield return sum;
        }
        otherIter = otherByColumn.GetEnumerator();
        otherIter.MoveNext();
      }
    }

    private IEnumerable<double> GetOnlineValues(double[,] matrix) {
      for (int i = 0; i < matrix.GetLength(0); i++)
        for (int j = 0; j < matrix.GetLength(1); j++) {
          yield return matrix[i, j];
        }
    }
  }
}
