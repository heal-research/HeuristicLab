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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.SupportVectorMachines {
  public class VariableEvaluationImpactCalculator : HeuristicLab.Modeling.VariableEvaluationImpactCalculator {

    public VariableEvaluationImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("SVMModel", "The model that should be evaluated", typeof(SVMModel), VariableKind.In));
    }


    protected override double[] GetOutputs(IScope scope, Dataset dataset, int targetVariable, ItemList<IntData> allowedFeatures, int start, int end) {
      SVMModel model = GetVariableValue<SVMModel>("SVMModel", scope, true);
      SVM.Problem problem = SVMHelper.CreateSVMProblem(dataset, allowedFeatures, targetVariable, start, end);
      SVM.Problem scaledProblem = SVM.Scaling.Scale(problem, model.RangeTransform);

      double[] values = new double[end - start];
      for (int i = 0; i < end - start; i++) {
        values[i] = SVM.Prediction.Predict(model.Model, scaledProblem.X[i]);
      }
      return values;
    }
  }
}
