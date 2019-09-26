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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("A4F688CD-1F42-4103-8449-7DE52AEF6C69")]
  [Item("RandomForestModelSurrogate", "Represents a random forest for regression and classification.")]
  public sealed class RandomForestModelSurrogate : ClassificationModel, IRandomForestModel {

    #region parameters for recalculation of the model
    [Storable]
    private readonly int seed;
    [Storable]
    private readonly IDataAnalysisProblemData originalTrainingData;
    [Storable]
    private readonly double[] classValues;
    [Storable]
    private readonly int nTrees;
    [Storable]
    private readonly double r;
    [Storable]
    private readonly double m;
    #endregion


    // don't store the actual model!
    // the actual model is only recalculated when necessary
    private IRandomForestModel fullModel = null;
    private readonly Lazy<IRandomForestModel> actualModel;

    private IRandomForestModel ActualModel {
      get { return actualModel.Value; }
    }

    public int NumberOfTrees => ActualModel.NumberOfTrees;
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return ActualModel.VariablesUsedForPrediction; }
    }

    public RandomForestModelSurrogate(string targetVariable, IDataAnalysisProblemData originalTrainingData,
      int seed, int nTrees, double r, double m, double[] classValues = null)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;

      // data which is necessary for recalculation of the model
      this.seed = seed;
      this.originalTrainingData = (IDataAnalysisProblemData)originalTrainingData.Clone();
      this.classValues = classValues;
      this.nTrees = nTrees;
      this.r = r;
      this.m = m;

      actualModel = CreateLazyInitFunc();
    }

    // wrap an actual model in a surrogate
    public RandomForestModelSurrogate(IRandomForestModel model, string targetVariable, IDataAnalysisProblemData originalTrainingData,
      int seed, int nTrees, double r, double m, double[] classValues = null) : this(targetVariable, originalTrainingData, seed, nTrees, r, m, classValues) {
      fullModel = model;
      actualModel = CreateLazyInitFunc();
    }

    [StorableConstructor]
    private RandomForestModelSurrogate(StorableConstructorFlag _) : base(_) {
      actualModel = CreateLazyInitFunc();
    }

    private RandomForestModelSurrogate(RandomForestModelSurrogate original, Cloner cloner) : base(original, cloner) {
      // clone data which is necessary to rebuild the model
      this.originalTrainingData = cloner.Clone(original.originalTrainingData);
      this.seed = original.seed;
      this.classValues = original.classValues;
      this.nTrees = original.nTrees;
      this.r = original.r;
      this.m = original.m;

      // clone full model if it has already been created
      if (original.fullModel != null) this.fullModel = cloner.Clone(original.fullModel);
      actualModel = CreateLazyInitFunc();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestModelSurrogate(this, cloner);
    }

    private Lazy<IRandomForestModel> CreateLazyInitFunc() {
      return new Lazy<IRandomForestModel>(() => {
        if (fullModel == null) fullModel = RecalculateModel();
        return fullModel;
      });
    }

    private IRandomForestModel RecalculateModel() {
      IRandomForestModel randomForestModel = null;

      double rmsError, oobRmsError, relClassError, oobRelClassError;
      var classificationProblemData = originalTrainingData as IClassificationProblemData;

      if (originalTrainingData is IRegressionProblemData regressionProblemData) {
        randomForestModel = RandomForestRegression.CreateRandomForestRegressionModel(regressionProblemData,
                                              nTrees, r, m, seed, out rmsError, out oobRmsError,
                                              out relClassError, out oobRelClassError);
      } else if (classificationProblemData != null) {
        randomForestModel = RandomForestClassification.CreateRandomForestClassificationModel(classificationProblemData,
                                              nTrees, r, m, seed, out rmsError, out oobRmsError,
                                              out relClassError, out oobRelClassError);
      }
      return randomForestModel;
    }

    //RegressionModel methods
    public bool IsProblemDataCompatible(IRegressionProblemData problemData, out string errorMessage) {
      return ActualModel.IsProblemDataCompatible(problemData, out errorMessage);
    }
    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return ActualModel.GetEstimatedValues(dataset, rows);
    }
    public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
      return ActualModel.GetEstimatedVariances(dataset, rows);
    }
    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RandomForestRegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }

    //ClassificationModel methods
    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      return ActualModel.GetEstimatedClassValues(dataset, rows);
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new RandomForestClassificationSolution(this, (IClassificationProblemData)problemData.Clone());
    }

    public ISymbolicExpressionTree ExtractTree(int treeIdx) {
      return ActualModel.ExtractTree(treeIdx);
    }
  }
}