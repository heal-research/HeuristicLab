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
      trainingMseEvaluator.GetVariableInfo("MSE").ActualName = "TrainingMSE";
      trainingMseEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMSEEvaluator validationMseEvaluator = new SimpleMSEEvaluator();
      validationMseEvaluator.Name = "ValidationMseEvaluator";
      validationMseEvaluator.GetVariableInfo("MSE").ActualName = "ValidationMSE";
      validationMseEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMSEEvaluator testMseEvaluator = new SimpleMSEEvaluator();
      testMseEvaluator.Name = "TestMseEvaluator";
      testMseEvaluator.GetVariableInfo("MSE").ActualName = "TestMSE";
      testMseEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region MAPE
      SimpleMeanAbsolutePercentageErrorEvaluator trainingMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      trainingMapeEvaluator.Name = "TrainingMapeEvaluator";
      trainingMapeEvaluator.GetVariableInfo("MAPE").ActualName = "TrainingMAPE";
      trainingMapeEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMeanAbsolutePercentageErrorEvaluator validationMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      validationMapeEvaluator.Name = "ValidationMapeEvaluator";
      validationMapeEvaluator.GetVariableInfo("MAPE").ActualName = "ValidationMAPE";
      validationMapeEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMeanAbsolutePercentageErrorEvaluator testMapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      testMapeEvaluator.Name = "TestMapeEvaluator";
      testMapeEvaluator.GetVariableInfo("MAPE").ActualName = "TestMAPE";
      testMapeEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region MAPRE
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator trainingMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      trainingMapreEvaluator.Name = "TrainingMapreEvaluator";
      trainingMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "TrainingMAPRE";
      trainingMapreEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator validationMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      validationMapreEvaluator.Name = "ValidationMapreEvaluator";
      validationMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "ValidationMAPRE";
      validationMapreEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator testMapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      testMapreEvaluator.Name = "TestMapreEvaluator";
      testMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "TestMAPRE";
      testMapreEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion MAPRE
      #region R2
      SimpleR2Evaluator trainingR2Evaluator = new SimpleR2Evaluator();
      trainingR2Evaluator.Name = "TrainingR2Evaluator";
      trainingR2Evaluator.GetVariableInfo("R2").ActualName = "TrainingR2";
      trainingR2Evaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleR2Evaluator validationR2Evaluator = new SimpleR2Evaluator();
      validationR2Evaluator.Name = "ValidationR2Evaluator";
      validationR2Evaluator.GetVariableInfo("R2").ActualName = "ValidationR2";
      validationR2Evaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleR2Evaluator testR2Evaluator = new SimpleR2Evaluator();
      testR2Evaluator.Name = "TestR2Evaluator";
      testR2Evaluator.GetVariableInfo("R2").ActualName = "TestR2";
      testR2Evaluator.GetVariableInfo("Values").ActualName = "TestValues";
      #endregion
      #region VAF
      SimpleVarianceAccountedForEvaluator trainingVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      trainingVAFEvaluator.Name = "TrainingVAFEvaluator";
      trainingVAFEvaluator.GetVariableInfo("VAF").ActualName = "TrainingVAF";
      trainingVAFEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      SimpleVarianceAccountedForEvaluator validationVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      validationVAFEvaluator.Name = "ValidationVAFEvaluator";
      validationVAFEvaluator.GetVariableInfo("VAF").ActualName = "ValidationVAF";
      validationVAFEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      SimpleVarianceAccountedForEvaluator testVAFEvaluator = new SimpleVarianceAccountedForEvaluator();
      testVAFEvaluator.Name = "TestVAFEvaluator";
      testVAFEvaluator.GetVariableInfo("VAF").ActualName = "TestVAF";
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

      model.SetResult("TrainingMeanSquaredError", modelScope.GetVariableValue<DoubleData>("TrainingMSE", false).Data);
      model.SetResult("ValidationMeanSquaredError", modelScope.GetVariableValue<DoubleData>("ValidationMSE", false).Data);
      model.SetResult("TestMeanSquaredError", modelScope.GetVariableValue<DoubleData>("TestMSE", false).Data);
      model.SetResult("TrainingCoefficientOfDetermination", modelScope.GetVariableValue<DoubleData>("TrainingR2", false).Data);
      model.SetResult("ValidationCoefficientOfDetermination", modelScope.GetVariableValue<DoubleData>("ValidationR2", false).Data);
      model.SetResult("TestCoefficientOfDetermination", modelScope.GetVariableValue<DoubleData>("TestR2", false).Data);
      model.SetResult("TrainingMeanAbsolutePercentageError", modelScope.GetVariableValue<DoubleData>("TrainingMAPE", false).Data);
      model.SetResult("ValidationMeanAbsolutePercentageError", modelScope.GetVariableValue<DoubleData>("ValidationMAPE", false).Data);
      model.SetResult("TestMeanAbsolutePercentageError", modelScope.GetVariableValue<DoubleData>("TestMAPE", false).Data);
      model.SetResult("TrainingMeanAbsolutePercentageOfRangeError", modelScope.GetVariableValue<DoubleData>("TrainingMAPRE", false).Data);
      model.SetResult("ValidationMeanAbsolutePercentageOfRangeError", modelScope.GetVariableValue<DoubleData>("ValidationMAPRE", false).Data);
      model.SetResult("TestMeanAbsolutePercentageOfRangeError", modelScope.GetVariableValue<DoubleData>("TestMAPRE", false).Data);
      model.SetResult("TrainingVarianceAccountedFor", modelScope.GetVariableValue<DoubleData>("TrainingVAF", false).Data);
      model.SetResult("ValidationVarianceAccountedFor", modelScope.GetVariableValue<DoubleData>("ValidationVAF", false).Data);
      model.SetResult("TestVarianceAccountedFor", modelScope.GetVariableValue<DoubleData>("TestVAF", false).Data);

      ItemList evaluationImpacts = modelScope.GetVariableValue<ItemList>("VariableEvaluationImpacts", false);
      ItemList qualityImpacts = modelScope.GetVariableValue<ItemList>("VariableQualityImpacts", false);
      foreach (ItemList row in evaluationImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableEvaluationImpact(variableName, impact);
        model.AddInputVariable(variableName);
      }
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableQualityImpact(variableName, impact);
        model.AddInputVariable(variableName);
      }

      return model;
    }
  }
}
