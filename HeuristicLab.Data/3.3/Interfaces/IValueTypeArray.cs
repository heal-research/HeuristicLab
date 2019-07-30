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
using System.Collections;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Data {
  [StorableType("548de1d4-69b4-40b4-ba37-e770575f7315")]
  public interface IValueTypeArray : IItem, IEnumerable {
    bool ReadOnly { get; }
    IValueTypeArray AsReadOnly();

    int Length { get; set; }
    bool Resizable { get; set; }
    event EventHandler ResizableChanged;

    IEnumerable<string> ElementNames { get; set; }
    event EventHandler ElementNamesChanged;

    event EventHandler<EventArgs<int>> ItemChanged;
    event EventHandler Reset;
  }

  [StorableType("f9db5740-1c4f-4f62-a9a8-84b32a461ea8")]
  public interface IValueTypeArray<T> : IValueTypeArray, IEnumerable<T> where T : struct {
    T this[int index] { get; set; }
  }
}


