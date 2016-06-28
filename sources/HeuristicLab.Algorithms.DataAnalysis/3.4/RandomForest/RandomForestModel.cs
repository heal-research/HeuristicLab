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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a random forest model for regression and classification
  /// </summary>
  [StorableClass]
  [Item("RandomForestModel", "Represents a random forest for regression and classification.")]
  public sealed class RandomForestModel : ClassificationModel, IRandomForestModel {
    // not persisted
    private alglib.decisionforest randomForest;
    private alglib.decisionforest RandomForest {
      get {
        // recalculate lazily
        if (randomForest.innerobj.trees == null || randomForest.innerobj.trees.Length == 0) RecalculateModel();
        return randomForest;
      }
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return originalTrainingData.AllowedInputVariables; }
    }


    // instead of storing the data of the model itself
    // we instead only store data necessary to recalculate the same model lazily on demand
    [Storable]
    private int seed;
    [Storable]
    private IDataAnalysisProblemData originalTrainingData;
    [Storable]
    private double[] classValues;
    [Storable]
    private int nTrees;
    [Storable]
    private double r;
    [Storable]
    private double m;


    [StorableConstructor]
    private RandomForestModel(bool deserializing)
      : base(deserializing) {
      // for backwards compatibility (loading old solutions)
      randomForest = new alglib.decisionforest();
    }
    private RandomForestModel(RandomForestModel original, Cloner cloner)
      : base(original, cloner) {
      randomForest = new alglib.decisionforest();
      randomForest.innerobj.bufsize = original.randomForest.innerobj.bufsize;
      randomForest.innerobj.nclasses = original.randomForest.innerobj.nclasses;
      randomForest.innerobj.ntrees = original.randomForest.innerobj.ntrees;
      randomForest.innerobj.nvars = original.randomForest.innerobj.nvars;
      // we assume that the trees array (double[]) is immutable in alglib
      randomForest.innerobj.trees = original.randomForest.innerobj.trees;

      // allowedInputVariables is immutable so we don't need to clone
      allowedInputVariables = original.allowedInputVariables;

      // clone data which is necessary to rebuild the model
      this.seed = original.seed;
      this.originalTrainingData = cloner.Clone(original.originalTrainingData);
      // classvalues is immutable so we don't need to clone
      this.classValues = original.classValues;
      this.nTrees = original.nTrees;
      this.r = original.r;
      this.m = original.m;
    }

    // random forest models can only be created through the static factory methods CreateRegressionModel and CreateClassificationModel
    private RandomForestModel(string targetVariable, alglib.decisionforest randomForest,
      int seed, IDataAnalysisProblemData originalTrainingData,
      int nTrees, double r, double m, double[] classValues = null)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      // the model itself
      this.randomForest = randomForest;
      // data which is necessary for recalculation of the model
      this.seed = seed;
      this.originalTrainingData = (IDataAnalysisProblemData)originalTrainingData.Clone();
      this.classValues = classValues;
      this.nTrees = nTrees;
      this.r = r;
      this.m = m;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestModel(this, cloner);
    }

    private void RecalculateModel() {
      double rmsError, oobRmsError, relClassError, oobRelClassError;
      var regressionProblemData = originalTrainingData as IRegressionProblemData;
      var classificationProblemData = originalTrainingData as IClassificationProblemData;
      if (regressionProblemData != null) {
        var model = CreateRegressionModel(regressionProblemData,
                                              nTrees, r, m, seed, out rmsError, out oobRmsError,
                                              out relClassError, out oobRelClassError);
        randomForest = model.randomForest;
      } else if (classificationProblemData != null) {
        var model = CreateClassificationModel(classificationProblemData,
                                              nTrees, r, m, seed, out rmsError, out oobRmsError,
                                              out relClassError, out oobRelClassError);
        randomForest = model.randomForest;
      }
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, AllowedInputVariables, rows);
      AssertInputMatrix(inputData);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.dfprocess(RandomForest, x, ref y);
        yield return y[0];
      }
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, AllowedInputVariables, rows);
      AssertInputMatrix(inputData);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[RandomForest.innerobj.nclasses];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.dfprocess(randomForest, x, ref y);
        // find class for with the largest probability value
        int maxProbClassIndex = 0;
        double maxProb = y[0];
        for (int i = 1; i < y.Length; i++) {
          if (maxProb < y[i]) {
            maxProb = y[i];
            maxProbClassIndex = i;
          }
        }
        yield return classValues[maxProbClassIndex];
      }
    }


    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RandomForestRegressionSolution(this, new RegressionProblemData(problemData));
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new RandomForestClassificationSolution(this, new ClassificationProblemData(problemData));
    }

    public static RandomForestModel CreateRegressionModel(IRegressionProblemData problemData, int nTrees, double r, double m, int seed,
      out double rmsError, out double outOfBagRmsError, out double avgRelError, out double outOfBagAvgRelError) {
      return CreateRegressionModel(problemData, problemData.TrainingIndices, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagAvgRelError, out outOfBagRmsError);
    }

    public static RandomForestModel CreateRegressionModel(IRegressionProblemData problemData, IEnumerable<int> trainingIndices, int nTrees, double r, double m, int seed,
      out double rmsError, out double outOfBagRmsError, out double avgRelError, out double outOfBagAvgRelError) {
      var variables = problemData.AllowedInputVariables.Concat(new string[] { problemData.TargetVariable });
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(problemData.Dataset, variables, trainingIndices);

      alglib.dfreport rep;
      var dForest = CreateRandomForestModel(seed, inputMatrix, nTrees, r, m, 1, out rep);

      rmsError = rep.rmserror;
      avgRelError = rep.avgrelerror;
      outOfBagAvgRelError = rep.oobavgrelerror;
      outOfBagRmsError = rep.oobrmserror;

      return new RandomForestModel(problemData.TargetVariable, dForest, seed, problemData, nTrees, r, m);
    }

    public static RandomForestModel CreateClassificationModel(IClassificationProblemData problemData, int nTrees, double r, double m, int seed,
      out double rmsError, out double outOfBagRmsError, out double relClassificationError, out double outOfBagRelClassificationError) {
      return CreateClassificationModel(problemData, problemData.TrainingIndices, nTrees, r, m, seed, out rmsError, out outOfBagRmsError, out relClassificationError, out outOfBagRelClassificationError);
    }

    public static RandomForestModel CreateClassificationModel(IClassificationProblemData problemData, IEnumerable<int> trainingIndices, int nTrees, double r, double m, int seed,
      out double rmsError, out double outOfBagRmsError, out double relClassificationError, out double outOfBagRelClassificationError) {

      var variables = problemData.AllowedInputVariables.Concat(new string[] { problemData.TargetVariable });
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(problemData.Dataset, variables, trainingIndices);

      var classValues = problemData.ClassValues.ToArray();
      int nClasses = classValues.Length;

      // map original class values to values [0..nClasses-1]
      var classIndices = new Dictionary<double, double>();
      for (int i = 0; i < nClasses; i++) {
        classIndices[classValues[i]] = i;
      }

      int nRows = inputMatrix.GetLength(0);
      int nColumns = inputMatrix.GetLength(1);
      for (int row = 0; row < nRows; row++) {
        inputMatrix[row, nColumns - 1] = classIndices[inputMatrix[row, nColumns - 1]];
      }

      alglib.dfreport rep;
      var dForest = CreateRandomForestModel(seed, inputMatrix, nTrees, r, m, nClasses, out rep);

      rmsError = rep.rmserror;
      outOfBagRmsError = rep.oobrmserror;
      relClassificationError = rep.relclserror;
      outOfBagRelClassificationError = rep.oobrelclserror;

      return new RandomForestModel(problemData.TargetVariable, dForest, seed, problemData, nTrees, r, m, classValues);
    }

    private static alglib.decisionforest CreateRandomForestModel(int seed, double[,] inputMatrix, int nTrees, double r, double m, int nClasses, out alglib.dfreport rep) {
      AssertParameters(r, m);
      AssertInputMatrix(inputMatrix);

      int info = 0;
      alglib.math.rndobject = new System.Random(seed);
      var dForest = new alglib.decisionforest();
      rep = new alglib.dfreport();
      int nRows = inputMatrix.GetLength(0);
      int nColumns = inputMatrix.GetLength(1);
      int sampleSize = Math.Max((int)Math.Round(r * nRows), 1);
      int nFeatures = Math.Max((int)Math.Round(m * (nColumns - 1)), 1);

      alglib.dforest.dfbuildinternal(inputMatrix, nRows, nColumns - 1, nClasses, nTrees, sampleSize, nFeatures, alglib.dforest.dfusestrongsplits + alglib.dforest.dfuseevs, ref info, dForest.innerobj, rep.innerobj);
      if (info != 1) throw new ArgumentException("Error in calculation of random forest model");
      return dForest;
    }

    private static void AssertParameters(double r, double m) {
      if (r <= 0 || r > 1) throw new ArgumentException("The R parameter for random forest modeling must be between 0 and 1.");
      if (m <= 0 || m > 1) throw new ArgumentException("The M parameter for random forest modeling must be between 0 and 1.");
    }

    private static void AssertInputMatrix(double[,] inputMatrix) {
      if (inputMatrix.Cast<double>().Any(x => Double.IsNaN(x) || Double.IsInfinity(x)))
        throw new NotSupportedException("Random forest modeling does not support NaN or infinity values in the input dataset.");
    }

    #region persistence for backwards compatibility
    // when the originalTrainingData is null this means the model was loaded from an old file 
    // therefore, we cannot use the new persistence mechanism because the original data is not available anymore
    // in such cases we still store the compete model
    private bool IsCompatibilityLoaded { get { return originalTrainingData == null; } }

    private string[] allowedInputVariables;
    [Storable(Name = "allowedInputVariables")]
    private string[] AllowedInputVariables {
      get {
        if (IsCompatibilityLoaded) return allowedInputVariables;
        else return originalTrainingData.AllowedInputVariables.ToArray();
      }
      set { allowedInputVariables = value; }
    }
    [Storable]
    private int RandomForestBufSize {
      get {
        if (IsCompatibilityLoaded) return randomForest.innerobj.bufsize;
        else return 0;
      }
      set {
        randomForest.innerobj.bufsize = value;
      }
    }
    [Storable]
    private int RandomForestNClasses {
      get {
        if (IsCompatibilityLoaded) return randomForest.innerobj.nclasses;
        else return 0;
      }
      set {
        randomForest.innerobj.nclasses = value;
      }
    }
    [Storable]
    private int RandomForestNTrees {
      get {
        if (IsCompatibilityLoaded) return randomForest.innerobj.ntrees;
        else return 0;
      }
      set {
        randomForest.innerobj.ntrees = value;
      }
    }
    [Storable]
    private int RandomForestNVars {
      get {
        if (IsCompatibilityLoaded) return randomForest.innerobj.nvars;
        else return 0;
      }
      set {
        randomForest.innerobj.nvars = value;
      }
    }
    [Storable]
    private double[] RandomForestTrees {
      get {
        if (IsCompatibilityLoaded) return randomForest.innerobj.trees;
        else return new double[] { };
      }
      set {
        randomForest.innerobj.trees = value;
      }
    }
    #endregion
  }
}
