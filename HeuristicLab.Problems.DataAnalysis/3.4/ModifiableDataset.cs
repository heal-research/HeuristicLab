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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("ModifiableDataset", "Represents a dataset containing data that should be analyzed, which can be modified by adding or replacing variables and values.")]
  [StorableClass]
  public sealed class ModifiableDataset : Dataset, IStringConvertibleMatrix {
    [StorableConstructor]
    private ModifiableDataset(bool deserializing) : base(deserializing) { }

    private ModifiableDataset(ModifiableDataset original, Cloner cloner) : base(original, cloner) {
      variableNames = new List<string>(original.variableNames);
      variableValues = CloneValues(original.variableValues);
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new ModifiableDataset(this, cloner); }

    public ModifiableDataset() { }

    public ModifiableDataset(IEnumerable<string> variableNames, IEnumerable<IList> variableValues, bool cloneValues = false) :
      base(variableNames, variableValues, cloneValues) { }

    public Dataset ToDataset() {
      return new Dataset(variableNames, variableNames.Select(v => variableValues[v]));
    }


    public IEnumerable<object> GetRow(int row) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException(string.Format("Invalid row {0} specified. The dataset contains {1} row(s).", row, Rows));

      return variableValues.Select(x => x.Value[row]);
    }

    public void AddRow(IEnumerable<object> values) {
      var list = values.ToList();
      if (list.Count != variableNames.Count)
        throw new ArgumentException("The number of values must be equal to the number of variable names.");
      // check if all the values are of the correct type
      for (int i = 0; i < list.Count; ++i) {
        if (list[i].GetType() != GetVariableType(variableNames[i])) {
          throw new ArgumentException("The type of the provided value does not match the variable type.");
        }
      }
      // add values
      for (int i = 0; i < list.Count; ++i) {
        variableValues[variableNames[i]].Add(list[i]);
      }
      Rows++;
      OnRowsChanged();
      OnReset();
    }

    public void ReplaceRow(int row, IEnumerable<object> values) {
      var list = values.ToList();
      if (list.Count != variableNames.Count)
        throw new ArgumentException("The number of values must be equal to the number of variable names.");
      // check if all the values are of the correct type
      for (int i = 0; i < list.Count; ++i) {
        if (list[i].GetType() != GetVariableType(variableNames[i])) {
          throw new ArgumentException("The type of the provided value does not match the variable type.");
        }
      }
      // replace values
      for (int i = 0; i < list.Count; ++i) {
        variableValues[variableNames[i]][row] = list[i];
      }
      OnReset();
    }

    // slow, avoid using this
    public void RemoveRow(int row) {
      foreach (var list in variableValues.Values)
        list.RemoveAt(row);
      Rows--;
      OnRowsChanged();
      OnReset();
    }

    // adds a new variable to the dataset
    public void AddVariable(string variableName, IList values) {
      InsertVariable(variableName, Columns, values);
    }

    public void InsertVariable(string variableName, int position, IList values) {
      if (variableValues.ContainsKey(variableName))
        throw new ArgumentException(string.Format("Variable {0} is already present in the dataset.", variableName));

      if (position < 0 || position > Columns)
        throw new ArgumentException(string.Format("Incorrect position {0} specified. The position must be between 0 and {1}.", position, Columns));

      if (values == null)
        throw new ArgumentNullException("values", "Values must not be null. At least an empty list of values has to be provided.");

      if (values.Count != Rows)
        throw new ArgumentException(string.Format("{0} values are provided, but {1} rows are present in the dataset.", values.Count, Rows));

      if (!IsAllowedType(values))
        throw new ArgumentException(string.Format("Unsupported type {0} for variable {1}.", GetElementType(values), variableName));

      variableNames.Insert(position, variableName);
      variableValues[variableName] = values;

      OnColumnsChanged();
      OnColumnNamesChanged();
      OnReset();
    }

    public void ReplaceVariable(string variableName, IList values) {
      if (!variableValues.ContainsKey(variableName))
        throw new ArgumentException(string.Format("Variable {0} is not present in the dataset.", variableName));
      if (values.Count != variableValues[variableName].Count)
        throw new ArgumentException("The number of values must coincide with the number of dataset rows.");
      if (GetVariableType(variableName) != values[0].GetType())
        throw new ArgumentException("The type of the provided value does not match the variable type.");
      variableValues[variableName] = values;
    }


    public void RemoveVariable(string variableName) {
      if (!variableValues.ContainsKey(variableName))
        throw new ArgumentException(string.Format("The variable {0} does not exist in the dataset.", variableName));
      variableValues.Remove(variableName);
      variableNames.Remove(variableName);
      OnColumnsChanged();
      OnColumnNamesChanged();
      OnReset();
    }

    public void ClearValues() {
      foreach (var list in variableValues.Values) {
        list.Clear();
      }
      Rows = 0;
      OnRowsChanged();
      OnReset();
    }


    public void SetVariableValue(object value, string variableName, int row) {
      IList list;
      variableValues.TryGetValue(variableName, out list);
      if (list == null)
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      if (row < 0 || list.Count < row)
        throw new ArgumentOutOfRangeException("Invalid row value");
      if (GetVariableType(variableName) != value.GetType())
        throw new ArgumentException("The type of the provided value does not match the variable type.");

      list[row] = value;
      OnItemChanged(row, variableNames.IndexOf(variableName));
    }

    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      var variableName = variableNames[columnIndex];
      // if value represents a double
      double dv;
      if (double.TryParse(value, out dv)) {
        SetVariableValue(dv, variableName, rowIndex);
        return true;
      }
      // if value represents a DateTime object
      DateTime dt;
      if (DateTime.TryParse(value, out dt)) {
        SetVariableValue(dt, variableName, rowIndex);
        return true;
      }
      // if value is simply a string
      SetVariableValue(value, variableName, rowIndex);
      return true;
    }

    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      errorMessage = string.Empty;
      return true;
    }

    #region event handlers
    public override event EventHandler RowsChanged;
    private void OnRowsChanged() {
      var handler = RowsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public override event EventHandler ColumnsChanged;
    private void OnColumnsChanged() {
      var handler = ColumnsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public override event EventHandler ColumnNamesChanged;
    private void OnColumnNamesChanged() {
      var handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public override event EventHandler Reset;
    private void OnReset() {
      var handler = Reset;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public override event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int rowIndex, int columnIndex) {
      var handler = ItemChanged;
      if (handler != null) {
        handler(this, new EventArgs<int, int>(rowIndex, columnIndex));
      }
    }
    #endregion
  }
}
