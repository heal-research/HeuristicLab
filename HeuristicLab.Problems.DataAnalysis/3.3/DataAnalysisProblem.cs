#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Data Analysis Problem", "Represents a data analysis problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class DataAnalysisProblem : ParameterizedNamedItem, IDataAnalysisProblem, IStorableContent {
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";

    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ValueParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected DataAnalysisProblem(bool deserializing) : base(deserializing) { }
    protected DataAnalysisProblem(DataAnalysisProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public DataAnalysisProblem()
      : base() {
      Parameters.Add(new ValueParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data set, target variable and input variables of the data analysis problem.", new DataAnalysisProblemData()));
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    #region events
    protected virtual void OnDataAnalysisProblemChanged(EventArgs e) {
      RaiseReset(e);
    }

    private void RegisterParameterEvents() {
      DataAnalysisProblemDataParameter.ValueChanged += new EventHandler(DataAnalysisProblemDataParameter_ValueChanged);
    }

    private void RegisterParameterValueEvents() {
      DataAnalysisProblemData.ProblemDataChanged += new EventHandler(DataAnalysisProblemData_ProblemDataChanged);
    }

    private void DataAnalysisProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      OnDataAnalysisProblemChanged(e);
    }

    private void DataAnalysisProblemData_ProblemDataChanged(object sender, EventArgs e) {
      OnDataAnalysisProblemChanged(e);
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataAnalysisProblem(this, cloner);
    }

    #region IProblem Members

    public virtual IParameter SolutionCreatorParameter {
      get { return null; }
    }

    public virtual ISolutionCreator SolutionCreator {
      get { return null; }
    }

    public virtual IParameter EvaluatorParameter {
      get { return null; }
    }

    public virtual IEvaluator Evaluator {
      get { return null; }
    }

    public virtual IEnumerable<IOperator> Operators {
      get { return new IOperator[0]; }
    }

    public event EventHandler SolutionCreatorChanged;
    protected void RaiseSolutionCreatorChanged(EventArgs e) {
      var changed = SolutionCreatorChanged;
      if (changed != null)
        changed(this, e);
    }

    public event EventHandler EvaluatorChanged;
    protected void RaiseEvaluatorChanged(EventArgs e) {
      var changed = EvaluatorChanged;
      if (changed != null)
        changed(this, e);
    }

    public event EventHandler OperatorsChanged;
    protected void RaiseOperatorsChanged(EventArgs e) {
      var changed = OperatorsChanged;
      if (changed != null)
        changed(this, e);
    }

    public event EventHandler Reset;
    protected void RaiseReset(EventArgs e) {
      var changed = Reset;
      if (changed != null)
        changed(this, e);
    }
    #endregion
  }
}
