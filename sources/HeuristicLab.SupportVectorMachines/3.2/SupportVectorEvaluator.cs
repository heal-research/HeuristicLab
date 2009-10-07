#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using SVM;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorEvaluator : OperatorBase {

    public SupportVectorEvaluator()
      : base() {
      //Dataset infos
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "(optional) Maximal allowed time offset for input variables", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "(optional) Minimal allowed time offset for input variables", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMModel", "Represent the model learned by the SVM", typeof(SVMModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(DoubleMatrixData), VariableKind.New | VariableKind.Out));
    }


    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      IntData minTimeOffsetData = GetVariableValue<IntData>("MinTimeOffset", scope, true, false);
      int minTimeOffset = minTimeOffsetData == null ? 0 : minTimeOffsetData.Data;
      IntData maxTimeOffsetData = GetVariableValue<IntData>("MaxTimeOffset", scope, true, false);
      int maxTimeOffset = maxTimeOffsetData == null ? 0 : maxTimeOffsetData.Data;
      SVMModel modelData = GetVariableValue<SVMModel>("SVMModel", scope, true);
      SVM.Problem problem = SVMHelper.CreateSVMProblem(dataset, targetVariable, start, end, minTimeOffset, maxTimeOffset);
      SVM.Problem scaledProblem = modelData.RangeTransform.Scale(problem);

      double[,] values = new double[scaledProblem.Count, 2];
      for (int i = 0; i < scaledProblem.Count; i++) {
        values[i, 0] = dataset.GetValue(start + i, targetVariable);
        values[i, 1] = SVM.Prediction.Predict(modelData.Model, scaledProblem.X[i]);
      }

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Values"), new DoubleMatrixData(values)));
      return null;
    }
  }
}
