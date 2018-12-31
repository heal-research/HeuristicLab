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
using HEAL.Fossil;

namespace HeuristicLab.Core {
  [StorableType("e4920b59-6bf5-4c43-997c-7f5434cd98d2")]
  public interface IValueParameter : IParameter {
    IItem Value { get; set; }
    bool GetsCollected { get; set; }
    event EventHandler ValueChanged;
    event EventHandler GetsCollectedChanged;
  }

  [StorableType("645945d2-9cd7-45cd-8507-575b2ed53de4")]
  public interface IValueParameter<T> : IValueParameter where T : class, IItem {
    new T Value { get; set; }
  }
}
