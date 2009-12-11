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

    public static IOperator CreateSimpleFunctionLibraryInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "FunctionLibraryInjector";
      SequentialProcessor seq = new SequentialProcessor();
      var funLibInjector = new FunctionLibraryInjector();
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.CONSTANTS_ALLOWED, null, false).Data = true;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.VARIABLES_ALLOWED, null, false).Data = true;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.ADDITION_ALLOWED, null, false).Data = true;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.SUBTRACTION_ALLOWED, null, false).Data = true;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.MULTIPLICATION_ALLOWED, null, false).Data = true;

      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.AND_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.AVERAGE_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.COSINUS_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.DIFFERENTIALS_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.DIVISION_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.EQUAL_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.EXPONENTIAL_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.GREATERTHAN_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.IFTHENELSE_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.LESSTHAN_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.LOGARTIHM_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.NOT_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.OR_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.POWER_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.SIGNUM_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.SINUS_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.SQRT_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.TANGENS_ALLOWED, null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>(FunctionLibraryInjector.XOR_ALLOWED, null, false).Data = false;
      seq.AddSubOperator(funLibInjector);
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

      Counter bestValidationSolutionAgeCounter = new Counter();
      bestValidationSolutionAgeCounter.Name = "BestSolutionAgeCounter";
      bestValidationSolutionAgeCounter.GetVariableInfo("Value").ActualName = "BestValidationSolutionAge";

      BestSolutionStorer solutionStorer = new BestSolutionStorer();
      solutionStorer.GetVariableInfo("BestSolution").ActualName = "BestValidationSolution";
      solutionStorer.GetVariableInfo("Quality").ActualName = "ValidationQuality";

      OperatorExtractor bestSolutionProcessor = new OperatorExtractor();
      bestSolutionProcessor.Name = "BestSolutionProcessor (extr.)";
      bestSolutionProcessor.GetVariableInfo("Operator").ActualName = "BestSolutionProcessor";

      solutionStorer.AddSubOperator(bestSolutionProcessor);

      BestAverageWorstQualityCalculator validationQualityCalculator = new BestAverageWorstQualityCalculator();
      validationQualityCalculator.Name = "BestAverageWorstValidationQualityCalculator";
      validationQualityCalculator.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityCalculator.GetVariableInfo("BestQuality").ActualName = "BestValidationQuality";
      validationQualityCalculator.GetVariableInfo("AverageQuality").ActualName = "AverageValidationQuality";
      validationQualityCalculator.GetVariableInfo("WorstQuality").ActualName = "WorstValidationQuality";

      subScopesProc.AddSubOperator(individualProc);

      seq.AddSubOperator(subScopesProc);
      seq.AddSubOperator(bestValidationSolutionAgeCounter);
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
      VariableQualityImpactCalculator qualityImpactCalculator = new VariableQualityImpactCalculator();
      qualityImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      qualityImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

      solutionProc.AddSubOperator(qualityImpactCalculator);

      NodeBasedVariableImpactCalculator nodeImpactCalculator = new NodeBasedVariableImpactCalculator();
      nodeImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      nodeImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

      solutionProc.AddSubOperator(nodeImpactCalculator);

      #endregion

      return seq;
    }

    public static void PopulateAnalyzerModel(IScope bestModelScope, IAnalyzerModel model) {
      model.SetMetaData("EvaluatedSolutions", bestModelScope.GetVariableValue<IntData>("EvaluatedSolutions", false).Data);
      IGeneticProgrammingModel gpModel = bestModelScope.GetVariableValue<IGeneticProgrammingModel>("FunctionTree", false);
      model.SetMetaData("TreeSize", gpModel.Size);
      model.SetMetaData("TreeHeight", gpModel.Height);
      #region variable impacts
      ItemList qualityImpacts = bestModelScope.GetVariableValue<ItemList>(ModelingResult.VariableQualityImpact.ToString(), false);
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableResult(ModelingResult.VariableQualityImpact, variableName, impact);
        model.AddInputVariable(variableName);
      }
      ItemList nodeImpacts = bestModelScope.GetVariableValue<ItemList>(ModelingResult.VariableNodeImpact.ToString(), false);
      foreach (ItemList row in nodeImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableResult(ModelingResult.VariableNodeImpact, variableName, impact);
        model.AddInputVariable(variableName);
      }
      #endregion

    }
  }
}
