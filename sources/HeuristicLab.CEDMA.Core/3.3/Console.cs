#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Modeling.Database.SQLServerCompact;
using HeuristicLab.SparseMatrix;
using HeuristicLab.Modeling.Database;
using HeuristicLab.CEDMA.Charting;

namespace HeuristicLab.CEDMA.Core {
  public class Console : ItemBase, IEditable {
    private static readonly string sqlServerCompactConnectionString = @"Data Source=";

    public Console()
      : base() {
    }

    private string database;
    public string Database {
      get {
        return database;
      }
      set {
        if (value != database) {
          database = value;
          matrix = null;
          visualMatrix = null;
        }
      }
    }

    private Matrix matrix;
    public Matrix Matrix {
      get {
        if (matrix == null) LoadResults();
        return matrix;
      }
    }

    private VisualMatrix visualMatrix;
    public VisualMatrix VisualMatrix {
      get {
        if (matrix == null)
          visualMatrix = CreateVisualMatrix();
        return visualMatrix;
      }
    }

    public IEditor CreateEditor() {
      return new ConsoleEditor(this);
    }

    public override IView CreateView() {
      return new ConsoleEditor(this);
    }

    private void LoadResults() {
      matrix = new Matrix();
      DatabaseService db = new DatabaseService(sqlServerCompactConnectionString + database);
      db.Connect();

      foreach (var model in db.GetAllModels()) {
        MatrixRow row = new MatrixRow();
        foreach (var modelResult in db.GetModelResults(model)) {
          row.Set(modelResult.Result.Name, modelResult.Value);
        }
        row.Set("PersistedData", db.GetModelData(model));
        row.Set("TargetVariable", model.TargetVariable.Name);
        row.Set("Algorithm", model.Algorithm.Name);
        Dictionary<HeuristicLab.Modeling.Database.IVariable, MatrixRow> inputVariableResultsEntries =
          new Dictionary<HeuristicLab.Modeling.Database.IVariable, MatrixRow>();

        foreach (IInputVariableResult inputVariableResult in db.GetInputVariableResults(model)) {
          if (!inputVariableResultsEntries.ContainsKey(inputVariableResult.Variable)) {
            inputVariableResultsEntries[inputVariableResult.Variable] = new MatrixRow();
            inputVariableResultsEntries[inputVariableResult.Variable].Set("InputVariableName", inputVariableResult.Variable.Name);
          }
          inputVariableResultsEntries[inputVariableResult.Variable].Set(inputVariableResult.Result.Name, inputVariableResult.Value);
        }
        row.Set("VariableImpacts", inputVariableResultsEntries.Values);
        matrix.AddRow(row);
      }
      db.Disconnect();
    }

    private VisualMatrix CreateVisualMatrix() {      
      DatabaseService db = new DatabaseService(sqlServerCompactConnectionString + database);
      db.Connect();
      string[] multiDimensionalCategoricalVariables = new string[] { "VariableImpacts: InputVariableName" };
      string[] multiDimensionalOrdinalVariables = db.GetAllResultsForInputVariables().Select(x => "VariableImpacts: " + x.Name).ToArray();
      string[] ordinalVariables = db.GetAllResults().Select(r => r.Name).ToArray();
      string[] categoricalVariables = new string[] { "TargetVariable", "Algorithm" };

      db.Disconnect();
      VisualMatrix m = new VisualMatrix(Matrix, categoricalVariables, ordinalVariables, multiDimensionalCategoricalVariables, multiDimensionalOrdinalVariables);
      return m;
    }
  }
}
