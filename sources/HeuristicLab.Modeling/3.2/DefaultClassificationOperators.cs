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
  public static class DefaultClassificationOperators {
    public static IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Classification model analyzer";

      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(DefaultRegressionOperators.CreatePostProcessingOperator());

      SimpleAccuracyEvaluator trainingAccuracy = new SimpleAccuracyEvaluator();
      trainingAccuracy.Name = "TrainingAccuracyEvaluator";
      trainingAccuracy.GetVariableInfo("Accuracy").ActualName = ModelingResult.TrainingAccuracy.ToString();
      trainingAccuracy.GetVariableInfo("Values").ActualName = "TrainingValues";

      SimpleAccuracyEvaluator validationAccuracy = new SimpleAccuracyEvaluator();
      validationAccuracy.Name = "ValidationAccuracyEvaluator";
      validationAccuracy.GetVariableInfo("Accuracy").ActualName = ModelingResult.ValidationAccuracy.ToString();
      validationAccuracy.GetVariableInfo("Values").ActualName = "ValidationValues";

      SimpleAccuracyEvaluator testAccuracy = new SimpleAccuracyEvaluator();
      testAccuracy.Name = "TestAccuracyEvaluator";
      testAccuracy.GetVariableInfo("Accuracy").ActualName = ModelingResult.TestAccuracy.ToString();
      testAccuracy.GetVariableInfo("Values").ActualName = "TestValues";

      SimpleConfusionMatrixEvaluator trainingConfusionMatrixEvaluator = new SimpleConfusionMatrixEvaluator();
      trainingConfusionMatrixEvaluator.Name = "TrainingConfusionMatrixEvaluator";
      trainingConfusionMatrixEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingConfusionMatrixEvaluator.GetVariableInfo("ConfusionMatrix").ActualName = "TrainingConfusionMatrix";
      SimpleConfusionMatrixEvaluator validationConfusionMatrixEvaluator = new SimpleConfusionMatrixEvaluator();
      validationConfusionMatrixEvaluator.Name = "ValidationConfusionMatrixEvaluator";
      validationConfusionMatrixEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationConfusionMatrixEvaluator.GetVariableInfo("ConfusionMatrix").ActualName = "ValidationConfusionMatrix";
      SimpleConfusionMatrixEvaluator testConfusionMatrixEvaluator = new SimpleConfusionMatrixEvaluator();
      testConfusionMatrixEvaluator.Name = "TestConfusionMatrixEvaluator";
      testConfusionMatrixEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      testConfusionMatrixEvaluator.GetVariableInfo("ConfusionMatrix").ActualName = "TestConfusionMatrix";

      seq.AddSubOperator(trainingAccuracy);
      seq.AddSubOperator(validationAccuracy);
      seq.AddSubOperator(testAccuracy);
      seq.AddSubOperator(trainingConfusionMatrixEvaluator);
      seq.AddSubOperator(validationConfusionMatrixEvaluator);
      seq.AddSubOperator(testConfusionMatrixEvaluator);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;

      return op;
    }

    public static IOperator CreateProblemInjector() {
      return DefaultRegressionOperators.CreateProblemInjector();
    }

    public static IAnalyzerModel PopulateAnalyzerModel(IScope modelScope, IAnalyzerModel model) {
      DefaultRegressionOperators.PopulateAnalyzerModel(modelScope, model);
      model.ExtractResult(modelScope, ModelingResult.TrainingAccuracy);
      model.ExtractResult(modelScope, ModelingResult.ValidationAccuracy);
      model.ExtractResult(modelScope, ModelingResult.TestAccuracy);
      model.Type = ModelType.Classification;
      return model;
    }
  }
}
