#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  public interface IPreprocessingData : INamedItem {
    T GetCell<T>(int columnIndex, int rowIndex);

    void SetCell<T>(int columnIndex, int rowIndex, T value);

    string GetCellAsString(int columnIndex, int rowIndex);

    IList<T> GetValues<T>(int columnIndex, bool considerSelection = false);

    void SetValues<T>(int columnIndex, IList<T> values);
    bool SetValue(string value, int columnIndex, int rowIndex);

    void InsertRow(int rowIndex);
    void DeleteRow(int rowIndex);
    void DeleteRowsWithIndices(IEnumerable<int> rows);
    void InsertColumn<T>(string variableName, int columnIndex);

    void DeleteColumn(int columnIndex);

    void RenameColumn(int columnIndex, string name);
    void RenameColumns(IList<string> names);

    bool AreAllStringColumns(IEnumerable<int> columnIndices);
    bool Validate(string value, out string errorMessage, int columnIndex);

    IntRange TrainingPartition { get; }
    IntRange TestPartition { get; }

    IList<ITransformation> Transformations { get; }

    IEnumerable<string> VariableNames { get; }
    IEnumerable<string> GetDoubleVariableNames();
    string GetVariableName(int columnIndex);
    int GetColumnIndex(string variableName);

    bool VariableHasType<T>(int columnIndex);

    IList<string> InputVariables { get; }
    string TargetVariable { get; } // optional

    int Columns { get; }
    int Rows { get; }

    Dataset ExportToDataset();

    IDictionary<int, IList<int>> Selection { get; set; }
    void ClearSelection();

    event EventHandler SelectionChanged;
  }
}
