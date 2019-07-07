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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a symbolic classification model
  /// </summary>
  [StorableType("99332204-4097-496A-AB05-4DB9478DB159")]
  [Item(Name = "SymbolicDiscriminantFunctionClassificationModel", Description = "Represents a symbolic classification model unsing a discriminant function.")]
  public class SymbolicDiscriminantFunctionClassificationModel : SymbolicClassificationModel, ISymbolicDiscriminantFunctionClassificationModel {

    [Storable]
    private double[] thresholds;
    public IEnumerable<double> Thresholds {
      get { return (IEnumerable<double>)thresholds.Clone(); }
      private set { thresholds = value.ToArray(); }
    }
    [Storable]
    private double[] classValues;
    public IEnumerable<double> ClassValues {
      get { return (IEnumerable<double>)classValues.Clone(); }
      private set { classValues = value.ToArray(); }
    }

    private IDiscriminantFunctionThresholdCalculator thresholdCalculator;
    [Storable]
    public IDiscriminantFunctionThresholdCalculator ThresholdCalculator {
      get { return thresholdCalculator; }
      private set { thresholdCalculator = value; }
    }


    [StorableConstructor]
    protected SymbolicDiscriminantFunctionClassificationModel(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDiscriminantFunctionClassificationModel(SymbolicDiscriminantFunctionClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      classValues = (double[])original.classValues.Clone();
      thresholds = (double[])original.thresholds.Clone();
      thresholdCalculator = cloner.Clone(original.thresholdCalculator);
    }
    public SymbolicDiscriminantFunctionClassificationModel(string targetVariable, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDiscriminantFunctionThresholdCalculator thresholdCalculator,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(targetVariable, tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.thresholds = new double[0];
      this.classValues = new double[0];
      this.ThresholdCalculator = thresholdCalculator;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (ThresholdCalculator == null) ThresholdCalculator = new AccuracyMaximizationThresholdCalculator();
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDiscriminantFunctionClassificationModel(this, cloner);
    }

    public void SetThresholdsAndClassValues(IEnumerable<double> thresholds, IEnumerable<double> classValues) {
      var classValuesArr = classValues.ToArray();
      var thresholdsArr = thresholds.ToArray();
      if (thresholdsArr.Length != classValuesArr.Length || thresholdsArr.Length < 1)
        throw new ArgumentException();
      if (!double.IsNegativeInfinity(thresholds.First()))
        throw new ArgumentException();

      this.classValues = classValuesArr;
      this.thresholds = thresholdsArr;
      OnThresholdsChanged(EventArgs.Empty);
    }

    public override void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows) {
      double[] classValues;
      double[] thresholds;
      var targetClassValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var estimatedTrainingValues = GetEstimatedValues(problemData.Dataset, rows);
      thresholdCalculator.Calculate(problemData, estimatedTrainingValues, targetClassValues, out classValues, out thresholds);
      SetThresholdsAndClassValues(thresholds, classValues);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows).LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      if (!Thresholds.Any() && !ClassValues.Any()) throw new ArgumentException("No thresholds and class values were set for the current symbolic classification model.");
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


    public override ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateDiscriminantClassificationSolution(problemData);
    }
    public SymbolicDiscriminantFunctionClassificationSolution CreateDiscriminantClassificationSolution(IClassificationProblemData problemData) {
      return new SymbolicDiscriminantFunctionClassificationSolution(this, new ClassificationProblemData(problemData));
    }
    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateDiscriminantClassificationSolution(problemData);
    }
    IDiscriminantFunctionClassificationSolution IDiscriminantFunctionClassificationModel.CreateDiscriminantFunctionClassificationSolution(IClassificationProblemData problemData) {
      return CreateDiscriminantClassificationSolution(problemData);
    }

    #region events
    public event EventHandler ThresholdsChanged;
    protected virtual void OnThresholdsChanged(EventArgs e) {
      var listener = ThresholdsChanged;
      if (listener != null) listener(this, e);
    }
    #endregion
  }
}
