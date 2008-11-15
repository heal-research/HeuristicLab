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
  public class NormalRandomizer : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;

    public override string Description {
      get { return "Initializes the value of variable 'Value' to a random value normally distributed with 'Mu' and 'Sigma'."; }
    }

    public double Mu {
      get { return ((DoubleData)GetVariable("Mu").Value).Data; }
      set { ((DoubleData)GetVariable("Mu").Value).Data = value; }
    }
    public double Sigma {
      get { return ((DoubleData)GetVariable("Sigma").Value).Data; }
      set { ((DoubleData)GetVariable("Sigma").Value).Data = value; }
    }

    public NormalRandomizer() {
      AddVariableInfo(new VariableInfo("Mu", "Parameter mu of the normal distribution", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Mu").Local = true;
      AddVariable(new Variable("Mu", new DoubleData(0.0)));

      AddVariableInfo(new VariableInfo("Sigma", "Parameter sigma of the normal distribution", typeof(DoubleData), VariableKind.None));
      GetVariableInfo("Sigma").Local = true;
      AddVariable(new Variable("Sigma", new DoubleData(1.0)));

      AddVariableInfo(new VariableInfo("Value", "The value to manipulate (actual type is one of: IntData, DoubleData, ConstrainedIntData, ConstrainedDoubleData)", typeof(IObjectData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The random generator to use", typeof(MersenneTwister), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double mu = GetVariableValue<DoubleData>("Mu", scope, true).Data;
      double sigma = GetVariableValue<DoubleData>("Sigma", scope, true).Data;

      NormalDistributedRandom n = new NormalDistributedRandom(mt, mu, sigma);
      RandomizeNormal(value, n);
      return null;
    }

    private void RandomizeNormal(IObjectData value, NormalDistributedRandom n) {
      // dispatch manually based on dynamic type
      if (value is IntData)
        RandomizeNormal((IntData)value, n);
      else if (value is ConstrainedIntData)
        RandomizeNormal((ConstrainedIntData)value, n);
      else if (value is DoubleData)
        RandomizeNormal((DoubleData)value, n);
      else if (value is ConstrainedDoubleData)
        RandomizeNormal((ConstrainedDoubleData)value, n);
      else throw new InvalidOperationException("Can't handle type " + value.GetType().Name);
    }

    public void RandomizeNormal(ConstrainedDoubleData data, NormalDistributedRandom normal) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        double r = normal.NextDouble();
        if (IsIntegerConstrained(data)) {
          r = Math.Round(r);
        }
        if (data.TrySetData(r)) {
          return;
        }
      }
      throw new InvalidOperationException("Couldn't find a valid value in 100 tries with mu=" + normal.Mu + " sigma=" + normal.Sigma);
    }

    public void RandomizeNormal(ConstrainedIntData data, NormalDistributedRandom normal) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        double r = normal.NextDouble();
        if (data.TrySetData((int)Math.Round(r))) // since r is a continuous normally distributed random variable rounding should be OK
          return;
      }
      throw new InvalidOperationException("Couldn't find a valid value");
    }

    public void RandomizeNormal(DoubleData data, NormalDistributedRandom normal) {
      data.Data = normal.NextDouble();
    }

    public void RandomizeNormal(IntData data, NormalDistributedRandom normal) {
      data.Data = (int)Math.Round(normal.NextDouble());
    }


    private bool IsIntegerConstrained(ConstrainedDoubleData data) {
      foreach (IConstraint constraint in data.Constraints) {
        if (constraint is IsIntegerConstraint) {
          return true;
        }
      }
      return false;
    }
  }
}
