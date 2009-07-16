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
using HeuristicLab.GP;
using HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.LinearRegression {
  public class LinearRegressionOperator : OperatorBase {
    private static double constant = 1.0;

    public LinearRegressionOperator() {
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LinearRegressionModel", "Formula that was calculated by linear regression", typeof(IFunctionTree), VariableKind.Out | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      List<int> allowedRows = CalculateAllowedRows(dataset, targetVariable, start, end);
      List<int> allowedColumns = CalculateAllowedColumns(dataset, targetVariable, start, end);

      double[,] inputMatrix = PrepareInputMatrix(dataset, allowedColumns, allowedRows);
      double[] targetVector = PrepareTargetVector(dataset, targetVariable, allowedRows);
      double[] coefficients = CalculateCoefficients(inputMatrix, targetVector);
      IFunctionTree tree = CreateModel(coefficients, allowedColumns);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("LinearRegressionModel"), tree));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(tree.Size)));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(tree.Height)));
      return null;
    }

    private bool IsAlmost(double x, double y) {
      return Math.Abs(x - y) < 1.0E-12;
    }

    private IFunctionTree CreateModel(double[] coefficients, List<int> allowedColumns) {
      IFunctionTree root = new Addition().GetTreeNode();
      IFunctionTree actNode = root;

      Queue<IFunctionTree> nodes = new Queue<IFunctionTree>();
      GP.StructureIdentification.Variable v;
      for (int i = 0; i < coefficients.Length - 1; i++) {
        v = new GP.StructureIdentification.Variable();
        v.GetVariable(GP.StructureIdentification.Variable.INDEX).Value = new ConstrainedIntData(allowedColumns[i]);
        v.GetVariable(GP.StructureIdentification.Variable.WEIGHT).Value = new ConstrainedDoubleData(coefficients[i]);
        v.GetVariable(GP.StructureIdentification.Variable.OFFSET).Value = new ConstrainedIntData(0);
        nodes.Enqueue(v.GetTreeNode());
      }
      GP.StructureIdentification.Constant c = new Constant();
      c.GetVariable(GP.StructureIdentification.Constant.VALUE).Value = new ConstrainedDoubleData(coefficients[coefficients.Length - 1]);
      nodes.Enqueue(c.GetTreeNode());

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
    private List<int> CalculateAllowedRows(Dataset dataset, int targetVariable, int start, int end) {
      List<int> allowedRows = new List<int>();
      bool add;
      for (int row = start; row < end; row++) {
        add = true;
        for (int col = 0; col < dataset.Columns && add == true; col++) {
          if (double.IsNaN(dataset.GetValue(row, col)) ||
              double.IsNaN(dataset.GetValue(row, targetVariable)))
            add = false;
        }
        if (add)
          allowedRows.Add(row);
        add = true;
      }
      return allowedRows;
    }

    //returns list of valid column indexes (columns which contain at least one non-zero value)
    private List<int> CalculateAllowedColumns(Dataset dataset, int targetVariable, int start, int end) {
      List<int> allowedColumns = new List<int>();
      for (int i = 0; i < dataset.Columns; i++) {
        if (i == targetVariable) continue;
        if (!IsAlmost(dataset.GetMinimum(i, start, end), 0.0) ||
            !IsAlmost(dataset.GetMaximum(i, start, end), 0.0))
          allowedColumns.Add(i);
      }
      return allowedColumns;
    }

    private double[,] PrepareInputMatrix(Dataset dataset, List<int> allowedColumns, List<int> allowedRows) {
      int rowCount = allowedRows.Count;
      double[,] matrix = new double[rowCount, allowedColumns.Count + 1];
      for (int col = 0; col < allowedColumns.Count; col++) {
        for (int row = 0; row < allowedRows.Count; row++)
          matrix[row, col] = dataset.GetValue(allowedRows[row], allowedColumns[col]);
      }
      //add constant 1.0 in last column
      for (int i = 0; i < rowCount; i++)
        matrix[i, allowedColumns.Count] = constant;
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
