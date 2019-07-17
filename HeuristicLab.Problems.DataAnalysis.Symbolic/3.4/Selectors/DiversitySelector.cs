#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Selection;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("3E8EA052-3B86-4609-BD38-E3FE78DAD2FF")]
  [Item("DiversitySelector", "A selection operator that applies a diversity penalty to the objective function before applying an inner selector.")]
  public sealed class DiversitySelector : StochasticSingleObjectiveSelector, ISingleObjectiveSelector {
    private const string StrictSimilarityParameterName = "StrictSimilarity";
    private const string SimilarityWeightParameterName = "SimilarityWeight";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SelectorParameterName = "Selector";
    private const string DiversityParameterName = "Diversity";

    public IValueParameter<ISingleObjectiveSelector> SelectorParameter {
      get { return (IValueParameter<ISingleObjectiveSelector>)Parameters[SelectorParameterName]; }
    }

    public ISingleObjectiveSelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }

    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> DiversityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[DiversityParameterName]; }
    }

    public IFixedValueParameter<BoolValue> StrictSimilarityParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StrictSimilarityParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> SimilarityWeightParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[SimilarityWeightParameterName]; }
    }

    public bool StrictSimilarity {
      get { return StrictSimilarityParameter.Value.Value; }
      set { StrictSimilarityParameter.Value.Value = value; }
    }

    public double SimilarityWeight {
      get { return SimilarityWeightParameter.Value.Value; }
      set { SimilarityWeightParameter.Value.Value = value; }
    }

    public DiversitySelector() : base() {
      Parameters.Add(new FixedValueParameter<BoolValue>(StrictSimilarityParameterName, "Calculate strict similarity.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(SimilarityWeightParameterName, "Weight of the diversity term.", new DoubleValue(1)));
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees that should be analyzed."));
      Parameters.Add(new ValueParameter<ISingleObjectiveSelector>(SelectorParameterName, "The inner selection operator to select the parents.", new TournamentSelector()));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(DiversityParameterName, "The diversity value calcuated by the operator (output). The inner selector uses this value."));

      RegisterParameterEventHandlers();
    }

    [StorableConstructor]
    private DiversitySelector(StorableConstructorFlag _) : base(_) { }

    private DiversitySelector(DiversitySelector original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DiversitySelector(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(DiversityParameterName)) {
        Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(DiversityParameterName));
      }

      RegisterParameterEventHandlers();
    }

    #region Events
    private void RegisterParameterEventHandlers() {
      SelectorParameter.ValueChanged += SelectorParameter_ValueChanged;
      CopySelectedParameter.ValueChanged += CopySelectedParameter_ValueChanged;
      CopySelected.ValueChanged += CopySelected_ValueChanged;

      MaximizationParameter.NameChanged += MaximizationParameter_NameChanged;
      QualityParameter.NameChanged += QualityParameter_NameChanged;
      RandomParameter.NameChanged += RandomParameter_NameChanged;
    }

    private void RandomParameter_NameChanged(object sender, EventArgs e) { ParameterizeSelector(Selector); }
    private void QualityParameter_NameChanged(object sender, EventArgs e) { ParameterizeSelector(Selector); }
    private void MaximizationParameter_NameChanged(object sender, EventArgs e) { ParameterizeSelector(Selector); }

    private void CopySelectedParameter_ValueChanged(object sender, EventArgs e) {
      if (CopySelected.Value != true) {
        CopySelected.Value = true;
      }
      CopySelected.ValueChanged += CopySelected_ValueChanged;
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

    protected override IScope[] Select(List<IScope> scopes) {
      var w = SimilarityWeight;
      if (w.IsAlmost(0)) {
        ApplyInnerSelector();
        return CurrentScope.SubScopes[1].SubScopes.ToArray();  // return selected individuals (selectors create two sub-scopes with remaining and selected)
      }

      var trees = SymbolicExpressionTreeParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;

      // calculate average similarity for each tree
      var similarityMatrix = SymbolicExpressionTreeHash.ComputeSimilarityMatrix(trees, simplify: false, strict: StrictSimilarity);
      var similarities = new double[trees.Length];
      for (int i = 0; i < trees.Length; ++i) {
        for (int j = 0; j < trees.Length; ++j) {
          if (i != j) {
            similarities[i] += similarityMatrix[i, j];
          }
        }
        similarities[i] /= (trees.Length - 1);
      }

      var v = 1 - w;

      var maximization = MaximizationParameter.ActualValue.Value;
      var diversities = new ItemArray<DoubleValue>(trees.Length);
      for (int i = 0; i < trees.Length; ++i) {
        var q = qualities[i].Value;
        var d = 1 - similarities[i]; // average distance

        // assuming both q and d are in the interval [0, 1]
        var value = maximization
            ? (v * q) + (w * d)
            : (v * q) - (w * d);

        diversities[i] = new DoubleValue(value);
      }

      Selector.QualityParameter.ActualName = "Diversity";
      DiversityParameter.ActualValue = diversities;
      ApplyInnerSelector(); // apply inner selector

      return CurrentScope.SubScopes[1].SubScopes.ToArray();
    }

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
  }
}
