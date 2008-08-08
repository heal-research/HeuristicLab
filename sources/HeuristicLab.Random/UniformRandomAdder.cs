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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.Random {
  public class UniformRandomAdder : OperatorBase {

    private static int MAX_NUMBER_OF_TRIES = 100;

    public override string Description {
      get { return @"Samples a uniformly distributed random variable 'U' with range = [min,max] and E(u) = (max-min)/2
and adds the result to the variable 'Value'. ShakingFactor influences the effective range of U. 
If r=(max-min) then the effective range of U is [E(u) - shakingFactor * r/2, E(u) + shakingFactor * r/2].

If a constraint for the allowed range of 'Value' is defined and the result of the operation would be smaller then 
the smallest allowed value then 'Value' is set to the lower bound and vice versa for the upper bound."; }
    }

    public UniformRandomAdder() {
      AddVariableInfo(new VariableInfo("Value", "The value to manipulate (type is one of: IntData, ConstrainedIntData, DoubleData, ConstrainedDoubleData)", typeof(IObjectData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Determines the force of the shaking factor", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The random generator to use", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("Min", "Lower bound of the uniform distribution (inclusive)", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Min").Local = true;
      AddVariable(new Variable("Min", new DoubleData(-1.0)));

      AddVariableInfo(new VariableInfo("Max", "Upper bound of the uniform distribution (exclusive)", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Max").Local = true;
      AddVariable(new Variable("Max", new DoubleData(1.0)));
    }

    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double factor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      double min = GetVariableValue<DoubleData>("Min", null, false).Data;
      double max = GetVariableValue<DoubleData>("Max", null, false).Data;

      double ex = (max - min) / 2.0 + min;
      double newRange = (max - min) * factor;
      min = ex - newRange / 2;
      max = ex + newRange / 2;

      value.Accept(new RandomAdderVisitor(mt, min, max));

      return null;
    }


    private class RandomAdderVisitor : ObjectDataVisitorBase {
      private double min;
      private double max;
      private MersenneTwister mt;

      public RandomAdderVisitor(MersenneTwister mt, double min, double max) {
        this.mt = mt;
        this.min = min;
        this.max = max;
      }

      public override void Visit(ConstrainedDoubleData data) {
        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          double newValue = data.Data + mt.NextDouble() * (max - min) + min;
          if(IsIntegerConstrained(data)) {
            newValue = Math.Floor(newValue);
          }
          if(data.TrySetData(newValue)) {
            return;
          }
        }
        throw new InvalidProgramException("Couldn't find a valid value");
      }

      public override void Visit(ConstrainedIntData data) {
        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          int newValue = (int)Math.Floor(data.Data + mt.NextDouble() * (max - min) + min);
          if(data.TrySetData(newValue)) {
            return;
          }
        }
        throw new InvalidProgramException("Couldn't find a valid value");
      }

      public override void Visit(DoubleData data) {
        data.Data = data.Data + mt.NextDouble() * (max - min) + min;
      }

      public override void Visit(IntData data) {
        data.Data = (int)Math.Floor(data.Data + mt.NextDouble() * (max - min) + min);
      }
      private bool IsIntegerConstrained(ConstrainedDoubleData data) {
        foreach(IConstraint constraint in data.Constraints) {
          if(constraint is IsIntegerConstraint) {
            return true;
          }
        }
        return false;
      }
    }
  }
}
