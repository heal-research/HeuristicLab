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
  public static class DefaultRegressionOperators {
    public static IOperator CreateProblemInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ProblemInjector";
      SequentialProcessor seq = new SequentialProcessor();

      seq.AddSubOperator(new ProblemInjector());
      DatasetShuffler shuffler = new DatasetShuffler();
      shuffler.GetVariableInfo("ShuffleStart").ActualName = "TrainingSamplesStart";
      shuffler.GetVariableInfo("ShuffleEnd").ActualName = "TrainingSamplesEnd";
      seq.AddSubOperator(shuffler);
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Regression model analyser";
      SequentialProcessor seq = new SequentialProcessor();
      #region MSE
      SimpleMSEEvaluator trainingMseEvaluator = new SimpleMSEEvaluator();
      trainingMseEvaluator.Name = "TrainingMseEvaluator";
      trainingMseEvaluator.GetVariableInfo("MSE").ActualName = ModelingResult.TrainingMeanSquaredError.ToString();
      trainingMseEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMSEEvaluator validationMseEvaluator = new SimpleMSEEvaluator();
      validationMseEvaluator.Name = "ValidationMseEvaluator";
      validationMseEvaluator.GetVariableInfo("MSE").ActualName = ModelingResult.ValidationMeanSquaredError.ToString();
      validationMseEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMSEEvaluator testMseEvaluator = new SimpleMSEEvaluator();
      testMseEvaluator.Name = "TestMseEvaluator";
      testMseEvaluator.GetVariableInfo("MSE").ActualName = ModelingResult.TestMeanSquaredError.ToString();
      testMseEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region MAPE
      SimpleMeanAbsolutePercentageErrorEvaluator trainingMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      trainingMapeEvaluator.Name = "TrainingMapeEvaluator";
      trainingMapeEvaluator.GetVariableInfo("MAPE").ActualName = ModelingResult.TrainingMeanAbsolutePercentageError.ToString();
      trainingMapeEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMeanAbsolutePercentageErrorEvaluator validationMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      validationMapeEvaluator.Name = "ValidationMapeEvaluator";
      validationMapeEvaluator.GetVariableInfo("MAPE").ActualName = ModelingResult.ValidationMeanAbsolutePercentageError.ToString();
      validationMapeEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMeanAbsolutePercentageErrorEvaluator testMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      testMapeEvaluator.Name = "TestMapeEvaluator";
      testMapeEvaluator.GetVariableInfo("MAPE").ActualName = ModelingResult.TestMeanAbsolutePercentageError.ToString();
      testMapeEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region MAPRE
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator trainingMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      trainingMapreEvaluator.Name = "TrainingMapreEvaluator";
      trainingMapreEvaluator.GetVariableInfo("MAPRE").ActualName = ModelingResult.TrainingMeanAbsolutePercentageOfRangeError.ToString();
      trainingMapreEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator validationMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      validationMapreEvaluator.Name = "ValidationMapreEvaluator";
      validationMapreEvaluator.GetVariableInfo("MAPRE").ActualName = ModelingResult.ValidationMeanAbsolutePercentageOfRangeError.ToString();
      validationMapreEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator testMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      testMapreEvaluator.Name = "TestMapreEvaluator";
      testMapreEvaluator.GetVariableInfo("MAPRE").ActualName = ModelingResult.TestMeanAbsolutePercentageOfRangeError.ToString();
      testMapreEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion MAPRE
      #region R2
      SimpleR2Evaluator trainingR2Evaluator = new SimpleR2Evaluator();
      trainingR2Evaluator.Name = "TrainingR2Evaluator";
      trainingR2Evaluator.GetVariableInfo("R2").ActualName = ModelingResult.TrainingCoefficientOfDetermination.ToString();
      trainingR2Evaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleR2Evaluator validationR2Evaluator = new SimpleR2Evaluator();
      validationR2Evaluator.Name = "ValidationR2Evaluator";
      validationR2Evaluator.GetVariableInfo("R2").ActualName = ModelingResult.ValidationCoefficientOfDetermination.ToString();
      validationR2Evaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleR2Evaluator testR2Evaluator = new SimpleR2Evaluator();
      testR2Evaluator.Name = "TestR2Evaluator";
      testR2Evaluator.GetVariableInfo("R2").ActualName = ModelingResult.TestCoefficientOfDetermination.ToString();
      testR2Evaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region VAF
      SimpleVarianceAccountedForEvaluator trainingVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      trainingVAFEvaluator.Name = "TrainingVAFEvaluator";
      trainingVAFEvaluator.GetVariableInfo("VAF").ActualName = ModelingResult.TrainingVarianceAccountedFor.ToString();
      trainingVAFEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleVarianceAccountedForEvaluator validationVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      validationVAFEvaluator.Name = "ValidationVAFEvaluator";
      validationVAFEvaluator.GetVariableInfo("VAF").ActualName = ModelingResult.ValidationVarianceAccountedFor.ToString();
      validationVAFEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleVarianceAccountedForEvaluator testVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      testVAFEvaluator.Name = "TestVAFEvaluator";
      testVAFEvaluator.GetVariableInfo("VAF").ActualName = ModelingResult.TestVarianceAccountedFor.ToString();
      testVAFEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion

      seq.AddSubOperator(trainingMseEvaluator);
      seq.AddSubOperator(validationMseEvaluator);
      seq.AddSubOperator(testMseEvaluator);
      seq.AddSubOperator(trainingMapeEvaluator);
      seq.AddSubOperator(validationMapeEvaluator);
      seq.AddSubOperator(testMapeEvaluator);
      seq.AddSubOperator(trainingMapreEvaluator);
      seq.AddSubOperator(validationMapreEvaluator);
      seq.AddSubOperator(testMapreEvaluator);
      seq.AddSubOperator(trainingR2Evaluator);
      seq.AddSubOperator(validationR2Evaluator);
      seq.AddSubOperator(testR2Evaluator);
      seq.AddSubOperator(trainingVAFEvaluator);
      seq.AddSubOperator(validationVAFEvaluator);
      seq.AddSubOperator(testVAFEvaluator);

      #region variable impacts
      VariableEvaluationImpactCalculator evaluationImpactCalculator = new VariableEvaluationImpactCalculator();
      evaluationImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      evaluationImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      VariableQualityImpactCalculator qualityImpactCalculator = new VariableQualityImpactCalculator();
      qualityImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      qualityImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";

      seq.AddSubOperator(evaluationImpactCalculator);
      seq.AddSubOperator(qualityImpactCalculator);
      #endregion

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IAnalyzerModel PopulateAnalyzerModel(IScope modelScope, IAnalyzerModel model) {
      model.Predictor = modelScope.GetVariableValue<IPredictor>("Predictor", false);
      Dataset ds = modelScope.GetVariableValue<Dataset>("Dataset", true);
      model.Dataset = ds;
      model.TargetVariable = ds.GetVariableName(modelScope.GetVariableValue<IntData>("TargetVariable", true).Data);
      model.Type = ModelType.Regression;
      model.TrainingSamplesStart = modelScope.GetVariableValue<IntData>("TrainingSamplesStart", true).Data;
      model.TrainingSamplesEnd = modelScope.GetVariableValue<IntData>("TrainingSamplesEnd", true).Data;
      model.ValidationSamplesStart = modelScope.GetVariableValue<IntData>("ValidationSamplesStart", true).Data;
      model.ValidationSamplesEnd = modelScope.GetVariableValue<IntData>("ValidationSamplesEnd", true).Data;
      model.TestSamplesStart = modelScope.GetVariableValue<IntData>("TestSamplesStart", true).Data;
      model.TestSamplesEnd = modelScope.GetVariableValue<IntData>("TestSamplesEnd", true).Data;

      model.ExtractResult(modelScope, ModelingResult.TrainingMeanSquaredError);
      model.ExtractResult(modelScope, ModelingResult.ValidationMeanSquaredError);
      model.ExtractResult(modelScope, ModelingResult.TestMeanSquaredError);
      model.ExtractResult(modelScope, ModelingResult.TrainingMeanAbsolutePercentageError);
      model.ExtractResult(modelScope, ModelingResult.ValidationMeanAbsolutePercentageError);
      model.ExtractResult(modelScope, ModelingResult.TestMeanAbsolutePercentageError);
      model.ExtractResult(modelScope, ModelingResult.TrainingMeanAbsolutePercentageOfRangeError);
      model.ExtractResult(modelScope, ModelingResult.ValidationMeanAbsolutePercentageOfRangeError);
      model.ExtractResult(modelScope, ModelingResult.TestMeanAbsolutePercentageOfRangeError);
      model.ExtractResult(modelScope, ModelingResult.TrainingCoefficientOfDetermination);
      model.ExtractResult(modelScope, ModelingResult.ValidationCoefficientOfDetermination);
      model.ExtractResult(modelScope, ModelingResult.TestCoefficientOfDetermination);
      model.ExtractResult(modelScope, ModelingResult.TrainingVarianceAccountedFor);
      model.ExtractResult(modelScope, ModelingResult.ValidationVarianceAccountedFor);
      model.ExtractResult(modelScope, ModelingResult.TestVarianceAccountedFor);

      ItemList evaluationImpacts = modelScope.GetVariableValue<ItemList>(ModelingResult.VariableEvaluationImpact.ToString(), false);
      ItemList qualityImpacts = modelScope.GetVariableValue<ItemList>(ModelingResult.VariableQualityImpact.ToString(), false);
      foreach (ItemList row in evaluationImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableResult(ModelingResult.VariableEvaluationImpact, variableName, impact);
        model.AddInputVariable(variableName);
      }
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableResult(ModelingResult.VariableQualityImpact, variableName, impact);
        model.AddInputVariable(variableName);
      }

      return model;
    }
  }
}
