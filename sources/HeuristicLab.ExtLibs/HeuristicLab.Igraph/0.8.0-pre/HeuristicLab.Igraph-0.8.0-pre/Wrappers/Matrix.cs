using System;
using System.Runtime.InteropServices;

namespace HeuristicLab.igraph.Wrappers {
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  public class Matrix : IDisposable {
    igraph_matrix_t matrix;
    internal igraph_matrix_t NativeInstance { get { return matrix; } }
    public int Rows { get { return matrix.nrow; } }
    public int Columns { get { return matrix.ncol; } }

    public Matrix(int nrow, int ncol) {
      matrix = new igraph_matrix_t();
      DllImporter.igraph_matrix_init(matrix, nrow, ncol);
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
