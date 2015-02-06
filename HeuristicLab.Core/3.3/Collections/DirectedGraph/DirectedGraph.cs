#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("DirectedGraph", "Generic class representing a directed graph with custom vertices and content")]
  [StorableClass]
  public class DirectedGraph : Item, IDirectedGraph {
    public override Image ItemImage { get { return VSImageLibrary.Graph; } }

    private HashSet<IVertex> vertices;
    [Storable]
    public IEnumerable<IVertex> Vertices {
      get { return vertices; }
      private set { vertices = new HashSet<IVertex>(value); }
    }

    private HashSet<IArc> arcs;
    [Storable]
    public IEnumerable<IArc> Arcs {
      get { return arcs; }
      private set { arcs = new HashSet<IArc>(value); }
    }

    public DirectedGraph() {
      vertices = new HashSet<IVertex>();
      arcs = new HashSet<IArc>();
    }

    protected DirectedGraph(DirectedGraph original, Cloner cloner)
      : base(original, cloner) {
      vertices = new HashSet<IVertex>(original.vertices.Select(cloner.Clone));
      arcs = new HashSet<IArc>(original.arcs.Select(cloner.Clone));

      // add the arcs to the newly cloned vertices
      foreach (var arc in arcs) {
        arc.Source.AddArc(arc);
        arc.Target.AddArc(arc);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectedGraph(this, cloner);
    }

    [StorableConstructor]
    protected DirectedGraph(bool serializing)
      : base(serializing) {
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var vertex in vertices) {
        vertex.ArcAdded += Vertex_ArcAdded;
        vertex.ArcRemoved += Vertex_ArcRemoved;
      }

      foreach (var arc in arcs) {
        var source = arc.Source;
        var target = arc.Target;
        source.AddArc(arc);
        target.AddArc(arc);
      }
    }

    public virtual void Clear() {
      vertices.Clear();
      arcs.Clear();
    }

    public virtual void AddVertex(IVertex vertex) {
      if (!vertices.Contains(vertex) && vertex.Degree > 0)
        throw new ArgumentException("New vertices cannot have any arcs.");

      if (vertices.Add(vertex)) {
        // register event handlers
        vertex.ArcAdded += Vertex_ArcAdded;
        vertex.ArcRemoved += Vertex_ArcRemoved;
        OnVertexAdded(this, new EventArgs<IVertex>(vertex));
      }
    }

    public virtual void AddVertices(IEnumerable<IVertex> vertexList) {
      foreach (var v in vertexList) { AddVertex(v); }
    }

    public virtual void RemoveVertices(IEnumerable<IVertex> vertexList) {
      foreach (var v in vertexList) { RemoveVertex(v); }
    }

    public virtual void RemoveVertex(IVertex vertex) {
      vertices.Remove(vertex);
      // remove connections to/from the removed vertex
      var arcList = vertex.InArcs.Concat(vertex.OutArcs).ToList(); // avoid invalid operation exception: "collection was modified" below
      foreach (var arc in arcList)
        RemoveArc(arc);
      // deregister event handlers
      vertex.ArcAdded -= Vertex_ArcAdded;
      vertex.ArcRemoved -= Vertex_ArcRemoved;
      OnVertexRemoved(this, new EventArgs<IVertex>(vertex));
    }

    public virtual IArc AddArc(IVertex source, IVertex target) {
      var arc = new Arc(source, target);
      AddArc(arc);
      return arc;
    }

    public virtual void AddArc(IArc arc) {
      var source = arc.Source;
      var target = arc.Target;

      if (source == target)
        throw new InvalidOperationException("Source and target cannot be the same.");

      if (!vertices.Contains(source) || !vertices.Contains(target))
        throw new InvalidOperationException("Cannot add arc connecting vertices that are not in the graph.");

      source.AddArc(arc);
      target.AddArc(arc);
      arcs.Add(arc);
    }

    public virtual void AddArcs(IEnumerable<IArc> arcList) {
      foreach (var a in arcList) { AddArc(a); }
    }

    public virtual void RemoveArc(IArc arc) {
      arcs.Remove(arc);
      var source = (Vertex)arc.Source;
      var target = (Vertex)arc.Target;
      source.RemoveArc(arc);
      target.RemoveArc(arc);
    }

    public virtual void RemoveArcs(IEnumerable<IArc> arcList) {
      foreach (var a in arcList) { RemoveArc(a); }
    }

    protected virtual void Vertex_ArcAdded(object sender, EventArgs<IArc> args) {
      // the ArcAdded event is fired by a vertex when an arc from/to another vertex is added to its list of connections
      // because the arc is added in both directions by both the source and the target, this event will get fired twice here
      var arc = args.Value;
      if (arcs.Add(arc)) OnArcAdded(this, new EventArgs<IArc>(arc));
    }

    protected virtual void Vertex_ArcRemoved(object sender, EventArgs<IArc> args) {
      var arc = args.Value;
      if (arcs.Remove(arc)) OnArcRemoved(this, new EventArgs<IArc>(arc));
    }

    // events
    public event EventHandler<EventArgs<IVertex>> VertexAdded;
    protected virtual void OnVertexAdded(object sender, EventArgs<IVertex> args) {
      var added = VertexAdded;
      if (added != null)
        added(sender, args);
    }

    public event EventHandler<EventArgs<IVertex>> VertexRemoved;
    protected virtual void OnVertexRemoved(object sender, EventArgs<IVertex> args) {
      var removed = VertexRemoved;
      if (removed != null)
        removed(sender, args);
    }

    public event EventHandler<EventArgs<IArc>> ArcAdded;
    protected virtual void OnArcAdded(object sender, EventArgs<IArc> args) {
      var added = ArcAdded;
      if (added != null)
        added(sender, args);
    }

    public event EventHandler<EventArgs<IArc>> ArcRemoved;
    protected virtual void OnArcRemoved(object sender, EventArgs<IArc> args) {
      var removed = ArcRemoved;
      if (removed != null)
        removed(sender, args);
    }
  }
}
