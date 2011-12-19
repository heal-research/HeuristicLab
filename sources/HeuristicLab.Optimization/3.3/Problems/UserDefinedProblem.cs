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
using System.Linq;
using System.Threading;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A problem which can be defined by the user.
  /// </summary>
  [Item("User-Defined Problem", "A problem which can be defined by the user.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class UserDefinedProblem : ParameterizedNamedItem, ISingleObjectiveHeuristicOptimizationProblem, IStorableContent {
    public string Filename { get; set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }
    public new ParameterCollection Parameters {
      get { return base.Parameters; }
    }
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get { return Parameters; }
    }

    #region Parameters
    public IValueParameter<ISingleObjectiveEvaluator> EvaluatorParameter {
      get { return (IValueParameter<ISingleObjectiveEvaluator>)Parameters["Evaluator"]; }
    }
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<ISolutionCreator> SolutionCreatorParameter {
      get { return (ValueParameter<ISolutionCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public OptionalValueParameter<IScope> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<IScope>)Parameters["BestKnownSolution"]; }
    }
    public ValueParameter<ItemList<IOperator>> OperatorsParameter {
      get { return (ValueParameter<ItemList<IOperator>>)Parameters["Operators"]; }
    }
    #endregion

    #region Properties
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public ISolutionCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ISingleObjectiveEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    public IEnumerable<IOperator> Operators {
      get { return OperatorsParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private UserDefinedProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }
    public UserDefinedProblem()
      : base() {
      Parameters.Add(new ValueParameter<ISingleObjectiveEvaluator>("Evaluator", "The evaluator that collects the values to exchange.", new EmptyUserDefinedProblemEvaluator()));
      Parameters.Add(new ValueParameter<ISolutionCreator>("SolutionCreator", "An operator to create the solution components."));
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as most test functions are minimization problems.", new BoolValue(false)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this problem."));
      Parameters.Add(new OptionalValueParameter<IScope>("BestKnownSolution", "The best known solution for this external evaluation problem."));
      Parameters.Add(new ValueParameter<ItemList<IOperator>>("Operators", "The operators that are passed to the algorithm.", new ItemList<IOperator>()));

      AttachEventHandlers();
    }

    private UserDefinedProblem(UserDefinedProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserDefinedProblem(this, cloner);
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    private void OnReset() {
      EventHandler handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region Event handlers
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      if (Evaluator != null)
        Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeOperators();
      OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }
    private void OperatorsParameter_ValueChanged(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_ItemsAdded(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_ItemsRemoved(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_CollectionReset(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    #endregion

    #region Helpers
    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      if (Evaluator != null)
        Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      OperatorsParameter.ValueChanged += new EventHandler(OperatorsParameter_ValueChanged);
      OperatorsParameter.Value.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsAdded);
      OperatorsParameter.Value.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsRemoved);
      OperatorsParameter.Value.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_CollectionReset);
    }

    private void ParameterizeOperators() {
      // A best effort approach to wiring
      if (Evaluator != null) {
        string qualityName = Evaluator.QualityParameter.ActualName;
        foreach (IOperator op in OperatorsParameter.Value) {
          foreach (ILookupParameter<DoubleValue> param in op.Parameters.OfType<ILookupParameter<DoubleValue>>()) {
            if (param.Name.Equals("Quality")) param.ActualName = qualityName;
          }
          foreach (IScopeTreeLookupParameter<DoubleValue> param in op.Parameters.OfType<IScopeTreeLookupParameter<DoubleValue>>()) {
            if (param.Name.Equals("Quality")) param.ActualName = qualityName;
          }
        }
      }
    }
    #endregion

    [Item("EmptyUserDefinedProblemEvaluator", "A dummy evaluator that will throw an exception when executed.")]
    [StorableClass]
    [NonDiscoverableType]
    private sealed class EmptyUserDefinedProblemEvaluator : ParameterizedNamedItem, ISingleObjectiveEvaluator {

      [StorableConstructor]
      private EmptyUserDefinedProblemEvaluator(bool deserializing) : base(deserializing) { }
      private EmptyUserDefinedProblemEvaluator(EmptyUserDefinedProblemEvaluator original, Cloner cloner)
        : base(original, cloner) {
      }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new EmptyUserDefinedProblemEvaluator(this, cloner);
      }

      #region ISingleObjectiveEvaluator Members

      public ILookupParameter<DoubleValue> QualityParameter {
        get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
      }

      #endregion

      public EmptyUserDefinedProblemEvaluator() {
        Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The solution quality."));
      }

      #region IOperator Members

      public bool Breakpoint { get; set; }

      public IOperation Execute(IExecutionContext context, CancellationToken cancellationToken) {
        throw new InvalidOperationException("Please choose an appropriate evaluation operator.");
      }

#pragma warning disable 67
      public event EventHandler BreakpointChanged;

      public event EventHandler Executed;
#pragma warning restore 67

      #endregion

      public static new Image StaticItemImage {
        get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
      }
    }
  }
}
