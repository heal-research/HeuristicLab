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
  public static class DefaultModelAnalyzerOperators {
    public static IOperator CreatePostProcessingOperator(ModelType modelType) {
      CombinedOperator op = new CombinedOperator();
      op.Name = modelType + " model analyser";
      SequentialProcessor seq = new SequentialProcessor();
      var modelingResults = ModelingResultCalculators.GetModelingResult(modelType);
      foreach (var r in modelingResults.Keys) {
        seq.AddSubOperator(ModelingResultCalculators.CreateModelingResultEvaluator(r));
      }

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

    public static IAnalyzerModel PopulateAnalyzerModel(IScope modelScope, IAnalyzerModel model, ModelType modelType) {
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

      var modelingResults = ModelingResultCalculators.GetModelingResult(modelType);
      foreach (var r in modelingResults.Keys) {
        model.ExtractResult(modelScope, r);
      }

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
