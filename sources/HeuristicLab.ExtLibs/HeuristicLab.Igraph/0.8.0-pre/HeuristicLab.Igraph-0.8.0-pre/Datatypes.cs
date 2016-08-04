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
using System.Runtime.InteropServices;

namespace HeuristicLab.igraph {
  #region Structs
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal class igraph_rng_type_t {
    IntPtr name;
    internal uint min;
    internal uint max;
  };

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal class igraph_rng_t {
    IntPtr type;
    IntPtr state;
    internal int def;
  };


  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal class igraph_vector_t {
    IntPtr stor_begin;
    IntPtr stor_end;
    IntPtr end;
  };

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal class igraph_matrix_t {
    igraph_vector_t data = new igraph_vector_t();
    internal int nrow;
    internal int ncol;
  };

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal class igraph_t {
    internal int n;
    internal bool directed;
    igraph_vector_t from = new igraph_vector_t();
    igraph_vector_t to = new igraph_vector_t();
    igraph_vector_t oi = new igraph_vector_t();
    igraph_vector_t ii = new igraph_vector_t();
    igraph_vector_t os = new igraph_vector_t();
    igraph_vector_t @is = new igraph_vector_t();
    IntPtr attr;
  };
  #endregion

  #region Enums
  internal enum igraph_layout_grid_t {
    IGRAPH_LAYOUT_GRID,
    IGRAPH_LAYOUT_NOGRID,
    IGRAPH_LAYOUT_AUTOGRID
  };
  #endregion
}
