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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Creators {
  public class MultiSymbolicDataAnalysisExpressionCreator : StochasticMultiBranch<ISymbolicDataAnalysisSolutionCreator>,
    ISymbolicDataAnalysisSolutionCreator,
    ISymbolicExpressionTreeSizeConstraintOperator,
  ISymbolicExpressionTreeGrammarBasedOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string ClonedSymbolicExpressionTreeGrammarParameterName = "ClonedSymbolicExpressionTreeGrammar";

    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    #region parameter properties
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionGrammar> ClonedSymbolicExpressionTreeGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[ClonedSymbolicExpressionTreeGrammarParameterName]; }
    }
    #endregion

    [StorableConstructor]
    protected MultiSymbolicDataAnalysisExpressionCreator(bool deserializing) : base(deserializing) { }
    protected MultiSymbolicDataAnalysisExpressionCreator(MultiSymbolicDataAnalysisExpressionCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new MultiSymbolicDataAnalysisExpressionCreator(this, cloner); }
    public MultiSymbolicDataAnalysisExpressionCreator()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionTreeGrammarParameterName, "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(ClonedSymbolicExpressionTreeGrammarParameterName, "An immutable clone of the concrete grammar that is actually used to create and manipulate trees."));

      List<ISymbolicDataAnalysisSolutionCreator> list = new List<ISymbolicDataAnalysisSolutionCreator>();
      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(ISymbolicDataAnalysisSolutionCreator))) {
        if (this.GetType().Assembly != type.Assembly) continue;
        if (typeof(IMultiOperator<ISymbolicDataAnalysisSolutionCreator>).IsAssignableFrom(type)) continue;
        list.Add((ISymbolicDataAnalysisSolutionCreator)Activator.CreateInstance(type));
      }
      CheckedItemList<ISymbolicDataAnalysisSolutionCreator> checkedItemList = new CheckedItemList<ISymbolicDataAnalysisSolutionCreator>();
      checkedItemList.AddRange(list.OrderBy(op => op.Name));
      Operators = checkedItemList.AsReadOnly();
      Operators_ItemsAdded(this, new CollectionItemsChangedEventArgs<IndexedItem<ISymbolicDataAnalysisSolutionCreator>>(Operators.CheckedItems));

    }

    public override IOperation InstrumentedApply() {
      if (ClonedSymbolicExpressionTreeGrammarParameter.ActualValue == null) {
        SymbolicExpressionTreeGrammarParameter.ActualValue.ReadOnly = true;
        IScope globalScope = ExecutionContext.Scope;
        while (globalScope.Parent != null)
          globalScope = globalScope.Parent;

        globalScope.Variables.Add(new Core.Variable(ClonedSymbolicExpressionTreeGrammarParameterName, (ISymbolicExpressionGrammar)SymbolicExpressionTreeGrammarParameter.ActualValue.Clone()));
      }
      return base.InstrumentedApply();
    }

    public ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      double sum = Operators.CheckedItems.Sum(o => Probabilities[o.Index]);
      if (sum.IsAlmost(0)) throw new InvalidOperationException(Name + ": All selected operators have zero probability.");
      double r = random.NextDouble() * sum;
      sum = 0;
      int index = -1;
      foreach (var indexedItem in Operators.CheckedItems) {
        sum += Probabilities[indexedItem.Index];
        if (sum > r) {
          index = indexedItem.Index;
          break;
        }
      }
      return Operators[index].CreateTree(random, grammar, maxTreeLength, maxTreeDepth);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicDataAnalysisSolutionCreator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeTreeCreators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicDataAnalysisSolutionCreator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeTreeCreators();
    }
    private void ParameterizeTreeCreators() {
      foreach (IStochasticOperator creator in Operators.OfType<IStochasticOperator>()) {
        creator.RandomParameter.ActualName = RandomParameter.Name;
      }
      foreach (ISymbolicExpressionTreeCreator creator in Operators.OfType<ISymbolicExpressionTreeCreator>()) {
        creator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      }
      foreach (ISymbolicExpressionTreeSizeConstraintOperator creator in Operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        creator.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaximumSymbolicExpressionTreeDepthParameter.Name;
        creator.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
      }

      foreach (ISymbolicExpressionTreeGrammarBasedOperator creator in Operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>()) {
        creator.SymbolicExpressionTreeGrammarParameter.ActualName = SymbolicExpressionTreeGrammarParameter.Name;
      }
    }

  }
}
