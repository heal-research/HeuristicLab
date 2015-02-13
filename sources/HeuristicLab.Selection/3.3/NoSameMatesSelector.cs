#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A selector which tries to select two parents which differ in quality
  /// as described in: "S. Gustafson, E. K. Burke, N. Krasnogor, On improving genetic programming for symbolic regression, 
  /// The 2005 IEEE Congress on Evolutionary Computation, pp. 912-919, 2005."
  /// </summary>
  [Item("NoSameMatesSelector", "A selector which tries to select two parents which differ in quality as described in: \"S. Gustafson, E. K. Burke, N. Krasnogor, On improving genetic programming for symbolic regression, The 2005 IEEE Congress on Evolutionary Computation, pp. 912-919, 2005.\"")]
  [StorableClass]
  public class NoSameMatesSelector : StochasticSingleObjectiveSelector, ISingleObjectiveSelector {
    private const string SelectorParameterName = "Selector";
    private const string QualityDifferencePercentageParameterName = "QualityDifferencePercentage";
    private const string QualityDifferenceMaxAttemptsParameterName = "QualityDifferenceMaxAttempts";
    private const string QualityDifferenceUseRangeParameterName = "QualityDifferenceUseRange";

    #region Parameters
    public IValueParameter<ISingleObjectiveSelector> SelectorParameter {
      get { return (IValueParameter<ISingleObjectiveSelector>)Parameters[SelectorParameterName]; }
    }
    public IFixedValueParameter<PercentValue> QualityDifferencePercentageParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[QualityDifferencePercentageParameterName]; }
    }
    public IFixedValueParameter<IntValue> QualityDifferenceMaxAttemptsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[QualityDifferenceMaxAttemptsParameterName]; }
    }
    public IFixedValueParameter<BoolValue> QualityDifferenceUseRangeParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[QualityDifferenceUseRangeParameterName]; }
    }
    #endregion

    #region Properties
    public ISingleObjectiveSelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public PercentValue QualityDifferencePercentage {
      get { return QualityDifferencePercentageParameter.Value; }
    }
    public IntValue QualityDifferenceMaxAttempts {
      get { return QualityDifferenceMaxAttemptsParameter.Value; }
    }
    public BoolValue QualityDifferenceUseRange {
      get { return QualityDifferenceUseRangeParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected NoSameMatesSelector(bool deserializing) : base(deserializing) { }
    protected NoSameMatesSelector(NoSameMatesSelector original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NoSameMatesSelector(this, cloner);
    }

    public NoSameMatesSelector()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueParameter<ISingleObjectiveSelector>(SelectorParameterName, "The inner selection operator to select the parents.", new TournamentSelector()));
      Parameters.Add(new FixedValueParameter<PercentValue>(QualityDifferencePercentageParameterName, "The minimum quality difference from parent1 to parent2 to accept the selection.", new PercentValue(0.05)));
      Parameters.Add(new FixedValueParameter<IntValue>(QualityDifferenceMaxAttemptsParameterName, "The maximum number of attempts to find parents which differ in quality.", new IntValue(5)));
      Parameters.Add(new FixedValueParameter<BoolValue>(QualityDifferenceUseRangeParameterName, "Use the range from minimum to maximum quality as basis for QualityDifferencePercentage.", new BoolValue(true)));
      #endregion

      RegisterParameterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region conversion of old NSM parameters
      if (Parameters.ContainsKey(SelectorParameterName)) { // change SelectorParameter type from ISelector to ISingleObjectiveSelector
        ValueParameter<ISelector> param = Parameters[SelectorParameterName] as ValueParameter<ISelector>;
        if (param != null) {
          ISingleObjectiveSelector selector = param.Value as ISingleObjectiveSelector;
          if (selector == null) selector = new TournamentSelector();
          Parameters.Remove(SelectorParameterName);
          Parameters.Add(new ValueParameter<ISingleObjectiveSelector>(SelectorParameterName, "The inner selection operator to select the parents.", selector));
        }
      }
      // FixedValueParameter for quality difference percentage, max attempts, use range
      if (Parameters.ContainsKey(QualityDifferencePercentageParameterName)) {
        ValueParameter<PercentValue> param = Parameters[QualityDifferencePercentageParameterName] as ValueParameter<PercentValue>;
        if (!(param is FixedValueParameter<PercentValue>)) {
          PercentValue diff = param != null ? param.Value as PercentValue : null;
          if (diff == null) diff = new PercentValue(0.05);
          Parameters.Remove(QualityDifferencePercentageParameterName);
          Parameters.Add(new FixedValueParameter<PercentValue>(QualityDifferencePercentageParameterName, "The minimum quality difference from parent1 to parent2 to accept the selection.", diff));
        }
      }
      if (Parameters.ContainsKey(QualityDifferenceMaxAttemptsParameterName)) {
        ValueParameter<IntValue> param = Parameters[QualityDifferenceMaxAttemptsParameterName] as ValueParameter<IntValue>;
        if (!(param is FixedValueParameter<IntValue>)) {
          IntValue attempts = param != null ? param.Value as IntValue : null;
          if (attempts == null) attempts = new IntValue(5);
          Parameters.Remove(QualityDifferenceMaxAttemptsParameterName);
          Parameters.Add(new FixedValueParameter<IntValue>(QualityDifferenceMaxAttemptsParameterName, "The maximum number of attempts to find parents which differ in quality.", attempts));
        }
      }
      if (Parameters.ContainsKey(QualityDifferenceUseRangeParameterName)) {
        ValueParameter<BoolValue> param = Parameters[QualityDifferenceUseRangeParameterName] as ValueParameter<BoolValue>;
        if (!(param is FixedValueParameter<BoolValue>)) {
          BoolValue range = param != null ? param.Value as BoolValue : null;
          if (range == null) range = new BoolValue(true);
          Parameters.Remove(QualityDifferenceUseRangeParameterName);
          Parameters.Add(new FixedValueParameter<BoolValue>(QualityDifferenceUseRangeParameterName, "Use the range from minimum to maximum quality as basis for QualityDifferencePercentage.", range));
        }
      }
      if (!Parameters.ContainsKey(QualityDifferenceUseRangeParameterName)) // add use range parameter
        Parameters.Add(new FixedValueParameter<BoolValue>(QualityDifferenceUseRangeParameterName, "Use the range from minimum to maximum quality as basis for QualityDifferencePercentage.", new BoolValue(true)));
      #endregion

      RegisterParameterEventHandlers();
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int parentsToSelect = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      if (parentsToSelect % 2 > 0) throw new InvalidOperationException(Name + ": There must be an equal number of sub-scopes to be selected.");
      IScope[] selected = new IScope[parentsToSelect];
      IScope[] parentsPool = new IScope[parentsToSelect];

      double qualityDifferencePercentage = QualityDifferencePercentage.Value;
      int qualityDifferenceMaxAttempts = QualityDifferenceMaxAttempts.Value;
      bool qualityDifferenceUseRange = QualityDifferenceUseRange.Value;
      string qualityName = QualityParameter.ActualName;

      // calculate quality offsets   
      double absoluteQualityOffset = 0;
      double minRelativeQualityOffset = 0;
      double maxRelativeQualityOffset = 0;
      if (qualityDifferenceUseRange) {
        // maximization flag is not needed because only the range is relevant
        double minQuality = QualityParameter.ActualValue.Min(x => x.Value);
        double maxQuality = QualityParameter.ActualValue.Max(x => x.Value);
        absoluteQualityOffset = (maxQuality - minQuality) * qualityDifferencePercentage;
      } else {
        maxRelativeQualityOffset = 1.0 + qualityDifferencePercentage;
        minRelativeQualityOffset = 1.0 - qualityDifferencePercentage;
      }

      int selectedParents = 0;
      int poolCount = 0;
      // repeat until enough parents are selected or max attempts are reached
      for (int attempts = 1; attempts <= qualityDifferenceMaxAttempts && selectedParents < parentsToSelect - 1; attempts++) {
        ApplyInnerSelector();
        ScopeList parents = CurrentScope.SubScopes[1].SubScopes;

        for (int indexParent1 = 0, indexParent2 = 1;
             indexParent1 < parents.Count - 1 && selectedParents < parentsToSelect - 1;
             indexParent1 += 2, indexParent2 += 2) {
          double qualityParent1 = ((DoubleValue)parents[indexParent1].Variables[qualityName].Value).Value;
          double qualityParent2 = ((DoubleValue)parents[indexParent2].Variables[qualityName].Value).Value;

          bool parentsDifferent;
          if (qualityDifferenceUseRange) {
            parentsDifferent = (qualityParent2 > qualityParent1 - absoluteQualityOffset ||
                                qualityParent2 < qualityParent1 + absoluteQualityOffset);
          } else {
            parentsDifferent = (qualityParent2 > qualityParent1 * maxRelativeQualityOffset ||
                                qualityParent2 < qualityParent1 * minRelativeQualityOffset);
          }

          if (parentsDifferent) {
            // inner selector already copied scopes, no cloning necessary here
            selected[selectedParents++] = parents[indexParent1];
            selected[selectedParents++] = parents[indexParent2];
          } else if (attempts == qualityDifferenceMaxAttempts &&
                     poolCount < parentsToSelect - selectedParents) {
            // last attempt: save parents to fill remaining positions
            parentsPool[poolCount++] = parents[indexParent1];
            parentsPool[poolCount++] = parents[indexParent2];
          }
        }
        // modify scopes
        ScopeList remaining = CurrentScope.SubScopes[0].SubScopes;
        CurrentScope.SubScopes.Clear();
        CurrentScope.SubScopes.AddRange(remaining);
      }
      // fill remaining positions with parents which don't meet the difference criterion 
      if (selectedParents < parentsToSelect - 1) {
        Array.Copy(parentsPool, 0, selected, selectedParents, parentsToSelect - selectedParents);
      }
      return selected;
    }

    #region Events
    private void RegisterParameterEventHandlers() {
      SelectorParameter.ValueChanged += new EventHandler(SelectorParameter_ValueChanged);
      CopySelected.ValueChanged += new EventHandler(CopySelected_ValueChanged);
    }

    private void SelectorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelector(Selector);
    }

    private void CopySelected_ValueChanged(object sender, EventArgs e) {
      if (CopySelected.Value != true) {
        CopySelected.Value = true;
      }
    }
    #endregion

    #region Helpers
    private void ParameterizeSelector(ISingleObjectiveSelector selector) {
      selector.CopySelected = new BoolValue(true); // must always be true
      selector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      selector.QualityParameter.ActualName = QualityParameter.Name;

      IStochasticOperator stoOp = (selector as IStochasticOperator);
      if (stoOp != null) stoOp.RandomParameter.ActualName = RandomParameter.Name;
    }

    private void ApplyInnerSelector() {
      // necessary for inner GenderSpecificSelector to execute all operations in OperationCollection
      Stack<IOperation> executionStack = new Stack<IOperation>();
      executionStack.Push(ExecutionContext.CreateChildOperation(Selector));
      while (executionStack.Count > 0) {
        CancellationToken.ThrowIfCancellationRequested();
        IOperation next = executionStack.Pop();
        if (next is OperationCollection) {
          OperationCollection coll = (OperationCollection)next;
          for (int i = coll.Count - 1; i >= 0; i--)
            if (coll[i] != null) executionStack.Push(coll[i]);
        } else if (next is IAtomicOperation) {
          IAtomicOperation operation = (IAtomicOperation)next;
          next = operation.Operator.Execute((IExecutionContext)operation, CancellationToken);
          if (next != null) executionStack.Push(next);
        }
      }
    }
    #endregion
  }
}
