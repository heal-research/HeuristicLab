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
  internal static class DllImporter {
    private const string X86Dll = "igraph-0.8.0-pre-x86.dll";
    private const string X64Dll = "igraph-0.8.0-pre-x64.dll";
    private readonly static bool X64 = false;

    static DllImporter() {
      X64 = Environment.Is64BitProcess;
    }

    #region igraph
    #region igraph init/finalize
    internal static void igraph_empty(igraph_t graph, int n, bool directed) {
      if (X64) igraph_empty_x64(graph, n, directed);
      else igraph_empty_x86(graph, n, directed);
    }
    internal static int igraph_destroy(igraph_t graph) {
      return X64 ? igraph_destroy_x64(graph) : igraph_destroy_x86(graph);
    }
    #endregion

    #region igraph query
    internal static int igraph_vcount(igraph_t graph) {
      return X64 ? igraph_vcount_x64(graph) : igraph_vcount_x86(graph);
    }
    #endregion

    #region igraph manipulation
    internal static int igraph_add_edge(igraph_t graph, int from, int to) {
      return X64 ? igraph_add_edge_x64(graph, from, to) : igraph_add_edge_x86(graph, from, to);
    }
    #endregion

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_empty", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_empty_x86([In, Out]igraph_t graph, int n, bool directed);
    [DllImport(X64Dll, EntryPoint = "igraph_empty", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_empty_x64([In, Out]igraph_t graph, int n, bool directed);
    [DllImport(X86Dll, EntryPoint = "igraph_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_destroy_x86([In, Out]igraph_t graph);
    [DllImport(X64Dll, EntryPoint = "igraph_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_destroy_x64([In, Out]igraph_t graph);
    [DllImport(X86Dll, EntryPoint = "igraph_vcount", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vcount_x86(igraph_t graph);
    [DllImport(X64Dll, EntryPoint = "igraph_vcount", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vcount_x64(igraph_t graph);
    [DllImport(X86Dll, EntryPoint = "igraph_add_edge", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_add_edge_x86([In, Out]igraph_t graph, int from, int to);
    [DllImport(X64Dll, EntryPoint = "igraph_add_edge", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_add_edge_x64([In, Out]igraph_t graph, int from, int to);
    #endregion
    #endregion

    #region igraph_rng
    internal static int igraph_rng_get_integer(int l, int h) {
      return X64 ? igraph_rng_get_integer_x64(igraph_rng_default_x64(), l, h) : igraph_rng_get_integer_x86(igraph_rng_default_x86(), l, h);
    }
    internal static int igraph_rng_seed(uint seed) {
      return X64 ? igraph_rng_seed_x64(igraph_rng_default_x64(), seed) : igraph_rng_seed_x86(igraph_rng_default_x86(), seed);
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_rng_default", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr igraph_rng_default_x86();
    [DllImport(X64Dll, EntryPoint = "igraph_rng_default", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr igraph_rng_default_x64();
    [DllImport(X86Dll, EntryPoint = "igraph_rng_get_integer", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_rng_get_integer_x86(IntPtr rng, int l, int h);
    [DllImport(X64Dll, EntryPoint = "igraph_rng_get_integer", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_rng_get_integer_x64(IntPtr rng, int l, int h);
    [DllImport(X86Dll, EntryPoint = "igraph_rng_seed", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_rng_seed_x86(IntPtr rng, uint seed);
    [DllImport(X64Dll, EntryPoint = "igraph_rng_seed", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_rng_seed_x64(IntPtr rng, uint seed);
    #endregion
    #endregion

    #region igraph_matrix
    internal static int igraph_matrix_init(igraph_matrix_t matrix, int nrows, int ncols) {
      return X64 ? igraph_matrix_init_x64(matrix, nrows, ncols) : igraph_matrix_init_x86(matrix, nrows, ncols);
    }
    internal static void igraph_matrix_destroy(igraph_matrix_t matrix) {
      if (X64) igraph_matrix_destroy_x64(matrix);
      else igraph_matrix_destroy_x86(matrix);
    }

    internal static double igraph_matrix_e(igraph_matrix_t matrix, int row, int col) {
      return X64 ? igraph_matrix_e_x64(matrix, row, col) : igraph_matrix_e_x86(matrix, row, col);
    }

    internal static void igraph_matrix_set(igraph_matrix_t matrix, int row, int col, double value) {
      if (X64) igraph_matrix_set_x64(matrix, row, col, value);
      else igraph_matrix_set_x86(matrix, row, col, value);
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_init", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_init_x86([In, Out]igraph_matrix_t matrix, int nrow, int ncol);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_init", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_init_x64([In, Out]igraph_matrix_t matrix, int nrow, int ncol);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_matrix_destroy_x86([In, Out]igraph_matrix_t matrix);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_matrix_destroy_x64([In, Out]igraph_matrix_t matrix);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_e", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_matrix_e_x86(igraph_matrix_t matrix, int row, int col);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_e", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_matrix_e_x64(igraph_matrix_t matrix, int row, int col);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_set", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_matrix_set_x86(igraph_matrix_t matrix, int row, int col, double value);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_set", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_matrix_set_x64(igraph_matrix_t matrix, int row, int col, double value);
    #endregion
    #endregion

    #region igraph_layout
    internal static int igraph_layout_fruchterman_reingold(igraph_t graph, igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy) {
      return X64 ? igraph_layout_fruchterman_reingold_x64(graph, res, use_seed, niter, start_temp, grid, weights, minx, maxx, miny, maxy) : igraph_layout_fruchterman_reingold_x86(graph, res, use_seed, niter, start_temp, grid, weights, minx, maxx, miny, maxy);
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_layout_fruchterman_reingold", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_fruchterman_reingold_x86(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    [DllImport(X64Dll, EntryPoint = "igraph_layout_fruchterman_reingold", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_fruchterman_reingold_x64(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    #endregion
    #endregion

  }
}
