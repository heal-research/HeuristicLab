#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    [ThreadStatic]
    private static double[] targetVariableCache;
    [ThreadStatic]
    private static List<int> invalidateCacheIndexes;

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
    public IEnumerable<IEnumerable<double>> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows, int horizon) {
      return GetSymbolicExpressionTreeValues(tree, dataset, rows, rows.Select(row => horizon));
    }

    public IEnumerable<IEnumerable<double>> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons) {
      if (CheckExpressionsWithIntervalArithmetic.Value)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");
      if (targetVariableCache == null || targetVariableCache.GetLength(0) < dataset.Rows)
        targetVariableCache = dataset.GetDoubleValues(TargetVariable).ToArray();
      if (invalidateCacheIndexes == null)
        invalidateCacheIndexes = new List<int>(10);

      string targetVariable = TargetVariable;
      lock (EvaluatedSolutions) {
        EvaluatedSolutions.Value++; // increment the evaluated solutions counter
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
          invalidateCacheIndexes.Add(localRow);
          state.Reset();
        }
        yield return vProgs;

        int j = 0;
        foreach (var targetValue in dataset.GetDoubleValues(targetVariable, invalidateCacheIndexes)) {
          targetVariableCache[invalidateCacheIndexes[j]] = targetValue;
          j++;
        }
        invalidateCacheIndexes.Clear();
      }

      if (rowsEnumerator.MoveNext() || horizonsEnumerator.MoveNext())
        throw new ArgumentException("Number of elements in rows and horizon enumerations doesn't match.");
    }

    private static InterpreterState PrepareInterpreterState(ISymbolicExpressionTree tree, Dataset dataset, double[] targetVariableCache, string targetVariable) {
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
