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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate {
  [Item("Multi Variate Data Analysis Problem", "Represents a multi variate data analysis problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class MultiVariateDataAnalysisProblem : ParameterizedNamedItem, IMultiVariateDataAnalysisProblem {
    private const string MultiVariateDataAnalysisProblemDataParameterName = "MultiVariateDataAnalysisProblem";
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<MultiVariateDataAnalysisProblemData> MultiVariateDataAnalysisProblemDataParameter {
      get { return (ValueParameter<MultiVariateDataAnalysisProblemData>)Parameters[MultiVariateDataAnalysisProblemDataParameterName]; }
    }
    #endregion
    #region properties
    public MultiVariateDataAnalysisProblemData MultiVariateDataAnalysisProblemData {
      get { return MultiVariateDataAnalysisProblemDataParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiVariateDataAnalysisProblem(bool deserializing) : base(deserializing) { }
    public MultiVariateDataAnalysisProblem()
      : base() {
      Parameters.Add(new ValueParameter<MultiVariateDataAnalysisProblemData>(MultiVariateDataAnalysisProblemDataParameterName, "The data set, target variables and input variables of the multi-variate data analysis problem.", new MultiVariateDataAnalysisProblemData()));
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    #region events
    protected virtual void OnMultiVariateDataAnalysisProblemChanged(EventArgs e) {
      RaiseReset(e);
    }

    private void RegisterParameterEvents() {
      MultiVariateDataAnalysisProblemDataParameter.ValueChanged += new EventHandler(MultiVariateDataAnalysisProblemDataParameter_ValueChanged);
    }

    private void RegisterParameterValueEvents() {
      MultiVariateDataAnalysisProblemData.ProblemDataChanged += new EventHandler(MultiVariateDataAnalysisProblemData_ProblemDataChanged);
    }

    private void MultiVariateDataAnalysisProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      OnMultiVariateDataAnalysisProblemChanged(e);
    }

    private void MultiVariateDataAnalysisProblemData_ProblemDataChanged(object sender, EventArgs e) {
      OnMultiVariateDataAnalysisProblemChanged(e);
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      MultiVariateDataAnalysisProblem clone = (MultiVariateDataAnalysisProblem)base.Clone(cloner);
      clone.RegisterParameterEvents();
      clone.RegisterParameterValueEvents();
      return clone;
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
