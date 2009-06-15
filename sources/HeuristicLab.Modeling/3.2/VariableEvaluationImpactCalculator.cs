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

namespace HeuristicLab.Modeling {
  public abstract class VariableEvaluationImpactCalculator : VariableImpactCalculatorBase<double[]> {
    public override string OutputVariableName {
      get { return "VariableEvaluationImpacts"; }
    }

    public override string Description {
      get { return @"Calculates the impact of all allowed input variables on the model outputs using evaluator supplied as suboperator."; }
    }

    private double[,] CombineOutputs(double[] referenceOutputs, double[] newOutputs) {
      if (referenceOutputs.Length != newOutputs.Length) throw new InvalidProgramException();
      double[,] result = new double[referenceOutputs.Length, 2];
      for (int i = 0; i < referenceOutputs.Length; i++) {
        result[i, 0] = referenceOutputs[i];
        result[i, 1] = newOutputs[i];
      }
      return result;
    }

    protected override double CalculateImpact(double[] referenceValue, double[] newValue) {
      return SimpleMSEEvaluator.Calculate(CombineOutputs(referenceValue, newValue));
    }

    protected override double[] CalculateValue(IScope scope, Dataset dataset, int targetVariable, ItemList<IntData> allowedFeatures, int start, int end) {
      return GetOutputs(scope, dataset, targetVariable, allowedFeatures, start, end);
    }

    protected override double[] PostProcessImpacts(double[] impacts) {
      double mseSum = impacts.Sum();
      if (mseSum == 0.0) mseSum = 1.0;
      for (int i = 0; i < impacts.Length; i++) {
        impacts[i] = impacts[i] / mseSum;
      }
      return impacts;
    }

    protected abstract double[] GetOutputs(IScope scope, Dataset dataset, int targetVariable, ItemList<IntData> allowedFeatures, int start, int end);
  }
}
