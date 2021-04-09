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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a nearest neighbour model for regression and classification
  /// </summary>
  [StorableType("04A07DF6-6EB5-4D29-B7AE-5BE204CAF6BC")]
  [Item("NearestNeighbourModel", "Represents a nearest neighbour model for regression and classification.")]
  public sealed class NearestNeighbourModel : ClassificationModel, INearestNeighbourModel {

    private alglib.knnmodel model;
    [Storable]
    private string SerializedModel {
      get { alglib.knnserialize(model, out var ser); return ser; }
      set { if (value != null) alglib.knnunserialize(value, out model); }
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [Storable]
    private int k;
    [Storable]
    private double[] weights;
    [Storable]
    private double[] offsets;

    [StorableConstructor]
    private NearestNeighbourModel(StorableConstructorFlag _) : base(_) { }
    private NearestNeighbourModel(NearestNeighbourModel original, Cloner cloner)
      : base(original, cloner) {
      if (original.model != null)
        model = (alglib.knnmodel)original.model.make_copy();
      k = original.k;
      weights = new double[original.weights.Length];
      Array.Copy(original.weights, weights, weights.Length);
      offsets = new double[original.offsets.Length];
      Array.Copy(original.offsets, this.offsets, this.offsets.Length);

      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NearestNeighbourModel(IDataset dataset, IEnumerable<int> rows, int k, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<double> weights = null, double[] classValues = null)
      : base(targetVariable) {
      Name = ItemName;
      Description = ItemDescription;
      this.k = k;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      double[,] inputMatrix;
      this.offsets = this.allowedInputVariables
        .Select(name => dataset.GetDoubleValues(name, rows).Average() * -1)
        .Concat(new double[] { 0 }) // no offset for target variable
        .ToArray();
      if (weights == null) {
        // automatic determination of weights (all features should have variance = 1)
        this.weights = this.allowedInputVariables
          .Select(name => {
            var pop = dataset.GetDoubleValues(name, rows).StandardDeviationPop();
            return pop.IsAlmost(0) ? 1.0 : 1.0 / pop;
          })
          .Concat(new double[] { 1.0 }) // no scaling for target variable
          .ToArray();
      } else {
        // user specified weights (+ 1 for target)
        this.weights = weights.Concat(new double[] { 1.0 }).ToArray();
        if (this.weights.Length - 1 != this.allowedInputVariables.Length)
          throw new ArgumentException("The number of elements in the weight vector must match the number of input variables");
      }
      inputMatrix = CreateScaledData(dataset, this.allowedInputVariables.Concat(new string[] { targetVariable }), rows, this.offsets, this.weights);

      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException(
          "Nearest neighbour model does not support NaN or infinity values in the input dataset.");

      var nRows = inputMatrix.GetLength(0);
      var nFeatures = inputMatrix.GetLength(1) - 1;

      if (classValues != null) {
        this.classValues = (double[])classValues.Clone();
        int nClasses = classValues.Length;
        // map original class values to values [0..nClasses-1]
        var classIndices = new Dictionary<double, double>();
        for (int i = 0; i < nClasses; i++)
          classIndices[classValues[i]] = i;

        for (int row = 0; row < nRows; row++) {
          inputMatrix[row, nFeatures] = classIndices[inputMatrix[row, nFeatures]];
        }
      }

      alglib.knnbuildercreate(out var knnbuilder);
      if (classValues == null) {
        alglib.knnbuildersetdatasetreg(knnbuilder, inputMatrix, nRows, nFeatures, nout: 1);
      } else {
        alglib.knnbuildersetdatasetcls(knnbuilder, inputMatrix, nRows, nFeatures, classValues.Length);
      }
      alglib.knnbuilderbuildknnmodel(knnbuilder, k, eps: 0.0, out model, out var report); // eps=0 (exact k-nn search is performed)

    }

    private static double[,] CreateScaledData(IDataset dataset, IEnumerable<string> variables, IEnumerable<int> rows, double[] offsets, double[] factors) {
      var transforms =
        variables.Select(
          (_, colIdx) =>
            new LinearTransformation(variables) { Addend = offsets[colIdx] * factors[colIdx], Multiplier = factors[colIdx] });
      return dataset.ToArray(variables, transforms, rows);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData;
      inputData = CreateScaledData(dataset, allowedInputVariables, rows, offsets, weights);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];

      alglib.knncreatebuffer(model, out var buf);
      var y = new double[1];
      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.knntsprocess(model, buf, x, ref y); // thread-safe process
        yield return y[0];
      }
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      if (classValues == null) throw new InvalidOperationException("No class values are defined.");
      double[,] inputData;
      inputData = CreateScaledData(dataset, allowedInputVariables, rows, offsets, weights);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];

      alglib.knncreatebuffer(model, out var buf);
      var y = new double[classValues.Length];
      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.knntsprocess(model, buf, x, ref y); // thread-safe process
        // find most probably class
        var maxC = 0;
        for (int i = 1; i < y.Length; i++)
          if (maxC < y[i]) maxC = i;
        yield return classValues[maxC];
      }
    }


    public bool IsProblemDataCompatible(IRegressionProblemData problemData, out string errorMessage) {
      return RegressionModel.IsProblemDataCompatible(this, problemData, out errorMessage);
    }

    public override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");

      var regressionProblemData = problemData as IRegressionProblemData;
      if (regressionProblemData != null)
        return IsProblemDataCompatible(regressionProblemData, out errorMessage);

      var classificationProblemData = problemData as IClassificationProblemData;
      if (classificationProblemData != null)
        return IsProblemDataCompatible(classificationProblemData, out errorMessage);

      throw new ArgumentException("The problem data is not compatible with this nearest neighbour model. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
    }

    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NearestNeighbourRegressionSolution(this, new RegressionProblemData(problemData));
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NearestNeighbourClassificationSolution(this, new ClassificationProblemData(problemData));
    }
  }
}
