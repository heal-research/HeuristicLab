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
using HeuristicLab.Common;
using HEAL.Fossil;

namespace HeuristicLab.Core {
  [StorableType("6d358590-409c-4fbb-944a-01f8e99be025")]
  public interface IArc : IItem {
    IVertex Source { get; }
    IVertex Target { get; }
    string Label { get; set; }
    double Weight { get; set; }

    event EventHandler Changed; // generic event for when the label, weight or data were changed
  }

  [StorableType("4acdc291-84ea-4da3-95b8-046f973db256")]
  public interface IArc<T> : IArc where T : class, IDeepCloneable {
    T Data { get; set; }
  }
}