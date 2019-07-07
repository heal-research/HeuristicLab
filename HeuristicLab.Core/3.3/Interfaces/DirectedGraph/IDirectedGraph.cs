#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("e79c962e-b662-4502-bc32-cac6a83e4e83")]
  public interface IDirectedGraph : IItem {
    IEnumerable<IVertex> Vertices { get; }
    IEnumerable<IArc> Arcs { get; }

    void Clear();
    void AddVertex(IVertex vertex);
    void RemoveVertex(IVertex vertex);

    void AddVertices(IEnumerable<IVertex> vertexList);
    void RemoveVertices(IEnumerable<IVertex> vertexList);

    IArc AddArc(IVertex source, IVertex target);
    void AddArc(IArc arc);
    void RemoveArc(IArc arc);

    void AddArcs(IEnumerable<IArc> arcs);
    void RemoveArcs(IEnumerable<IArc> removeArcs);

    event EventHandler<EventArgs<IVertex>> VertexAdded;
    event EventHandler<EventArgs<IVertex>> VertexRemoved;
    event EventHandler<EventArgs<IArc>> ArcAdded;
    event EventHandler<EventArgs<IArc>> ArcRemoved;
  }
}
