#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class Vector : IDisposable {
    private igraph_vector_t vector;

    internal igraph_vector_t NativeInstance {
      get { return vector; }
    }

    public int Length {
      get { return DllImporter.igraph_vector_size(vector); }
    }

    public Vector(int length) {
      if (length < 0) throw new ArgumentException("Rows and Columns must be >= 0");
      vector = new igraph_vector_t();
      DllImporter.igraph_vector_init(vector, length);
    }

    public Vector(Vector other) {
      if (other == null) throw new ArgumentNullException("other");
      vector = new igraph_vector_t();
      DllImporter.igraph_vector_copy(vector, other.NativeInstance);
    }

    ~Vector() {
      DllImporter.igraph_vector_destroy(vector);
    }

    public void Dispose() {
      if (vector == null) return;
      DllImporter.igraph_vector_destroy(vector);
      vector = null;
      GC.SuppressFinalize(this);
    }

    public double this[int index] {
      get {
        if (index < 0 || index > Length) throw new IndexOutOfRangeException("Trying to get index(" + index + ") of vector(" + Length + ").");
        return DllImporter.igraph_vector_e(vector, index);
      }
      set {
        if (index < 0 || index > Length) throw new IndexOutOfRangeException("Trying to set index(" + index + ") of vector(" + Length + ").");
        DllImporter.igraph_vector_set(vector, index, value);
      }
    }

    public double[] ToArray() {
      var result = new double[Length];
      for (var i = 0; i < result.Length; i++) {
        result[i] = DllImporter.igraph_vector_e(vector, i);
      }
      return result;
    }
  }
}
