#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {

  [Item("PreprocessingData", "Represents data used for preprocessing.")]
  public abstract class PreprocessingData : NamedItem, IPreprocessingData {

    public IntRange TrainingPartition { get; set; }
    public IntRange TestPartition { get; set; }

    protected IList<ITransformation> transformations;
    public IList<ITransformation> Transformations {
      get { return transformations; }
    }

    protected IList<IList> variableValues;
    protected IList<string> variableNames;

    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

    public IEnumerable<string> GetDoubleVariableNames() {
      var doubleVariableNames = new List<string>();
      for (int i = 0; i < Columns; ++i) {
        if (VariableHasType<double>(i)) {
          doubleVariableNames.Add(variableNames[i]);
        }
      }
      return doubleVariableNames;
    }

    public int Columns {
      get { return variableNames.Count; }
    }

    public int Rows {
      get { return variableValues.Count > 0 ? variableValues[0].Count : 0; }
    }

    protected IDictionary<int, IList<int>> selection;
    public IDictionary<int, IList<int>> Selection {
      get { return selection; }
      set {
        selection = value;
        OnSelectionChanged();
      }
    }

    protected PreprocessingData(PreprocessingData original, Cloner cloner)
      : base(original, cloner) {
      variableValues = CopyVariableValues(original.variableValues);
      variableNames = new List<string>(original.variableNames);
      TrainingPartition = (IntRange)original.TrainingPartition.Clone(cloner);
      TestPartition = (IntRange)original.TestPartition.Clone(cloner);
      transformations = new List<ITransformation>();

      RegisterEventHandler();
    }

    protected PreprocessingData(IDataAnalysisProblemData problemData)
      : base() {
      Name = "Preprocessing Data";

      transformations = new List<ITransformation>();
      selection = new Dictionary<int, IList<int>>();

      Dataset dataset = problemData.Dataset;
      variableNames = new List<string>(problemData.Dataset.VariableNames);

      int columnIndex = 0;
      variableValues = new List<IList>();
      foreach (var variableName in problemData.Dataset.VariableNames) {
        if (dataset.VariableHasType<double>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetDoubleValues(variableName).ToList());
        } else if (dataset.VariableHasType<string>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetStringValues(variableName).ToList());
        } else if (dataset.VariableHasType<DateTime>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetDateTimeValues(variableName).ToList());
        } else {
          throw new ArgumentException("The datatype of column " + variableName + " must be of type double, string or DateTime");
        }
        ++columnIndex;
      }

      TrainingPartition = new IntRange(problemData.TrainingPartition.Start, problemData.TrainingPartition.End);
      TestPartition = new IntRange(problemData.TestPartition.Start, problemData.TestPartition.End);

      RegisterEventHandler();
    }

    private void RegisterEventHandler() {
      Changed += (s, e) => {
        switch (e.Type) {
          case DataPreprocessingChangedEventType.DeleteRow:
            CheckPartitionRanges();
            break;
          case DataPreprocessingChangedEventType.Any:
            CheckPartitionRanges();
            break;
          case DataPreprocessingChangedEventType.Transformation:
            CheckPartitionRanges();
            break;
        }
      };
    }

    private static IList CreateColumn<T>(Dataset ds, int column, Func<string, T> selector) {
      var list = new List<T>(ds.Rows);
      for (int row = 0; row < ds.Rows; ++row) {
        list.Add(selector(ds.GetValue(row, column)));
      }
      return list;
    }

    private void CheckPartitionRanges() {
      int maxRowIndex = Math.Max(0, Rows - 1);
      TrainingPartition.Start = Math.Min(TrainingPartition.Start, maxRowIndex);
      TrainingPartition.End = Math.Min(TrainingPartition.End, maxRowIndex);
      TestPartition.Start = Math.Min(TestPartition.Start, maxRowIndex);
      TestPartition.End = Math.Min(TestPartition.End, maxRowIndex);
    }

    protected IList<IList> CopyVariableValues(IList<IList> original) {
      var copy = new List<IList>(original);
      for (int i = 0; i < original.Count; ++i) {
        copy[i] = (IList)Activator.CreateInstance(original[i].GetType(), original[i]);
      }
      return copy;
    }


    #region IPreprocessingData Members

    public abstract T GetCell<T>(int columnIndex, int rowIndex);

    public abstract void SetCell<T>(int columnIndex, int rowIndex, T value);

    public abstract string GetCellAsString(int columnIndex, int rowIndex);

    public abstract string GetVariableName(int columnIndex);

    public abstract int GetColumnIndex(string variableName);

    public abstract bool VariableHasType<T>(int columnIndex);

    [Obsolete("use the index based variant, is faster")]
    public abstract IList<T> GetValues<T>(string variableName, bool considerSelection);

    public abstract IList<T> GetValues<T>(int columnIndex, bool considerSelection);

    public abstract void SetValues<T>(int columnIndex, IList<T> values);

    public abstract bool SetValue(string value, int columnIndex, int rowIndex);

    public abstract bool Validate(string value, out string errorMessage, int columnIndex);

    public abstract bool AreAllStringColumns(IEnumerable<int> columnIndices);

    public abstract void DeleteRowsWithIndices(IEnumerable<int> rows);

    public abstract void InsertRow(int rowIndex);

    public abstract void DeleteRow(int rowIndex);

    public abstract void InsertColumn<T>(string variableName, int columnIndex);

    public abstract void DeleteColumn(int columnIndex);

    public abstract Dataset ExportToDataset();

    public abstract void ClearSelection();

    public abstract event EventHandler SelectionChanged;
    protected abstract void OnSelectionChanged();

    public event DataPreprocessingChangedEventHandler Changed;
    protected virtual void OnChanged(DataPreprocessingChangedEventType type, int column, int row) {
      var listeners = Changed;
      if (listeners != null) listeners(this, new DataPreprocessingChangedEventArgs(type, column, row));
    }
    #endregion
  }
}
