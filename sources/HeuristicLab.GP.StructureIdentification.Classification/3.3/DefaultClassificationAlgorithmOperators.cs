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
using HeuristicLab.Data;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public static class DefaultClassificationAlgorithmOperators {
    public static IOperator CreatePostProcessingOperator() {
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(DefaultStructureIdentificationAlgorithmOperators.CreatePostProcessingOperator());

      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualProc = new SequentialProcessor();
      subScopesProc.AddSubOperator(individualProc);
      seq.AddSubOperator(subScopesProc);
      SimpleAccuracyEvaluator trainingAccuracy = new SimpleAccuracyEvaluator();
      trainingAccuracy.Name = "TrainingAccuracyEvaluator";
      trainingAccuracy.GetVariableInfo("Accuracy").ActualName = "TrainingAccuracy";
      trainingAccuracy.GetVariableInfo("Values").ActualName = "TrainingValues";

      SimpleAccuracyEvaluator validationAccuracy = new SimpleAccuracyEvaluator();
      validationAccuracy.Name = "ValidationAccuracyEvaluator";
      validationAccuracy.GetVariableInfo("Accuracy").ActualName = "ValidationAccuracy";
      validationAccuracy.GetVariableInfo("Values").ActualName = "ValidationValues";

      SimpleAccuracyEvaluator testAccuracy = new SimpleAccuracyEvaluator();
      testAccuracy.Name = "TestAccuracyEvaluator";
      testAccuracy.GetVariableInfo("Accuracy").ActualName = "TestAccuracy";
      testAccuracy.GetVariableInfo("Values").ActualName = "TestValues";

      individualProc.AddSubOperator(trainingAccuracy);
      individualProc.AddSubOperator(validationAccuracy);
      individualProc.AddSubOperator(testAccuracy);
      return seq;
    }

    public static IOperator CreateProblemInjector() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ProblemInjector";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(new ProblemInjector());
      seq.AddSubOperator(new TargetClassesCalculator());
      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    public static void SetModelData(IAnalyzerModel model, IScope scope) {
      model.SetResult("TrainingAccuracy", scope.GetVariableValue<DoubleData>("TrainingAccuracy", true).Data);
      model.SetResult("ValidationAccuracy", scope.GetVariableValue<DoubleData>("ValidationAccuracy", true).Data);
      model.SetResult("TestAccuracy", scope.GetVariableValue<DoubleData>("TestAccuracy", true).Data);
    }
  }
}
