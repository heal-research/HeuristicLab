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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("67267E9F-8A34-4CB6-9776-DD1483131987")]
  public abstract class DataAnalysisProblem<T> : Problem, IDataAnalysisProblem<T>,
    IProblemInstanceConsumer<T>, IProblemInstanceExporter<T>
    where T : class, IDataAnalysisProblemData {
    private const string ProblemDataParameterName = "ProblemData";
    private const string ProblemDataParameterDescription = "The data set, target variable and input variables of the data analysis problem.";
    #region parameter properties
    IParameter IDataAnalysisProblem.ProblemDataParameter {
      get { return ProblemDataParameter; }
    }
    public IValueParameter<T> ProblemDataParameter {
      get { return (IValueParameter<T>)Parameters[ProblemDataParameterName]; }
    }
    #endregion
    #region properties
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData {
      get { return ProblemData; }
    }
    public T ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }
    #endregion
    protected DataAnalysisProblem(DataAnalysisProblem<T> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    [StorableConstructor]
    protected DataAnalysisProblem(StorableConstructorFlag _) : base(_) { }

    protected DataAnalysisProblem(T problemData)
      : base() {
      Parameters.Add(new ValueParameter<T>(ProblemDataParameterName, ProblemDataParameterDescription, problemData));
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      ProblemDataParameter.ValueChanged += new EventHandler(ProblemDataParameter_ValueChanged);
      if (ProblemDataParameter.Value != null) ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
    }

    private void ProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
      OnProblemDataChanged();
      OnReset();
    }

    private void ProblemData_Changed(object sender, EventArgs e) {
      OnReset();
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      var handler = ProblemDataChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    #region Import & Export
    public void Load(T data) {
      Name = data.Name;
      Description = data.Description;
      ProblemData = data;
    }

    public T Export() {
      return ProblemData;
    }
    #endregion
  }
}
