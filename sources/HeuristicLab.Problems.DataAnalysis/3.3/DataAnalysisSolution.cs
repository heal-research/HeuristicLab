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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a solution for a data analysis problem which can be visualized in the GUI.
  /// </summary>
  [Item("DataAnalysisSolution", "Represents a solution for a data analysis problem which can be visualized in the GUI.")]
  [StorableClass]
  public abstract class DataAnalysisSolution : NamedItem, IStorableContent {
    #region IStorableContent Members
    public string Filename { get; set; }
    #endregion
    
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
      if (problemData != null)
        RegisterProblemDataEvents();
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
      var listeners = EstimatedValuesChanged;
      if (listeners != null)
        listeners(this, EventArgs.Empty);
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisSolution clone = (DataAnalysisSolution)base.Clone(cloner);
      clone.problemData = (DataAnalysisProblemData)cloner.Clone(problemData);
      clone.model = (IDataAnalysisModel)cloner.Clone(model);
      clone.lowerEstimationLimit = lowerEstimationLimit;
      clone.upperEstimationLimit = upperEstimationLimit;
      clone.Initialize();

      return clone;
    }
  }
}
