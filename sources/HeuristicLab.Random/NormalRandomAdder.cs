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
  public class NormalRandomAdder : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;

    public override string Description {
      get {
        return @"Samples a normally distributed (mu, sigma * shakingFactor) random variable and adds the result to variable 'Value'.
        
If a constraint for the allowed range of 'Value' is defined and the result of the operation would be smaller then 
the smallest allowed value then 'Value' is set to the lower bound and vice versa for the upper bound.";
      }
    }

    public double Mu {
      get { return ((DoubleData)GetVariable("Mu").Value).Data; }
      set { ((DoubleData)GetVariable("Mu").Value).Data = value; }
    }
    public double Sigma {
      get { return ((DoubleData)GetVariable("Sigma").Value).Data; }
      set { ((DoubleData)GetVariable("Sigma").Value).Data = value; }
    }

    public NormalRandomAdder() {
      AddVariableInfo(new VariableInfo("Mu", "Parameter mu of the normal distribution", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Mu").Local = true;
      AddVariable(new Variable("Mu", new DoubleData(0.0)));

      AddVariableInfo(new VariableInfo("Sigma", "Parameter sigma of the normal distribution", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Sigma").Local = true;
      AddVariable(new Variable("Sigma", new DoubleData(1.0)));

      AddVariableInfo(new VariableInfo("Value", "The value to manipulate (actual type is one of: IntData, DoubleData, ConstrainedIntData, ConstrainedDoubleData)", typeof(IObjectData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Determines the force of the shaking factor (effective sigma = sigma * shakingFactor)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The random generator to use", typeof(MersenneTwister), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double factor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      double mu = GetVariableValue<DoubleData>("Mu", null, false).Data;
      double sigma = GetVariableValue<DoubleData>("Sigma", null, false).Data;
      NormalDistributedRandom normal = new NormalDistributedRandom(mt, mu, sigma * factor);

      value.Accept(new RandomAdderVisitor(normal));

      return null;
    }


    private class RandomAdderVisitor : ObjectDataVisitorBase {
      private NormalDistributedRandom normal;
      public RandomAdderVisitor(NormalDistributedRandom normal) {
        this.normal = normal;
      }

      public override void Visit(DoubleData data) {
        data.Data += normal.NextDouble();
      }

      public override void Visit(ConstrainedDoubleData data) {

        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          double newValue = data.Data + normal.NextDouble();

          if(IsIntegerConstrained(data)) {
            newValue = Math.Round(newValue);
          }
          if(data.TrySetData(newValue)) {
            return;
          }
        }

        throw new InvalidProgramException("Coudn't find a valid value");
      }

      public override void Visit(IntData data) {
        data.Data = (int)Math.Round(data.Data + normal.NextDouble());
      }

      public override void Visit(ConstrainedIntData data) {
        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          if(data.TrySetData((int)Math.Round(data.Data + normal.NextDouble())))
            return;
        }

        throw new InvalidProgramException("Couldn't find a valid value.");
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
