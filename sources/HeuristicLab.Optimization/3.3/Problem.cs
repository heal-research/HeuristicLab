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

using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System;
using HeuristicLab.Common;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for optimization problems.
  /// </summary>
  [Item("Problem", "A base class for optimization problems.")]
  [EmptyStorableClass]
  public abstract class Problem : ParameterizedNamedItem, IProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    private IValueParameter<ISolutionCreator> SolutionCreatorParameter {
      get { return (IValueParameter<ISolutionCreator>)Parameters["SolutionCreator"]; }
    }
    private IValueParameter<IEvaluator> EvaluatorParameter {
      get { return (IValueParameter<IEvaluator>)Parameters["Evaluator"]; }
    }
    public ISolutionCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    public IEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    private OperatorSet operators;
    public OperatorSet Operators {
      get { return operators; }
    }

    protected Problem()
      : base() {
      AddParameters();
    }
    protected Problem(string name)
      : base(name) {
      AddParameters();
    }
    protected Problem(string name, ParameterCollection parameters)
      : base(name, parameters) {
      AddParameters();
    }
    protected Problem(string name, string description)
      : base(name, description) {
      AddParameters();
    }
    protected Problem(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      AddParameters();
    }

    private void AddParameters() {
      ValueParameter<ISolutionCreator> solutionCreatorParameter = new ValueParameter<ISolutionCreator>("SolutionCreator", "The operator which should be used to create new solutions.");
      solutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      Parameters.Add(solutionCreatorParameter);

      ValueParameter<IEvaluator> evaluatorParameter = new ValueParameter<IEvaluator>("Evaluator", "The operator which should be used to evaluate solutions.");
      evaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Parameters.Add(evaluatorParameter);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged();
    }

    public event EventHandler SolutionCreatorChanged;
    protected virtual void OnSolutionCreatorChanged() {
      if (SolutionCreatorChanged != null)
        SolutionCreatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    protected virtual void OnEvaluatorChanged() {
      if (EvaluatorChanged != null)
        EvaluatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Type>> OperatorsChanged;
    protected virtual void OnOperatorsChanged(Type operatorType) {
      if (OperatorsChanged != null)
        OperatorsChanged(this, new EventArgs<Type>(operatorType));
    }
  }
}
