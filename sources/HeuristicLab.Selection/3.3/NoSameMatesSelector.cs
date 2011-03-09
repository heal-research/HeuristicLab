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
    private const string QualityDifferenceUseRangeParameterName = "QualityDifferenceUseRange";

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
    public IValueParameter<BoolValue> QualityDifferenceUseRangeParameter {
      get { return (IValueParameter<BoolValue>)Parameters[QualityDifferenceUseRangeParameterName]; }
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
    public BoolValue QualityDifferenceUseRange {
      get { return QualityDifferenceUseRangeParameter.Value; }
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
      Parameters.Add(new ValueParameter<BoolValue>(QualityDifferenceUseRangeParameterName, "Use the range from minimum to maximum quality as basis for QualityDifferencePercentage.", new BoolValue(false)));
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(QualityDifferenceUseRangeParameterName))
        Parameters.Add(new ValueParameter<BoolValue>(QualityDifferenceUseRangeParameterName, "Use the range from minimum to maximum quality as basis for QualityDifferencePercentage.", new BoolValue(false)));
      Initialize();
    }

    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopes.Value;
      if (count % 2 > 0) throw new InvalidOperationException(Name + ": There must be an equal number of sub-scopes to be selected.");
      int limit = count - 1;
      IScope[] selected = new IScope[count];

      double qualityDifferencePercentage = QualityDifferencePercentage.Value;
      int qualityDifferenceMaxAttempts = QualityDifferenceMaxAttempts.Value;
      bool qualityDifferenceUseRange = QualityDifferenceUseRange.Value;
      string qualityName = QualityParameter.ActualName;
      
      // get minimum and maximum quality, calculate quality offsets
      double minQualityOffset = 0;
      double maxQualityOffset = 0;
      if (qualityDifferenceUseRange) {
        ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
        double minQuality = double.MaxValue, maxQuality = double.MinValue;
        for (int l = 0; l < qualities.Length; l++) {
          if (qualities[l].Value < minQuality) minQuality = qualities[l].Value;
          if (qualities[l].Value > maxQuality) maxQuality = qualities[l].Value;
        } // maximization flag is not needed because only the range is relevant
        minQualityOffset = (maxQuality - minQuality) * qualityDifferencePercentage;
      } else {
        maxQualityOffset = 1.0 + qualityDifferencePercentage;
        minQualityOffset = 1.0 - qualityDifferencePercentage;
      }

      ScopeList parents, remaining;
      double qualityParent1, qualityParent2;
      bool parentsDifferent;
      int attempts = 1;
      int i, j, k = 0;
      while (k < limit) { // repeat until enough parents are selected
        ApplyInnerSelector();
        parents = CurrentScope.SubScopes[1].SubScopes;

        for (i = 0; k < limit && i < parents.Count - 1; i += 2) {
          j = i + 1;
          qualityParent1 = ((DoubleValue)parents[i].Variables[qualityName].Value).Value;
          qualityParent2 = ((DoubleValue)parents[j].Variables[qualityName].Value).Value;

          if (qualityDifferenceUseRange) {
            parentsDifferent = (qualityParent2 > qualityParent1 - minQualityOffset ||
                                qualityParent2 < qualityParent1 + minQualityOffset);
          } else {
            parentsDifferent = (qualityParent2 > qualityParent1 * maxQualityOffset ||
                                qualityParent2 < qualityParent1 * minQualityOffset);
          }

          // parents meet difference criterion or max attempts reached
          if (attempts >= qualityDifferenceMaxAttempts || parentsDifferent) {
            // inner selector already copied scopes
            selected[k++] = parents[i];
            selected[k++] = parents[j];
            if (!CopySelected.Value) {
              scopes.Remove(parents[i]);
              scopes.Remove(parents[j]);
            }
            attempts = 1;
          } else { // skip parents
            attempts++;
          }
        }
        // modify scopes
        remaining = CurrentScope.SubScopes[0].SubScopes;
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
