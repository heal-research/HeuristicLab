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
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("f7a3227e-17b9-4af6-bf6d-0801cbdca286")]
  public interface IVertex : IItem {
    IEnumerable<IArc> InArcs { get; }
    IEnumerable<IArc> OutArcs { get; }

    int InDegree { get; }
    int OutDegree { get; }
    int Degree { get; }

    string Label { get; set; }
    double Weight { get; set; }

    void AddArc(IArc arc);
    void RemoveArc(IArc arc);

    event EventHandler Changed; // generic event for when the content, label or weight have changed
    event EventHandler<EventArgs<IArc>> ArcAdded;
    event EventHandler<EventArgs<IArc>> ArcRemoved;
  }

  [StorableType("f856ed8e-1259-4949-9784-78f1f4da1abb")]
  public interface IVertex<T> : IVertex where T : class, IDeepCloneable {
    T Data { get; set; }
  }
}
