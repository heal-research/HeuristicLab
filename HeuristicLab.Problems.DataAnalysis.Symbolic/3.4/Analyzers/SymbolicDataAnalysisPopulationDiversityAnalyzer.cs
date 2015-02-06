#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

// type definitions for ease of use

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Population diversity analyzer
  /// </summary>
  [Item("SymbolicDataAnalysisPopulationDiversityAnalyzer", "An operator that tracks population diversity")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisPopulationDiversityAnalyzer : SingleSuccessorOperator, IAnalyzer {
    #region Parameter names
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string UpdateIntervalParameterName = "UpdateInterval";
    private const string UpdateCounterParameterName = "UpdateCounter";
    private const string ResultsParameterName = "Results";
    private const string GenerationsParameterName = "Generations";
    private const string StoreHistoryParameterName = "StoreHistory";
    // comparer parameters
    private const string SimilarityValuesParmeterName = "Similarity";

    private const string DistanceCalculatorParameterName = "DistanceCalculator";

    #endregion

    #region Parameters
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    public ValueParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters[UpdateIntervalParameterName]; }
    }
    public ValueParameter<IntValue> UpdateCounterParameter {
      get { return (ValueParameter<IntValue>)Parameters[UpdateCounterParameterName]; }
    }
    public LookupParameter<ResultCollection> ResultsParameter {
      get { return (LookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public LookupParameter<IntValue> GenerationsParameter {
      get { return (LookupParameter<IntValue>)Parameters[GenerationsParameterName]; }
    }
    public ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters[StoreHistoryParameterName]; }
    }
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleValue> SimilarityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[SimilarityValuesParmeterName]; }
    }
    public IValueParameter<ISymbolicExpressionTreeDistanceCalculator> DistanceCalculatorParameter {
      get { return (IValueParameter<ISymbolicExpressionTreeDistanceCalculator>)Parameters[DistanceCalculatorParameterName]; }
    }
    #endregion

    #region Parameter properties
    public IntValue MaximumSymbolicExpressionTreeDepth { get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; } }
    public IntValue UpdateInterval { get { return UpdateIntervalParameter.Value; } }
    public IntValue UpdateCounter { get { return UpdateCounterParameter.Value; } }
    public ResultCollection Results { get { return ResultsParameter.ActualValue; } }
    public IntValue Generations { get { return GenerationsParameter.ActualValue; } }
    public BoolValue StoreHistory { get { return StoreHistoryParameter.Value; } }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisPopulationDiversityAnalyzer(bool deserializing)
      : base(deserializing) {
    }

    private SymbolicDataAnalysisPopulationDiversityAnalyzer(
      SymbolicDataAnalysisPopulationDiversityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisPopulationDiversityAnalyzer(this, cloner);
    }

    public SymbolicDataAnalysisPopulationDiversityAnalyzer() {
      // add parameters
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueParameter<IntValue>(UpdateIntervalParameterName, "The interval in which the tree length analysis should be applied.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>(UpdateCounterParameterName, "The value which counts how many times the operator was called since the last update", new IntValue(0)));
      Parameters.Add(new ValueLookupParameter<ResultCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));
      Parameters.Add(new LookupParameter<IntValue>(GenerationsParameterName, "The number of generations so far."));
      Parameters.Add(new ValueParameter<BoolValue>(StoreHistoryParameterName, "True if the tree lengths history of the population should be stored.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<DoubleValue>(SimilarityValuesParmeterName, ""));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeDistanceCalculator>(DistanceCalculatorParameterName, "The distance calculator between trees.", new BottomUpTreeDistanceCalculator()));
      UpdateCounterParameter.Hidden = true;
      UpdateIntervalParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(MaximumSymbolicExpressionTreeDepthParameterName)) {
        Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      }
    }

    #region IStatefulItem members
    public override void InitializeState() {
      UpdateCounter.Value = 0;
      base.InitializeState();
    }
    #endregion

    public bool EnabledByDefault {
      get { return true; }
    }

    public override IOperation Apply() {
      if (SimilarityParameter.ActualValue == null || SimilarityParameter.ActualValue.Value.IsAlmost(-1.0)) {
        UpdateCounter.Value++;
        if (UpdateCounter.Value != UpdateInterval.Value) return base.Apply();
        UpdateCounter.Value = 0;
        var trees = SymbolicExpressionTreeParameter.ActualValue.ToList();

        SimilarityParameter.ActualValue = new DoubleValue();

        var operations = new OperationCollection { Parallel = true };
        foreach (var tree in trees) {
          var op = new SymbolicDataAnalysisExpressionTreeSimilarityCalculator(DistanceCalculatorParameter.Value) {
            CurrentSymbolicExpressionTree = tree,
            MaximumTreeDepth = MaximumSymbolicExpressionTreeDepth.Value
          };
          var operation = ExecutionContext.CreateChildOperation(op, ExecutionContext.Scope);
          operations.Add(operation);
        }
        return new OperationCollection { operations, ExecutionContext.CreateOperation(this) };
      }

      var results = ResultsParameter.ActualValue;
      // population diversity
      DataTable populationDiversityTable;
      if (!results.ContainsKey("PopulationDiversity")) {
        populationDiversityTable = new DataTable("PopulationDiversity") { VisualProperties = { YAxisTitle = "Diversity" } };
        results.Add(new Result("PopulationDiversity", populationDiversityTable));
      }
      populationDiversityTable = (DataTable)results["PopulationDiversity"].Value;
      if (!populationDiversityTable.Rows.ContainsKey("Diversity"))
        populationDiversityTable.Rows.Add(new DataRow("Diversity") { VisualProperties = { StartIndexZero = true } });

      int length = SymbolicExpressionTreeParameter.ActualValue.Length;
      var similarity = SimilarityParameter.ActualValue.Value / (length * (length - 1) / 2.0);
      var diversity = 1 - similarity;
      SimilarityParameter.ActualValue.Value = -1.0;

      populationDiversityTable.Rows["Diversity"].Values.Add(diversity);

      return base.Apply();
    }
  }
}
