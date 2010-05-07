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
  public abstract class DataAnalysisSolution : NamedItem {
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
          OnProblemDataChanged(EventArgs.Empty);
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
          OnEstimatedValuesChanged(EventArgs.Empty);
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
          OnEstimatedValuesChanged(EventArgs.Empty);
        }
      }
    }

    public abstract IEnumerable<double> EstimatedValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingValues { get; }
    public abstract IEnumerable<double> EstimatedTestValues { get; }

    protected DataAnalysisSolution() : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
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

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisSolution clone = (DataAnalysisSolution)base.Clone(cloner);
      // don't clone the problem data!
      clone.problemData = problemData;
      clone.lowerEstimationLimit = lowerEstimationLimit;
      clone.upperEstimationLimit = upperEstimationLimit;
      clone.Initialize();
      return clone;
    }

    #region Events
    protected virtual void RegisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }
    protected virtual void DeregisterProblemDataEvents() {
      ProblemData.ProblemDataChanged += new EventHandler(ProblemData_Changed);
    }

    private void ProblemData_Changed(object sender, EventArgs e) {
      OnProblemDataChanged(EventArgs.Empty);
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged(EventArgs e) {
      var listeners = ProblemDataChanged;
      if (listeners != null)
        listeners(this, e);
    }

    public event EventHandler EstimatedValuesChanged;
    protected virtual void OnEstimatedValuesChanged(EventArgs e) {
      var listeners = EstimatedValuesChanged;
      if (listeners != null)
        listeners(this, e);
    }
    #endregion
  }
}
