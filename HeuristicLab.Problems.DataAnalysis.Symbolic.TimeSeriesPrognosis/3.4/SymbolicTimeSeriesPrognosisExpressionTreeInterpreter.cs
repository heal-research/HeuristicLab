#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  [StorableClass]
  [Item("SymbolicTimeSeriesPrognosisInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.")]
  public sealed class SymbolicTimeSeriesPrognosisExpressionTreeInterpreter : SymbolicDataAnalysisExpressionTreeInterpreter, ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter {
    private const string TargetVariableParameterName = "TargetVariable";

    public IFixedValueParameter<StringValue> TargetVariableParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[TargetVariableParameterName]; }
    }

    public string TargetVariable {
      get { return TargetVariableParameter.Value.Value; }
      set { TargetVariableParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private SymbolicTimeSeriesPrognosisExpressionTreeInterpreter(bool deserializing) : base(deserializing) { }
    private SymbolicTimeSeriesPrognosisExpressionTreeInterpreter(SymbolicTimeSeriesPrognosisExpressionTreeInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisExpressionTreeInterpreter(this, cloner);
    }

    internal SymbolicTimeSeriesPrognosisExpressionTreeInterpreter()
      : base("SymbolicTimeSeriesPrognosisInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.") {
      Parameters.Add(new FixedValueParameter<StringValue>(TargetVariableParameterName));
      TargetVariableParameter.Hidden = true;
    }

    public SymbolicTimeSeriesPrognosisExpressionTreeInterpreter(string targetVariable)
      : this() {
      TargetVariable = targetVariable;
    }

    // for each row several (=#horizon) future predictions
    public IEnumerable<IEnumerable<double>> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows, int horizon) {
      return GetSymbolicExpressionTreeValues(tree, dataset, rows, rows.Select(row => horizon));
    }

    private readonly object syncRoot = new object();
    public IEnumerable<IEnumerable<double>> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons) {
      if (CheckExpressionsWithIntervalArithmetic)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");

      string targetVariable = TargetVariable;
      double[] targetVariableCache = dataset.GetDoubleValues(targetVariable).ToArray();
      lock (syncRoot) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }
      var state = PrepareInterpreterState(tree, dataset, targetVariableCache, TargetVariable);
      var rowsEnumerator = rows.GetEnumerator();
      var horizonsEnumerator = horizons.GetEnumerator();

      // produce a n-step forecast for all rows
      while (rowsEnumerator.MoveNext() & horizonsEnumerator.MoveNext()) {
        int row = rowsEnumerator.Current;
        int horizon = horizonsEnumerator.Current;
        double[] vProgs = new double[horizon];

        for (int i = 0; i < horizon; i++) {
          int localRow = i + row; // create a local variable for the ref parameter
          vProgs[i] = Evaluate(dataset, ref localRow, state);
          targetVariableCache[localRow] = vProgs[i];
          state.Reset();
        }
        yield return vProgs;
      }

      if (rowsEnumerator.MoveNext() || horizonsEnumerator.MoveNext())
        throw new ArgumentException("Number of elements in rows and horizon enumerations doesn't match.");
    }

    private static InterpreterState PrepareInterpreterState(ISymbolicExpressionTree tree, IDataset dataset, double[] targetVariableCache, string targetVariable) {
      Instruction[] code = SymbolicExpressionTreeCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      int necessaryArgStackSize = 0;
      foreach (Instruction instr in code) {
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
          if (variableTreeNode.VariableName == targetVariable)
            instr.data = targetVariableCache;
          else
            instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.LagVariable) {
          var variableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
          if (variableTreeNode.VariableName == targetVariable)
            instr.data = targetVariableCache;
          else
            instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
          if (variableTreeNode.VariableName == targetVariable)
            instr.data = targetVariableCache;
          else
            instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }

      return new InterpreterState(code, necessaryArgStackSize);
    }
  }
}
