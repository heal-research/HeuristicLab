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
using HeuristicLab.Selection;
using HeuristicLab.Data;

namespace HeuristicLab.GP.StructureIdentification {
  public static class DefaultStructureIdentificationAlgorithmOperators {
    public static IOperator CreateFunctionLibraryInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "FunctionLibraryInjector";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(new FunctionLibraryInjector());
      seq.AddSubOperator(new HL3TreeEvaluatorInjector());
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

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

    public static IOperator CreateInitialPopulationEvaluator() {
      MeanSquaredErrorEvaluator eval = new MeanSquaredErrorEvaluator();
      eval.Name = "Evaluator";
      eval.GetVariableInfo("MSE").ActualName = "Quality";
      eval.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      eval.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      return eval;
    }

    public static IOperator CreateEvaluator() {
      return CreateInitialPopulationEvaluator();
    }

    public static IOperator CreateGenerationStepHook() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualProc = new SequentialProcessor();
      MeanSquaredErrorEvaluator validationEvaluator = new MeanSquaredErrorEvaluator();
      validationEvaluator.Name = "ValidationEvaluator";
      validationEvaluator.GetVariableInfo("MSE").ActualName = "ValidationQuality";
      validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

      individualProc.AddSubOperator(validationEvaluator);

      BestSolutionStorer solutionStorer = new BestSolutionStorer();
      solutionStorer.GetVariableInfo("BestSolution").ActualName = "BestValidationSolution";
      solutionStorer.GetVariableInfo("Quality").ActualName = "ValidationQuality";

      BestAverageWorstQualityCalculator validationQualityCalculator = new BestAverageWorstQualityCalculator();
      validationQualityCalculator.Name = "BestAverageWorstValidationQualityCalculator";
      validationQualityCalculator.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityCalculator.GetVariableInfo("BestQuality").ActualName = "BestValidationQuality";
      validationQualityCalculator.GetVariableInfo("AverageQuality").ActualName = "AverageValidationQuality";
      validationQualityCalculator.GetVariableInfo("WorstQuality").ActualName = "WorstValidationQuality";

      subScopesProc.AddSubOperator(individualProc);

      seq.AddSubOperator(subScopesProc);
      seq.AddSubOperator(solutionStorer);
      seq.AddSubOperator(validationQualityCalculator);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ModelAnalyser";
      SequentialProcessor seq = new SequentialProcessor();
      LeftReducer cleanUp = new LeftReducer();
      cleanUp.Name = "Reset Population";
      seq.AddSubOperator(cleanUp);

      SolutionExtractor extractor = new SolutionExtractor();
      extractor.GetVariableInfo("Scope").ActualName = "BestValidationSolution";
      SequentialSubScopesProcessor seqSubScopeProc = new SequentialSubScopesProcessor();
      SequentialProcessor solutionProc = new SequentialProcessor();

      seq.AddSubOperator(extractor);
      seq.AddSubOperator(seqSubScopeProc);
      seqSubScopeProc.AddSubOperator(solutionProc);

