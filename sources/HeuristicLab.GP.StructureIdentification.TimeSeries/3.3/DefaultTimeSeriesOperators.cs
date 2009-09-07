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

using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Modeling;
using HeuristicLab.Logging;
using HeuristicLab.Data;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  internal static class DefaultTimeSeriesOperators {
    internal static IOperator CreateFunctionLibraryInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "FunctionLibraryInjector";
      SequentialProcessor seq = new SequentialProcessor();
      FunctionLibraryInjector funLibInjector = new FunctionLibraryInjector();
      funLibInjector.GetVariable("Differentials").Value = new BoolData(true);
      seq.AddSubOperator(funLibInjector);
      seq.AddSubOperator(new HL3TreeEvaluatorInjector());
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    internal static IOperator CreateProblemInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ProblemInjector";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(new ProblemInjector());
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    internal static IOperator CreatePostProcessingOperator() {
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(DefaultStructureIdentificationAlgorithmOperators.CreatePostProcessingOperator());

      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualProc = new SequentialProcessor();
      subScopesProc.AddSubOperator(individualProc);
      seq.AddSubOperator(subScopesProc);

      TheilInequalityCoefficientEvaluator trainingTheil = new TheilInequalityCoefficientEvaluator();
      trainingTheil.Name = "TrainingTheilInequalityEvaluator";
      trainingTheil.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingTheil.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TrainingTheilInequalityCoefficient";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "TrainingTheilInequalityCoefficientBias";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "TrainingTheilInequalityCoefficientVariance";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "TrainingTheilInequalityCoefficientCovariance";

      TheilInequalityCoefficientEvaluator validationTheil = new TheilInequalityCoefficientEvaluator();
      validationTheil.Name = "ValidationTheilInequalityEvaluator";
      validationTheil.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationTheil.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      validationTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "ValidationTheilInequalityCoefficient";
      validationTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "ValidationTheilInequalityCoefficientBias";
      validationTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "ValidationTheilInequalityCoefficientVariance";
      validationTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "ValidationTheilInequalityCoefficientCovariance";

      TheilInequalityCoefficientEvaluator testTheil = new TheilInequalityCoefficientEvaluator();
      testTheil.Name = "TestTheilInequalityEvaluator";
      testTheil.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testTheil.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      testTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TestTheilInequalityCoefficient";
      testTheil.GetVariableInfo("TheilInequalityCoefficientBias").ActualName = "TestTheilInequalityCoefficientBias";
      testTheil.GetVariableInfo("TheilInequalityCoefficientVariance").ActualName = "TestTheilInequalityCoefficientVariance";
      testTheil.GetVariableInfo("TheilInequalityCoefficientCovariance").ActualName = "TestTheilInequalityCoefficientCovariance";

      individualProc.AddSubOperator(trainingTheil);
      individualProc.AddSubOperator(validationTheil);
      individualProc.AddSubOperator(testTheil);
      return seq;
    }
  }
}
