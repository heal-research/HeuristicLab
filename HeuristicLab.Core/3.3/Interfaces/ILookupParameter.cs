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
  [StorableType("78e29fa3-e603-4a2e-a3e0-2dee459891f1")]
  public interface ILookupParameter : IParameter {
    string ActualName { get; set; }
    string TranslatedName { get; }
    IExecutionContext ExecutionContext { get; set; }
    event EventHandler ActualNameChanged;
  }

  [StorableType("61868c33-20be-4577-94bf-7efff9e9cf73")]
  public interface ILookupParameter<T> : ILookupParameter where T : class, IItem {
    new T ActualValue { get; set; }
  }
}
