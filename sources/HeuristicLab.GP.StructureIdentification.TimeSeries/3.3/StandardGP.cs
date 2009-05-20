#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Logging;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class StandardGP : HeuristicLab.GP.StructureIdentification.StandardGP, ITimeSeriesAlgorithm {
    public bool Autoregressive {
      get { return ProblemInjector.GetVariable("Autoregressive").GetValue<BoolData>().Data; }
      set { ProblemInjector.GetVariable("Autoregressive").GetValue<BoolData>().Data = value; }
    }

    protected override IOperator CreateFunctionLibraryInjector() {
      return new FunctionLibraryInjector();
    }

    protected override IOperator CreateProblemInjector() {
      return new ProblemInjector();
    }

    protected override IOperator CreateBestSolutionProcessor() {
      IOperator seq = base.CreateBestSolutionProcessor();
      seq.AddSubOperator(BestSolutionProcessor);
      return seq;
    }

    public override IEditor CreateEditor() {
      return new StandardGpEditor(this);
    }

    public override IView CreateView() {
      return new StandardGpEditor(this);
    }

    internal static IOperator BestSolutionProcessor {
      get {
        SequentialProcessor seq = new SequentialProcessor();
        TheilInequalityCoefficientEvaluator trainingTheil = new TheilInequalityCoefficientEvaluator();
        trainingTheil.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
        trainingTheil.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
        trainingTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TrainingTheilInequalityCoefficient";
        trainingTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "TrainingTheilInequalityCoefficientBias";
        trainingTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "TrainingTheilInequalityCoefficientVariance";
        trainingTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "TrainingTheilInequalityCoefficientCovariance";

        TheilInequalityCoefficientEvaluator validationTheil = new TheilInequalityCoefficientEvaluator();
        validationTheil.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
        validationTheil.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
        validationTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "ValidationTheilInequalityCoefficient";
        validationTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "ValidationTheilInequalityCoefficientBias";
        validationTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "ValidationTheilInequalityCoefficientVariance";
        validationTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "ValidationTheilInequalityCoefficientCovariance";

        TheilInequalityCoefficientEvaluator testTheil = new TheilInequalityCoefficientEvaluator();
        testTheil.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
        testTheil.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
        testTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TestTheilInequalityCoefficient";
        testTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "TestTheilInequalityCoefficientBias";
        testTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "TestTheilInequalityCoefficientVariance";
        testTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "TestTheilInequalityCoefficientCovariance";

        SimpleEvaluator trainingEvaluator = new SimpleEvaluator();
        trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
        trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
        trainingEvaluator.GetVariableInfo("Values").ActualName = "PredictedValuesTraining";

        SimpleEvaluator validationEvaluator = new SimpleEvaluator();
        validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
        validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
        validationEvaluator.GetVariableInfo("Values").ActualName = "PredictedValuesValidation";

        SimpleEvaluator testEvaluator = new SimpleEvaluator();
        testEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
        testEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
        testEvaluator.GetVariableInfo("Values").ActualName = "PredictedValuesTest";

        LinechartInjector trainingChartInjector = new LinechartInjector();
        trainingChartInjector.GetVariableValue<IntData>("NumberOfLines", null, false).Data = 2;
        trainingChartInjector.GetVariableInfo("Values").ActualName = "PredictedValuesTraining";
        trainingChartInjector.GetVariableInfo("Linechart").ActualName = "LinechartTrainingPredictedValues";

        LinechartInjector validationChartInjector = new LinechartInjector();
        validationChartInjector.GetVariableValue<IntData>("NumberOfLines", null, false).Data = 2;
        validationChartInjector.GetVariableInfo("Values").ActualName = "PredictedValuesValidation";
        validationChartInjector.GetVariableInfo("Linechart").ActualName = "LinechartValidationPredictedValues";

        LinechartInjector testChartInjector = new LinechartInjector();
        testChartInjector.GetVariableValue<IntData>("NumberOfLines", null, false).Data = 2;
        testChartInjector.GetVariableInfo("Values").ActualName = "PredictedValuesTest";
        testChartInjector.GetVariableInfo("Linechart").ActualName = "LinechartTestPredictedValues";

        seq.AddSubOperator(trainingTheil);
        seq.AddSubOperator(validationTheil);
        seq.AddSubOperator(testTheil);
        seq.AddSubOperator(trainingEvaluator);
        seq.AddSubOperator(trainingChartInjector);
        seq.AddSubOperator(validationEvaluator);
        seq.AddSubOperator(validationChartInjector);
        seq.AddSubOperator(testEvaluator);
        seq.AddSubOperator(testChartInjector);

        return seq;
      }
    }
  }
}
