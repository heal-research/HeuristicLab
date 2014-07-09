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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Error Characteristics Curve")]
  [Content(typeof(ITimeSeriesPrognosisSolution))]
  public partial class TimeSeriesPrognosisSolutionErrorCharacteristicsCurveView : RegressionSolutionErrorCharacteristicsCurveView {


    public TimeSeriesPrognosisSolutionErrorCharacteristicsCurveView()
      : base() {
      InitializeComponent();
    }

    public new ITimeSeriesPrognosisSolution Content {
      get { return (ITimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }
    public new ITimeSeriesPrognosisProblemData ProblemData {
      get {
        if (Content == null) return null;
        return Content.ProblemData;
      }
    }

    protected override void UpdateChart() {
      base.UpdateChart();
      if (Content == null) return;

      IEnumerable<double> trainingStartValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices.Select(r => r - 1).Where(r => r > 0)).ToList();
      if (trainingStartValues.Any()) {
        //AR1 model
        double alpha, beta;
        OnlineCalculatorError errorState;
        OnlineLinearScalingParameterCalculator.Calculate(ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices.Where(x => x > 0)), trainingStartValues, out alpha, out beta, out errorState);
        var ar1model = new TimeSeriesPrognosisAutoRegressiveModel(ProblemData.TargetVariable, new double[] { beta }, alpha).CreateTimeSeriesPrognosisSolution(ProblemData);
        ar1model.Name = "AR(1) Model";
        AddRegressionSolution(ar1model);
      }
    }
  }
}
