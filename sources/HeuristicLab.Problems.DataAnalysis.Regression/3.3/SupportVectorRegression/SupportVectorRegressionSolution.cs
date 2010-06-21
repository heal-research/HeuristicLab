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
    public SupportVectorRegressionSolution() : base() { }
    public SupportVectorRegressionSolution(DataAnalysisProblemData problemData, SupportVectorMachineModel model, IEnumerable<string> inputVariables, double lowerEstimationLimit, double upperEstimationLimit)
      : base(problemData, lowerEstimationLimit, upperEstimationLimit) {
      this.Model = model;
    }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Function; }
    }

    public new SupportVectorMachineModel Model {
      get { return (SupportVectorMachineModel)base.Model; }
      set { base.Model = value; }
    }

    public Dataset SupportVectors {
      get { return CalculateSupportVectors(); }
    }

    protected override void OnProblemDataChanged() {
      Model.Model.SupportVectorIndizes = new int[0];
      base.OnProblemDataChanged();
    }

    private Dataset CalculateSupportVectors() {
      if (Model.Model.SupportVectorIndizes.Length == 0)
        return new Dataset(new List<string>(),new double[0,0]);

      double[,] data = new double[Model.Model.SupportVectorIndizes.Length, ProblemData.Dataset.Columns];
      for (int i = 0; i < Model.Model.SupportVectorIndizes.Length; i++) {
        for (int column = 0; column < ProblemData.Dataset.Columns; column++)
          data[i, column] = ProblemData.Dataset[Model.Model.SupportVectorIndizes[i], column];
      }
      return new Dataset(ProblemData.Dataset.VariableNames, data);
    }

    protected override void RecalculateEstimatedValues() {
      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(ProblemData, 0, ProblemData.Dataset.Rows);
      SVM.Problem scaledProblem = Scaling.Scale(Model.RangeTransform, problem);

      estimatedValues = (from row in Enumerable.Range(0, scaledProblem.Count)
                         let prediction = SVM.Prediction.Predict(Model.Model, scaledProblem.X[row])
                         let boundedX = Math.Min(UpperEstimationLimit, Math.Max(LowerEstimationLimit, prediction))
                         select double.IsNaN(boundedX) ? UpperEstimationLimit : boundedX).ToList();
      OnEstimatedValuesChanged();
      RecalculateResultValues();
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
  }
}
