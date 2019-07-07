using System;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

using static HeuristicLab.Problems.DataAnalysis.Symbolic.BatchOperations;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicDataAnalysisExpressionTreeBatchInterpreter", "An interpreter that uses batching and vectorization techniques to achieve faster performance.")]
  [StorableType("BEB15146-BB95-4838-83AC-6838543F017B")]
  public class SymbolicDataAnalysisExpressionTreeBatchInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
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

    public SymbolicDataAnalysisExpressionTreeBatchInterpreter() {
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionTreeBatchInterpreter(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisExpressionTreeBatchInterpreter(SymbolicDataAnalysisExpressionTreeBatchInterpreter original, Cloner cloner) : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeBatchInterpreter(this, cloner);
    }

    private void LoadData(BatchInstruction instr, int[] rows, int rowIndex, int batchSize) {
      for (int i = 0; i < batchSize; ++i) {
        var row = rows[rowIndex] + i;
        instr.buf[i] = instr.weight * instr.data[row];
      }
    }

    private void Evaluate(BatchInstruction[] code, int[] rows, int rowIndex, int batchSize) {
      for (int i = code.Length - 1; i >= 0; --i) {
        var instr = code[i];
        var c = instr.childIndex;
        var n = instr.narg;

        switch (instr.opcode) {
          case OpCodes.Variable: {
              LoadData(instr, rows, rowIndex, batchSize);
              break;
            }
          case OpCodes.Constant: break; // nothing to do here, don't remove because we want to prevent falling into the default case here.
          case OpCodes.Add: {
              Load(instr.buf, code[c].buf);
              for (int j = 1; j < n; ++j) {
                Add(instr.buf, code[c + j].buf);
              }
              break;
            }

          case OpCodes.Sub: {
              if (n == 1) {
                Neg(instr.buf, code[c].buf);
              } else {
                Load(instr.buf, code[c].buf);
                for (int j = 1; j < n; ++j) {
                  Sub(instr.buf, code[c + j].buf);
                }
              }
              break;
            }

          case OpCodes.Mul: {
              Load(instr.buf, code[c].buf);
              for (int j = 1; j < n; ++j) {
                Mul(instr.buf, code[c + j].buf);
              }
              break;
            }

          case OpCodes.Div: {
              if (n == 1) {
                Inv(instr.buf, code[c].buf);
              } else {
                Load(instr.buf, code[c].buf);
                for (int j = 1; j < n; ++j) {
                  Div(instr.buf, code[c + j].buf);
                }
              }
              break;
            }

          case OpCodes.Square: {
              Square(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Root: {
              Load(instr.buf, code[c].buf);
              Root(instr.buf, code[c + 1].buf);
              break;
            }

          case OpCodes.SquareRoot: {
              Sqrt(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Cube: {
              Cube(instr.buf, code[c].buf);
              break;
            }
          case OpCodes.CubeRoot: {
              CubeRoot(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Power: {
              Load(instr.buf, code[c].buf);
              Pow(instr.buf, code[c + 1].buf);
              break;
            }

          case OpCodes.Exp: {
              Exp(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Log: {
              Log(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Sin: {
              Sin(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Cos: {
              Cos(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Tan: {
              Tan(instr.buf, code[c].buf);
              break;
            }
          case OpCodes.Tanh: {
              Tanh(instr.buf, code[c].buf);
              break;
            }
          case OpCodes.Absolute: {
              Absolute(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.AnalyticQuotient: {
              Load(instr.buf, code[c].buf);
              AnalyticQuotient(instr.buf, code[c + 1].buf);
              break;
            }
          default: throw new NotSupportedException($"This interpreter does not support {(OpCode)instr.opcode}");
        }
      }
    }

    private readonly object syncRoot = new object();

    [ThreadStatic]
    private Dictionary<string, double[]> cachedData;

    [ThreadStatic]
    private IDataset dataset;

    private void InitCache(IDataset dataset) {
      this.dataset = dataset;
      cachedData = new Dictionary<string, double[]>();
      foreach (var v in dataset.DoubleVariables) {
        var values = dataset.GetDoubleValues(v).ToArray();
      }
    }

    public void InitializeState() {
      cachedData = null;
      dataset = null;
      EvaluatedSolutions = 0;
    }

    private double[] GetValues(ISymbolicExpressionTree tree, IDataset dataset, int[] rows) {
      if (cachedData == null || this.dataset != dataset) {
        InitCache(dataset);
      }

      var code = Compile(tree, dataset, OpCodes.MapSymbolToOpCode);
      var remainingRows = rows.Length % BATCHSIZE;
      var roundedTotal = rows.Length - remainingRows;

      var result = new double[rows.Length];

      for (int rowIndex = 0; rowIndex < roundedTotal; rowIndex += BATCHSIZE) {
        Evaluate(code, rows, rowIndex, BATCHSIZE);
        Array.Copy(code[0].buf, 0, result, rowIndex, BATCHSIZE);
      }

      if (remainingRows > 0) {
        Evaluate(code, rows, roundedTotal, remainingRows);
        Array.Copy(code[0].buf, 0, result, roundedTotal, remainingRows);
      }

      // when evaluation took place without any error, we can increment the counter
      lock (syncRoot) {
        EvaluatedSolutions++;
      }

      return result;
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, int[] rows) {
      return GetValues(tree, dataset, rows);
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      return GetSymbolicExpressionTreeValues(tree, dataset, rows.ToArray());
    }

    private BatchInstruction[] Compile(ISymbolicExpressionTree tree, IDataset dataset, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var code = new BatchInstruction[root.GetLength()];
      if (root.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
      int c = 1, i = 0;
      foreach (var node in root.IterateNodesBreadth()) {
        if (node.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
        code[i] = new BatchInstruction {
          opcode = opCodeMapper(node),
          narg = (ushort)node.SubtreeCount,
          buf = new double[BATCHSIZE],
          childIndex = c
        };
        if (node is VariableTreeNode variable) {
          code[i].weight = variable.Weight;
          if (cachedData.ContainsKey(variable.VariableName)) {
            code[i].data = cachedData[variable.VariableName];
          } else {
            code[i].data = dataset.GetReadOnlyDoubleValues(variable.VariableName).ToArray();
            cachedData[variable.VariableName] = code[i].data;
          }
        } else if (node is ConstantTreeNode constant) {
          code[i].value = constant.Value;
          for (int j = 0; j < BATCHSIZE; ++j)
            code[i].buf[j] = code[i].value;
        }
        c += node.SubtreeCount;
        ++i;
      }
      return code;
    }
  }
}
