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
using System.IO;
using System.Runtime.InteropServices;

namespace HeuristicLab.IGraph {
  internal static class DllImporter {
    private const string X86Dll = "igraph-0.8.0-pre-x86.dll";
    private const string X64Dll = "igraph-0.8.0-pre-x64.dll";
    private readonly static bool X86 = false;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetDllDirectory(string lpPathName);

    static DllImporter() {
      X86 = !Environment.Is64BitProcess;
      // Possible workaround as the builder doesn't execute the tests within the bin directory
      if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "bin")))
        SetDllDirectory(Path.Combine(Environment.CurrentDirectory, "bin"));
    }

    #region igraph
    #region igraph init/finalize
    internal static void igraph_empty(igraph_t graph, int n, bool directed) {
      if (X86) igraph_empty_x86(graph, n, directed);
      else igraph_empty_x64(graph, n, directed);
    }
    internal static int igraph_destroy(igraph_t graph) {
      return X86 ? igraph_destroy_x86(graph) : igraph_destroy_x64(graph);
    }
    #endregion

    #region igraph query
    internal static int igraph_vcount(igraph_t graph) {
      return X86 ? igraph_vcount_x86(graph) : igraph_vcount_x64(graph);
    }
    #endregion

    #region igraph characteristics
    internal static int igraph_density(igraph_t graph, out double density, bool loops) {
      density = double.NaN;
      return X86 ? igraph_density_x86(graph, ref density, loops) : igraph_density_x64(graph, ref density, loops);
    }

    internal static int igraph_pagerank(igraph_t graph, igraph_pagerank_algo_t algo, igraph_vector_t vector, out double value, igraph_vs_t vids, bool directed, double damping, igraph_vector_t weights) {
      value = 1;
      var options = IntPtr.Zero;
      if (algo == igraph_pagerank_algo_t.IGRAPH_PAGERANK_ALGO_ARPACK) {
        var arpackoptions = GetDefaultArpackOptions();
        options = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(igraph_arpack_options_t)));
        Marshal.StructureToPtr(arpackoptions, options, false);
      } else if (algo == igraph_pagerank_algo_t.IGRAPH_PAGERANK_ALGO_POWER) {
        var poweroptions = GetDefaultPowerOptions();
        options = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(igraph_pagerank_power_options_t)));
        Marshal.StructureToPtr(poweroptions, options, false);
      }
      try {
        return X86 ? igraph_pagerank_x86(graph, algo, vector, ref value, vids, directed, damping, weights, options) : igraph_pagerank_x64(graph, algo, vector, ref value, vids, directed, damping, weights, options);
      } finally {
        if (algo == igraph_pagerank_algo_t.IGRAPH_PAGERANK_ALGO_ARPACK) {
          Marshal.DestroyStructure(options, typeof(igraph_arpack_options_t));
          Marshal.FreeHGlobal(options);
        } else if (algo == igraph_pagerank_algo_t.IGRAPH_PAGERANK_ALGO_POWER) {
          Marshal.DestroyStructure(options, typeof(igraph_pagerank_power_options_t));
          Marshal.FreeHGlobal(options);
        }
      }
    }

    unsafe private static igraph_arpack_options_t GetDefaultArpackOptions() {
      var arpackoptions = new igraph_arpack_options_t();
      arpackoptions.bmat[0] = 'I';
      arpackoptions.which[0] = arpackoptions.which[1] = 'X';
      arpackoptions.nev = 1;
      arpackoptions.ishift = 1;
      arpackoptions.mxiter = 1000;
      arpackoptions.nb = 1;
      arpackoptions.mode = 1;
      arpackoptions.iparam[0] = arpackoptions.ishift;
      arpackoptions.iparam[1] = arpackoptions.iparam[4] = arpackoptions.iparam[5] = arpackoptions.iparam[7] = arpackoptions.iparam[8] = arpackoptions.iparam[9] = arpackoptions.iparam[10] = 0;
      arpackoptions.iparam[2] = arpackoptions.mxiter;
      arpackoptions.iparam[3] = arpackoptions.nb;
      arpackoptions.iparam[6] = arpackoptions.mode;
      return arpackoptions;
    }

    private static igraph_pagerank_power_options_t GetDefaultPowerOptions() {
      var poweroptions = new igraph_pagerank_power_options_t();
      poweroptions.niter = 50;
      poweroptions.eps = 1e-5;
      return poweroptions;
    }
    #endregion

    #region igraph manipulation
    internal static int igraph_add_edge(igraph_t graph, int from, int to) {
      return X86 ? igraph_add_edge_x86(graph, from, to) : igraph_add_edge_x64(graph, from, to);
    }
    #endregion

    #region igraph visits
    internal static int igraph_bfs(igraph_t graph, int root, igraph_vector_t roots, igraph_neimode_t mode, bool unreachable, igraph_vector_t restricted, igraph_vector_t order, igraph_vector_t rank, igraph_vector_t father, igraph_vector_t pred, igraph_vector_t succ, igraph_vector_t dist, igraph_bfshandler_t callback, object tag) {
      return MarshalIfExistsAndCall(tag, ptr => X86 ? igraph_bfs_x86(graph, root, roots, mode, unreachable, restricted, order, rank, father, pred, succ, dist, callback, ptr) : igraph_bfs_x64(graph, root, roots, mode, unreachable, restricted, order, rank, father, pred, succ, dist, callback, ptr));
    }
    internal static int igraph_dfs(igraph_t graph, int root, igraph_neimode_t mode, bool unreachable, igraph_vector_t order, igraph_vector_t order_out, igraph_vector_t father, igraph_vector_t dist, igraph_dfshandler_t inWrapper, igraph_dfshandler_t outWrapper, object tag) {
      return MarshalIfExistsAndCall(tag, ptr => X86 ? igraph_dfs_x86(graph, root, mode, unreachable, order, order_out, father, dist, inWrapper, outWrapper, ptr) : igraph_dfs_x64(graph, root, mode, unreachable, order, order_out, father, dist, inWrapper, outWrapper, ptr));
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
    [DllImport(X86Dll, EntryPoint = "igraph_density", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_density_x86(igraph_t graph, ref double density, bool loops);
    [DllImport(X64Dll, EntryPoint = "igraph_density", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_density_x64(igraph_t graph, ref double density, bool loops);
    [DllImport(X86Dll, EntryPoint = "igraph_pagerank", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_pagerank_x86(igraph_t graph, igraph_pagerank_algo_t algo, [In, Out]igraph_vector_t vector, ref double value, igraph_vs_t vids, bool directed, double damping, igraph_vector_t weights, IntPtr options);
    [DllImport(X64Dll, EntryPoint = "igraph_pagerank", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_pagerank_x64(igraph_t graph, igraph_pagerank_algo_t algo, [In, Out]igraph_vector_t vector, ref double value, igraph_vs_t vids, bool directed, double damping, igraph_vector_t weights, IntPtr options);
    [DllImport(X86Dll, EntryPoint = "igraph_bfs", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_bfs_x86(igraph_t graph, int root, igraph_vector_t roots, igraph_neimode_t mode, bool unreachable, igraph_vector_t restricted, [In, Out]igraph_vector_t order, [In, Out]igraph_vector_t rank, [In, Out]igraph_vector_t father, [In, Out]igraph_vector_t pred, [In, Out]igraph_vector_t succ, [In, Out]igraph_vector_t dist, igraph_bfshandler_t callback, IntPtr extra);
    [DllImport(X64Dll, EntryPoint = "igraph_bfs", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_bfs_x64(igraph_t graph, int root, igraph_vector_t roots, igraph_neimode_t mode, bool unreachable, igraph_vector_t restricted, [In, Out]igraph_vector_t order, [In, Out]igraph_vector_t rank, [In, Out]igraph_vector_t father, [In, Out]igraph_vector_t pred, [In, Out]igraph_vector_t succ, [In, Out]igraph_vector_t dist, igraph_bfshandler_t callback, IntPtr extra);
    [DllImport(X86Dll, EntryPoint = "igraph_dfs", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_dfs_x86(igraph_t graph, int root, igraph_neimode_t mode, bool unreachable, [In, Out]igraph_vector_t order, [In, Out]igraph_vector_t order_out, [In, Out]igraph_vector_t father, [In, Out]igraph_vector_t dist, igraph_dfshandler_t in_callback, igraph_dfshandler_t out_callback, IntPtr extra);
    [DllImport(X64Dll, EntryPoint = "igraph_dfs", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_dfs_x64(igraph_t graph, int root, igraph_neimode_t mode, bool unreachable, [In, Out]igraph_vector_t order, [In, Out]igraph_vector_t order_out, [In, Out]igraph_vector_t father, [In, Out]igraph_vector_t dist, igraph_dfshandler_t in_callback, igraph_dfshandler_t out_callback, IntPtr extra);
    #endregion
    #endregion

    #region igraph_rng
    internal static int igraph_rng_get_integer(int l, int h) {
      return X86 ? igraph_rng_get_integer_x86(igraph_rng_default_x86(), l, h) : igraph_rng_get_integer_x64(igraph_rng_default_x64(), l, h);
    }
    internal static int igraph_rng_seed(uint seed) {
      return X86 ? igraph_rng_seed_x86(igraph_rng_default_x86(), seed) : igraph_rng_seed_x64(igraph_rng_default_x64(), seed);
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

    #region igraph_vector
    internal static int igraph_vector_init(igraph_vector_t vector, int length) {
      return X86 ? igraph_vector_init_x86(vector, length) : igraph_vector_init_x64(vector, length);
    }
    internal static int igraph_vector_init_copy(igraph_vector_t vector, double[] vec) {
      return X86 ? igraph_vector_init_copy_x86(vector, vec, vec.Length) : igraph_vector_init_copy_x64(vector, vec, vec.Length);
    }
    internal static void igraph_vector_destroy(igraph_vector_t vector) {
      if (X86) igraph_vector_destroy_x86(vector);
      else igraph_vector_destroy_x64(vector);
    }
    internal static int igraph_vector_copy(igraph_vector_t to, igraph_vector_t from) {
      return X86 ? igraph_vector_copy_x86(to, from) : igraph_vector_copy_x64(to, from);
    }
    internal static double[] igraph_vector_to_array(igraph_vector_t from) {
      var len = igraph_vector_size(from);
      var result = new double[len];
      if (X86) igraph_vector_copy_to_x86(from, result);
      else igraph_vector_copy_to_x64(from, result);
      return result;
    }

    internal static int igraph_vector_size(igraph_vector_t vector) {
      return X86 ? igraph_vector_size_x86(vector) : igraph_vector_size_x64(vector);
    }

    internal static double igraph_vector_e(igraph_vector_t vector, int index) {
      return X86 ? igraph_vector_e_x86(vector, index) : igraph_vector_e_x64(vector, index);
    }

    internal static void igraph_vector_set(igraph_vector_t vector, int index, double value) {
      if (X86) igraph_vector_set_x86(vector, index, value);
      else igraph_vector_set_x64(vector, index, value);
    }
    internal static void igraph_vector_fill(igraph_vector_t vector, double v) {
      if (X86) igraph_vector_fill_x86(vector, v);
      else igraph_vector_fill_x64(vector, v);
    }
    internal static void igraph_vector_scale(igraph_vector_t vector, double by) {
      if (X86) igraph_vector_scale_x86(vector, by);
      else igraph_vector_scale_x64(vector, by);
    }

    internal static int igraph_vector_reverse(igraph_vector_t vector) {
      return X86 ? igraph_vector_reverse_x86(vector) : igraph_vector_reverse_x64(vector);
    }
    internal static int igraph_vector_shuffle(igraph_vector_t vector) {
      return X86 ? igraph_vector_shuffle_x86(vector) : igraph_vector_shuffle_x64(vector);
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_vector_init", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_init_x86([In, Out]igraph_vector_t vector, int length);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_init", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_init_x64([In, Out]igraph_vector_t vector, int length);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_init_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_init_copy_x86([In, Out]igraph_vector_t vector, double[] vec, int length);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_init_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_init_copy_x64([In, Out]igraph_vector_t vector, double[] vec, int length);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_destroy_x86([In, Out]igraph_vector_t vector);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_destroy_x64([In, Out]igraph_vector_t vector);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_size", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_size_x86(igraph_vector_t vector);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_size", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_size_x64(igraph_vector_t vector);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_e", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_vector_e_x86(igraph_vector_t vector, int index);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_e", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_vector_e_x64(igraph_vector_t vector, int index);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_set", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_vector_set_x86([In, Out]igraph_vector_t vector, int index, double value);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_set", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_vector_set_x64([In, Out]igraph_vector_t vector, int index, double value);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_copy_x86([In, Out]igraph_vector_t to, igraph_vector_t from);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_copy_x64([In, Out]igraph_vector_t to, igraph_vector_t from);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_copy_to", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_copy_to_x86(igraph_vector_t from, [In, Out]double[] to);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_copy_to_x64(igraph_vector_t from, [In, Out]double[] to);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_fill", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_fill_x86([In, Out]igraph_vector_t vector, double v);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_fill", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_fill_x64([In, Out]igraph_vector_t vector, double v);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_reverse", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_reverse_x86([In, Out]igraph_vector_t vector);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_reverse", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_reverse_x64([In, Out]igraph_vector_t vector);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_shuffle", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_shuffle_x86([In, Out]igraph_vector_t vector);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_shuffle", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vector_shuffle_x64([In, Out]igraph_vector_t vector);
    [DllImport(X86Dll, EntryPoint = "igraph_vector_scale", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_scale_x86([In, Out]igraph_vector_t vector, double by);
    [DllImport(X64Dll, EntryPoint = "igraph_vector_scale", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vector_scale_x64([In, Out]igraph_vector_t vector, double by);
    #endregion
    #endregion

    #region igraph_matrix
    internal static int igraph_matrix_init(igraph_matrix_t matrix, int nrows, int ncols) {
      return X86 ? igraph_matrix_init_x86(matrix, nrows, ncols) : igraph_matrix_init_x64(matrix, nrows, ncols);
    }
    internal static void igraph_matrix_destroy(igraph_matrix_t matrix) {
      if (X86) igraph_matrix_destroy_x86(matrix);
      else igraph_matrix_destroy_x64(matrix);
    }
    internal static int igraph_matrix_copy(igraph_matrix_t to, igraph_matrix_t from) {
      return X86 ? igraph_matrix_copy_x86(to, from) : igraph_matrix_copy_x64(to, from);
    }

    internal static double igraph_matrix_e(igraph_matrix_t matrix, int row, int col) {
      return X86 ? igraph_matrix_e_x86(matrix, row, col) : igraph_matrix_e_x64(matrix, row, col);
    }

    internal static void igraph_matrix_set(igraph_matrix_t matrix, int row, int col, double value) {
      if (X86) igraph_matrix_set_x86(matrix, row, col, value);
      else igraph_matrix_set_x64(matrix, row, col, value);
    }

    internal static void igraph_matrix_fill(igraph_matrix_t matrix, double v) {
      if (X86) igraph_matrix_fill_x86(matrix, v);
      else igraph_matrix_fill_x64(matrix, v);
    }

    internal static int igraph_matrix_transpose(igraph_matrix_t matrix) {
      return X86 ? igraph_matrix_transpose_x86(matrix) : igraph_matrix_transpose_x64(matrix);
    }

    internal static void igraph_matrix_scale(igraph_matrix_t matrix, double by) {
      if (X86) igraph_matrix_scale_x86(matrix, by);
      else igraph_matrix_scale_x64(matrix, by);
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
    private static extern double igraph_matrix_set_x86([In, Out]igraph_matrix_t matrix, int row, int col, double value);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_set", CallingConvention = CallingConvention.Cdecl)]
    private static extern double igraph_matrix_set_x64([In, Out]igraph_matrix_t matrix, int row, int col, double value);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_copy_x86([In, Out]igraph_matrix_t to, igraph_matrix_t from);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_copy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_copy_x64([In, Out]igraph_matrix_t to, igraph_matrix_t from);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_fill", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_matrix_fill_x86([In, Out]igraph_matrix_t matrix, double v);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_fill", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_matrix_fill_x64([In, Out]igraph_matrix_t matrix, double v);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_transpose", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_transpose_x86([In, Out]igraph_matrix_t matrix);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_transpose", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_transpose_x64([In, Out]igraph_matrix_t matrix);
    [DllImport(X86Dll, EntryPoint = "igraph_matrix_scale", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_scale_x86([In, Out]igraph_matrix_t matrix, double by);
    [DllImport(X64Dll, EntryPoint = "igraph_matrix_scale", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_matrix_scale_x64([In, Out]igraph_matrix_t matrix, double by);
    #endregion
    #endregion

    #region igraph_layout
    internal static int igraph_layout_fruchterman_reingold(igraph_t graph, igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy) {
      return X86 ? igraph_layout_fruchterman_reingold_x86(graph, res, use_seed, niter, start_temp, grid, weights, minx, maxx, miny, maxy) : igraph_layout_fruchterman_reingold_x64(graph, res, use_seed, niter, start_temp, grid, weights, minx, maxx, miny, maxy);
    }
    internal static int igraph_layout_kamada_kawai(igraph_t graph, igraph_matrix_t res, bool use_seed, int maxiter, double epsilon, double kkconst, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy) {
      return X86 ? igraph_layout_kamada_kawai_x86(graph, res, use_seed, maxiter, epsilon, kkconst, weights, minx, maxx, miny, maxy) : igraph_layout_kamada_kawai_x64(graph, res, use_seed, maxiter, epsilon, kkconst, weights, minx, maxx, miny, maxy);
    }
    internal static int igraph_layout_davidson_harel(igraph_t graph, igraph_matrix_t res, bool use_seed, int maxiter, int fineiter, double cool_fact, double weight_node_dist, double weight_border, double weight_edge_lengths, double weight_edge_crossings, double weight_node_edge_dist) {
      return X86 ? igraph_layout_davidson_harel_x86(graph, res, use_seed, maxiter, fineiter, cool_fact, weight_node_dist, weight_border, weight_edge_lengths, weight_edge_crossings, weight_node_edge_dist) : igraph_layout_davidson_harel_x64(graph, res, use_seed, maxiter, fineiter, cool_fact, weight_node_dist, weight_border, weight_edge_lengths, weight_edge_crossings, weight_node_edge_dist);
    }
    internal static int igraph_layout_mds(igraph_t graph, igraph_matrix_t res, igraph_matrix_t dist = null, int dim = 2) {
      return MarshalIfExistsAndCall(GetDefaultArpackOptions(), ptr => X86 ? igraph_layout_mds_x86(graph, res, dist, dim, ptr) : igraph_layout_mds_x64(graph, res, dist, dim, ptr));
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_layout_fruchterman_reingold", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_fruchterman_reingold_x86(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    [DllImport(X64Dll, EntryPoint = "igraph_layout_fruchterman_reingold", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_fruchterman_reingold_x64(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int niter, double start_temp, igraph_layout_grid_t grid, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    [DllImport(X86Dll, EntryPoint = "igraph_layout_kamada_kawai", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_kamada_kawai_x86(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int maxiter, double epsilon, double kkconst, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    [DllImport(X64Dll, EntryPoint = "igraph_layout_kamada_kawai", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_kamada_kawai_x64(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int maxiter, double epsilon, double kkconst, igraph_vector_t weights, igraph_vector_t minx, igraph_vector_t maxx, igraph_vector_t miny, igraph_vector_t maxy);
    [DllImport(X86Dll, EntryPoint = "igraph_layout_davidson_harel", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_davidson_harel_x86(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int maxiter, int fineiter, double cool_fact, double weight_node_dist, double weight_border, double weight_edge_lengths, double weight_edge_crossings, double weight_node_edge_dist);
    [DllImport(X64Dll, EntryPoint = "igraph_layout_davidson_harel", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_davidson_harel_x64(igraph_t graph, [In, Out]igraph_matrix_t res, bool use_seed, int maxiter, int fineiter, double cool_fact, double weight_node_dist, double weight_border, double weight_edge_lengths, double weight_edge_crossings, double weight_node_edge_dist);
    [DllImport(X86Dll, EntryPoint = "igraph_layout_mds", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_mds_x86(igraph_t graph, [In, Out]igraph_matrix_t res, igraph_matrix_t dist, int dim, IntPtr options);
    [DllImport(X64Dll, EntryPoint = "igraph_layout_mds", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_layout_mds_x64(igraph_t graph, [In, Out]igraph_matrix_t res, igraph_matrix_t dist, int dim, IntPtr options);
    #endregion
    #endregion

    #region igraph_iterators
    internal static int igraph_vs_all(ref igraph_vs_t vs) {
      return X86 ? igraph_vs_all_x86(ref vs) : igraph_vs_all_x64(ref vs);
    }
    internal static void igraph_vs_destroy(ref igraph_vs_t vs) {
      if (X86) igraph_vs_destroy_x86(ref vs);
      else igraph_vs_destroy_x64(ref vs);
    }

    #region Platform specific DLL imports
    [DllImport(X86Dll, EntryPoint = "igraph_vs_all", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vs_all_x86([In, Out]ref igraph_vs_t vs);
    [DllImport(X64Dll, EntryPoint = "igraph_vs_all", CallingConvention = CallingConvention.Cdecl)]
    private static extern int igraph_vs_all_x64([In, Out]ref igraph_vs_t vs);
    [DllImport(X86Dll, EntryPoint = "igraph_vs_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vs_destroy_x86([In, Out]ref igraph_vs_t vs);
    [DllImport(X64Dll, EntryPoint = "igraph_vs_destroy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void igraph_vs_destroy_x64([In, Out]ref igraph_vs_t vs);
    #endregion
    #endregion

    private static int MarshalIfExistsAndCall(object o, Func<IntPtr, int> call) {
      var ptr = IntPtr.Zero;
      if (o != null) {
        try {
          ptr = Marshal.AllocHGlobal(Marshal.SizeOf(o));
        } catch (OutOfMemoryException e) { throw new InvalidOperationException("Not enough memory to perform operation.", e); }
        Marshal.StructureToPtr(o, ptr, false);
      }
      try {
        return call(ptr);
      } finally {
        if (o != null) {
          Marshal.DestroyStructure(ptr, o.GetType());
          Marshal.FreeHGlobal(ptr);
        }
      }
    }

    private static void MarshalIfExistsAndCall(object o, Action<IntPtr> call) {
      var ptr = IntPtr.Zero;
      if (o != null) {
        try {
          ptr = Marshal.AllocHGlobal(Marshal.SizeOf(o));
        } catch (OutOfMemoryException e) { throw new InvalidOperationException("Not enough memory to perform operation.", e); }
        Marshal.StructureToPtr(o, ptr, false);
      }
      try {
        call(ptr);
      } finally {
        if (o != null) {
          Marshal.DestroyStructure(ptr, o.GetType());
          Marshal.FreeHGlobal(ptr);
        }
      }
    }
  }
}
