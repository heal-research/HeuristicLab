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
using HeuristicLab.Operators;
using HeuristicLab.Modeling;
using HeuristicLab.Data;

namespace HeuristicLab.Modeling {
  public static class DefaultTimeSeriesOperators {
    public static IOperator CreateProblemInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ProblemInjector";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(new ProblemInjector());
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Time series prognosis model analyzer";

      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(DefaultRegressionOperators.CreatePostProcessingOperator());

      SimpleTheilInequalityCoefficientEvaluator trainingTheil = new SimpleTheilInequalityCoefficientEvaluator();
      trainingTheil.Name = "TrainingTheilInequalityEvaluator";
      trainingTheil.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TrainingTheilInequalityCoefficient";
      SimpleTheilInequalityCoefficientEvaluator validationTheil = new SimpleTheilInequalityCoefficientEvaluator();
      validationTheil.Name = "ValidationTheilInequalityEvaluator";
      validationTheil.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "ValidationTheilInequalityCoefficient";
      SimpleTheilInequalityCoefficientEvaluator testTheil = new SimpleTheilInequalityCoefficientEvaluator();
      testTheil.Name = "TestTheilInequalityEvaluator";
      testTheil.GetVariableInfo("Values").ActualName = "TestValues";
      testTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = "TestTheilInequalityCoefficient";

      seq.AddSubOperator(trainingTheil);
      seq.AddSubOperator(validationTheil);
      seq.AddSubOperator(testTheil);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IAnalyzerModel PopulateAnalyzerModel(IScope modelScope, IAnalyzerModel model) {
      DefaultRegressionOperators.PopulateAnalyzerModel(modelScope, model);
      model.SetResult("TrainingTheilInequalityCoefficient", modelScope.GetVariableValue<DoubleData>("TrainingTheilInequalityCoefficient", true).Data);
      model.SetResult("ValidationTheilInequalityCoefficient", modelScope.GetVariableValue<DoubleData>("ValidationTheilInequalityCoefficient", true).Data);
      model.SetResult("TestTheilInequalityCoefficient", modelScope.GetVariableValue<DoubleData>("TestTheilInequalityCoefficient", true).Data);

      return model;
    }
  }
}
