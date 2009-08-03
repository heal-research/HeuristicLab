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
using HeuristicLab.Modeling;
using HeuristicLab.Operators;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class StandardGP : HeuristicLab.GP.StructureIdentification.StandardGP, IClassificationAlgorithm {
    protected override IOperator CreateBestSolutionProcessor() {
      IOperator seq = base.CreateBestSolutionProcessor();
      seq.AddSubOperator(BestSolutionProcessor);
      return seq;
    }

    internal static IOperator BestSolutionProcessor {
      get {
        SequentialProcessor seq = new SequentialProcessor();
        AccuracyEvaluator trainingAccuracy = new AccuracyEvaluator();
        trainingAccuracy.GetVariableInfo("Accuracy").ActualName = "TrainingAccuracy";
        trainingAccuracy.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
        trainingAccuracy.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

        AccuracyEvaluator validationAccuracy = new AccuracyEvaluator();
        validationAccuracy.GetVariableInfo("Accuracy").ActualName = "ValidationAccuracy";
        validationAccuracy.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
        validationAccuracy.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

        AccuracyEvaluator testAccuracy = new AccuracyEvaluator();
        testAccuracy.GetVariableInfo("Accuracy").ActualName = "TestAccuracy";
        testAccuracy.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
        testAccuracy.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";

        ConfusionMatrixEvaluator trainingConfusionMatrix = new ConfusionMatrixEvaluator();
        trainingConfusionMatrix.GetVariableInfo("ConfusionMatrix").ActualName = "TrainingConfusionMatrix";
        trainingConfusionMatrix.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
        trainingConfusionMatrix.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

        ConfusionMatrixEvaluator validationConfusionMatrix = new ConfusionMatrixEvaluator();
        validationConfusionMatrix.GetVariableInfo("ConfusionMatrix").ActualName = "ValidationConfusionMatrix";
        validationConfusionMatrix.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
        validationConfusionMatrix.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

        ConfusionMatrixEvaluator testConfusionMatrix = new ConfusionMatrixEvaluator();
        testConfusionMatrix.GetVariableInfo("ConfusionMatrix").ActualName = "TestConfusionMatrix";
        testConfusionMatrix.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
        testConfusionMatrix.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";

        seq.AddSubOperator(trainingAccuracy);
        seq.AddSubOperator(validationAccuracy);
        seq.AddSubOperator(testAccuracy);
        seq.AddSubOperator(trainingConfusionMatrix);
        seq.AddSubOperator(validationConfusionMatrix);
        seq.AddSubOperator(testConfusionMatrix);
        return seq;
      }
    }
  }
}
