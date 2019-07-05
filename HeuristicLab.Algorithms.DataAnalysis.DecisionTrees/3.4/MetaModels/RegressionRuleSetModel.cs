#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("7B4D9AE9-0456-4029-80A6-CCB5E33CE356")]
  public class RegressionRuleSetModel : RegressionModel, IDecisionTreeModel {
    private const string NumRulesResultName = "Number of rules";
    private const string CoveredInstancesResultName = "Covered instances";
    public const string RuleSetStateVariableName = "RuleSetState";

    #region Properties
    [Storable]
    internal List<RegressionRuleModel> Rules { get; private set; }
    #endregion

    #region HLConstructors & Cloning
    [StorableConstructor]
    protected RegressionRuleSetModel(StorableConstructorFlag _) : base(_) { }
    protected RegressionRuleSetModel(RegressionRuleSetModel original, Cloner cloner) : base(original, cloner) {
      if (original.Rules != null) Rules = original.Rules.Select(cloner.Clone).ToList();
    }
    protected RegressionRuleSetModel(string targetVariable) : base(targetVariable) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionRuleSetModel(this, cloner);
    }
    #endregion

    internal static RegressionRuleSetModel CreateRuleModel(string targetAttr, RegressionTreeParameters regressionTreeParams) {
      return regressionTreeParams.LeafModel.ProvidesConfidence ? new ConfidenceRegressionRuleSetModel(targetAttr) : new RegressionRuleSetModel(targetAttr);
    }

    #region RegressionModel
    public override IEnumerable<string> VariablesUsedForPrediction {
      get {
        var f = Rules.FirstOrDefault();
        return f != null ? (f.VariablesUsedForPrediction ?? new List<string>()) : new List<string>();
      }
    }
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      if (Rules == null) throw new NotSupportedException("The model has not been built yet");
      return rows.Select(row => GetEstimatedValue(dataset, row));
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }
    #endregion

    #region IDecisionTreeModel
    public void Build(IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope stateScope, ResultCollection results, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)stateScope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      var ruleSetState = (RuleSetState)stateScope.Variables[RuleSetStateVariableName].Value;

      if (ruleSetState.Code <= 0) {
        ruleSetState.Rules.Clear();
        ruleSetState.TrainingRows = trainingRows;
        ruleSetState.PruningRows = pruningRows;
        ruleSetState.Code = 1;
      }

      do {
        var tempRule = RegressionRuleModel.CreateRuleModel(regressionTreeParams.TargetVariable, regressionTreeParams);
        cancellationToken.ThrowIfCancellationRequested();

        if (!results.ContainsKey(NumRulesResultName)) results.Add(new Result(NumRulesResultName, new IntValue(0)));
        if (!results.ContainsKey(CoveredInstancesResultName)) results.Add(new Result(CoveredInstancesResultName, new IntValue(0)));

        var t1 = ruleSetState.TrainingRows.Count;
        tempRule.Build(ruleSetState.TrainingRows, ruleSetState.PruningRows, stateScope, results, cancellationToken);
        ruleSetState.TrainingRows = ruleSetState.TrainingRows.Where(i => !tempRule.Covers(regressionTreeParams.Data, i)).ToArray();
        ruleSetState.PruningRows = ruleSetState.PruningRows.Where(i => !tempRule.Covers(regressionTreeParams.Data, i)).ToArray();
        ruleSetState.Rules.Add(tempRule);
        ((IntValue)results[NumRulesResultName].Value).Value++;
        ((IntValue)results[CoveredInstancesResultName].Value).Value += t1 - ruleSetState.TrainingRows.Count;
      }
      while (ruleSetState.TrainingRows.Count > 0);
      Rules = ruleSetState.Rules;
    }
    public void Update(IReadOnlyList<int> rows, IScope stateScope, CancellationToken cancellationToken) {
      foreach (var rule in Rules) rule.Update(rows, stateScope, cancellationToken);
    }
    public static void Initialize(IScope stateScope) {
      stateScope.Variables.Add(new Variable(RuleSetStateVariableName, new RuleSetState()));
    }
    #endregion

    #region Helpers
    private double GetEstimatedValue(IDataset dataset, int row) {
      foreach (var rule in Rules) {
        if (rule.Covers(dataset, row))
          return rule.GetEstimatedValues(dataset, row.ToEnumerable()).Single();
      }
      throw new ArgumentException("Instance is not covered by any rule");
    }
    #endregion

    [StorableType("E114F3C9-3C1F-443D-8270-0E10CE12F2A0")]
    public class RuleSetState : Item {
      [Storable]
      public List<RegressionRuleModel> Rules = new List<RegressionRuleModel>();
      [Storable]
      public IReadOnlyList<int> TrainingRows = new List<int>();
      [Storable]
      public IReadOnlyList<int> PruningRows = new List<int>();

      //State.Code values denote the current action (for pausing)
      //0...nothing has been done;
      //1...splitting nodes;
      [Storable]
      public int Code = 0;

      #region HLConstructors & Cloning
      [StorableConstructor]
      protected RuleSetState(StorableConstructorFlag _) : base(_) { }
      protected RuleSetState(RuleSetState original, Cloner cloner) : base(original, cloner) {
        Rules = original.Rules.Select(cloner.Clone).ToList();
        TrainingRows = original.TrainingRows.ToList();
        PruningRows = original.PruningRows.ToList();

        Code = original.Code;
      }
      public RuleSetState() { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new RuleSetState(this, cloner);
      }
      #endregion
    }

    [StorableType("52E7992B-94CC-4960-AA82-1A399BE735C6")]
    private sealed class ConfidenceRegressionRuleSetModel : RegressionRuleSetModel, IConfidenceRegressionModel {
      #region HLConstructors & Cloning
      [StorableConstructor]
      private ConfidenceRegressionRuleSetModel(StorableConstructorFlag _) : base(_) { }
      private ConfidenceRegressionRuleSetModel(ConfidenceRegressionRuleSetModel original, Cloner cloner) : base(original, cloner) { }
      public ConfidenceRegressionRuleSetModel(string targetVariable) : base(targetVariable) { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new ConfidenceRegressionRuleSetModel(this, cloner);
      }
      #endregion

      #region IConfidenceRegressionModel
      public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
        if (Rules == null) throw new NotSupportedException("The model has not been built yet");
        return rows.Select(row => GetEstimatedVariance(dataset, row));
      }
      public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
        return new ConfidenceRegressionSolution(this, problemData);
      }
      private double GetEstimatedVariance(IDataset dataset, int row) {
        foreach (var rule in Rules) {
          if (rule.Covers(dataset, row)) return ((IConfidenceRegressionModel)rule).GetEstimatedVariances(dataset, row.ToEnumerable()).Single();
        }
        throw new ArgumentException("Instance is not covered by any rule");
      }
      #endregion
    }
  }
}