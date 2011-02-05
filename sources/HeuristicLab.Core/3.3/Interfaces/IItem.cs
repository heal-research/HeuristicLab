#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent (almost) every HeuristicLab object (an object, an operator,...).
  /// </summary>
  public interface IItem : IContent, IDeepCloneable {
    string ItemName { get; }
    string ItemDescription { get; }
    Version ItemVersion { get; }
    Image ItemImage { get; }

    event EventHandler ItemImageChanged;
    event EventHandler ToStringChanged;
  }
}
