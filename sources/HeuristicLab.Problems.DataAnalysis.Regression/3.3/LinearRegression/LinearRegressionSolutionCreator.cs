#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Regression.LinearRegression {
  /// <summary>
  /// A base class for operators which evaluates OneMax solutions given in BinaryVector encoding.
  /// </summary>
  [Item("LinearRegressionSolutionCreator", "Uses linear regression to create a structure tree.")]
  [StorableClass]
  public class LinearRegressionSolutionCreator : SingleSuccessorOperator, ISolutionCreator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";

    public LinearRegressionSolutionCreator() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The resulting solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The problem data on which the linear regression should be calculated."));
    }
    [StorableConstructor]
    public LinearRegressionSolutionCreator(bool deserializing)
      : base(deserializing) {
    }

    #region parameter properties
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
      set { SymbolicExpressionTreeParameter.ActualValue = value; }
    }

    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
      set { DataAnalysisProblemDataParameter.ActualValue = value; }
    }
    #endregion


    public override IOperation Apply() {
      SymbolicExpressionTree = CreateSymbolicExpressionTree(DataAnalysisProblemData);
      return null;
    }

    public static SymbolicExpressionTree CreateSymbolicExpressionTree(DataAnalysisProblemData problem) {
      List<int> allowedRows = CalculateAllowedRows(problem);
      double[,] inputMatrix = PrepareInputMatrix(problem, allowedRows);

      alglib.linreg.linearmodel lm = new alglib.linreg.linearmodel();
      alglib.linreg.lrreport ar = new alglib.linreg.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) -1;
      double[] coefficients = new double[nFeatures +1]; //last coefficient is for the constant

      int retVal = 1;
      alglib.linreg.lrbuild(ref inputMatrix, nRows, nFeatures, ref retVal, ref lm, ref ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression model");

      for (int i = 0; i < nFeatures+1; i++)
        coefficients[i] = lm.w[i + 4];

      SymbolicExpressionTree tree = new SymbolicExpressionTree(new ProgramRootSymbol().CreateTreeNode());
      SymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      tree.Root.AddSubTree(start);
      SymbolicExpressionTreeNode addition = new Addition().CreateTreeNode();
      start.AddSubTree(addition);

      int col = 0;
      foreach (string column in problem.InputVariables.CheckedItems.Select(c => c.Value.Value)) {
        VariableTreeNode vNode = (VariableTreeNode)new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable().CreateTreeNode();
        vNode.VariableName = column;
        vNode.Weight = coefficients[col];
        addition.AddSubTree(vNode);
        col++;
      }

      ConstantTreeNode cNode = (ConstantTreeNode)new Constant().CreateTreeNode();
      cNode.Value = coefficients[coefficients.Length - 1];

      return tree;
    }

    private static List<int> CalculateAllowedRows(DataAnalysisProblemData problem) {
      List<int> allowedRows = new List<int>();
      bool add = false;

      for (int row = problem.TrainingSamplesStart.Value; row < problem.TrainingSamplesEnd.Value; row++) {
        add = true;
        foreach (string column in problem.InputVariables.CheckedItems.Select(c => c.Value.Value)) {
          double value = problem.Dataset[column, row];
          if (double.IsInfinity(value) ||
            double.IsNaN(value))
            add = false;
        }
        if (double.IsNaN(problem.Dataset[problem.TargetVariable.Value, row]))
          add = false;
        if (add)
          allowedRows.Add(row);
        add = true;
      }
      return allowedRows;
    }
    private static double[,] PrepareInputMatrix(DataAnalysisProblemData problem, IList<int> allowedRows) {
      double[,] matrix = new double[allowedRows.Count, problem.InputVariables.CheckedItems.Count()+1];
      for (int row = 0; row < allowedRows.Count; row++) {
        int col = 0;
        foreach (string column in problem.InputVariables.CheckedItems.Select(c => c.Value.Value)) {
          matrix[row, col] = problem.Dataset[column, row];
          col++;
        }
        matrix[row, problem.InputVariables.CheckedItems.Count()] = problem.Dataset[problem.TargetVariable.Value, row];
      }
      return matrix;
    }
  }
}
