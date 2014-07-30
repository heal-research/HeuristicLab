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
        vertex.ArcAdded += OnArcAdded;
        vertex.ArcRemoved += OnArcRemoved;
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
      vertices.Add(vertex);
      // register event handlers
      vertex.ArcAdded += OnArcAdded;
      vertex.ArcRemoved += OnArcRemoved;
    }

    public virtual void RemoveVertex(IVertex vertex) {
      vertices.Remove(vertex);
      // remove connections to/from the removed vertex
      var arcList = vertex.InArcs.Concat(vertex.OutArcs).ToList(); // avoid invalid operation exception: "collection was modified" below
      foreach (var arc in arcList)
        RemoveArc(arc);
      // deregister event handlers
      vertex.ArcAdded -= OnArcAdded;
      vertex.ArcRemoved -= OnArcRemoved;
    }

    public virtual IArc AddArc(IVertex source, IVertex target) {
      var arc = new Arc(source, target);
      AddArc(arc);
      return arc;
    }

    public virtual void AddArc(IArc arc) {
      var source = (Vertex)arc.Source;
      var target = (Vertex)arc.Target;
      source.AddArc(arc);
      target.AddArc(arc);
      arcs.Add(arc);
    }

    public virtual void RemoveArc(IArc arc) {
      arcs.Remove(arc);
      var source = (Vertex)arc.Source;
      var target = (Vertex)arc.Target;
      source.RemoveArc(arc);
      target.RemoveArc(arc);
    }

    public event EventHandler ArcAdded;
    protected virtual void OnArcAdded(object sender, EventArgs<IArc> args) {
      var arc = args.Value;
      // the ArcAdded event is fired by a vertex when an arc from or towards another vertex is added to his list of connections
      // because the arc is added in both directions by both the source and the target, this event will get fired twice 
      // here, we only want to add the arc once, so if its already contained, we return without complaining
      if (arcs.Contains(arc)) return;
      arcs.Add(arc);
    }


    public event EventHandler ArcRemoved;
    protected virtual void OnArcRemoved(object sender, EventArgs<IArc> args) {
      var arc = args.Value;
      if (!arcs.Contains(arc)) return; // the same rationale as above 
      arcs.Remove(arc);
    }

    // events
    public event EventHandler VertexAdded;
    public event EventHandler VertexRemoved;
  }
}
