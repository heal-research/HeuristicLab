#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a solution for a data analysis problem which can be visualized in the GUI.
  /// </summary>
  [Item("DataAnalysisSolution", "Represents a solution for a data analysis problem which can be visualized in the GUI.")]
  [StorableClass]
  public class DataAnalysisSolution : Item {
    [Storable]
    private IModel model;
    public IModel Model {
      get { return model; }
      set {
        if (model != value) {
          model = value;
          OnModelChanged();
        }
      }
    }

    [Storable]
    private DataAnalysisProblemData problemData;
    public DataAnalysisProblemData ProblemData {
      get { return problemData; }
      set {
        if (problemData != value) {
          if (value == null) throw new ArgumentNullException();
          if (problemData != null) DeregisterProblemDataEvents();
          problemData = value;
          RegisterProblemDataEvents();
          OnProblemDataChanged();
        }
      }
    }

    private List<double> estimatedValues;
    public IEnumerable<double> EstimatedValues {
      get {
        return estimatedValues;
      }
    }

    private List<double> estimatedTrainingValues;
    public IEnumerable<double> EstimatedTrainingValues {
      get {
        return estimatedTrainingValues;
      }
    }

    private List<double> estimatedTestValues;
    public IEnumerable<double> EstimatedTestValues {
      get {
        return estimatedTestValues;
      }
    }

    public DataAnalysisSolution() : base() { }
    public DataAnalysisSolution(DataAnalysisProblemData problemData, IModel model)
      : this() {
      this.problemData = problemData;
      this.model = model;
      Initialize();
    }

    [StorableConstructor]
    private DataAnalysisSolution(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (problemData != null) RegisterProblemDataEvents();
    }

    private void RecalculateEstimatedValues() {
      estimatedValues = GetEstimatedValues(0, problemData.Dataset.Rows).ToList();
      int nTrainingValues = problemData.TrainingSamplesEnd.Value - problemData.TrainingSamplesStart.Value;
      estimatedTrainingValues = estimatedValues.Skip(problemData.TrainingSamplesStart.Value).Take(nTrainingValues).ToList();
      int nTestValues = problemData.TestSamplesEnd.Value - problemData.TestSamplesStart.Value;
      estimatedTestValues = estimatedValues.Skip(problemData.TestSamplesStart.Value).Take(nTestValues).ToList();
    }

    private IEnumerable<double> GetEstimatedValues(int start, int end) {
      double[] xs = new double[ProblemData.InputVariables.Count];
      for (int row = 0; row < ProblemData.Dataset.Rows; row++) {
        for (int i = 0; i < xs.Length; i++) {
          var variableIndex = ProblemData.Dataset.GetVariableIndex(ProblemData.InputVariables[i].Value);
          xs[i] = ProblemData.Dataset[row, variableIndex];
        }
        yield return model.GetValue(xs);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisSolution clone = new DataAnalysisSolution();
      cloner.RegisterClonedObject(this, clone);
      clone.model = (IModel)cloner.Clone(model);
      clone.problemData = problemData;
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler ModelChanged;
    private void OnModelChanged() {
      RecalculateEstimatedValues();
      var changed = ModelChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler ProblemDataChanged;
    private void OnProblemDataChanged() {
      RecalculateEstimatedValues();
      var changed = ProblemDataChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }
    private void DeregisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }

    private void ProblemData_Changed(object sender, EventArgs e) {
      OnProblemDataChanged();
    }
    #endregion
  }
}
