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
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification.ConditionalEvaluation {
  public class ConditionalMeanSquaredErrorEvaluator : ConditionalEvaluatorBase {
    public override string OutputVariableName {
      get {
        return "MSE";
      }
    }
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'DataSet' and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.";
      }
    }

    public override double Evaluate(double[,] values) {
      try {
        return SimpleMSEEvaluator.Calculate(values);
      }
      catch (ArgumentException) {
        return double.PositiveInfinity;
      }
    }
  }
}
