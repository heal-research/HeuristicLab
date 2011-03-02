using System;
using System.Collections.Generic;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A selector which tries to select two parents which differ in quality.
  /// </summary>
  [Item("NoSameMatesSelector", "A selector which tries to select two parents which differ in quality.")]
  [StorableClass]
  public class NoSameMatesSelector : StochasticSingleObjectiveSelector, ISingleObjectiveSelector {
    private const string SelectorParameterName = "Selector";
    private const string QualityDifferencePercentageParameterName = "QualityDifferencePercentage";
    private const string QualityDifferenceMaxAttemptsParameterName = "QualityDifferenceMaxAttempts";

    #region Parameters
    public IValueParameter<ISelector> SelectorParameter {
      get { return (IValueParameter<ISelector>)Parameters[SelectorParameterName]; }
    }
    public IValueParameter<PercentValue> QualityDifferencePercentageParameter {
      get { return (IValueParameter<PercentValue>)Parameters[QualityDifferencePercentageParameterName]; }
    }
    public IValueParameter<IntValue> QualityDifferenceMaxAttemptsParameter {
      get { return (IValueParameter<IntValue>)Parameters[QualityDifferenceMaxAttemptsParameterName]; }
    }
    #endregion

    #region Properties
    public IntValue NumberOfSelectedSubScopes {
      get { return NumberOfSelectedSubScopesParameter.ActualValue; }
    }
    public ISelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public PercentValue QualityDifferencePercentage {
      get { return QualityDifferencePercentageParameter.Value; }
    }
    public IntValue QualityDifferenceMaxAttempts {
      get { return QualityDifferenceMaxAttemptsParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected NoSameMatesSelector(bool deserializing) : base(deserializing) { }
    protected NoSameMatesSelector(NoSameMatesSelector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NoSameMatesSelector(this, cloner);
    }

    public NoSameMatesSelector() : base() {
      #region Create parameters
      Parameters.Add(new ValueParameter<ISelector>(SelectorParameterName, "The inner selection operator to select the parents."));
      Parameters.Add(new ValueParameter<PercentValue>(QualityDifferencePercentageParameterName, "The minimum quality difference from parent1 to parent2 to accept the selection.", new PercentValue(0.05)));
      Parameters.Add(new ValueParameter<IntValue>(QualityDifferenceMaxAttemptsParameterName, "The maximum number of attempts to find parents which differ in quality.", new IntValue(5)));
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopes.Value;
      if (count % 2 > 0) throw new InvalidOperationException(Name + ": There must be an equal number of sub-scopes to be selected.");
      IScope[] selected = new IScope[count];

      double qualityDifferencePercentage = QualityDifferencePercentage.Value;
      int qualityDifferenceMaxAttempts = QualityDifferenceMaxAttempts.Value;
    
      int attempts = 1;
      int j = 0;
      while (j < count - 1) { // repeat until enough parents are selected
        ApplyInnerSelector();
        ScopeList parents = CurrentScope.SubScopes[1].SubScopes;

        for (int i = 0; j < count - 1 && i < parents.Count / 2; i++) {
          double qualityParent1 = ((DoubleValue)parents[i * 2].Variables[QualityParameter.ActualName].Value).Value;
          double qualityParent2 = ((DoubleValue)parents[i * 2 + 1].Variables[QualityParameter.ActualName].Value).Value;

          bool parentsDifferent = (qualityParent2 > qualityParent1 * (1.0 + qualityDifferencePercentage) ||
                                   qualityParent2 < qualityParent1 * (1.0 - qualityDifferencePercentage));
          // parents meet difference criterion or max attempts reached
          if (attempts >= qualityDifferenceMaxAttempts || parentsDifferent) {
      
            // inner selector already copied scopes
            selected[j++] = parents[i * 2];
            selected[j++] = parents[i * 2 + 1];
            if (!CopySelected.Value) {
              scopes.Remove(parents[i * 2]);
              scopes.Remove(parents[i * 2 + 1]);
            }
            attempts = 1;
          } else { // skip parents
            attempts++;
          }
        }
        // modify scopes
        ScopeList remaining = CurrentScope.SubScopes[0].SubScopes;
        CurrentScope.SubScopes.Clear();
        CurrentScope.SubScopes.AddRange(remaining);
      }
      return selected;
    }

    #region Events
    private void SelectorParameter_ValueChanged(object sender, EventArgs e) {
      IValueParameter<ISelector> selectorParam = (sender as IValueParameter<ISelector>);
      if (selectorParam != null)
        ParameterizeSelector(selectorParam.Value);
    }
    #endregion

    #region Helpers
    private void ApplyInnerSelector() {
      IOperation next;
      IAtomicOperation operation;
      OperationCollection coll;
      Stack<IOperation> executionStack = new Stack<IOperation>();

      executionStack.Push(ExecutionContext.CreateChildOperation(Selector));
      while (executionStack.Count > 0) {
        CancellationToken.ThrowIfCancellationRequested();
        next = executionStack.Pop();
        if (next is OperationCollection) {
          coll = (OperationCollection)next;
          for (int i = coll.Count - 1; i >= 0; i--)
            if (coll[i] != null) executionStack.Push(coll[i]);
        } else if (next is IAtomicOperation) {
          operation = (IAtomicOperation)next;
          next = operation.Operator.Execute((IExecutionContext)operation, CancellationToken);
          if (next != null) executionStack.Push(next);
        }
      }
    }

    private void Initialize() {
      SelectorParameter.ValueChanged += new EventHandler(SelectorParameter_ValueChanged);
      if (Selector == null) Selector = new TournamentSelector();
    }

    private void ParameterizeSelector(ISelector selector) {
      selector.CopySelected = new BoolValue(true); // must always be true
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
