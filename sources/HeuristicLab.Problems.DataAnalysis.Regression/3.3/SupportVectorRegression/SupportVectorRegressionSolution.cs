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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;
using SVM;

namespace HeuristicLab.Problems.DataAnalysis.Regression.SupportVectorRegression {
  /// <summary>
  /// Represents a support vector solution for a regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("SupportVectorRegressionSolution", "Represents a support vector solution for a regression problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class SupportVectorRegressionSolution : DataAnalysisSolution {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Function; }
    }

    [Storable]
    private SupportVectorMachineModel model;
    public SupportVectorMachineModel Model {
      get { return model; }
    }

    [Storable]
    private List<string> inputVariables;

    public SupportVectorRegressionSolution() : base() { }
    public SupportVectorRegressionSolution(DataAnalysisProblemData problemData, SupportVectorMachineModel model, IEnumerable<string> inputVariables, double lowerEstimationLimit, double upperEstimationLimit)
      : base(problemData, lowerEstimationLimit, upperEstimationLimit) {
      this.model = model;
      this.inputVariables = new List<string>(inputVariables);
    }

    protected override void OnProblemDataChanged(EventArgs e) {
      RecalculateEstimatedValues();
    }

    private void RecalculateEstimatedValues() {
      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(ProblemData, 0, ProblemData.Dataset.Rows);
      SVM.Problem scaledProblem = Scaling.Scale(model.RangeTransform, problem);

      estimatedValues = (from row in Enumerable.Range(0, scaledProblem.Count)
                         let prediction = SVM.Prediction.Predict(model.Model, scaledProblem.X[row])
                         let boundedX = Math.Min(UpperEstimationLimit, Math.Max(LowerEstimationLimit, prediction))
                         select double.IsNaN(boundedX) ? UpperEstimationLimit : boundedX).ToList();
      OnEstimatedValuesChanged(EventArgs.Empty);
    }

    private List<double> estimatedValues;
    public override IEnumerable<double> EstimatedValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        return estimatedValues.AsEnumerable();
      }
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        int start = ProblemData.TrainingSamplesStart.Value;
        int n = ProblemData.TrainingSamplesEnd.Value - start;
        return estimatedValues.Skip(start).Take(n).ToList();
      }
    }

    public override IEnumerable<double> EstimatedTestValues {
      get {
        if (estimatedValues == null) RecalculateEstimatedValues();
        int start = ProblemData.TestSamplesStart.Value;
        int n = ProblemData.TestSamplesEnd.Value - start;
        return estimatedValues.Skip(start).Take(n).ToList();
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SupportVectorRegressionSolution clone = (SupportVectorRegressionSolution)base.Clone(cloner);
      clone.model = (SupportVectorMachineModel)cloner.Clone(model);
      return clone;
    }
  }
}
