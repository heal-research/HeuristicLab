#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.ObjectModel;
using System.Text;

namespace HeuristicLab.Charting {
  public interface IGroup : IPrimitive {
    ReadOnlyCollection<IPrimitive> Primitives { get; }
    ReadOnlyCollection<IPrimitive> SelectedPrimitives { get; }

    void Add(IPrimitive primitive);
    void AddRange(IEnumerable<IPrimitive> primitives);
    bool Contains(IPrimitive primitive);
    bool Remove(IPrimitive primitive);
    void Clear();

    IPrimitive GetPrimitive(PointD point);
    IPrimitive GetPrimitive(double x, double y);
    IList<IPrimitive> GetAllPrimitives(PointD point);
    IList<IPrimitive> GetAllPrimitives(double x, double y);

    void OneLayerUp(IPrimitive primitive);
    void OneLayerDown(IPrimitive primitive);
    void IntoForeground(IPrimitive primitive);
    void IntoBackground(IPrimitive primitive);
  }
}
