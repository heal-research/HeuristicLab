#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Linear discriminant analysis classification algorithm.
  /// </summary>
  [Item("Linear Discriminant Analysis", "Linear discriminant analysis classification algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class LinearDiscriminantAnalysis : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string LinearDiscriminantAnalysisSolutionResultName = "Linear discriminant analysis solution";

    [StorableConstructor]
    private LinearDiscriminantAnalysis(bool deserializing) : base(deserializing) { }
    private LinearDiscriminantAnalysis(LinearDiscriminantAnalysis original, Cloner cloner)
      : base(original, cloner) {
    }
    public LinearDiscriminantAnalysis()
      : base() {
      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearDiscriminantAnalysis(this, cloner);
    }

    #region Fisher LDA
    protected override void Run() {
      var solution = CreateLinearDiscriminantAnalysisSolution(Problem.ProblemData);
      Results.Add(new Result(LinearDiscriminantAnalysisSolutionResultName, "The linear discriminant analysis.", solution));
    }

    public static IClassificationSolution CreateLinearDiscriminantAnalysisSolution(IClassificationProblemData problemData) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      int nClasses = problemData.ClassNames.Count();
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Linear discriminant analysis does not support NaN or infinity values in the input dataset.");

      // change class values into class index
      int targetVariableColumn = inputMatrix.GetLength(1) - 1;
      List<double> classValues = problemData.ClassValues.OrderBy(x => x).ToList();
      for (int row = 0; row < inputMatrix.GetLength(0); row++) {
        inputMatrix[row, targetVariableColumn] = classValues.IndexOf(inputMatrix[row, targetVariableColumn]);
      }
      int info;
      double[] w;
      alglib.fisherlda(inputMatrix, inputMatrix.GetLength(0), allowedInputVariables.Count(), nClasses, out info, out w);
      if (info < 1) throw new ArgumentException("Error in calculation of linear discriminant analysis solution");

      ISymbolicExpressionTree tree = new SymbolicExpressionTree(new ProgramRootSymbol().CreateTreeNode());
      ISymbolicExpressionTreeNode startNode = new StartSymbol().CreateTreeNode();
      tree.Root.AddSubtree(startNode);
      ISymbolicExpressionTreeNode addition = new Addition().CreateTreeNode();
      startNode.AddSubtree(addition);

      int col = 0;
      foreach (string column in allowedInputVariables) {
        VariableTreeNode vNode = (VariableTreeNode)new HeuristicLab.Problems.DataAnalysis.Symbolic.Variable().CreateTreeNode();
        vNode.VariableName = column;
        vNode.Weight = w[col];
        addition.AddSubtree(vNode);
        col++;
      }

      var model = LinearDiscriminantAnalysis.CreateDiscriminantFunctionModel(tree, new SymbolicDataAnalysisExpressionTreeInterpreter(), problemData, rows);
      SymbolicDiscriminantFunctionClassificationSolution solution = new SymbolicDiscriminantFunctionClassificationSolution(model, (IClassificationProblemData)problemData.Clone());

      return solution;
    }
    #endregion

    private static SymbolicDiscriminantFunctionClassificationModel CreateDiscriminantFunctionModel(ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      IClassificationProblemData problemData,
      IEnumerable<int> rows) {
      var model = new SymbolicDiscriminantFunctionClassificationModel(tree, interpreter, new AccuracyMaximizationThresholdCalculator());
      model.RecalculateModelParameters(problemData, rows);
      return model;
    }
  }
}
