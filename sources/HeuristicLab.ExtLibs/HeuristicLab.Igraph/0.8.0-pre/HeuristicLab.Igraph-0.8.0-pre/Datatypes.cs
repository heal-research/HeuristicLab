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

namespace HeuristicLab.IGraph {
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
    internal igraph_vector_t data = new igraph_vector_t();
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

  [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
  internal struct igraph_vs_t {
    [FieldOffset(0)]
    internal int type;
    [FieldOffset(4)]
    int vid;
    [FieldOffset(4)]
    IntPtr vecptr;
    [FieldOffset(4)]
    int adj_vid;
    [FieldOffset(8)]
    igraph_neimode_t adj_mode;
    [FieldOffset(4)]
    int seq_from;
    [FieldOffset(8)]
    int seq_to;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  unsafe internal struct igraph_arpack_options_t {
    /* INPUT */
    internal fixed char bmat[1]; /* I-standard problem, G-generalized */
    internal int n; /* Dimension of the eigenproblem */
    internal fixed char which[2]; /* LA, SA, LM, SM, BE */
    internal int nev; /* Number of eigenvalues to be computed */
    internal double tol; /* Stopping criterion */
    internal int ncv; /* Number of columns in V */
    internal int ldv; /* Leading dimension of V */
    internal int ishift; /* 0-reverse comm., 1-exact with tridiagonal */
    internal int mxiter; /* Maximum number of update iterations to take */
    internal int nb; /* Block size on the recurrence, only 1 works */
    internal int mode; /* The kind of problem to be solved (1-5)
                          1: A*x=l*x, A symmetric
                          2: A*x=l*M*x, A symm. M pos. def.
                          3: K*x = l*M*x, K symm., M pos. semidef.
                          4: K*x = l*KG*x, K s. pos. semidef. KG s. indef.
                          5: A*x = l*M*x, A symm., M symm. pos. semidef. */
    internal int start; /* 0: random, 1: use the supplied vector */
    internal int lworkl; /* Size of temporary storage, default is fine */
    internal double sigma; /* The shift for modes 3,4,5 */
    internal double sigmai; /* The imaginary part of shift for rnsolve */
    /* OUTPUT */
    internal int info; /* What happened, see docs */
    internal int ierr; /* What happened  in the dseupd call */
    internal int noiter; /* The number of iterations taken */
    internal int nconv;
    internal int numop; /* Number of OP*x operations */
    internal int numopb; /* Number of B*x operations if BMAT='G' */
    internal int numreo; /* Number of steps of re-orthogonalizations */
    /* INTERNAL */
    internal fixed int iparam[11];
    internal fixed int ipntr[14];
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  internal struct igraph_pagerank_power_options_t {
    internal int niter;
    internal double eps;
  }
  #endregion

  #region Enums
  internal enum igraph_layout_grid_t {
    IGRAPH_LAYOUT_GRID,
    IGRAPH_LAYOUT_NOGRID,
    IGRAPH_LAYOUT_AUTOGRID
  };
  internal enum igraph_pagerank_algo_t {
    IGRAPH_PAGERANK_ALGO_POWER = 0,
    IGRAPH_PAGERANK_ALGO_ARPACK = 1,
    IGRAPH_PAGERANK_ALGO_PRPACK = 2
  }
  internal enum igraph_neimode_t {
    IGRAPH_OUT = 1,
    IGRAPH_IN = 2,
    IGRAPH_ALL = 3,
    IGRAPH_TOTAL = 3
  }
  #endregion
}
