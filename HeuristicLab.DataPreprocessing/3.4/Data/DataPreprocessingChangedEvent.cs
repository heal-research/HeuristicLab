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

namespace HeuristicLab.DataPreprocessing {
  [StorableType("f06da8c5-5c95-4e20-81c7-d264517d9981")]
  public enum DataPreprocessingChangedEventType {
    DeleteColumn,
    AddColumn,
    ChangeColumn,
    DeleteRow,
    AddRow,
    ChangeItem,
    Any,
    Transformation
  }

  public class DataPreprocessingChangedEventArgs : EventArgs {
    public DataPreprocessingChangedEventType Type { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }

    public DataPreprocessingChangedEventArgs(DataPreprocessingChangedEventType type, int column, int row) {
      Type = type;
      Column = column;
      Row = row;
    }
  }

  public delegate void DataPreprocessingChangedEventHandler(object sender, DataPreprocessingChangedEventArgs e);
}
