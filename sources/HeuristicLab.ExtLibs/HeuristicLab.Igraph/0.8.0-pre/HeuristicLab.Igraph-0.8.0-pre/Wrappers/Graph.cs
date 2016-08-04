using System;
using System.Collections.Generic;

namespace HeuristicLab.igraph.Wrappers {
  public class Graph : IDisposable {
    private igraph_t graph;
    public int Vertices { get { return graph.n; } }

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

    public int RandomInteger(int inclLower, int inclUpper) {
      return DllImporter.igraph_rng_get_integer(inclLower, inclUpper);
    }

    public double[,] LayoutWithFruchtermanReingold() {
      return LayoutWithFruchtermanReingold(500, Math.Sqrt(Vertices));
    }
    public double[,] LayoutWithFruchtermanReingold(int niter, double startTemp) {
      using (var coords = new Matrix(graph.n, 2)) {
        DllImporter.igraph_layout_fruchterman_reingold(graph, coords.NativeInstance, false, niter, startTemp, igraph_layout_grid_t.IGRAPH_LAYOUT_AUTOGRID, null, null, null, null, null);
        return coords.ToMatrix();
      }
    }
  }
}
