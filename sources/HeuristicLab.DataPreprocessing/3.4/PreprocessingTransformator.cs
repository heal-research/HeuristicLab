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
using System.Linq;
using System.Text;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  public class PreprocessingTransformator {
    private readonly ITransactionalPreprocessingData preprocessingData;

    private readonly IDictionary<string, IList<double>> originalColumns;

    private readonly IDictionary<string, string> renamedColumns;

    public PreprocessingTransformator(IPreprocessingData preprocessingData) {
      this.preprocessingData = (ITransactionalPreprocessingData)preprocessingData;
      originalColumns = new Dictionary<string, IList<double>>();
      renamedColumns = new Dictionary<string, string>();
    }

    public bool ApplyTransformations(IEnumerable<ITransformation> transformations, bool preserveColumns, out string errorMsg) {
      bool success = false;
      errorMsg = string.Empty;
      preprocessingData.BeginTransaction(DataPreprocessingChangedEventType.Transformation);

      try {
        var doubleTransformations = transformations.OfType<Transformation<double>>().ToList();

        if (preserveColumns) {
          PreserveColumns(doubleTransformations);
        }

        // all transformations are performed inplace. no creation of new columns for transformations
        ApplyDoubleTranformationsInplace(doubleTransformations, preserveColumns, out success, out errorMsg);

        if (preserveColumns) {
          RenameTransformedColumsAndRestorePreservedColumns(doubleTransformations);
          RenameTransformationColumnParameter(doubleTransformations);
          InsertCopyColumTransformations(doubleTransformations);

          originalColumns.Clear();
          renamedColumns.Clear();
        }
        // only accept changes if everything was successful
        if (!success) {
          preprocessingData.Undo();
        }
      }
      catch (Exception e) {
        preprocessingData.Undo();
        if (string.IsNullOrEmpty(errorMsg)) errorMsg = e.Message;
      }
      finally {
        preprocessingData.EndTransaction();
      }

      return success;
    }

    private void PreserveColumns(IEnumerable<Transformation<double>> transformations) {
      foreach (var transformation in transformations) {
        if (!originalColumns.ContainsKey(transformation.Column)) {
          int colIndex = preprocessingData.GetColumnIndex(transformation.Column);
          var originalData = preprocessingData.GetValues<double>(colIndex);
          originalColumns.Add(transformation.Column, originalData);
        }
      }
    }

    private void ApplyDoubleTranformationsInplace(IEnumerable<Transformation<double>> transformations, bool preserveColumns, out bool success, out string errorMsg) {
      errorMsg = string.Empty;
      success = true;
      foreach (var transformation in transformations) {
        int colIndex = preprocessingData.GetColumnIndex(transformation.Column);

        var originalData = preprocessingData.GetValues<double>(colIndex);

        string errorMsgPart;
        bool successPart;
        var transformedData = ApplyDoubleTransformation(transformation, originalData, out successPart, out errorMsgPart);
        errorMsg += errorMsgPart + Environment.NewLine;

        if (!successPart) success = false;
        preprocessingData.SetValues(colIndex, transformedData.ToList());
        preprocessingData.Transformations.Add(transformation);
      }
    }

    private IEnumerable<double> ApplyDoubleTransformation(Transformation<double> transformation, IList<double> data, out bool success, out string errorMsg) {
      success = transformation.Check(data, out errorMsg);
      // don't apply when the check fails
      if (success)
        return transformation.Apply(data);
      else
        return data;
    }

    private void RenameTransformationColumnParameter(List<Transformation<double>> transformations) {
      foreach (var transformation in transformations) {
        var newColumnName = new StringValue(renamedColumns[transformation.Column]);
        transformation.ColumnParameter.ValidValues.Add(newColumnName);
        transformation.ColumnParameter.Value = newColumnName;
      }
    }

    private void InsertCopyColumTransformations(IList<Transformation<double>> transformations) {
      foreach (var renaming in renamedColumns) {
        string oldName = renaming.Key;
        string newName = renaming.Value;

        var copyTransformation = CreateCopyTransformation(oldName, newName);
        preprocessingData.Transformations.Insert(0, copyTransformation);
      }
    }

    private CopyColumnTransformation CreateCopyTransformation(string oldColumn, string newColumn) {
      var newColumName = new StringValue(newColumn);

      var copyTransformation = new CopyColumnTransformation();
      copyTransformation.ColumnParameter.ValidValues.Add(newColumName);
      copyTransformation.ColumnParameter.Value = newColumName;

      copyTransformation.CopiedColumnNameParameter.Value.Value = oldColumn;
      return copyTransformation;
    }

    private void RenameTransformedColumsAndRestorePreservedColumns(IList<Transformation<double>> transformations) {
      foreach (var column in originalColumns) {
        int originalColumnIndex = preprocessingData.GetColumnIndex(column.Key);
        int newColumnIndex = originalColumnIndex + 1;
        string newColumnName = GetTransformatedColumnName(transformations, column.Key);
        // save renaming mapping
        renamedColumns[column.Key] = newColumnName;
        // create new transformed column
        preprocessingData.InsertColumn<double>(newColumnName, newColumnIndex);
        preprocessingData.SetValues(newColumnIndex, preprocessingData.GetValues<double>(originalColumnIndex));
        // restore old values
        preprocessingData.SetValues(originalColumnIndex, column.Value);
      }
    }

    private string GetTransformatedColumnName(IList<Transformation<double>> transformations, string column) {
      string suffix = GetTransformationSuffix(transformations, column);
      return column + "_" + suffix;
    }

    private string GetTransformationSuffix(IList<Transformation<double>> transformations, string column) {
      var suffixes = transformations.Where(t => t.Column == column).Select(t => t.ShortName);
      var builder = new StringBuilder();
      foreach (var suffix in suffixes) {
        builder.Append(suffix);
      }
      return builder.ToString();
    }
  }
}
