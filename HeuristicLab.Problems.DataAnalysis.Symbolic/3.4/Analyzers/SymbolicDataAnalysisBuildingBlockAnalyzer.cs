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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using static HeuristicLab.Problems.DataAnalysis.Symbolic.SymbolicExpressionHashExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Analyzers {
  [Item("SymbolicDataAnalysisBuildingBlockAnalyzer", "An analyzer that uses tree hashing to identify the most common subtrees (building blocks) in the population")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisBuildingBlockAnalyzer : SymbolicDataAnalysisAnalyzer {
    private const string BuildingBlocksResultName = "BuildingBlocks";
    private const string MinimumSubtreeLengthParameterName = "MinimumSubtreeLength";
    private const string SimplifyTreesParameterName = "SimplifyTrees";

    private readonly InfixExpressionFormatter formatter = new InfixExpressionFormatter();
    private Dictionary<int, DataRow> hashToRow = new Dictionary<int, DataRow>();

    public IValueLookupParameter<IntValue> MinimumSubtreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MinimumSubtreeLengthParameterName]; }
    }

    public IValueLookupParameter<BoolValue> SimplifyTreesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[SimplifyTreesParameterName]; }
    }

    public IntValue MinimumSubtreeLength {
      get { return MinimumSubtreeLengthParameter.ActualValue; }
    }

    public BoolValue SimplifyTrees {
      get { return SimplifyTreesParameter.ActualValue; }
    }

    public override void InitializeState() {
      base.InitializeState();

      hashToRow = new Dictionary<int, DataRow>();
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(SimplifyTreesParameterName)) {
        Parameters.Add(new ValueLookupParameter<BoolValue>(SimplifyTreesParameterName, new BoolValue(false)));
      }
    }

    public SymbolicDataAnalysisBuildingBlockAnalyzer() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MinimumSubtreeLengthParameterName, new IntValue(3)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(SimplifyTreesParameterName, new BoolValue(false)));
    }

    private SymbolicDataAnalysisBuildingBlockAnalyzer(SymbolicDataAnalysisBuildingBlockAnalyzer original, Cloner cloner) : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisBuildingBlockAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      DataTable dt;

      if (!ResultCollection.ContainsKey(BuildingBlocksResultName)) {
        dt = new DataTable(BuildingBlocksResultName);
        ResultCollection.Add(new Result(BuildingBlocksResultName, dt));
      } else {
        dt = (DataTable)ResultCollection[BuildingBlocksResultName].Value;
      }

      var minLength = MinimumSubtreeLength.Value - 1; // -1 because the HashNode.Size property returns the size without current node (-1)
      var simplify = SimplifyTrees.Value;

      var expressions = new Dictionary<int, string>();
      var expressionCounts = new Dictionary<int, int>();

      int totalCount = 0; // total number of subtrees examined
      foreach (var tree in SymbolicExpressionTree) {
        var hashNodes = tree.Root.GetSubtree(0).GetSubtree(0).MakeNodes();
        var simplified = simplify ? hashNodes.Simplify() : hashNodes.Sort();

        for (int i = 0; i < simplified.Length; i++) {
          HashNode<ISymbolicExpressionTreeNode> s = simplified[i];
          if (s.IsChild || s.Size < minLength) {
            continue;
          }
          ++totalCount;
          var hash = s.CalculatedHashValue;
          if (expressions.TryGetValue(hash, out string str)) {
            expressionCounts[hash]++;
          } else {
            // set constant and weight values so the tree is formatted nicely by the formatter
            var nodes = new HashNode<ISymbolicExpressionTreeNode>[1 + s.Size];
            Array.Copy(simplified, i - s.Size, nodes, 0, nodes.Length);
            var subtree = nodes.ToSubtree();

            foreach (var node in subtree.IterateNodesPostfix()) {
              if (node is ConstantTreeNode constantTreeNode) {
                constantTreeNode.Value = 0;
              } else if (node is VariableTreeNode variableTreeNode) {
                variableTreeNode.Weight = 1;
              }
            }

            expressions[hash] = formatter.Format(subtree);
            expressionCounts[hash] = 1;
          }
        }
      }

      var mostCommon = expressionCounts.OrderByDescending(x => x.Value).Take(10).ToList();
      var mostCommonLabels = mostCommon.Select(x => expressions[x.Key]).ToList();

      foreach (var t in hashToRow) {
        var hash = t.Key;
        var row = t.Value;

        if (expressionCounts.TryGetValue(hash, out int count)) {
          row.Values.Add((double)count / totalCount);
        } else {
          row.Values.Add(0);
        }
      }

      var nValues = dt.Rows.Any() ? dt.Rows.Max(x => x.Values.Count) : 0;

      for (int i = 0; i < mostCommon.Count; ++i) {
        var hash = mostCommon[i].Key;
        var count = mostCommon[i].Value;

        if (hashToRow.ContainsKey(hash)) {
          continue;
        }
        var label = mostCommonLabels[i];
        var row = new DataRow(label) { VisualProperties = { StartIndexZero = true } };
        // pad with zeroes
        for (int j = 0; j < nValues - 1; ++j) {
          row.Values.Add(0);
        }
        row.Values.Add((double)count / totalCount);
        dt.Rows.Add(row);
        hashToRow[hash] = row;
      }
      return base.Apply();
    }
  }
}
