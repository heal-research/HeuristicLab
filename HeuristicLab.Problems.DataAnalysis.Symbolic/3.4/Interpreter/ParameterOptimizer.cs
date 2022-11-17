#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.NativeInterpreter;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("A624630B-0CEB-4D06-9B26-708987A7AE8F")]
  [Item("ParameterOptimizer", "Operator calling into native C++ code for tree interpretation.")]
  public sealed class ParameterOptimizer : ParameterizedNamedItem {
    private const string IterationsParameterName = "Iterations";

    #region parameters
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    #endregion

    #region parameter properties
    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }
    #endregion

    #region storable ctor and cloning
    [StorableConstructor]
    private ParameterOptimizer(StorableConstructorFlag _) : base(_) { }

    public ParameterOptimizer(ParameterOptimizer original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParameterOptimizer(this, cloner);
    }
    #endregion

    public ParameterOptimizer() {
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "The number of iterations for the nonlinear least squares optimizer.", new IntValue(10)));
    }

    private static byte MapSupportedSymbols(ISymbolicExpressionTreeNode node) {
      var opCode = OpCodes.MapSymbolToOpCode(node);
      if (supportedOpCodes.Contains(opCode)) return opCode;
      else throw new NotSupportedException($"The native interpreter does not support {node.Symbol.Name}");
    }

    public static Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(
              ISymbolicExpressionTree tree, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize,
              IDataset dataset, string targetVariable, IEnumerable<int> rows,
              SolverOptions options, ref SolverSummary summary) {
      return OptimizeTree(tree, nodesToOptimize, dataset, targetVariable, rows, weights: Enumerable.Empty<double>(), options, ref summary);
    }


    public static Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(
            ISymbolicExpressionTree tree, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize,
            IDataset dataset, string targetVariable, IEnumerable<int> rows, IEnumerable<double> weights,
            SolverOptions options, ref SolverSummary summary) {
      var code = NativeInterpreter.Compile(tree, dataset, MapSupportedSymbols, out List<ISymbolicExpressionTreeNode> nodes);
      for (int i = 0; i < code.Length; ++i) {
        code[i].Optimize = nodesToOptimize.Contains(nodes[i]) ? 1 : 0;
      }

      if (options.Iterations > 0) {
        var target = dataset.GetDoubleValues(targetVariable, rows).ToArray();
        var rowsArray = rows.ToArray();
        var result = new double[rowsArray.Length];
        double[] weightsArray = null;
        if (weights.Any()) weightsArray = weights.ToArray();

        NativeWrapper.Optimize(code, rowsArray, target, weightsArray, options, result, out summary);
      }
      return Enumerable.Range(0, code.Length).Where(i => nodes[i] is SymbolicExpressionTreeTerminalNode).ToDictionary(i => nodes[i], i => code[i].Coeff);
    }

    public Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(ISymbolicExpressionTree tree, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize, IDataset dataset, string targetVariable, IEnumerable<int> rows,
      IEnumerable<double> weights) {
      var options = new SolverOptions { Iterations = Iterations };
      var summary = new SolverSummary();

      // if no nodes are specified, use all the nodes
      if (nodesToOptimize == null) {
        nodesToOptimize = new HashSet<ISymbolicExpressionTreeNode>(tree.IterateNodesPrefix().Where(x => x is SymbolicExpressionTreeTerminalNode));
      }

      return OptimizeTree(tree, nodesToOptimize, dataset, targetVariable, rows, weights, options, ref summary);
    }

    public static Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(ISymbolicExpressionTree[] terms, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize, IDataset dataset, string targetVariable, IEnumerable<int> rows, IEnumerable<double> weights, SolverOptions options, double[] coeff, ref SolverSummary summary) {
      if (options.Iterations == 0) {
        // throw exception? set iterations to 100? return empty dictionary?
        return new Dictionary<ISymbolicExpressionTreeNode, double>();
      }

      var termIndices = new int[terms.Length];
      var totalCodeSize = 0;
      var totalCode = new List<NativeInstruction>();
      var totalNodes = new List<ISymbolicExpressionTreeNode>();

      // internally the native wrapper takes a single array of NativeInstructions where the indices point to the individual terms
      for (int i = 0; i < terms.Length; ++i) {
        var code = NativeInterpreter.Compile(terms[i], dataset, MapSupportedSymbols, out List<ISymbolicExpressionTreeNode> nodes);
        for (int j = 0; j < code.Length; ++j) {
          code[j].Optimize = nodesToOptimize.Contains(nodes[j]) ? 1 : 0;
        }
        totalCode.AddRange(code);
        totalNodes.AddRange(nodes);

        termIndices[i] = code.Length + totalCodeSize - 1;
        totalCodeSize += code.Length;
      }
      var target = dataset.GetDoubleValues(targetVariable, rows).ToArray();
      var rowsArray = rows.ToArray();
      var result = new double[rowsArray.Length];
      var codeArray = totalCode.ToArray();
      var weightsArray = weights.Any() ? weights.ToArray() : null;

      NativeWrapper.OptimizeVarPro(codeArray, termIndices, rowsArray, target, weightsArray, coeff, options, result, out summary);
      return Enumerable.Range(0, totalCodeSize).Where(i => codeArray[i].Optimize != 0).ToDictionary(i => totalNodes[i], i => codeArray[i].Coeff);
    }

    public static bool CanOptimizeParameters(ISymbolicExpressionTree tree) {
      var actualRoot = tree.Root.GetSubtree(0).GetSubtree(0);
      return actualRoot.IterateNodesPrefix()
        .Select(OpCodes.MapSymbolToOpCode)
        .All(supportedOpCodes.Contains);
    }

    private static readonly HashSet<byte> supportedOpCodes = new HashSet<byte>() {
      (byte)OpCode.Number,
      (byte)OpCode.Constant,
      (byte)OpCode.Variable,
      (byte)OpCode.Add,
      (byte)OpCode.Sub,
      (byte)OpCode.Mul,
      (byte)OpCode.Div,
      (byte)OpCode.Exp,
      (byte)OpCode.Log,
      (byte)OpCode.Sin,
      (byte)OpCode.Cos,
      (byte)OpCode.Tan,
      (byte)OpCode.Tanh,
      // (byte)OpCode.Power, // these symbols are handled differently in the NativeInterpreter than in HL
      // (byte)OpCode.Root,
      (byte)OpCode.SquareRoot,
      (byte)OpCode.Square,
      (byte)OpCode.CubeRoot,
      (byte)OpCode.Cube,
      (byte)OpCode.Absolute,
      (byte)OpCode.AnalyticQuotient
    };
  }
}
