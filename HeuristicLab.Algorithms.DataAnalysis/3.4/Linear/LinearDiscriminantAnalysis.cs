#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Linear discriminant analysis classification algorithm.
  /// </summary>
  [Item("Linear Discriminant Analysis (LDA)", "Linear discriminant analysis classification algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisClassification, Priority = 100)]
  [StorableType("032E1FDE-D140-47BF-8EB1-D63EC33B0629")]
  public sealed class LinearDiscriminantAnalysis : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string LinearDiscriminantAnalysisSolutionResultName = "Linear discriminant analysis solution";

    [StorableConstructor]
    private LinearDiscriminantAnalysis(StorableConstructorFlag _) : base(_) { }
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
    protected override void Run(CancellationToken cancellationToken) {
      var solution = CreateLinearDiscriminantAnalysisSolution(Problem.ProblemData);
      Results.Add(new Result(LinearDiscriminantAnalysisSolutionResultName, "The linear discriminant analysis.", solution));
    }

    public static IClassificationSolution CreateLinearDiscriminantAnalysisSolution(IClassificationProblemData problemData) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      int nClasses = problemData.ClassNames.Count();
      var doubleVariableNames = allowedInputVariables.Where(dataset.VariableHasType<double>).ToArray();
      var factorVariableNames = allowedInputVariables.Where(dataset.VariableHasType<string>).ToArray();
      double[,] inputMatrix = dataset.ToArray(doubleVariableNames.Concat(new string[] { targetVariable }), rows);

      var factorVariables = dataset.GetFactorVariableValues(factorVariableNames, rows);
      var factorMatrix = dataset.ToArray(factorVariables, rows);

      inputMatrix = factorMatrix.HorzCat(inputMatrix);

      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException("Linear discriminant analysis does not support NaN or infinity values in the input dataset.");

      // change class values into class index
      int targetVariableColumn = inputMatrix.GetLength(1) - 1;
      List<double> classValues = problemData.ClassValues.OrderBy(x => x).ToList();
      for (int row = 0; row < inputMatrix.GetLength(0); row++) {
        inputMatrix[row, targetVariableColumn] = classValues.IndexOf(inputMatrix[row, targetVariableColumn]);
      }
      int info;
      double[] w;
      alglib.fisherlda(inputMatrix, inputMatrix.GetLength(0), inputMatrix.GetLength(1) - 1, nClasses, out info, out w);
      if (info < 1) throw new ArgumentException("Error in calculation of linear discriminant analysis solution");

      var nFactorCoeff = factorMatrix.GetLength(1);
      var tree = LinearModelToTreeConverter.CreateTree(factorVariables, w.Take(nFactorCoeff).ToArray(),
        doubleVariableNames, w.Skip(nFactorCoeff).Take(doubleVariableNames.Length).ToArray());

      var model = CreateDiscriminantFunctionModel(tree, new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), problemData, rows);
      SymbolicDiscriminantFunctionClassificationSolution solution = new SymbolicDiscriminantFunctionClassificationSolution(model, (IClassificationProblemData)problemData.Clone());

      return solution;
    }
    #endregion

    private static SymbolicDiscriminantFunctionClassificationModel CreateDiscriminantFunctionModel(ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      IClassificationProblemData problemData,
      IEnumerable<int> rows) {
      var model = new SymbolicDiscriminantFunctionClassificationModel(problemData.TargetVariable, tree, interpreter, new AccuracyMaximizationThresholdCalculator());
      model.RecalculateModelParameters(problemData, rows);
      return model;
    }
  }
}
