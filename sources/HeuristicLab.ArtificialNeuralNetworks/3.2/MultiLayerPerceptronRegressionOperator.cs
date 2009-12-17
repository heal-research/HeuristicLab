#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Modeling;

namespace HeuristicLab.ArtificialNeuralNetworks {
  public class MultiLayerPerceptronRegressionOperator : OperatorBase {

    public MultiLayerPerceptronRegressionOperator() {
      AddVariableInfo(new VariableInfo("TargetVariable", "Name of the target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("NumberOfHiddenLayerNeurons", "The number of nodes in the hidden layer.", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "(optional) Maximal time offset for time-series prognosis", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "(optional) Minimal time offset for time-series prognosis", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MultiLayerPerceptron", "Formula that was calculated by multi layer perceptron regression", typeof(MultiLayerPerceptron), VariableKind.Out | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      string targetVariable = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      IntData maxTimeOffsetData = GetVariableValue<IntData>("MaxTimeOffset", scope, true, false);
      int maxTimeOffset = maxTimeOffsetData == null ? 0 : maxTimeOffsetData.Data;
      IntData minTimeOffsetData = GetVariableValue<IntData>("MinTimeOffset", scope, true, false);
      int minTimeOffset = minTimeOffsetData == null ? 0 : minTimeOffsetData.Data;
      int nHiddenNodes = GetVariableValue<IntData>("NumberOfHiddenLayerNeurons", scope, true).Data;

      var perceptron = CreateModel(dataset, targetVariable, dataset.VariableNames, start, end, minTimeOffset, maxTimeOffset, nHiddenNodes);
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MultiLayerPerceptron"), perceptron));
      return null;
    }

    public static MultiLayerPerceptron CreateModel(Dataset dataset, string targetVariable, IEnumerable<string> inputVariables, int start, int end, int nHiddenNodes) {
      return CreateModel(dataset, targetVariable, inputVariables, start, end, 0, 0, nHiddenNodes);
    }

    public static MultiLayerPerceptron CreateModel(Dataset dataset, string targetVariable, IEnumerable<string> inputVariables,
        int start, int end,
        int minTimeOffset, int maxTimeOffset, int nHiddenNodes) {
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      List<int> allowedColumns = CalculateAllowedColumns(dataset, targetVariableIndex, inputVariables.Select(x => dataset.GetVariableIndex(x)), start, end);
      List<int> allowedRows = CalculateAllowedRows(dataset, targetVariableIndex, allowedColumns, start, end, minTimeOffset, maxTimeOffset);

      double[,] inputMatrix = PrepareInputMatrix(dataset, allowedColumns, allowedRows, minTimeOffset, maxTimeOffset);
      double[] targetVector = PrepareTargetVector(dataset, targetVariableIndex, allowedRows);

      var perceptron = TrainPerceptron(inputMatrix, targetVector, nHiddenNodes);
      return new MultiLayerPerceptron(perceptron, inputVariables, minTimeOffset, maxTimeOffset);
    }



    private static alglib.mlpbase.multilayerperceptron TrainPerceptron(double[,] inputMatrix, double[] targetVector, int nHiddenNodes) {
      int retVal = 0;
      int n = targetVector.Length;
      int p = inputMatrix.GetLength(1);
      alglib.mlpbase.multilayerperceptron perceptron = new alglib.mlpbase.multilayerperceptron();
      alglib.mlpbase.mlpcreate1(p - 1, nHiddenNodes, 1, ref perceptron);
      alglib.mlptrain.mlpreport report = new alglib.mlptrain.mlpreport();
      double[,] dataset = new double[n, p];
      for (int row = 0; row < n; row++) {
        for (int column = 0; column < p - 1; column++) {
          dataset[row, column] = inputMatrix[row, column];
        }
        dataset[row, p - 1] = targetVector[row];
      }
      alglib.mlptrain.mlptrainlbfgs(ref perceptron, ref dataset, n, 0.001, 2, 0.01, 0, ref retVal, ref report);
      if (retVal != 2) throw new ArgumentException("Error in training of multi layer perceptron");
      return perceptron;
    }

    //returns list of valid row indexes (rows without NaN values)
    private static List<int> CalculateAllowedRows(Dataset dataset, int targetVariable, IList<int> allowedColumns, int start, int end, int minTimeOffset, int maxTimeOffset) {
      List<int> allowedRows = new List<int>();
      bool add;
      for (int row = start; row < end; row++) {
        add = true;
        for (int colIndex = 0; colIndex < allowedColumns.Count && add == true; colIndex++) {
          for (int timeOffset = minTimeOffset; timeOffset <= maxTimeOffset; timeOffset++) {
            if (
              row + timeOffset < 0 ||
              row + timeOffset > dataset.Rows ||
              double.IsNaN(dataset.GetValue(row + timeOffset, allowedColumns[colIndex])) ||
              double.IsInfinity(dataset.GetValue(row + timeOffset, allowedColumns[colIndex])) ||
              double.IsNaN(dataset.GetValue(row + timeOffset, targetVariable))) {
              add = false;
            }
          }
        }
        if (add)
          allowedRows.Add(row);
        add = true;
      }
      return allowedRows;
    }

    //returns list of valid column indexes (columns which contain max. 10% NaN (or infinity) and contain at least two different values)
    private static List<int> CalculateAllowedColumns(Dataset dataset, int targetVariable, IEnumerable<int> inputVariables, int start, int end) {
      List<int> allowedColumns = new List<int>();
      double n = end - start;
      foreach (int inputVariable in inputVariables) {// = 0; i < dataset.Columns; i++) {
        double nanRatio = dataset.CountMissingValues(inputVariable, start, end) / n;
        if (inputVariable != targetVariable && nanRatio < 0.1 && dataset.GetRange(inputVariable, start, end) > 0.0) {
          allowedColumns.Add(inputVariable);
        }
      }
      return allowedColumns;
    }

    private static double[,] PrepareInputMatrix(Dataset dataset, List<int> allowedColumns, List<int> allowedRows, int minTimeOffset, int maxTimeOffset) {
      int rowCount = allowedRows.Count;
      int timeOffsetRange = (maxTimeOffset - minTimeOffset + 1);
      double[,] matrix = new double[rowCount, (allowedColumns.Count * timeOffsetRange) + 1];
      for (int row = 0; row < allowedRows.Count; row++)
        for (int col = 0; col < allowedColumns.Count; col++) {
          for (int timeOffset = minTimeOffset; timeOffset <= maxTimeOffset; timeOffset++)
            matrix[row, (col * timeOffsetRange) + (timeOffset - minTimeOffset)] = dataset.GetValue(allowedRows[row] + timeOffset, allowedColumns[col]);
        }
      //add constant 1.0 in last column
      for (int i = 0; i < rowCount; i++)
        matrix[i, allowedColumns.Count * timeOffsetRange] = 1.0;
      return matrix;
    }

    private static double[] PrepareTargetVector(Dataset dataset, int targetVariable, List<int> allowedRows) {
      int rowCount = allowedRows.Count;
      double[] targetVector = new double[rowCount];
      double[] samples = dataset.Samples;
      for (int row = 0; row < rowCount; row++) {
        targetVector[row] = dataset.GetValue(allowedRows[row], targetVariable);
      }
      return targetVector;
    }
  }
}
