#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("MultiSymbolicDataAnalysisExpressionCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiSymbolicDataAnalysisExpressionCrossover<T> : StochasticMultiBranch<ISymbolicExpressionTreeCrossover>,
    ISymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    private const string ParentsParameterName = "Parents";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string EvaluatorParameterName = "Evaluator";
    private const string SymbolicDataAnalysisEvaluationPartitionParameterName = "EvaluationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string ProblemDataParameterName = "ProblemData";

    protected override bool CreateChildOperation {
      get { return true; }
    }

    public override bool CanChangeName {
      get { return false; }
    }

    #region parameter properties
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public ILookupParameter<ItemArray<ISymbolicExpressionTree>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[ParentsParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>)Parameters[EvaluatorParameterName]; }
    }
    public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[SymbolicDataAnalysisEvaluationPartitionParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public IValueLookupParameter<T> ProblemDataParameter {
      get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
    }
    #endregion

    [StorableConstructor]
    protected MultiSymbolicDataAnalysisExpressionCrossover(bool deserializing) : base(deserializing) { }
    protected MultiSymbolicDataAnalysisExpressionCrossover(MultiSymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new MultiSymbolicDataAnalysisExpressionCrossover<T>(this, cloner); }
    public MultiSymbolicDataAnalysisExpressionCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
      Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>(EvaluatorParameterName, "The single objective solution evaluator"));
      Parameters.Add(new ValueLookupParameter<IntRange>(SymbolicDataAnalysisEvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(ParentsParameterName, "The parent symbolic expression trees which should be crossed."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The child symbolic expression tree resulting from the crossover."));

      EvaluatorParameter.Hidden = true;
      EvaluationPartitionParameter.Hidden = true;
      SymbolicDataAnalysisTreeInterpreterParameter.Hidden = true;
      ProblemDataParameter.Hidden = true;
      RelativeNumberOfEvaluatedSamplesParameter.Hidden = true;

      InitializeOperators();
      name = "MultiSymbolicDataAnalysisExpressionCrossover";

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(SymbolicExpressionTreeParameterName))
        Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
      #endregion
    }

    private void InitializeOperators() {
      var list = ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeCrossover>().ToList();
      var dataAnalysisCrossovers = from type in ApplicationManager.Manager.GetTypes(typeof(ISymbolicDataAnalysisExpressionCrossover<T>))
                                   where this.GetType().Assembly == type.Assembly
                                   where !typeof(IMultiOperator<ISymbolicExpressionTreeCrossover>).IsAssignableFrom(type)
                                   select (ISymbolicDataAnalysisExpressionCrossover<T>)Activator.CreateInstance(type);
      list.AddRange(dataAnalysisCrossovers);

      var checkedItemList = new CheckedItemList<ISymbolicExpressionTreeCrossover>();
      checkedItemList.AddRange(list.OrderBy(op => op.Name));
      Operators = checkedItemList;
      Operators_ItemsAdded(this, new CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeCrossover>>(Operators.CheckedItems));
    }

    public ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
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
      return Operators[index].Crossover(random, parent0, parent1);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbolicExpressionTreeCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (ISymbolicExpressionTreeCrossover op in Operators) {
        op.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
        op.ParentsParameter.ActualName = ParentsParameter.Name;
      }
      foreach (IStochasticOperator op in Operators.OfType<IStochasticOperator>()) {
        op.RandomParameter.ActualName = RandomParameter.Name;
      }
      foreach (ISymbolicExpressionTreeSizeConstraintOperator op in Operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        op.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaximumSymbolicExpressionTreeDepthParameter.Name;
        op.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
      }

      foreach (ISymbolicDataAnalysisInterpreterOperator op in Operators.OfType<ISymbolicDataAnalysisInterpreterOperator>()) {
        op.SymbolicDataAnalysisTreeInterpreterParameter.ActualName = SymbolicDataAnalysisTreeInterpreterParameter.Name;
      }
      foreach (var op in Operators.OfType<ISymbolicDataAnalysisExpressionCrossover<T>>()) {
        op.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
        op.EvaluationPartitionParameter.ActualName = EvaluationPartitionParameter.Name;
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      }
    }
  }
}
