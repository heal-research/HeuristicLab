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
using System.Collections.Generic;
using System.Linq;

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
    public Vector(IEnumerable<double> data) {
      if (data == null) throw new ArgumentNullException("data");
      var vec = data.ToArray();
      vector = new igraph_vector_t();
      DllImporter.igraph_vector_init_copy(vector, vec);
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

    public void Fill(double v) {
      DllImporter.igraph_vector_fill(vector, v);
    }

    public void Reverse() {
      DllImporter.igraph_vector_reverse(vector);
    }

    public void Shuffle() {
      DllImporter.igraph_vector_shuffle(vector);
    }

    public void Scale(double by) {
      DllImporter.igraph_vector_scale(vector, by);
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
      return DllImporter.igraph_vector_to_array(vector);
    }
  }
}
