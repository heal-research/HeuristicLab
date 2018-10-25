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

using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    private Dictionary<int, DataRow> hashToRow = new Dictionary<int, DataRow>();

    #region parameters
    public IValueLookupParameter<IntValue> MinimumSubtreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MinimumSubtreeLengthParameterName]; }
    }

    public IValueLookupParameter<BoolValue> SimplifyTreesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[SimplifyTreesParameterName]; }
    }
    #endregion

    #region parameter properties
    public IntValue MinimumSubtreeLength {
      get { return MinimumSubtreeLengthParameter.ActualValue; }
    }

    public BoolValue SimplifyTrees {
      get { return SimplifyTreesParameter.ActualValue; }
    }
    #endregion

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

    [StorableConstructor]
    private SymbolicDataAnalysisBuildingBlockAnalyzer(bool deserializing) : base(deserializing) { }

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

      int totalCount = 0; // total number of examined subtrees

      // count hashes
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
          if (expressions.ContainsKey(hash)) {
            expressionCounts[hash]++;
            continue;
          }

          var sb = new StringBuilder();
          for (int j = i - s.Size; j < i; ++j) {
            sb.Append(GetLabel(simplified[j].Data)).Append(" ");
          }
          sb.Append(GetLabel(simplified[i].Data));
          expressions[hash] = sb.ToString();
          expressionCounts[hash] = 1;
        }
      }

      // fill in values for existing rows
      foreach (var t in hashToRow) {
        var hash = t.Key;
        var row = t.Value;

        expressionCounts.TryGetValue(hash, out int count);
        row.Values.Add(count);
      }

      var nValues = dt.Rows.Any() ? dt.Rows.Max(x => x.Values.Count) : 0;

      // check if we have new rows
      foreach (var t in expressionCounts.OrderByDescending(x => x.Value).Take(10)) {
        var hash = t.Key;
        var count = t.Value;
        var label = expressions[hash];

        if (hashToRow.ContainsKey(hash)) {
          continue;
        }
        var row = new DataRow(label) { VisualProperties = { StartIndexZero = true } };
        if (nValues > 0) {
          row.Values.AddRange(Enumerable.Repeat<double>(0, nValues - 1)); // pad with zeroes
        }
        row.Values.Add(count);
        dt.Rows.Add(row);
        hashToRow[hash] = row;
      }

      return base.Apply();
    }

    private static string GetLabel(ISymbolicExpressionTreeNode node) {
      if (node is ConstantTreeNode constant) {
        return "C";
      }
      if (node is VariableTreeNode variable) {
        return variable.VariableName;
      }
      if (node.Symbol is Addition) {
        return "+";
      }
      if (node.Symbol is Subtraction) {
        return "-";
      }
      if (node.Symbol is Multiplication) {
        return "*";
      }
      if (node.Symbol is Division) {
        return "/";
      }
      return node.Symbol.ToString();
    }
  }
}
