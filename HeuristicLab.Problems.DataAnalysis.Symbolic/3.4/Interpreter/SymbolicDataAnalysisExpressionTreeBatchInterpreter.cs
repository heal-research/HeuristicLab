using System;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

using static HeuristicLab.Problems.DataAnalysis.Symbolic.BatchOperations;
//using static HeuristicLab.Problems.DataAnalysis.Symbolic.BatchOperationsVector;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicDataAnalysisExpressionTreeBatchInterpreter", "An interpreter that uses batching and vectorization techniques to achieve faster performance.")]
  [StorableClass]
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
    protected SymbolicDataAnalysisExpressionTreeBatchInterpreter(bool deserializing) : base(deserializing) { }
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
                break;
              } else {
                Load(instr.buf, code[c].buf);
                for (int j = 1; j < n; ++j) {
                  Sub(instr.buf, code[c + j].buf);
                }
                break;
              }
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
                break;
              } else {
                Load(instr.buf, code[c].buf);
                for (int j = 1; j < n; ++j) {
                  Div(instr.buf, code[c + j].buf);
                }
                break;
              }
            }

          case OpCodes.Exp: {
              Exp(instr.buf, code[c].buf);
              break;
            }

          case OpCodes.Log: {
              Log(instr.buf, code[c].buf);
              break;
            }
        }
      }
    }

    private double[] GetValues(ISymbolicExpressionTree tree, IDataset dataset, int[] rows) {
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

      return result;
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      return GetValues(tree, dataset, rows.ToArray());
    }

    public void InitializeState() {
    }

    private BatchInstruction[] Compile(ISymbolicExpressionTree tree, IDataset dataset, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var code = new BatchInstruction[root.GetLength()];
      if (root.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
      code[0] = new BatchInstruction { narg = (ushort)root.SubtreeCount, opcode = opCodeMapper(root) };
      int c = 1, i = 0;
      foreach (var node in root.IterateNodesBreadth()) {
        for (int j = 0; j < node.SubtreeCount; ++j) {
          var s = node.GetSubtree(j);
          if (s.SubtreeCount > ushort.MaxValue) throw new ArgumentException("Number of subtrees is too big (>65.535)");
          code[c + j] = new BatchInstruction { narg = (ushort)s.SubtreeCount, opcode = opCodeMapper(s) };
        }

        if (node is VariableTreeNode variable) {
          code[i].weight = variable.Weight;
          code[i].data = dataset.GetReadOnlyDoubleValues(variable.VariableName).ToArray();
          code[i].buf = new double[BATCHSIZE];
        } else if (node is ConstantTreeNode constant) {
          code[i].value = constant.Value;
          code[i].buf = Enumerable.Repeat(code[i].value, BATCHSIZE).ToArray();
        } else if (node.SubtreeCount > 0) {
          code[i].buf = new double[BATCHSIZE];
        }

        code[i].childIndex = c;
        c += node.SubtreeCount;
        ++i;
      }
      return code;
    }
  }
}
