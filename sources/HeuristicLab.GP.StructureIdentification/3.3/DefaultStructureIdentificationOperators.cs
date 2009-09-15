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
  public static class DefaultStructureIdentificationOperators {
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


    public static IOperator CreatePreparationForPostProcessingOperator() {
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
      solutionProc.AddSubOperator(evaluatorInjector);
      solutionProc.AddSubOperator(trainingEvaluator);
      solutionProc.AddSubOperator(validationEvaluator);
      solutionProc.AddSubOperator(testEvaluator);
      #endregion

      #region variable impacts
      // calculate and set variable impacts
      VariableNamesExtractor namesExtractor = new VariableNamesExtractor();
      namesExtractor.GetVariableInfo("VariableNames").ActualName = "InputVariableNames";
      PredictorBuilder predictorBuilder = new PredictorBuilder();
      predictorBuilder.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";

      solutionProc.AddSubOperator(namesExtractor);
      solutionProc.AddSubOperator(predictorBuilder);
      #endregion

      return seq;
    }

    public static void PopulateAnalyzerModel(IScope bestModelScope, IAnalyzerModel model) {
      model.SetMetaData("EvaluatedSolutions", bestModelScope.GetVariableValue<DoubleData>("EvaluatedSolutions", false).Data);
      IGeneticProgrammingModel gpModel = bestModelScope.GetVariableValue<IGeneticProgrammingModel>("FunctionTree", false);
      model.SetMetaData("TreeSize", gpModel.Size);
      model.SetMetaData("TreeHeight", gpModel.Height);
    }
  }
}
