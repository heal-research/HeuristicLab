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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Problem", "Represents the base class for a problem.")]
  [StorableClass]
  public abstract class Problem<T, U> : ParameterizedNamedItem, IProblem
    where T : class,IEvaluator
    where U : class,ISolutionCreator {
    private const string EvaluatorParameterName = "Evaluator";
    private const string SolutionCreateParameterName = "SolutionCreator";

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    [StorableConstructor]
    protected Problem(bool deserializing) : base(deserializing) { }
    protected Problem()
      : base() {
      operators = new OperatorCollection();
      Parameters.Add(new ValueParameter<T>(EvaluatorParameterName, "The operator used to evaluate a solution."));
      Parameters.Add(new ValueParameter<U>(SolutionCreateParameterName, "The operator to create a solution."));
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Problem<T, U> clone = (Problem<T, U>)base.Clone(cloner);
      clone.operators = new OperatorCollection(operators.Select(x => (IOperator)cloner.Clone(x)));
      clone.RegisterEventHandlers();
      return clone;
    }

    private void RegisterEventHandlers() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);
      Operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);
      Operators.CollectionReset += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);

      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
    }

    #region properties
    private OperatorCollection operators;
    [Storable(Name = "Operators")]
    private IEnumerable<IOperator> StorableOperators {
      get { return operators; }
      set { operators = new OperatorCollection(value); }
    }
    protected OperatorCollection Operators {
      get { return this.operators; }
    }
    IEnumerable<IOperator> IProblem.Operators { get { return operators; } }

    public T Evaluator {
      get { return EvaluatorParameter.Value; }
      protected set { EvaluatorParameter.Value = value; }
    }
    public ValueParameter<T> EvaluatorParameter {
      get { return (ValueParameter<T>)Parameters[EvaluatorParameterName]; }
    }
    IEvaluator IProblem.Evaluator { get { return Evaluator; } }
    IParameter IProblem.EvaluatorParameter { get { return EvaluatorParameter; } }

    public U SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      protected set { SolutionCreatorParameter.Value = value; }
    }
    public ValueParameter<U> SolutionCreatorParameter {
      get { return (ValueParameter<U>)Parameters[SolutionCreateParameterName]; }
    }
    ISolutionCreator IProblem.SolutionCreator { get { return SolutionCreator; } }
    IParameter IProblem.SolutionCreatorParameter { get { return SolutionCreatorParameter; } }
    #endregion

    #region events
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged();
    }
    public event EventHandler EvaluatorChanged;
    protected virtual void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged();
    }
    public event EventHandler SolutionCreatorChanged;
    protected virtual void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    private void Operators_Changed(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    public event EventHandler OperatorsChanged;
    protected virtual void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public event EventHandler Reset;
    protected virtual void OnReset() {
      EventHandler handler = Reset;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