      HL3TreeEvaluatorInjector evaluatorInjector = new HL3TreeEvaluatorInjector();
      evaluatorInjector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000.0)));
      evaluatorInjector.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";

      #region simple evaluators
      SimpleEvaluator trainingEvaluator = new SimpleEvaluator();
      trainingEvaluator.Name = "TrainingEvaluator";
      trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      SimpleEvaluator validationEvaluator = new SimpleEvaluator();
      validationEvaluator.Name = "ValidationEvaluator";
      validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      validationEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      SimpleEvaluator testEvaluator = new SimpleEvaluator();
      testEvaluator.Name = "TestEvaluator";
      testEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      testEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      testEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      #endregion
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

      solutionProc.AddSubOperator(evaluatorInjector);
      solutionProc.AddSubOperator(trainingEvaluator);
      solutionProc.AddSubOperator(validationEvaluator);
      solutionProc.AddSubOperator(testEvaluator);
      solutionProc.AddSubOperator(trainingMseEvaluator);
      solutionProc.AddSubOperator(validationMseEvaluator);
      solutionProc.AddSubOperator(testMseEvaluator);
      solutionProc.AddSubOperator(trainingMapeEvaluator);
      solutionProc.AddSubOperator(validationMapeEvaluator);
      solutionProc.AddSubOperator(testMapeEvaluator);
      solutionProc.AddSubOperator(trainingMapreEvaluator);
      solutionProc.AddSubOperator(validationMapreEvaluator);
      solutionProc.AddSubOperator(testMapreEvaluator);
      solutionProc.AddSubOperator(trainingR2Evaluator);
      solutionProc.AddSubOperator(validationR2Evaluator);
      solutionProc.AddSubOperator(testR2Evaluator);
      solutionProc.AddSubOperator(trainingVAFEvaluator);
      solutionProc.AddSubOperator(validationVAFEvaluator);
      solutionProc.AddSubOperator(testVAFEvaluator);

      #region variable impacts
      // calculate and set variable impacts
      VariableNamesExtractor namesExtractor = new VariableNamesExtractor();
      namesExtractor.GetVariableInfo("VariableNames").ActualName = "InputVariableNames";
      PredictorBuilder predictorBuilder = new PredictorBuilder();
      predictorBuilder.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";

      VariableEvaluationImpactCalculator evaluationImpactCalculator = new VariableEvaluationImpactCalculator();
      evaluationImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      evaluationImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      VariableQualityImpactCalculator qualityImpactCalculator = new VariableQualityImpactCalculator();
      qualityImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      qualityImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";

      solutionProc.AddSubOperator(namesExtractor);
      solutionProc.AddSubOperator(predictorBuilder);
      solutionProc.AddSubOperator(evaluationImpactCalculator);
      solutionProc.AddSubOperator(qualityImpactCalculator);
      #endregion

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IAnalyzerModel CreateGPModel(IScope bestModelScope) {
      IAnalyzerModel model = new AnalyzerModel();
      model.Predictor = bestModelScope.GetVariableValue<IPredictor>("Predictor", true);
      Dataset ds = bestModelScope.GetVariableValue<Dataset>("Dataset", true);
      model.Dataset = ds;
      model.TargetVariable = ds.GetVariableName(bestModelScope.GetVariableValue<IntData>("TargetVariable", true).Data);
      model.TrainingSamplesStart = bestModelScope.GetVariableValue<IntData>("TrainingSamplesStart", true).Data;
      model.TrainingSamplesEnd = bestModelScope.GetVariableValue<IntData>("TrainingSamplesEnd", true).Data;
      model.ValidationSamplesStart = bestModelScope.GetVariableValue<IntData>("ValidationSamplesStart", true).Data;
      model.ValidationSamplesEnd = bestModelScope.GetVariableValue<IntData>("ValidationSamplesEnd", true).Data;
      model.TestSamplesStart = bestModelScope.GetVariableValue<IntData>("TestSamplesStart", true).Data;
      model.TestSamplesEnd = bestModelScope.GetVariableValue<IntData>("TestSamplesEnd", true).Data;

      model.SetResult("TrainingMeanSquaredError", bestModelScope.GetVariableValue<DoubleData>("TrainingMSE", false).Data);
      model.SetResult("ValidationMeanSquaredError", bestModelScope.GetVariableValue<DoubleData>("ValidationMSE", false).Data);
      model.SetResult("TestMeanSquaredError", bestModelScope.GetVariableValue<DoubleData>("TestMSE", false).Data);
      model.SetResult("TrainingCoefficientOfDetermination", bestModelScope.GetVariableValue<DoubleData>("TrainingR2", false).Data);
      model.SetResult("ValidationCoefficientOfDetermination", bestModelScope.GetVariableValue<DoubleData>("ValidationR2", false).Data);
      model.SetResult("TestCoefficientOfDetermination", bestModelScope.GetVariableValue<DoubleData>("TestR2", false).Data);
      model.SetResult("TrainingMeanAbsolutePercentageError", bestModelScope.GetVariableValue<DoubleData>("TrainingMAPE", false).Data);
      model.SetResult("ValidationMeanAbsolutePercentageError", bestModelScope.GetVariableValue<DoubleData>("ValidationMAPE", false).Data);
      model.SetResult("TestMeanAbsolutePercentageError", bestModelScope.GetVariableValue<DoubleData>("TestMAPE", false).Data);
      model.SetResult("TrainingMeanAbsolutePercentageOfRangeError", bestModelScope.GetVariableValue<DoubleData>("TrainingMAPRE", false).Data);
      model.SetResult("ValidationMeanAbsolutePercentageOfRangeError", bestModelScope.GetVariableValue<DoubleData>("ValidationMAPRE", false).Data);
      model.SetResult("TestMeanAbsolutePercentageOfRangeError", bestModelScope.GetVariableValue<DoubleData>("TestMAPRE", false).Data);
      model.SetResult("TrainingVarianceAccountedFor", bestModelScope.GetVariableValue<DoubleData>("TrainingVAF", false).Data);
      model.SetResult("ValidationVarianceAccountedFor", bestModelScope.GetVariableValue<DoubleData>("ValidationVAF", false).Data);
      model.SetResult("TestVarianceAccountedFor", bestModelScope.GetVariableValue<DoubleData>("TestVAF", false).Data);

      model.SetMetaData("EvaluatedSolutions", bestModelScope.GetVariableValue<DoubleData>("EvaluatedSolutions", true).Data);
      IGeneticProgrammingModel gpModel = bestModelScope.GetVariableValue<IGeneticProgrammingModel>("FunctionTree", true);
      model.SetMetaData("TreeSize", gpModel.Size);
      model.SetMetaData("TreeHeight", gpModel.Height);

      ItemList evaluationImpacts = bestModelScope.GetVariableValue<ItemList>("VariableEvaluationImpacts", false);
      ItemList qualityImpacts = bestModelScope.GetVariableValue<ItemList>("VariableQualityImpacts", false);
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
