#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HeuristicLab.Problems.Instances.DIMACS {
  public class GcolParser {
    private enum LineType { Unknown, Comment, Problem, Edge }

    public string Comments { get; private set; }
    public int Nodes { get; private set; }
    public int Edges { get; private set; }
    public IEnumerable<Tuple<int, int>> AdjacencyList {
      get { return edges.Select(x => Tuple.Create(x.First.Id, x.Second.Id)); }
    }

    public int UnconnectedNodes { get; private set; }

    private Node[] nodes;
    private HashSet<Edge> edges;
    
    public GcolParser() {
      Reset();
    }

    public void Reset() {
      Nodes = 0;
      Edges = 0;
      nodes = new Node[0];
      edges = new HashSet<Edge>();
    }

    public void Parse(string file) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the GCOL format:
    /// http://prolland.free.fr/works/research/dsat/dimacs.html
    /// The number of nodes and edges are calculated from the actual edges defined.
    /// The number of nodes stated must be a maximum number.
    /// Nodes without edges are not considered.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    public void Parse(Stream stream) {
      char[] delim = new char[] { ' ', '\t' };
      var comments = new StringBuilder();
      var reader = new StreamReader(stream);
      while (!reader.EndOfStream) {
        var line = reader.ReadLine().Trim();
        switch (GetLineType(line)) {
          case LineType.Comment:
            comments.AppendLine(line);
            break;
          case LineType.Problem:
            var pData = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            if (pData.Length < 3)
              throw new InvalidOperationException("Invalid problem entry: " + line + Environment.NewLine + "Must be p FORMAT NODES [EDGES]");
            if (!pData[1].Equals("edge", StringComparison.InvariantCultureIgnoreCase)
              && !pData[1].Equals("edges", StringComparison.InvariantCultureIgnoreCase))
              throw new InvalidOperationException("Unsupported problem format type " + pData[1]);
            Nodes = int.Parse(pData[2], CultureInfo.InvariantCulture.NumberFormat);
            nodes = new Node[Nodes];
            break;
          case LineType.Edge:
            if (nodes == null) throw new InvalidOperationException("Missing \"p\" line before \"e\" lines.");
            var eData = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            var src = int.Parse(eData[1], CultureInfo.InvariantCulture.NumberFormat);
            var tgt = int.Parse(eData[2], CultureInfo.InvariantCulture.NumberFormat);
            if (src > Nodes || tgt > Nodes)
              throw new InvalidOperationException("An edge cannot be declared between " + src + " and " + tgt + " because only " + Nodes + " nodes were defined.");
            src--; // node indices are given 1-based in the files
            tgt--; // node indices are given 1-based in the files
            if (src > tgt) {
              var h = src;
              src = tgt;
              tgt = h;
            }
            if (nodes[src] == null) nodes[src] = new Node();
            if (nodes[tgt] == null) nodes[tgt] = new Node();
            if (edges.Add(new Edge(nodes[src], nodes[tgt]))) Edges++;
            break;
        }
      }

      if (nodes.Length == 0) throw new InvalidOperationException("There were no nodes defined.");

      // Give identies to the nodes and filter nodes without edges
      Nodes = 0; UnconnectedNodes = 0;
      for (var n = 0; n < nodes.Length; n++) {
        if (nodes[n] != null) {
          nodes[n].Id = Nodes;
          Nodes++;
        } else UnconnectedNodes++;
      }
      if (UnconnectedNodes > 0) {
        comments.AppendLine();
        comments.Append("There were ");
        comments.Append(UnconnectedNodes);
        comments.AppendLine(" unconnected nodes that have been removed compared to the original file.");
      }
      Comments = comments.ToString();
    }

    private LineType GetLineType(string line) {
      if (string.IsNullOrEmpty(line)) return LineType.Unknown;
      var first = line[0];
      switch (first) {
        case 'c': return LineType.Comment;
        case 'p': return LineType.Problem;
        case 'e': return LineType.Edge;
        default: throw new InvalidOperationException("This is not a valid .col file");
      }
    }

    private class Node {
      public int Id { get; set; }
      public Node() { }
    }

    private class Edge {
      public Node First { get; set; }
      public Node Second { get; set; }
      public Edge(Node first, Node second) {
        First = first;
        Second = second;
      }

      public override bool Equals(object obj) {
        var other = (obj as Edge);
        if (other == null) return false;
        return First == other.First && Second == other.Second
          || First == other.Second && Second == other.First;
      }

      public override int GetHashCode() {
        return First.GetHashCode() ^ Second.GetHashCode();
      }
    }
  }
}
