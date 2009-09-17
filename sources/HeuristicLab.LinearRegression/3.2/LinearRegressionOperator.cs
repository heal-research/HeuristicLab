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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Modeling;
using HeuristicLab.GP;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.LinearRegression {
  public class LinearRegressionOperator : OperatorBase {
    private static double constant = 1.0;

    public LinearRegressionOperator() {
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "(optional) Maximal time offset for time-series prognosis", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "(optional) Minimal time offset for time-series prognosis", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LinearRegressionModel", "Formula that was calculated by linear regression", typeof(IGeneticProgrammingModel), VariableKind.Out | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      IntData maxTimeOffsetData = GetVariableValue<IntData>("MaxTimeOffset", scope, true, false);
      int maxTimeOffset = maxTimeOffsetData == null ? 0 : maxTimeOffsetData.Data;
      IntData minTimeOffsetData = GetVariableValue<IntData>("MinTimeOffset", scope, true, false);
      int minTimeOffset = minTimeOffsetData == null ? 0 : minTimeOffsetData.Data;

      List<int> allowedColumns = CalculateAllowedColumns(dataset, targetVariable, start, end);
      List<int> allowedRows = CalculateAllowedRows(dataset, targetVariable, allowedColumns, start, end, minTimeOffset, maxTimeOffset);

      double[,] inputMatrix = PrepareInputMatrix(dataset, allowedColumns, allowedRows, minTimeOffset, maxTimeOffset);
      double[] targetVector = PrepareTargetVector(dataset, targetVariable, allowedRows);
      double[] coefficients = CalculateCoefficients(inputMatrix, targetVector);
      IFunctionTree tree = CreateModel(coefficients, allowedColumns.Select(i => dataset.GetVariableName(i)).ToList(), minTimeOffset, maxTimeOffset);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("LinearRegressionModel"), new GeneticProgrammingModel(tree)));
      return null;
    }

    private IFunctionTree CreateModel(double[] coefficients, List<string> allowedVariables, int minTimeOffset, int maxTimeOffset) {
      IFunctionTree root = new Addition().GetTreeNode();
      IFunctionTree actNode = root;
      int timeOffsetRange = (maxTimeOffset - minTimeOffset + 1);

      Queue<IFunctionTree> nodes = new Queue<IFunctionTree>();
      for (int i = 0; i < allowedVariables.Count; i++) {
        for (int timeOffset = minTimeOffset; timeOffset <= maxTimeOffset; timeOffset++) {
          var vNode = (VariableFunctionTree)new GP.StructureIdentification.Variable().GetTreeNode();
          vNode.VariableName = allowedVariables[i];
          vNode.Weight = coefficients[(i * timeOffsetRange) + (timeOffset - minTimeOffset)];
          vNode.SampleOffset = timeOffset;
          nodes.Enqueue(vNode);
        }
      }
      var cNode = (ConstantFunctionTree)new Constant().GetTreeNode();

      cNode.Value = coefficients[coefficients.Length - 1];
      nodes.Enqueue(cNode);

      IFunctionTree newTree;
      while (nodes.Count != 1) {
        newTree = new Addition().GetTreeNode();
        newTree.AddSubTree(nodes.Dequeue());
        newTree.AddSubTree(nodes.Dequeue());
        nodes.Enqueue(newTree);
      }

      return nodes.Dequeue();
    }

    private double[] CalculateCoefficients(double[,] inputMatrix, double[] targetVector) {
      double[] weights = new double[targetVector.Length];
      double[] coefficients = new double[inputMatrix.GetLength(1)];
      for (int i = 0; i < weights.Length; i++) weights[i] = 1.0;
      // call external ALGLIB solver
      leastsquares.buildgeneralleastsquares(ref targetVector, ref weights, ref inputMatrix, inputMatrix.GetLength(0), inputMatrix.GetLength(1), ref coefficients);

      return coefficients;
    }

    //returns list of valid row indexes (rows without NaN values)
    private List<int> CalculateAllowedRows(Dataset dataset, int targetVariable, IList<int> allowedColumns, int start, int end, int minTimeOffset, int maxTimeOffset) {
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
    private List<int> CalculateAllowedColumns(Dataset dataset, int targetVariable, int start, int end) {
      List<int> allowedColumns = new List<int>();
      double n = end - start;
      for (int i = 0; i < dataset.Columns; i++) {
        double nanRatio = CountNaN(dataset, i, start, end) / n;
        if (i != targetVariable && nanRatio < 0.1 && dataset.GetRange(i, start, end) > 0.0) {
          allowedColumns.Add(i);
        }
      }
      return allowedColumns;
    }

    private double CountNaN(Dataset dataset, int column, int start, int end) {
      double n = 0;
      for (int i = start; i < end; i++) {
        if (double.IsNaN(dataset.GetValue(i, column)) || double.IsInfinity(dataset.GetValue(i, column)))
          n++;
      }
      return n;
    }


    private double[,] PrepareInputMatrix(Dataset dataset, List<int> allowedColumns, List<int> allowedRows, int minTimeOffset, int maxTimeOffset) {
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
        matrix[i, allowedColumns.Count * timeOffsetRange] = constant;
      return matrix;
    }

    private double[] PrepareTargetVector(Dataset dataset, int targetVariable, List<int> allowedRows) {
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
