#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents discriminant function classification data analysis models.
  /// </summary>
  [StorableType("E7A8648D-C938-499F-A712-185542095708")]
  [Item("DiscriminantFunctionClassificationModel", "Represents a classification model that uses a discriminant function and classification thresholds.")]
  public class DiscriminantFunctionClassificationModel : ClassificationModel, IDiscriminantFunctionClassificationModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return model.VariablesUsedForPrediction; }
    }

    [Storable]
    private IRegressionModel model;
    public IRegressionModel Model {
      get { return model; }
      private set { model = value; }
    }

    [Storable]
    private double[] classValues;
    public IEnumerable<double> ClassValues {
      get { return (double[])classValues.Clone(); }
      private set { classValues = value.ToArray(); }
    }

    [Storable]
    private double[] thresholds;
    public IEnumerable<double> Thresholds {
      get { return (IEnumerable<double>)thresholds.Clone(); }
      private set { thresholds = value.ToArray(); }
    }

    private IDiscriminantFunctionThresholdCalculator thresholdCalculator;
    [Storable]
    public IDiscriminantFunctionThresholdCalculator ThresholdCalculator {
      get { return thresholdCalculator; }
      private set { thresholdCalculator = value; }
    }


    [StorableConstructor]
    protected DiscriminantFunctionClassificationModel(StorableConstructorFlag _) : base(_) { }
    protected DiscriminantFunctionClassificationModel(DiscriminantFunctionClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      model = cloner.Clone(original.model);
      classValues = (double[])original.classValues.Clone();
      thresholds = (double[])original.thresholds.Clone();
    }

    public DiscriminantFunctionClassificationModel(IRegressionModel model, IDiscriminantFunctionThresholdCalculator thresholdCalculator)
      : base(model.TargetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;

      this.model = model;
      this.classValues = new double[0];
      this.thresholds = new double[0];
      this.thresholdCalculator = thresholdCalculator;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (ThresholdCalculator == null) ThresholdCalculator = new AccuracyMaximizationThresholdCalculator();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DiscriminantFunctionClassificationModel(this, cloner);
    }

    public void SetThresholdsAndClassValues(IEnumerable<double> thresholds, IEnumerable<double> classValues) {
      var classValuesArr = classValues.ToArray();
      var thresholdsArr = thresholds.ToArray();
      if (thresholdsArr.Length != classValuesArr.Length) throw new ArgumentException();

      this.classValues = classValuesArr;
      this.thresholds = thresholdsArr;
      OnThresholdsChanged(EventArgs.Empty);
    }

    public virtual void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows) {
      double[] classValues;
      double[] thresholds;
      var targetClassValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var estimatedTrainingValues = GetEstimatedValues(problemData.Dataset, rows);
      thresholdCalculator.Calculate(problemData, estimatedTrainingValues, targetClassValues, out classValues, out thresholds);
      SetThresholdsAndClassValues(thresholds, classValues);
    }


    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return model.GetEstimatedValues(dataset, rows);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      if (!Thresholds.Any() && !ClassValues.Any()) throw new ArgumentException("No thresholds and class values were set for the current classification model.");
      foreach (var x in GetEstimatedValues(dataset, rows)) {
        int classIndex = 0;
        // find first threshold value which is larger than x => class index = threshold index + 1
        for (int i = 0; i < thresholds.Length; i++) {
          if (x > thresholds[i]) classIndex++;
          else break;
        }
        yield return classValues.ElementAt(classIndex - 1);
      }
    }
    #region events
    public event EventHandler ThresholdsChanged;
    protected virtual void OnThresholdsChanged(EventArgs e) {
      var listener = ThresholdsChanged;
      if (listener != null) listener(this, e);
    }
    #endregion

    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateDiscriminantFunctionClassificationSolution(problemData);
    }
    public virtual IDiscriminantFunctionClassificationSolution CreateDiscriminantFunctionClassificationSolution(
      IClassificationProblemData problemData) {
      return new DiscriminantFunctionClassificationSolution(this, new ClassificationProblemData(problemData));
    }
  }
}
