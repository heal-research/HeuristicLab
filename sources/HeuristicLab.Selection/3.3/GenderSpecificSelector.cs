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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using System;

namespace HeuristicLab.Selection {
  [Item("GenderSpecificSelection", "Brings two parents together by sampling each with a different selection scheme (Wagner, S. and Affenzeller, M. 2005. SexualGA: Gender-Specific Selection for Genetic Algorithms. Proceedings of the 9th World Multi-Conference on Systemics, Cybernetics and Informatics (WMSCI), pp. 76-81).")]
  [StorableClass]
  public class GenderSpecificSelector : AlgorithmOperator, ISingleObjectiveSelector, IStochasticOperator {
    private List<ISelector> femaleSelectors;
    private List<ISelector> maleSelectors;

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
    public IValueLookupParameter<BoolValue> CopySelectedParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["CopySelected"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ConstrainedValueParameter<ISelector> FemaleSelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["FemaleSelector"]; }
    }
    public ConstrainedValueParameter<ISelector> MaleSelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["MaleSelector"]; }
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
    private GenderSpecificSelector(bool deserializing) : base() { }
    public GenderSpecificSelector()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The quality of the solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSelectedSubScopes", "The number of scopes that should be selected."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("CopySelected", "True if the scopes should be copied, false if they should be moved.", new BoolValue(true)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("FemaleSelector", "The selection operator to select the first parent."));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("MaleSelector", "The selection operator to select the second parent."));
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

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeSelectors();
      UpdateSelectors();
    }

    private void InitializeSelectors() {
      femaleSelectors = new List<ISelector>();
      maleSelectors = new List<ISelector>();
      IEnumerable<Type> types = ApplicationManager.Manager.GetTypes(typeof(ISelector)).OrderBy(x => x.FullName);
      foreach (Type type in types) {
        if (type != typeof(IMultiObjectiveSelector) && type != typeof(GenderSpecificSelector)) {
          femaleSelectors.Add((ISelector)Activator.CreateInstance(type));
          maleSelectors.Add((ISelector)Activator.CreateInstance(type));
        }
      }
      ParameterizeSelectors(femaleSelectors);
      ParameterizeSelectors(maleSelectors);
    }
    private void ParameterizeSelectors(List<ISelector> selectors) {
      foreach (ISelector selector in selectors) {
        selector.CopySelected = new BoolValue(true);
      }
      foreach (IStochasticOperator op in selectors.OfType<IStochasticOperator>()) {
        op.RandomParameter.ActualName = RandomParameter.Name;
      }
      foreach (ISingleObjectiveSelector selector in selectors.OfType<ISingleObjectiveSelector>()) {
        selector.MaximizationParameter.ActualName = MaximizationParameter.Name;
        selector.QualityParameter.ActualName = QualityParameter.Name;
      }
    }
    private void UpdateSelectors() {
      ISelector oldFemaleSelector = FemaleSelector;
      FemaleSelectorParameter.ValidValues.Clear();
      foreach (ISelector selector in femaleSelectors)
        FemaleSelectorParameter.ValidValues.Add(selector);
      if (oldFemaleSelector == null) oldFemaleSelector = new ProportionalSelector();
      ISelector femaleSelector = FemaleSelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldFemaleSelector.GetType());
      if (femaleSelector != null) FemaleSelectorParameter.Value = femaleSelector;

      ISelector oldMaleSelector = MaleSelector;
      MaleSelectorParameter.ValidValues.Clear();
      foreach (ISelector selector in maleSelectors)
        MaleSelectorParameter.ValidValues.Add(selector);
      if (oldMaleSelector == null) oldMaleSelector = new RandomSelector();
      ISelector maleSelector = MaleSelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMaleSelector.GetType());
      if (maleSelector != null) MaleSelectorParameter.Value = maleSelector;
    }
    #endregion
  }
}
