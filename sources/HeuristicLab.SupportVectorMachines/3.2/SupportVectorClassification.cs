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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Logging;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorClassification : SupportVectorRegression {

    public override string Name { get { return "SupportVectorClassification"; } }

    public SupportVectorClassification()
      : base() {
      SvmType = "NU_SVC";
      CostList = new DoubleArrayData(new double[] { 1.0 });
      MaxCostIndex = 1;
    }

    protected override IOperator CreateModelAnalyser() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Model Analyzer";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(base.CreateModelAnalyser());
      SequentialSubScopesProcessor seqSubScopeProc = new SequentialSubScopesProcessor();
      SequentialProcessor seqProc = new SequentialProcessor();

      SupportVectorEvaluator trainingEvaluator = new SupportVectorEvaluator();
      trainingEvaluator.Name = "TrainingSimpleEvaluator";
      trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";

      SimpleAccuracyEvaluator trainingAccuracyEvaluator = new SimpleAccuracyEvaluator();
      trainingAccuracyEvaluator.Name = "TrainingAccuracyEvaluator";
      trainingAccuracyEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingAccuracyEvaluator.GetVariableInfo("Accuracy").ActualName = "TrainingAccuracy";
      SimpleAccuracyEvaluator validationAccuracyEvaluator = new SimpleAccuracyEvaluator();
      validationAccuracyEvaluator.Name = "ValidationAccuracyEvaluator";
      validationAccuracyEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationAccuracyEvaluator.GetVariableInfo("Accuracy").ActualName = "ValidationAccuracy";
      SimpleAccuracyEvaluator testAccuracyEvaluator = new SimpleAccuracyEvaluator();
      testAccuracyEvaluator.Name = "TestAccuracyEvaluator";
      testAccuracyEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      testAccuracyEvaluator.GetVariableInfo("Accuracy").ActualName = "TestAccuracy";

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

      seqProc.AddSubOperator(trainingEvaluator);
      seqProc.AddSubOperator(trainingAccuracyEvaluator);
      seqProc.AddSubOperator(validationAccuracyEvaluator);
      seqProc.AddSubOperator(testAccuracyEvaluator);
      seqProc.AddSubOperator(trainingConfusionMatrixEvaluator);
      seqProc.AddSubOperator(validationConfusionMatrixEvaluator);
      seqProc.AddSubOperator(testConfusionMatrixEvaluator);

      seq.AddSubOperator(seqSubScopeProc);
      seqSubScopeProc.AddSubOperator(seqProc);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }


    protected override IAnalyzerModel CreateSVMModel(IScope bestModelScope) {
      IAnalyzerModel model = base.CreateSVMModel(bestModelScope);

      model.SetResult("TrainingAccuracy", bestModelScope.GetVariableValue<DoubleData>("TrainingAccuracy", false).Data);
      model.SetResult("ValidationAccuracy", bestModelScope.GetVariableValue<DoubleData>("ValidationAccuracy", false).Data);
      model.SetResult("TestAccuracy", bestModelScope.GetVariableValue<DoubleData>("TestAccuracy", false).Data);

      return model;
    }
  }
}
