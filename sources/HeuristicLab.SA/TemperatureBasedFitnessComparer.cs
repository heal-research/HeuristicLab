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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SA {
  public class TemperatureBasedFitnessComparer : OperatorBase {

    public override string Description {
      get { return @"Compares the quality of two parents considering a certain dampening factor (temperature).
A mutant is successful if it is either better than its parent or with a probability e^(-FitnessDifference / Temperature)."; }
    }

    public TemperatureBasedFitnessComparer()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "The PRNG to use (Uniform)", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Whether the problem is a maximization or minimization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "The variable that holds the fitness value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Temperature", "The current temperature", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SuccessfulChild", "Boolean variable that tells if a child is successful (true) or not (false)", typeof(BoolData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      double mutantQuality = GetVariableValue<DoubleData>("Quality", scope, false).Data;
      double parentQuality = scope.SubScopes[0].GetVariableValue<DoubleData>("Quality", false).Data;
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;

      // if mutant is better, accept it
      if (maximization && mutantQuality > parentQuality || !maximization && mutantQuality < parentQuality) {
        BoolData sc = GetVariableValue<BoolData>("SuccessfulChild", scope, false, false);
        if (sc == null) {
          if (GetVariableInfo("SuccessfulChild").Local) {
            AddVariable(new Variable("SuccessfulChild", new BoolData(true)));
          } else {
            scope.AddVariable(new Variable("SuccessfulChild", new BoolData(true)));
          }
        } else sc.Data = true;
        return null;
      }

      IRandom random = scope.GetVariableValue<IRandom>("Random", true);
      double temperature = scope.GetVariableValue<DoubleData>("Temperature", true).Data;

      double probability = Math.Exp(-Math.Abs(mutantQuality - parentQuality) / temperature);
      bool success = random.NextDouble() < probability;
      BoolData sc2 = GetVariableValue<BoolData>("SuccessfulChild", scope, false, false);
      if (sc2 == null) {
        if (GetVariableInfo("SuccessfulChild").Local) {
          AddVariable(new Variable("SuccessfulChild", new BoolData(true)));
        } else {
          scope.AddVariable(new Variable("SuccessfulChild", new BoolData(true)));
        }
      } else sc2.Data = success;
      return null;
    }
  }
}
