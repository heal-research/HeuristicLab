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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class RegressionTreeAnalyzer {
    private const string ConditionResultName = "Condition";
    private const string CoverResultName = "Covered Instances";
    private const string CoverageDiagramResultName = "Coverage";
    private const string RuleModelResultName = "RuleModel";

    public static Dictionary<string, int> GetRuleVariableFrequences(RegressionRuleSetModel ruleSetModel) {
      var res = ruleSetModel.VariablesUsedForPrediction.ToDictionary(x => x, x => 0);
      foreach (var rule in ruleSetModel.Rules)
      foreach (var att in rule.SplitAttributes)
        res[att]++;
      return res;
    }

    public static Dictionary<string, int> GetTreeVariableFrequences(RegressionNodeTreeModel treeModel) {
      var res = treeModel.VariablesUsedForPrediction.ToDictionary(x => x, x => 0);
      var root = treeModel.Root;
      foreach (var cur in root.EnumerateNodes().Where(x => !x.IsLeaf))
        res[cur.SplitAttribute]++;
      return res;
    }

    public static Result CreateLeafDepthHistogram(RegressionNodeTreeModel treeModel) {
      var list = new List<int>();
      GetLeafDepths(treeModel.Root, 0, list);
      var row = new DataRow("Depths", "", list.Select(x => (double)x)) {
        VisualProperties = {ChartType = DataRowVisualProperties.DataRowChartType.Histogram}
      };
      var hist = new DataTable("LeafDepths");
      hist.Rows.Add(row);
      return new Result(hist.Name, hist);
    }

    public static Result CreateRulesResult(RegressionRuleSetModel ruleSetModel, IRegressionProblemData pd, string resultName, bool displayModels) {
      var res = new ResultCollection();
      var i = 0;
      foreach (var rule in ruleSetModel.Rules)
        res.Add(new Result("Rule" + i++, CreateRulesResult(rule, pd, displayModels, out pd)));
      return new Result(resultName, res);
    }

    public static IResult CreateCoverageDiagram(RegressionRuleSetModel setModel, IRegressionProblemData problemData) {
      var res = new DataTable(CoverageDiagramResultName);
      var training = CountCoverage(setModel, problemData.Dataset, problemData.TrainingIndices);
      var test = CountCoverage(setModel, problemData.Dataset, problemData.TestIndices);
      res.Rows.Add(new DataRow("Training", "", training));
      res.Rows.Add(new DataRow("Test", "", test));

      foreach (var row in res.Rows)
        row.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Columns;
      res.VisualProperties.XAxisMaximumFixedValue = training.Count + 1;
      res.VisualProperties.XAxisMaximumAuto = false;
      res.VisualProperties.XAxisMinimumFixedValue = 0;
      res.VisualProperties.XAxisMinimumAuto = false;
      res.VisualProperties.XAxisTitle = "Rule";
      res.VisualProperties.YAxisTitle = "Covered Instances";

      return new Result(CoverageDiagramResultName, res);
    }

    private static void GetLeafDepths(RegressionNodeModel n, int depth, ICollection<int> res) {
      if (n == null) return;
      if (n.Left == null && n.Right == null) res.Add(depth);
      else {
        GetLeafDepths(n.Left, depth + 1, res);
        GetLeafDepths(n.Right, depth + 1, res);
      }
    }

    private static IScope CreateRulesResult(RegressionRuleModel regressionRuleModel, IRegressionProblemData pd, bool displayModels, out IRegressionProblemData notCovered) {
      var training = pd.TrainingIndices.Where(x => !regressionRuleModel.Covers(pd.Dataset, x)).ToArray();
      var test = pd.TestIndices.Where(x => !regressionRuleModel.Covers(pd.Dataset, x)).ToArray();
      if (training.Length > 0 || test.Length > 0) {
        var data = new Dataset(pd.Dataset.DoubleVariables, pd.Dataset.DoubleVariables.Select(v => pd.Dataset.GetDoubleValues(v, training.Concat(test)).ToArray()));
        notCovered = new RegressionProblemData(data, pd.AllowedInputVariables, pd.TargetVariable);
        notCovered.TestPartition.Start = notCovered.TrainingPartition.End = training.Length;
        notCovered.TestPartition.End = training.Length + test.Length;
      } else notCovered = null;

      var training2 = pd.TrainingIndices.Where(x => regressionRuleModel.Covers(pd.Dataset, x)).ToArray();
      var test2 = pd.TestIndices.Where(x => regressionRuleModel.Covers(pd.Dataset, x)).ToArray();
      var data2 = new Dataset(pd.Dataset.DoubleVariables, pd.Dataset.DoubleVariables.Select(v => pd.Dataset.GetDoubleValues(v, training2.Concat(test2)).ToArray()));
      var covered = new RegressionProblemData(data2, pd.AllowedInputVariables, pd.TargetVariable);
      covered.TestPartition.Start = covered.TrainingPartition.End = training2.Length;
      covered.TestPartition.End = training2.Length + test2.Length;

      var res2 = new Scope("RuleModels");
      res2.Variables.Add(new Variable(ConditionResultName, new StringValue(regressionRuleModel.ToCompactString())));
      res2.Variables.Add(new Variable(CoverResultName, new IntValue(pd.TrainingIndices.Count() - training.Length)));
      if (displayModels)
        res2.Variables.Add(new Variable(RuleModelResultName, regressionRuleModel.CreateRegressionSolution(covered)));
      return res2;
    }

    private static IReadOnlyList<double> CountCoverage(RegressionRuleSetModel setModel, IDataset data, IEnumerable<int> rows) {
      var rules = setModel.Rules.ToArray();
      var res = new double[rules.Length];
      foreach (var row in rows)
        for (var i = 0; i < rules.Length; i++)
          if (rules[i].Covers(data, row)) {
            res[i]++;
            break;
          }
      return res;
    }

    public static void AnalyzeNodes(RegressionNodeTreeModel tree, ResultCollection results, IRegressionProblemData pd) {
      var dict = new Dictionary<int, RegressionNodeModel>();
      var trainingLeafRows = new Dictionary<int, IReadOnlyList<int>>();
      var testLeafRows = new Dictionary<int, IReadOnlyList<int>>();
      var modelNumber = new IntValue(1);
      var symtree = new SymbolicExpressionTree(MirrorTree(tree.Root, dict, trainingLeafRows, testLeafRows, modelNumber, pd.Dataset, pd.TrainingIndices.ToList(), pd.TestIndices.ToList()));
      results.AddOrUpdateResult("DecisionTree", symtree);

      if (dict.Count > 200) return;
      var models = new Scope("NodeModels");
      results.AddOrUpdateResult("NodeModels", models);
      foreach (var m in dict.Keys.OrderBy(x => x))
        models.Variables.Add(new Variable("Model " + m, dict[m].CreateRegressionSolution(Subselect(pd, trainingLeafRows[m], testLeafRows[m]))));
    }

    public static void PruningChart(RegressionNodeTreeModel tree, ComplexityPruning pruning, ResultCollection results) {
      var nodes = new Queue<RegressionNodeModel>();
      nodes.Enqueue(tree.Root);
      var max = 0.0;
      var strenghts = new SortedList<double, int>();
      while (nodes.Count > 0) {
        var n = nodes.Dequeue();

        if (n.IsLeaf) {
          max++;
          continue;
        }

        if (!strenghts.ContainsKey(n.PruningStrength)) strenghts.Add(n.PruningStrength, 0);
        strenghts[n.PruningStrength]++;
        nodes.Enqueue(n.Left);
        nodes.Enqueue(n.Right);
      }
      if (strenghts.Count == 0) return;

      var plot = new ScatterPlot("Pruned Sizes", "") {
        VisualProperties = {
          XAxisTitle = "Pruning Strength",
          YAxisTitle = "Tree Size",
          XAxisMinimumAuto = false,
          XAxisMinimumFixedValue = 0
        }
      };
      var row = new ScatterPlotDataRow("TreeSizes", "", new List<Point2D<double>>());
      row.Points.Add(new Point2D<double>(pruning.PruningStrength, max));

      var fillerDots = new Queue<double>();
      var minX = pruning.PruningStrength;
      var maxX = strenghts.Last().Key;
      var size = (maxX - minX) / 200;
      for (var x = minX; x <= maxX; x += size) {
        fillerDots.Enqueue(x);
      }

      foreach (var strenght in strenghts.Keys) {
        while (fillerDots.Count > 0 && strenght > fillerDots.Peek())
          row.Points.Add(new Point2D<double>(fillerDots.Dequeue(), max));
        max -= strenghts[strenght];
        row.Points.Add(new Point2D<double>(strenght, max));
      }


      row.VisualProperties.PointSize = 6;
      plot.Rows.Add(row);
      results.AddOrUpdateResult("PruningSizes", plot);
    }


    private static IRegressionProblemData Subselect(IRegressionProblemData data, IReadOnlyList<int> training, IReadOnlyList<int> test) {
      var dataset = RegressionTreeUtilities.ReduceDataset(data.Dataset, training.Concat(test).ToList(), data.AllowedInputVariables.ToList(), data.TargetVariable);
      var res = new RegressionProblemData(dataset, data.AllowedInputVariables, data.TargetVariable);
      res.TrainingPartition.Start = 0;
      res.TrainingPartition.End = training.Count;
      res.TestPartition.Start = training.Count;
      res.TestPartition.End = training.Count + test.Count;
      return res;
    }

    private static SymbolicExpressionTreeNode MirrorTree(RegressionNodeModel regressionNode, IDictionary<int, RegressionNodeModel> dict,
      IDictionary<int, IReadOnlyList<int>> trainingLeafRows,
      IDictionary<int, IReadOnlyList<int>> testLeafRows,
      IntValue nextId, IDataset data, IReadOnlyList<int> trainingRows, IReadOnlyList<int> testRows) {
      if (regressionNode.IsLeaf) {
        var i = nextId.Value++;
        dict.Add(i, regressionNode);
        trainingLeafRows.Add(i, trainingRows);
        testLeafRows.Add(i, testRows);
        return new SymbolicExpressionTreeNode(new TextSymbol("Model " + i + "\n(" + trainingRows.Count + "/" + testRows.Count + ")"));
      }

      var pftext = "\npf = " + regressionNode.PruningStrength.ToString("0.###");
      var text = regressionNode.SplitAttribute + " <= " + regressionNode.SplitValue.ToString("0.###");
      if (!double.IsNaN(regressionNode.PruningStrength)) text += pftext;

      var textNode = new SymbolicExpressionTreeNode(new TextSymbol(text));
      IReadOnlyList<int> lTrainingRows, rTrainingRows;
      IReadOnlyList<int> lTestRows, rTestRows;
      RegressionTreeUtilities.SplitRows(trainingRows, data, regressionNode.SplitAttribute, regressionNode.SplitValue, out lTrainingRows, out rTrainingRows);
      RegressionTreeUtilities.SplitRows(testRows, data, regressionNode.SplitAttribute, regressionNode.SplitValue, out lTestRows, out rTestRows);

      textNode.AddSubtree(MirrorTree(regressionNode.Left, dict, trainingLeafRows, testLeafRows, nextId, data, lTrainingRows, lTestRows));
      textNode.AddSubtree(MirrorTree(regressionNode.Right, dict, trainingLeafRows, testLeafRows, nextId, data, rTrainingRows, rTestRows));

      return textNode;
    }


    [StorableType("D5540C63-310B-4D6F-8A3D-6C1A08DE7F80")]
    private sealed class TextSymbol : Symbol {
      [StorableConstructor]
      private TextSymbol(StorableConstructorFlag _) : base(_) { }
      private TextSymbol(Symbol original, Cloner cloner) : base(original, cloner) { }
      public TextSymbol(string name) : base(name, "") {
        Name = name;
      }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new TextSymbol(this, cloner);
      }
      public override int MinimumArity {
        get { return 0; }
      }
      public override int MaximumArity {
        get { return int.MaxValue; }
      }
    }
  }
}