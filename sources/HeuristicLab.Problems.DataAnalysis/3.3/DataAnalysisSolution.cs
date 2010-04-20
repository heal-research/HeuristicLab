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

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a solution for a data analysis problem which can be visualized in the GUI.
  /// </summary>
  [Item("DataAnalysisSolution", "Represents a solution for a data analysis problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class DataAnalysisSolution : Item {
    [Storable]
    private IPredictor predictor;
    public IPredictor Predictor {
      get { return predictor; }
      set {
        if (predictor != value) {
          predictor = value;
          OnPredictorChanged();
        }
      }
    }

    [Storable]
    private DataAnalysisProblemData problemData;
    public DataAnalysisProblemData ProblemData {
      get { return problemData; }
      set {
        if (problemData != value) {
          if (problemData != null) DeregisterProblemDataEvents();
          problemData = value;
          if (problemData != null) RegisterProblemDataEvents();
          OnProblemDataChanged();
        }
      }
    }
    
    public DataAnalysisSolution() : base() { }
    public DataAnalysisSolution(DataAnalysisProblemData problemData, IPredictor predictor)
      : this() {
      this.problemData = problemData;
      this.predictor = predictor;
      Initialize();
    }
    [StorableConstructor]
    private DataAnalysisSolution(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (problemData != null) RegisterProblemDataEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisSolution clone = new DataAnalysisSolution();
      cloner.RegisterClonedObject(this, clone);
      clone.predictor = (IPredictor)cloner.Clone(predictor);
      clone.problemData = problemData;
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler PredictorChanged;
    private void OnPredictorChanged() {
      var changed = PredictorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler ProblemDataChanged;
    private void OnProblemDataChanged() {
      var changed = ProblemDataChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterProblemDataEvents() {
      ProblemData.DatasetChanged += new EventHandler(ProblemData_DataSetChanged);
    }
    private void DeregisterProblemDataEvents() {
      ProblemData.DatasetChanged += new EventHandler(ProblemData_DataSetChanged);
    }

    private void ProblemData_DataSetChanged(object sender, EventArgs e) {
      OnProblemDataChanged();
    }
    #endregion
  }
}
