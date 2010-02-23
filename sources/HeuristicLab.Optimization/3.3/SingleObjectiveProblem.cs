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
using HeuristicLab.Data;
using System;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for single-objective optimization problems.
  /// </summary>
  [Item("SingleObjectiveProblem", "A base class for single-objective optimization problems.")]
  [EmptyStorableClass]
  public abstract class SingleObjectiveProblem : Problem, ISingleObjectiveProblem {
    private ValueParameter<BoolData> MaximizationParameter {
      get { return (ValueParameter<BoolData>)Parameters["Maximization"]; }
    }
    public BoolData Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public new ISingleObjectiveEvaluator Evaluator {
      get { return (ISingleObjectiveEvaluator)base.Evaluator; }
      set { base.Evaluator = value; }
    }

    protected SingleObjectiveProblem()
      : base() {
      AddParameters();
    }
    protected SingleObjectiveProblem(string name)
      : base(name) {
      AddParameters();
    }
    protected SingleObjectiveProblem(string name, ParameterCollection parameters)
      : base(name, parameters) {
      AddParameters();
    }
    protected SingleObjectiveProblem(string name, string description)
      : base(name, description) {
      AddParameters();
    }
    protected SingleObjectiveProblem(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      AddParameters();
    }

    private void AddParameters() {
      ValueParameter<BoolData> maximizationParameter = new ValueParameter<BoolData>("Maximization", "True if the problem is a maximization problem, otherwise false.", new BoolData(true));
      maximizationParameter.ValueChanged += new System.EventHandler(MaximizationParameter_ValueChanged);
      Parameters.Add(maximizationParameter);
    }

    private void MaximizationParameter_ValueChanged(object sender, System.EventArgs e) {
      OnMaximizationChanged();
    }

    public event EventHandler MaximizationChanged;
    protected virtual void OnMaximizationChanged() {
      if (MaximizationChanged != null)
        MaximizationChanged(this, EventArgs.Empty);
    }
  }
}
