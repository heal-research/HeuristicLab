#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.LinearRegression {
  /// <summary>
  /// A base class for operators which evaluates OneMax solutions given in BinaryVector encoding.
  /// </summary>
  [Item("LinearRegressionSolutionCreator", "Uses linear regression to create a structure tree.")]
  [StorableClass]
  public sealed class LinearRegressionSolutionCreator : SingleSuccessorOperator, ISolutionCreator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";

    [StorableConstructor]
    private LinearRegressionSolutionCreator(bool deserializing) : base(deserializing) { }
    private LinearRegressionSolutionCreator(LinearRegressionSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public LinearRegressionSolutionCreator() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The resulting solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The problem data on which the linear regression should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The start of the samples on which the linear regression should be applied."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The end of the samples on which the linear regression should be applied."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearRegressionSolutionCreator(this, cloner);
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

    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
      set { SamplesStartParameter.ActualValue = value; }
    }

    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
      set { SamplesEndParameter.ActualValue = value; }
    }
    #endregion


    public override IOperation Apply() {
      double rmsError, cvRmsError;
      SymbolicExpressionTree = CreateSymbolicExpressionTree(DataAnalysisProblemData.Dataset, DataAnalysisProblemData.TargetVariable.Value, DataAnalysisProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value), SamplesStart.Value, SamplesEnd.Value, out rmsError, out cvRmsError);
      return base.Apply();
    }

    public static SymbolicExpressionTree CreateSymbolicExpressionTree(Dataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, int start, int end, out double rmsError, out double cvRmsError) {
      double[,] inputMatrix = LinearRegressionUtil.PrepareInputMatrix(dataset, targetVariable, allowedInputVariables, start, end);

      alglib.linreg.linearmodel lm = new alglib.linreg.linearmodel();
      alglib.linreg.lrreport ar = new alglib.linreg.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;
      double[] coefficients = new double[nFeatures + 1]; //last coefficient is for the constant

      int retVal = 1;
      alglib.linreg.lrbuild(inputMatrix, nRows, nFeatures, ref retVal, lm, ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression model");
      rmsError = ar.rmserror;
      cvRmsError = ar.cvrmserror;

      for (int i = 0; i < nFeatures + 1; i++)
        coefficients[i] = lm.w[i + 4];

      SymbolicExpressionTree tree = new SymbolicExpressionTree(new ProgramRootSymbol().CreateTreeNode());
      SymbolicExpressionTreeNode startNode = new StartSymbol().CreateTreeNode();
      tree.Root.AddSubTree(startNode);
      SymbolicExpressionTreeNode addition = new Addition().CreateTreeNode();
      startNode.AddSubTree(addition);

      int col = 0;
      foreach (string column in allowedInputVariables) {
        VariableTreeNode vNode = (VariableTreeNode)new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable().CreateTreeNode();
        vNode.VariableName = column;
        vNode.Weight = coefficients[col];
        addition.AddSubTree(vNode);
        col++;
      }

      ConstantTreeNode cNode = (ConstantTreeNode)new Constant().CreateTreeNode();
      cNode.Value = coefficients[coefficients.Length - 1];
      addition.AddSubTree(cNode);

      return tree;
    }
  }
}
