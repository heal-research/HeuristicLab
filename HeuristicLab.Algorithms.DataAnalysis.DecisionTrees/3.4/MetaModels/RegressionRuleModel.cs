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
using System.Text;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("425AF262-A756-4E9A-B76F-4D2480BEA4FD")]
  public class RegressionRuleModel : RegressionModel, IDecisionTreeModel {
    #region Properties
    [Storable]
    public string[] SplitAttributes { get; set; }
    [Storable]
    private double[] SplitValues { get; set; }
    [Storable]
    private Comparison[] Comparisons { get; set; }
    [Storable]
    private IRegressionModel RuleModel { get; set; }
    [Storable]
    private IReadOnlyList<string> variables;
    #endregion

    #region HLConstructors
    [StorableConstructor]
    protected RegressionRuleModel(StorableConstructorFlag _) : base(_) { }
    protected RegressionRuleModel(RegressionRuleModel original, Cloner cloner) : base(original, cloner) {
      if (original.SplitAttributes != null) SplitAttributes = original.SplitAttributes.ToArray();
      if (original.SplitValues != null) SplitValues = original.SplitValues.ToArray();
      if (original.Comparisons != null) Comparisons = original.Comparisons.ToArray();
      RuleModel = cloner.Clone(original.RuleModel);
      if (original.variables != null) variables = original.variables.ToList();
    }
    private RegressionRuleModel(string target) : base(target) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionRuleModel(this, cloner);
    }
    #endregion

    internal static RegressionRuleModel CreateRuleModel(string target, RegressionTreeParameters regressionTreeParams) {
      return regressionTreeParams.LeafModel.ProvidesConfidence ? new ConfidenceRegressionRuleModel(target) : new RegressionRuleModel(target);
    }

    #region IRegressionModel
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return variables; }
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      if (RuleModel == null) throw new NotSupportedException("The model has not been built correctly");
      return RuleModel.GetEstimatedValues(dataset, rows);
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }
    #endregion

    public void Build(IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope statescope, ResultCollection results, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      variables = regressionTreeParams.AllowedInputVariables.ToList();

      //build tree and select node with maximum coverage
      var tree = RegressionNodeTreeModel.CreateTreeModel(regressionTreeParams.TargetVariable, regressionTreeParams);
      tree.BuildModel(trainingRows, pruningRows, statescope, results, cancellationToken);
      var nodeModel = tree.Root.EnumerateNodes().Where(x => x.IsLeaf).MaxItems(x => x.NumSamples).First();

      var satts = new List<string>();
      var svals = new List<double>();
      var reops = new List<Comparison>();

      //extract splits
      for (var temp = nodeModel; temp.Parent != null; temp = temp.Parent) {
        satts.Add(temp.Parent.SplitAttribute);
        svals.Add(temp.Parent.SplitValue);
        reops.Add(temp.Parent.Left == temp ? Comparison.LessEqual : Comparison.Greater);
      }
      Comparisons = reops.ToArray();
      SplitAttributes = satts.ToArray();
      SplitValues = svals.ToArray();
      int np;
      RuleModel = regressionTreeParams.LeafModel.BuildModel(trainingRows.Union(pruningRows).Where(r => Covers(regressionTreeParams.Data, r)).ToArray(), regressionTreeParams, cancellationToken, out np);
    }

    public void Update(IReadOnlyList<int> rows, IScope statescope, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      int np;
      RuleModel = regressionTreeParams.LeafModel.BuildModel(rows, regressionTreeParams, cancellationToken, out np);
    }

    public bool Covers(IDataset dataset, int row) {
      return !SplitAttributes.Where((t, i) => !Comparisons[i].Compare(dataset.GetDoubleValue(t, row), SplitValues[i])).Any();
    }

    public string ToCompactString() {
      var mins = new Dictionary<string, double>();
      var maxs = new Dictionary<string, double>();
      for (var i = 0; i < SplitAttributes.Length; i++) {
        var n = SplitAttributes[i];
        var v = SplitValues[i];
        if (!mins.ContainsKey(n)) mins.Add(n, double.NegativeInfinity);
        if (!maxs.ContainsKey(n)) maxs.Add(n, double.PositiveInfinity);
        if (Comparisons[i] == Comparison.LessEqual) maxs[n] = Math.Min(maxs[n], v);
        else mins[n] = Math.Max(mins[n], v);
      }
      if (maxs.Count == 0) return "";
      var s = new StringBuilder();
      foreach (var key in maxs.Keys)
        s.Append(string.Format("{0} ∈ [{1:e2}; {2:e2}] && ", key, mins[key], maxs[key]));
      s.Remove(s.Length - 4, 4);
      return s.ToString();
    }

    [StorableType("7302AA30-9F58-42F3-BF6A-ECF1536508AB")]
    private sealed class ConfidenceRegressionRuleModel : RegressionRuleModel, IConfidenceRegressionModel {
      #region HLConstructors
      [StorableConstructor]
      private ConfidenceRegressionRuleModel(StorableConstructorFlag _) : base(_) { }
      private ConfidenceRegressionRuleModel(ConfidenceRegressionRuleModel original, Cloner cloner) : base(original, cloner) { }
      public ConfidenceRegressionRuleModel(string targetAttr) : base(targetAttr) { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new ConfidenceRegressionRuleModel(this, cloner);
      }
      #endregion

      public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
        return ((IConfidenceRegressionModel)RuleModel).GetEstimatedVariances(dataset, rows);
      }

      public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
        return new ConfidenceRegressionSolution(this, problemData);
      }
    }
  }

  [StorableType("152DECE4-2692-4D53-B290-974806ADCD72")]
  internal enum Comparison {
    LessEqual,
    Greater
  }

  internal static class ComparisonExtentions {
    public static bool Compare(this Comparison op, double x, double y) {
      switch (op) {
        case Comparison.Greater:
          return x > y;
        case Comparison.LessEqual:
          return x <= y;
        default:
          throw new ArgumentOutOfRangeException(op.ToString(), op, null);
      }
    }
  }
}