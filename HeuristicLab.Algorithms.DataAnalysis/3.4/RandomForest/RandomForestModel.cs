#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class RandomForestModel : NamedItem, IRandomForestModel {

    private alglib.decisionforest randomForest;
    public alglib.decisionforest RandomForest {
      get { return randomForest; }
      set {
        if (value != randomForest) {
          if (value == null) throw new ArgumentNullException();
          randomForest = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private RandomForestModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        randomForest = new alglib.decisionforest();
    }
    private RandomForestModel(RandomForestModel original, Cloner cloner)
      : base(original, cloner) {
      randomForest = new alglib.decisionforest();
      randomForest.innerobj.bufsize = original.randomForest.innerobj.bufsize;
      randomForest.innerobj.nclasses = original.randomForest.innerobj.nclasses;
      randomForest.innerobj.ntrees = original.randomForest.innerobj.ntrees;
      randomForest.innerobj.nvars = original.randomForest.innerobj.nvars;
      randomForest.innerobj.trees = (double[])original.randomForest.innerobj.trees.Clone();
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public RandomForestModel(alglib.decisionforest randomForest, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.randomForest = randomForest;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (classValues != null)
        this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.dfprocess(randomForest, x, ref y);
        yield return y[0];
      }
    }

    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[randomForest.innerobj.nclasses];

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

    public IRandomForestRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RandomForestRegressionSolution(new RegressionProblemData(problemData), this);
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    public IRandomForestClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new RandomForestClassificationSolution(new ClassificationProblemData(problemData), this);
    }
    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }

    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion

    #region persistence
    [Storable]
    private int RandomForestBufSize {
      get {
        return randomForest.innerobj.bufsize;
      }
      set {
        randomForest.innerobj.bufsize = value;
      }
    }
    [Storable]
    private int RandomForestNClasses {
      get {
        return randomForest.innerobj.nclasses;
      }
      set {
        randomForest.innerobj.nclasses = value;
      }
    }
    [Storable]
    private int RandomForestNTrees {
      get {
        return randomForest.innerobj.ntrees;
      }
      set {
        randomForest.innerobj.ntrees = value;
      }
    }
    [Storable]
    private int RandomForestNVars {
      get {
        return randomForest.innerobj.nvars;
      }
      set {
        randomForest.innerobj.nvars = value;
      }
    }
    [Storable]
    private double[] RandomForestTrees {
      get {
        return randomForest.innerobj.trees;
      }
      set {
        randomForest.innerobj.trees = value;
      }
    }
    #endregion
  }
}
