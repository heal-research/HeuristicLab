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

namespace HeuristicLab.IGraph.Wrappers {
  public sealed class Matrix : IDisposable {
    igraph_matrix_t matrix;
    internal igraph_matrix_t NativeInstance { get { return matrix; } }
    public int Rows { get { return matrix.nrow; } }
    public int Columns { get { return matrix.ncol; } }

    public Matrix(int nrow, int ncol) {
      if (nrow < 0 || ncol < 0) throw new ArgumentException("Rows and Columns must be >= 0");
      matrix = new igraph_matrix_t();
      DllImporter.igraph_matrix_init(matrix, nrow, ncol);
    }
    public Matrix(Matrix other) {
      if (other == null) throw new ArgumentNullException("other");
      matrix = new igraph_matrix_t();
      DllImporter.igraph_matrix_copy(matrix, other.NativeInstance);
    }
    public Matrix(double[,] mat) {
      if (mat == null) throw new ArgumentNullException("mat");
      matrix = new igraph_matrix_t();
      var nrows = mat.GetLength(0);
      var ncols = mat.GetLength(1);
      DllImporter.igraph_matrix_init(matrix, nrows, ncols);
      var colwise = new double[ncols * nrows];
      for (var j = 0; j < ncols; j++)
        for (var i = 0; i < nrows; i++)
          colwise[j * nrows + i] = mat[i, j];
      DllImporter.igraph_vector_init_copy(matrix.data, colwise);
    }
    ~Matrix() {
      DllImporter.igraph_matrix_destroy(matrix);
    }

    public void Dispose() {
      if (matrix == null) return;
      DllImporter.igraph_matrix_destroy(matrix);
      matrix = null;
      GC.SuppressFinalize(this);
    }

    public void Fill(double v) {
      DllImporter.igraph_matrix_fill(matrix, v);
    }

    public void Transpose() {
      DllImporter.igraph_matrix_transpose(matrix);
    }

    public void Scale(double by) {
      DllImporter.igraph_matrix_scale(matrix, by);
    }

    public double this[int row, int col] {
      get {
        if (row < 0 || row > Rows || col < 0 || col > Columns) throw new IndexOutOfRangeException("Trying to get cell(" + row + ";" + col + ") of matrix(" + Rows + ";" + Columns + ").");
        return DllImporter.igraph_matrix_e(matrix, row, col);
      }
      set {
        if (row < 0 || row > Rows || col < 0 || col > Columns) throw new IndexOutOfRangeException("Trying to set cell(" + row + ";" + col + ") of matrix(" + Rows + ";" + Columns + ").");
        DllImporter.igraph_matrix_set(matrix, row, col, value);
      }
    }

    public double[,] ToMatrix() {
      var result = new double[Rows, Columns];
      for (var i = 0; i < Rows; i++) {
        for (var j = 0; j < Columns; j++) {
          result[i, j] = DllImporter.igraph_matrix_e(matrix, i, j);
        }
      }
      return result;
    }
  }
}
