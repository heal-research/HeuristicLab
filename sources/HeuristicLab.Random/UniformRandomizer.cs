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
  public class UniformRandomizer : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;
    public override string Description {
      get { return "Initializes the value of variable 'Value' to a random value uniformly distributed between 'Min' and 'Max' (exclusive)"; }
    }

    public double Max {
      get { return ((DoubleData)GetVariable("Max").Value).Data; }
      set { ((DoubleData)GetVariable("Max").Value).Data = value; }
    }
    public double Min {
      get { return ((DoubleData)GetVariable("Min").Value).Data; }
      set { ((DoubleData)GetVariable("Min").Value).Data = value; }
    }

    public UniformRandomizer() {
      AddVariableInfo(new VariableInfo("Value", "The value to manipulate (type is one of: IntData, ConstrainedIntData, DoubleData, ConstrainedDoubleData)", typeof(IObjectData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The random generator to use", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("Min", "Lower bound of the uniform distribution (inclusive)", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Min").Local = true;
      AddVariable(new Variable("Min", new DoubleData(0.0)));

      AddVariableInfo(new VariableInfo("Max", "Upper bound of the uniform distribution (exclusive)", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Max").Local = true;
      AddVariable(new Variable("Max", new DoubleData(1.0)));
    }

    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;

      RandomizeUniform(value, mt, min, max);
      return null;
    }

    private void RandomizeUniform(IObjectData value, MersenneTwister mt, double min, double max) {
      // Dispatch manually based on dynamic type,
      // a bit awkward but necessary until we create a better type hierarchy for numeric types (gkronber 15.11.2008).
      if (value is DoubleData)
        RandomizeUniform((DoubleData)value, mt, min, max);
      else if (value is ConstrainedDoubleData)
        RandomizeUniform((ConstrainedDoubleData)value, mt, min, max);
      else if (value is IntData)
        RandomizeUniform((IntData)value, mt, min, max);
      else if (value is ConstrainedIntData)
        RandomizeUniform((ConstrainedIntData)value, mt, min, max);
      else throw new ArgumentException("Can't handle type " + value.GetType().Name);
    }


      public void RandomizeUniform(DoubleData data, MersenneTwister mt, double min, double max) {
        data.Data = mt.NextDouble() * (max - min) + min;
      }

      public void RandomizeUniform(IntData data, MersenneTwister mt, double min, double max) {
        data.Data = (int)Math.Floor(mt.NextDouble() * (max - min) + min);
      }

      public void RandomizeUniform(ConstrainedDoubleData data, MersenneTwister mt, double min, double max) {
        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          double r = mt.NextDouble() * (max - min) + min;
          if(IsIntegerConstrained(data)) {
            r = Math.Floor(r);
          }
          if(data.TrySetData(r)) {
            return;
          }
        }
        throw new InvalidOperationException("Couldn't find a valid value");
      }

      public void RandomizeUniform(ConstrainedIntData data, MersenneTwister mt, double min, double max) {
        for(int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
          int r = (int)Math.Floor(mt.NextDouble() * (max - min) + min);
          if(data.TrySetData(r)) {
            return;
          }
        }
        throw new InvalidOperationException("Couldn't find a valid value");
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
