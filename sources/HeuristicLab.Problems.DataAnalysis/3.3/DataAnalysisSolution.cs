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
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a solution for a data analysis problem which can be visualized in the GUI.
  /// </summary>
  [Item("DataAnalysisSolution", "Represents a solution for a data analysis problem which can be visualized in the GUI.")]
  [StorableClass]
  public abstract class DataAnalysisSolution : NamedItem, IStringConvertibleMatrix {
    protected DataAnalysisSolution()
      : base() { }
    protected DataAnalysisSolution(DataAnalysisProblemData problemData) : this(problemData, double.NegativeInfinity, double.PositiveInfinity) { }
    protected DataAnalysisSolution(DataAnalysisProblemData problemData, double lowerEstimationLimit, double upperEstimationLimit)
      : this() {
      this.problemData = problemData;
      this.lowerEstimationLimit = lowerEstimationLimit;
      this.upperEstimationLimit = upperEstimationLimit;
      Initialize();
    }

    [StorableConstructor]
    private DataAnalysisSolution(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (problemData != null) RegisterProblemDataEvents();
    }

    [Storable]
    private DataAnalysisProblemData problemData;
    public DataAnalysisProblemData ProblemData {
      get { return problemData; }
      set {
        if (problemData != value) {
          if (value == null) throw new ArgumentNullException();
          if (model != null && problemData != null && !problemData.InputVariables.Select(c => c.Value).SequenceEqual(
            value.InputVariables.Select(c => c.Value)))
            throw new ArgumentException("Could not set new problem data with different structure");

          if (problemData != null) DeregisterProblemDataEvents();
          problemData = value;
          RegisterProblemDataEvents();
          OnProblemDataChanged();
          RecalculateEstimatedValues();
        }
      }
    }

    [Storable]
    private IDataAnalysisModel model;
    public IDataAnalysisModel Model {
      get { return model; }
      set {
        if (model != value) {
          if (value == null) throw new ArgumentNullException();
          model = value;
          OnModelChanged();
          RecalculateEstimatedValues();
        }
      }
    }

    [Storable]
    private double lowerEstimationLimit;
    public double LowerEstimationLimit {
      get { return lowerEstimationLimit; }
      set {
        if (lowerEstimationLimit != value) {
          lowerEstimationLimit = value;
          RecalculateEstimatedValues();
        }
      }
    }

    [Storable]
    private double upperEstimationLimit;
    public double UpperEstimationLimit {
      get { return upperEstimationLimit; }
      set {
        if (upperEstimationLimit != value) {
          upperEstimationLimit = value;
          RecalculateEstimatedValues();
        }
      }
    }

    public abstract IEnumerable<double> EstimatedValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingValues { get; }
    public abstract IEnumerable<double> EstimatedTestValues { get; }
    protected abstract void RecalculateEstimatedValues();

    #region Events
    protected virtual void RegisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }
    protected virtual void DeregisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }
    private void ProblemData_Changed(object sender, EventArgs e) {
      OnProblemDataChanged();
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      var listeners = ProblemDataChanged;
      if (listeners != null)
        listeners(this, EventArgs.Empty);
    }

    public event EventHandler ModelChanged;
    protected virtual void OnModelChanged() {
      EventHandler handler = ModelChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public event EventHandler EstimatedValuesChanged;
    protected virtual void OnEstimatedValuesChanged() {
      RecalculateResultValues();
      var listeners = EstimatedValuesChanged;
      if (listeners != null)
        listeners(this, EventArgs.Empty);
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisSolution clone = (DataAnalysisSolution)base.Clone(cloner);
      // don't clone the problem data!
      clone.problemData = problemData;
      clone.Model = (IDataAnalysisModel)cloner.Clone(model);
      clone.lowerEstimationLimit = lowerEstimationLimit;
      clone.upperEstimationLimit = upperEstimationLimit;
      clone.Initialize();
      return clone;
    }

    #region IStringConvertibleMatrix implementation
    private List<string> rowNames = new List<string>() { "MeanSquaredError", "CoefficientOfDetermination" };
    private List<string> columnNames = new List<string>() { "Training", "Test" };
    private double[,] resultValues = new double[2, 2];
    int IStringConvertibleMatrix.Rows { get { return rowNames.Count; } set { } }
    int IStringConvertibleMatrix.Columns { get { return columnNames.Count; } set { } }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames { get { return columnNames; } set { } }
    IEnumerable<string> IStringConvertibleMatrix.RowNames { get { return rowNames; } set { } }
    bool IStringConvertibleMatrix.SortableView { get { return false; } set { } }
    bool IStringConvertibleMatrix.ReadOnly { get { return true; } }

    string IStringConvertibleMatrix.GetValue(int rowIndex, int columnIndex) {
      return resultValues[rowIndex, columnIndex].ToString();
    }
    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      errorMessage = "This matrix is readonly.";
      return false;
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) { return false; }

    protected void RecalculateResultValues() {
      IEnumerable<double> originalTrainingValues = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, problemData.TrainingSamplesStart.Value, problemData.TrainingSamplesEnd.Value);
      IEnumerable<double> originalTestValues = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, problemData.TestSamplesStart.Value, problemData.TestSamplesEnd.Value);
      resultValues[0, 0] = SimpleMSEEvaluator.Calculate(originalTrainingValues, EstimatedTrainingValues);
      resultValues[0, 1] = SimpleMSEEvaluator.Calculate(originalTestValues, EstimatedTestValues);
      resultValues[1, 0] = SimpleRSquaredEvaluator.Calculate(originalTrainingValues, EstimatedTrainingValues);
      resultValues[1, 1] = SimpleRSquaredEvaluator.Calculate(originalTestValues, EstimatedTestValues);
      this.OnReset();
    }

    public event EventHandler ColumnNamesChanged;
    public event EventHandler RowNamesChanged;
    public event EventHandler SortableViewChanged;
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    public event EventHandler Reset;
    protected virtual void OnReset() {
      EventHandler handler = Reset;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
