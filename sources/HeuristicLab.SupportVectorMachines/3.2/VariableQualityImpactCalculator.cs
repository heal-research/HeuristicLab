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
  public class VariableQualityImpactCalculator : HeuristicLab.Modeling.VariableQualityImpactCalculator {

    public VariableQualityImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("SVMModel", "The model that should be evaluated", typeof(SVMModel), VariableKind.In));
    }

    protected override double CalculateQuality(IScope scope, Dataset dataset, int targetVariable, int start, int end) {
      SVMModel model = GetVariableValue<SVMModel>("SVMModel", scope, true);
      SVM.Problem problem = SVMHelper.CreateSVMProblem(dataset, targetVariable, start, end);
      SVM.Problem scaledProblem = SVM.Scaling.Scale(problem, model.RangeTransform);

      double[,] values = new double[end - start, 2];
      for (int i = 0; i < end - start; i++) {
        values[i, 0] = SVM.Prediction.Predict(model.Model, scaledProblem.X[i]);
        values[i, 1] = dataset.GetValue(start + i, targetVariable);
      }

      try { return HeuristicLab.Modeling.SimpleMSEEvaluator.Calculate(values); }
      catch (ArgumentException) { return double.PositiveInfinity; }
    }
  }
}
