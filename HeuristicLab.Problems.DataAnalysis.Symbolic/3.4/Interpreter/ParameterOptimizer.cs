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
    private const string OptimizerIterationsParameterName = "OptimizerIterations";

    #region parameters
    public IFixedValueParameter<IntValue> OptimizerIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[OptimizerIterationsParameterName]; }
    }
    #endregion

    #region parameter properties
    public int OptimizerIterations {
      get { return OptimizerIterationsParameter.Value.Value; }
      set { OptimizerIterationsParameter.Value.Value = value; }
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
      Parameters.Add(new FixedValueParameter<IntValue>(OptimizerIterationsParameterName, "The number of iterations for the nonlinear least squares optimizer.", new IntValue(10)));
    }

    private static byte MapSupportedSymbols(ISymbolicExpressionTreeNode node) {
      var opCode = OpCodes.MapSymbolToOpCode(node);
      if (supportedOpCodes.Contains(opCode)) return opCode;
      else throw new NotSupportedException($"The native interpreter does not support {node.Symbol.Name}");
    }

    public static Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows, string targetVariable, IEnumerable<double> weights, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize, SolverOptions options, ref SolverSummary summary) {
      var code = NativeInterpreter.Compile(tree, dataset, MapSupportedSymbols, out List<ISymbolicExpressionTreeNode> nodes);

      for (int i = 0; i < code.Length; ++i) {
        code[i].Optimize = nodesToOptimize.Contains(nodes[i]) ? 1 : 0;
      }

      if (options.Iterations > 0) {
        var target = dataset.GetDoubleValues(targetVariable, rows).ToArray();
        var rowsArray = rows.ToArray();
        var result = new double[rowsArray.Length];
        var weightsArray = weights.Any() ? weights.ToArray() : null;

        NativeWrapper.Optimize(code, rowsArray, target, weightsArray, options, result, out summary);
      }
      return Enumerable.Range(0, code.Length).Where(i => nodes[i] is SymbolicExpressionTreeTerminalNode).ToDictionary(i => nodes[i], i => code[i].Coeff);
    }

    public Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows, string targetVariable, IEnumerable<double> weights,
      HashSet<ISymbolicExpressionTreeNode> nodesToOptimize = null) {
      var options = new SolverOptions { Iterations = OptimizerIterations };
      var summary = new SolverSummary();

      // if no nodes are specified, use all the nodes
      if (nodesToOptimize == null) {
        nodesToOptimize = new HashSet<ISymbolicExpressionTreeNode>(tree.IterateNodesPrefix().Where(x => x is SymbolicExpressionTreeTerminalNode));
      }

      return OptimizeTree(tree, dataset, rows, targetVariable, weights, nodesToOptimize, options, ref summary);
    }

    public static Dictionary<ISymbolicExpressionTreeNode, double> OptimizeTree(ISymbolicExpressionTree[] terms, IDataset dataset, IEnumerable<int> rows, string targetVariable, IEnumerable<double> weights, HashSet<ISymbolicExpressionTreeNode> nodesToOptimize, SolverOptions options, double[] coeff, ref SolverSummary summary) {
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
