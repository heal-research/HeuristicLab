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
using System.Runtime.InteropServices;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisExpressionTreeNativeInterpreter", "An interpreter that wraps a native dll")]
  public class SymbolicDataAnalysisExpressionTreeNativeInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

    #region parameters
    public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
    }
    #endregion

    #region properties
    public int EvaluatedSolutions {
      get { return EvaluatedSolutionsParameter.Value.Value; }
      set { EvaluatedSolutionsParameter.Value.Value = value; }
    }
    #endregion

    public void ClearState() { }

    public SymbolicDataAnalysisExpressionTreeNativeInterpreter() {
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionTreeNativeInterpreter(bool deserializing) : base(deserializing) { }


    protected SymbolicDataAnalysisExpressionTreeNativeInterpreter(SymbolicDataAnalysisExpressionTreeNativeInterpreter original, Cloner cloner) : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeNativeInterpreter(this, cloner);
    }

    private NativeInstruction[] Compile(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var code = new NativeInstruction[root.GetLength()];
      if (root.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
      code[0] = new NativeInstruction { narg = (ushort)root.SubtreeCount, opcode = opCodeMapper(root) };
      int c = 1, i = 0;
      foreach (var node in root.IterateNodesBreadth()) {
        for (int j = 0; j < node.SubtreeCount; ++j) {
          var s = node.GetSubtree(j);
          if (s.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
          code[c + j] = new NativeInstruction { narg = (ushort)s.SubtreeCount, opcode = opCodeMapper(s) };
        }

        if (node is VariableTreeNode variable) {
          code[i].weight = variable.Weight;
          code[i].data = cachedData[variable.VariableName].AddrOfPinnedObject();
        } else if (node is ConstantTreeNode constant) {
          code[i].value = constant.Value;
        }

        code[i].childIndex = c;
        c += node.SubtreeCount;
        ++i;
      }
      return code;
    }

    private readonly object syncRoot = new object();

    [ThreadStatic]
    private static Dictionary<string, GCHandle> cachedData;

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      if (!rows.Any()) return Enumerable.Empty<double>();

      lock (syncRoot) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }

      if (cachedData == null) {
        InitCache(dataset);
      }

      var code = Compile(tree, OpCodes.MapSymbolToOpCode);

      var rowsArray = rows.ToArray();
      var result = new double[rowsArray.Length];

      NativeWrapper.GetValuesVectorized(code, code.Length, rowsArray, rowsArray.Length, result);
      return result;
    }

    private void InitCache(IDataset dataset) {
      cachedData = new Dictionary<string, GCHandle>();
      foreach (var v in dataset.DoubleVariables) {
        var values = dataset.GetDoubleValues(v).ToArray();
        var gch = GCHandle.Alloc(values, GCHandleType.Pinned);
        cachedData[v] = gch;
      }
    }

    public void InitializeState() {
      if (cachedData != null) {
        foreach (var gch in cachedData.Values) {
          gch.Free();
        }
        cachedData = null;
      }
      EvaluatedSolutions = 0;
    }
  }
}
