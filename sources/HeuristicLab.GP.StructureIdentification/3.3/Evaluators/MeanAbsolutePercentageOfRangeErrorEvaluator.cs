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
using HeuristicLab.Operators;
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  public class MeanAbsolutePercentageOfRangeErrorEvaluator : SimpleGPEvaluatorBase {
    public override string OutputVariableName {
      get {
        return "MAPRE";
      }
    }
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the mean of the absolute percentage error (scale invariant) relative to the range of the target varibale of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public override double Evaluate(double[,] values) {
      try { return SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate(values); }
      catch (ArgumentException) {
        return double.PositiveInfinity;
      }
    }
  }
}
