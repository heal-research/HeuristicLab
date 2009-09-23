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

      #region theil inequality
      SimpleTheilInequalityCoefficientEvaluator trainingTheil = new SimpleTheilInequalityCoefficientEvaluator();
      trainingTheil.Name = "TrainingTheilInequalityEvaluator";
      trainingTheil.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = ModelingResult.TrainingTheilInequality.ToString();
      SimpleTheilInequalityCoefficientEvaluator validationTheil = new SimpleTheilInequalityCoefficientEvaluator();
      validationTheil.Name = "ValidationTheilInequalityEvaluator";
      validationTheil.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = ModelingResult.ValidationTheilInequality.ToString();
      SimpleTheilInequalityCoefficientEvaluator testTheil = new SimpleTheilInequalityCoefficientEvaluator();
      testTheil.Name = "TestTheilInequalityEvaluator";
      testTheil.GetVariableInfo("Values").ActualName = "TestValues";
      testTheil.GetVariableInfo("TheilInequalityCoefficient").ActualName = ModelingResult.TestTheilInequality.ToString();

      seq.AddSubOperator(trainingTheil);
      seq.AddSubOperator(validationTheil);
      seq.AddSubOperator(testTheil);
      #endregion

      #region directional symmetry
      SimpleDirectionalSymmetryEvaluator trainingDS = new SimpleDirectionalSymmetryEvaluator();
      trainingDS.Name = "TrainingDirectionalSymmetryEvaluator";
      trainingDS.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingDS.GetVariableInfo("DirectionalSymmetry").ActualName = ModelingResult.TrainingDirectionalSymmetry.ToString();
      SimpleDirectionalSymmetryEvaluator validationDS = new SimpleDirectionalSymmetryEvaluator();
      validationDS.Name = "ValidationDirectionalSymmetryEvaluator";
      validationDS.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationDS.GetVariableInfo("DirectionalSymmetry").ActualName = ModelingResult.ValidationDirectionalSymmetry.ToString();
      SimpleDirectionalSymmetryEvaluator testDS = new SimpleDirectionalSymmetryEvaluator();
      testDS.Name = "TestDirectionalSymmetryEvaluator";
      testDS.GetVariableInfo("Values").ActualName = "TestValues";
      testDS.GetVariableInfo("DirectionalSymmetry").ActualName = ModelingResult.TestDirectionalSymmetry.ToString();

      seq.AddSubOperator(trainingDS);
      seq.AddSubOperator(validationDS);
      seq.AddSubOperator(testDS);
      #endregion

      #region weighted directional symmetry
      SimpleWeightedDirectionalSymmetryEvaluator trainingWDS = new SimpleWeightedDirectionalSymmetryEvaluator();
      trainingWDS.Name = "TrainingWeightedDirectionalSymmetryEvaluator";
      trainingWDS.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingWDS.GetVariableInfo("WeightedDirectionalSymmetry").ActualName = ModelingResult.TrainingWeightedDirectionalSymmetry.ToString();
      SimpleWeightedDirectionalSymmetryEvaluator validationWDS = new SimpleWeightedDirectionalSymmetryEvaluator();
      validationWDS.Name = "ValidationWeightedDirectionalSymmetryEvaluator";
      validationWDS.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationWDS.GetVariableInfo("WeightedDirectionalSymmetry").ActualName = ModelingResult.ValidationWeightedDirectionalSymmetry.ToString();
      SimpleWeightedDirectionalSymmetryEvaluator testWDS = new SimpleWeightedDirectionalSymmetryEvaluator();
      testWDS.Name = "TestWeightedDirectionalSymmetryEvaluator";
      testWDS.GetVariableInfo("Values").ActualName = "TestValues";
      testWDS.GetVariableInfo("WeightedDirectionalSymmetry").ActualName = ModelingResult.TestWeightedDirectionalSymmetry.ToString();

      seq.AddSubOperator(trainingWDS);
      seq.AddSubOperator(validationWDS);
      seq.AddSubOperator(testWDS);
      #endregion

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static IAnalyzerModel PopulateAnalyzerModel(IScope modelScope, IAnalyzerModel model) {
      DefaultRegressionOperators.PopulateAnalyzerModel(modelScope, model);
      model.ExtractResult(modelScope, ModelingResult.TrainingTheilInequality);
      model.ExtractResult(modelScope, ModelingResult.ValidationTheilInequality);
      model.ExtractResult(modelScope, ModelingResult.TestTheilInequality);
      model.ExtractResult(modelScope, ModelingResult.TrainingDirectionalSymmetry);
      model.ExtractResult(modelScope, ModelingResult.ValidationDirectionalSymmetry);
      model.ExtractResult(modelScope, ModelingResult.TestDirectionalSymmetry);
      model.ExtractResult(modelScope, ModelingResult.TrainingWeightedDirectionalSymmetry);
      model.ExtractResult(modelScope, ModelingResult.ValidationWeightedDirectionalSymmetry);
      model.ExtractResult(modelScope, ModelingResult.TestWeightedDirectionalSymmetry);
      model.Type = ModelType.TimeSeriesPrognosis;
      return model;
    }
  }
}
