#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("GenderSpecificSelection", "Brings two parents together by sampling each with a different selection scheme (Wagner, S. and Affenzeller, M. 2005. SexualGA: Gender-Specific Selection for Genetic Algorithms. Proceedings of the 9th World Multi-Conference on Systemics, Cybernetics and Informatics (WMSCI), pp. 76-81).")]
  [StorableClass]
  public class GenderSpecificSelector : AlgorithmOperator, ISingleObjectiveSelector, IStochasticOperator {
    #region Parameters
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<IntValue> NumberOfSelectedSubScopesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NumberOfSelectedSubScopes"]; }
    }
    protected IValueLookupParameter<BoolValue> CopySelectedParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["CopySelected"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueParameter<ISelector> FemaleSelectorParameter {
      get { return (ValueParameter<ISelector>)Parameters["FemaleSelector"]; }
    }
    public ValueParameter<ISelector> MaleSelectorParameter {
      get { return (ValueParameter<ISelector>)Parameters["MaleSelector"]; }
    }
    #endregion

    #region Properties
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public IntValue NumberOfSelectedSubScopes {
      get { return NumberOfSelectedSubScopesParameter.Value; }
      set { NumberOfSelectedSubScopesParameter.Value = value; }
    }
    public BoolValue CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }
    public ISelector FemaleSelector {
      get { return FemaleSelectorParameter.Value; }
      set { FemaleSelectorParameter.Value = value; }
    }
    public ISelector MaleSelector {
      get { return MaleSelectorParameter.Value; }
      set { MaleSelectorParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected GenderSpecificSelector(bool deserializing) : base(deserializing) { }
    protected GenderSpecificSelector(GenderSpecificSelector original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }

    public GenderSpecificSelector()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSelectedSubScopes", "The number of scopes that should be selected."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("CopySelected", "True if the scopes should be copied, false if they should be moved.", new BoolValue(true)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueParameter<ISelector>("FemaleSelector", "The selection operator to select the first parent."));
      Parameters.Add(new ValueParameter<ISelector>("MaleSelector", "The selection operator to select the second parent."));
      CopySelectedParameter.Hidden = true;
      #endregion

      #region Create operators
      Placeholder femaleSelector = new Placeholder();
      SubScopesProcessor maleSelection = new SubScopesProcessor();
      Placeholder maleSelector = new Placeholder();
      EmptyOperator empty = new EmptyOperator();
      RightChildReducer rightChildReducer = new RightChildReducer();
      SubScopesMixer subScopesMixer = new SubScopesMixer();

      femaleSelector.OperatorParameter.ActualName = "FemaleSelector";

      maleSelector.OperatorParameter.ActualName = "MaleSelector";

      subScopesMixer.Partitions = new IntValue(2);
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = femaleSelector;
      femaleSelector.Successor = maleSelection;
      maleSelection.Operators.Add(maleSelector);
      maleSelection.Operators.Add(empty);
      maleSelection.Successor = rightChildReducer;
      rightChildReducer.Successor = subScopesMixer;
      #endregion

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GenderSpecificSelector(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    /// <summary>
    /// Sets how many sub-scopes male and female selectors should select.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="NumberOfSelectedSubScopesParameter"/> returns an odd number.</exception>
    /// <returns>Returns Apply of <see cref="AlgorithmOperator"/>.</returns>
    public override IOperation Apply() {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      if (count % 2 > 0) throw new InvalidOperationException(Name + ": There must be an equal number of sub-scopes to be selected.");
      FemaleSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(count / 2);
      MaleSelector.NumberOfSelectedSubScopesParameter.Value = new IntValue(count / 2);
      return base.Apply();
    }

    #region Events
    private void SelectorParameter_ValueChanged(object sender, EventArgs e) {
      IValueParameter<ISelector> selectorParam = (sender as IValueParameter<ISelector>);
      if (selectorParam != null)
        ParameterizeSelector(selectorParam.Value);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      FemaleSelectorParameter.ValueChanged += new EventHandler(SelectorParameter_ValueChanged);
      MaleSelectorParameter.ValueChanged += new EventHandler(SelectorParameter_ValueChanged);
      if (FemaleSelector == null) FemaleSelector = new ProportionalSelector();
      if (MaleSelector == null) MaleSelector = new RandomSelector();
    }
    private void ParameterizeSelector(ISelector selector) {
      selector.CopySelected = new BoolValue(true);
      IStochasticOperator stoOp = (selector as IStochasticOperator);
      if (stoOp != null) stoOp.RandomParameter.ActualName = RandomParameter.Name;
      ISingleObjectiveSelector soSelector = (selector as ISingleObjectiveSelector);
      if (soSelector != null) {
        soSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
        soSelector.QualityParameter.ActualName = QualityParameter.Name;
      }
    }
    #endregion
  }
}
