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
  [StorableType("5710751f-6af8-4bc8-b517-97218cb30d22")]
  public interface IScopeTreeLookupParameter : ILookupParameter {
    int Depth { get; set; }
    event EventHandler DepthChanged;
  }

  [StorableType("4a2fead8-d21c-49a0-b1f3-aece5fd99407")]
  public interface IScopeTreeLookupParameter<T> : IScopeTreeLookupParameter, ILookupParameter<ItemArray<T>> where T : class, IItem { }
}
