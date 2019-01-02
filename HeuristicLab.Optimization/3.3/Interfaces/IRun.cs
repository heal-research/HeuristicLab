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

using System.ComponentModel;
using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("bd021976-74c7-4e40-af7e-e10e316af64c")]
  /// <summary>
  /// Represents the parameters and results of an algorithm run.
  /// </summary>
  public interface IRun : INamedItem, INotifyPropertyChanged {
    IAlgorithm Algorithm { get; }
    IObservableDictionary<string, IItem> Parameters { get; }
    IObservableDictionary<string, IItem> Results { get; }

    Color Color { get; set; }
    bool Visible { get; set; }
  }
}
