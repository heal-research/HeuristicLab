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

namespace HeuristicLab.IGraph.Wrappers {
  public sealed class Graph : IDisposable {
    private igraph_t graph;
    internal igraph_t NativeInstance { get { return graph; } }

    public int Vertices { get { return graph.n; } }
    public bool IsDirected { get { return graph.directed; } }

    public Graph() : this(0) { }
    public Graph(int vertices) : this(vertices, false) { }
    public Graph(int vertices, bool directed) {
      graph = new igraph_t();
      DllImporter.igraph_empty(graph, vertices, directed);
    }
    public Graph(int vertices, IEnumerable<Tuple<int, int>> edges) : this(vertices, edges, false) { }
    public Graph(int vertices, IEnumerable<Tuple<int, int>> edges, bool directed) {
      graph = new igraph_t();
      DllImporter.igraph_empty(graph, vertices, directed);
      foreach (var e in edges)
        DllImporter.igraph_add_edge(graph, e.Item1, e.Item2);
    }
    ~Graph() {
      DllImporter.igraph_destroy(graph);
    }

    public void Dispose() {
      if (graph == null) return;
      DllImporter.igraph_destroy(graph);
      graph = null;
      GC.SuppressFinalize(this);
    }

    public void SetSeed(uint seed) {
      DllImporter.igraph_rng_seed(seed);
    }

    public double Density() {
      double density;
      DllImporter.igraph_density(graph, out density, false);
      return density;
    }

    public Vector PageRank(double damping = 0.85, Vector weights = null) {
      var vec = new Vector(Vertices);
      var all = new igraph_vs_t();
      DllImporter.igraph_vs_all(ref all);
      try {
        double eigenv = 0;
        DllImporter.igraph_pagerank(graph, igraph_pagerank_algo_t.IGRAPH_PAGERANK_ALGO_PRPACK, vec.NativeInstance, out eigenv, all, IsDirected, damping, weights != null ? weights.NativeInstance : null);
      } finally {
        DllImporter.igraph_vs_destroy(ref all);
      }
      return vec;
    }

    public Matrix LayoutWithFruchtermanReingold(Matrix initialCoords = null) {
      return LayoutWithFruchtermanReingold(500, Math.Sqrt(Vertices), initialCoords);
    }
    public Matrix LayoutWithFruchtermanReingold(int niter, double startTemp, Matrix initialCoords = null) {
      if (initialCoords != null && (initialCoords.Rows != Vertices || initialCoords.Columns != 2))
        throw new ArgumentException("Initial coordinate matrix does not contain the required number of rows and columns.", "initialCoords");
      var coords = initialCoords != null ? new Matrix(initialCoords) : new Matrix(Vertices, 2);
      DllImporter.igraph_layout_fruchterman_reingold(graph, coords.NativeInstance, initialCoords != null, niter, startTemp, igraph_layout_grid_t.IGRAPH_LAYOUT_AUTOGRID, null, null, null, null, null);
      return coords;
    }

    public Matrix LayoutWithKamadaKawai(Matrix initialCoords = null) {
      return LayoutWithKamadaKawai(50 * Vertices, 0, Vertices, initialCoords);
    }
    public Matrix LayoutWithKamadaKawai(int maxiter, double epsilon, double kkconst, Matrix initialCoords = null) {
      if (initialCoords != null && (initialCoords.Rows != Vertices || initialCoords.Columns != 2))
        throw new ArgumentException("Initial coordinate matrix does not contain the required number of rows and columns.", "initialCoords");
      var coords = initialCoords != null ? new Matrix(initialCoords) : new Matrix(Vertices, 2);
      DllImporter.igraph_layout_kamada_kawai(graph, coords.NativeInstance, initialCoords != null, maxiter, epsilon, kkconst, null, null, null, null, null);
      return coords;
    }

    public Matrix LayoutWithDavidsonHarel(Matrix initialCoords = null) {
      var density = Density();
      return LayoutWithDavidsonHarel(10, Math.Max(10, (int)Math.Log(Vertices, 2)), 0.75, 1.0, 0.0, density / 10.0, 1.0 - Math.Sqrt(density), 0.2 * (1 - density), initialCoords);
    }
    public Matrix LayoutWithDavidsonHarel(int maxiter, int fineiter, double cool_fact, double weight_node_dist, double weight_border, double weight_edge_lengths, double weight_edge_crossings, double weight_node_edge_dist, Matrix initialCoords = null) {
      if (initialCoords != null && (initialCoords.Rows != Vertices || initialCoords.Columns != 2))
        throw new ArgumentException("Initial coordinate matrix does not contain the required number of rows and columns.", "initialCoords");
      var coords = initialCoords != null ? new Matrix(initialCoords) : new Matrix(Vertices, 2);
      DllImporter.igraph_layout_davidson_harel(graph, coords.NativeInstance, initialCoords != null, maxiter, fineiter, cool_fact, weight_node_dist, weight_border, weight_edge_lengths, weight_edge_crossings, weight_node_edge_dist);
      return coords;
    }

    /// <summary>
    /// Use multi-dimensional scaling to layout vertices. 
    /// A distance matrix can be used to specify the distances between the vertices.
    /// Otherwise the distances will be calculated by shortest-path-length.
    /// </summary>
    /// <remarks>
    /// For disconnected graphs, dimension must be 2.
    /// </remarks>
    /// <param name="dist">The distance matrix to layout the vertices.</param>
    /// <param name="dim">How many dimensions should be used.</param>
    /// <returns>The coordinates matrix of the aligned vertices.</returns>
    public Matrix LayoutWithMds(Matrix dist = null, int dim = 2) {
      var coords = new Matrix(Vertices, dim);
      DllImporter.igraph_layout_mds(graph, coords.NativeInstance, dist != null ? dist.NativeInstance : null, dim);
      return coords;
    }

    public void BreadthFirstWalk(BreadthFirstHandler handler, int root, DirectedWalkMode mode, bool includeUnreachableFromRoot = false, object tag = null) {
      igraph_bfshandler_t wrapper = (t, vid, pred, succ, rank, dist, extra) => handler != null && handler(this, vid, pred, succ, rank, dist, tag);
      DllImporter.igraph_bfs(graph, root, null, (igraph_neimode_t)mode, includeUnreachableFromRoot, null, null, null, null, null, null, null, wrapper, tag);
    }

    public void DepthFirstWalk(DepthFirstHandler inHandler, DepthFirstHandler outHandler, int root, DirectedWalkMode mode, bool includeUnreachableFromRoot = false, object tag = null) {
      igraph_dfshandler_t inWrapper = (t, vid, dist, extra) => inHandler != null && inHandler(this, vid, dist, tag);
      igraph_dfshandler_t outWrapper = (t, vid, dist, extra) => outHandler != null && outHandler(this, vid, dist, tag);
      DllImporter.igraph_dfs(graph, root, (igraph_neimode_t)mode, includeUnreachableFromRoot, null, null, null, null, inWrapper, outWrapper, tag);
    }
  }
}
