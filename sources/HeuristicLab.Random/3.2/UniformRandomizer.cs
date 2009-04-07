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
  /// <summary>
  /// Uniformly distributed random number generator.
  /// </summary>
  public class UniformRandomizer : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Initializes the value of variable 'Value' to a random value uniformly distributed between 'Min' and 'Max' (exclusive)"; }
    }

    /// <summary>
    /// Gets or sets the maximum value of the random number generator (exclusive).
    /// </summary>
    /// <remarks>Gets or sets the variable with name <c>Max</c> through the 
    /// <see cref="OperatorBase.GetVariable"/> method of class <see cref="OperatorBase"/>.</remarks>
    public double Max {
      get { return ((DoubleData)GetVariable("Max").Value).Data; }
      set { ((DoubleData)GetVariable("Max").Value).Data = value; }
    }
    /// <summary>
    /// Gets or sets the minimum value of the random number generator.
    /// </summary>
    /// <remarks>Gets or sets the variable with name <c>Min</c> through the 
    /// <see cref="OperatorBase.GetVariable"/> method of class <see cref="OperatorBase"/>.</remarks>
    public double Min {
      get { return ((DoubleData)GetVariable("Min").Value).Data; }
      set { ((DoubleData)GetVariable("Min").Value).Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformRandomizer"/> with four variable infos 
    /// (<c>Value</c>, <c>Random</c>, <c>Max</c> and <c>Min</c>), being a random number generator 
    /// between 0.0 and 1.0.
    /// </summary>
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

    /// <summary>
    /// Generates a new uniformly distributed random variable.
    /// </summary>
    /// <param name="scope">The scope where to apply the random number generator.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;

      RandomizeUniform(value, mt, min, max);
      return null;
    }

    /// <summary>
    /// Generates a new random number depending on the type of the <paramref name="value"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when an unhandleable type appears.</exception>
    /// <param name="value">The object whose data should be a new randomly generated number.</param>
    /// <param name="mt">The MersenneTwister to generate a new random number.</param>
    /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
    /// <param name="max">The right border (exclusive) of the interval in which the next random number
    /// has to lie.</param>
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

      /// <summary>
      /// Generates a new double random number.
      /// </summary>
      /// <param name="data">The double object which the new value is assigned to.</param>
      /// <param name="mt">The random number generator.</param>
      /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
      /// <param name="max">The right border (exclusive) of the interval in which the next random number
      /// has to lie.</param>
      public void RandomizeUniform(DoubleData data, MersenneTwister mt, double min, double max) {
        data.Data = mt.NextDouble() * (max - min) + min;
      }

      /// <summary>
      /// Generates a new int random number.
      /// </summary>
      /// <param name="data">The int object which the new value is assigned to.</param>
      /// <param name="mt">The random number generator.</param>
      /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
      /// <param name="max">The right border (exclusive) of the interval in which the next random number
      /// has to lie.</param>
      public void RandomizeUniform(IntData data, MersenneTwister mt, double min, double max) {
        data.Data = (int)Math.Floor(mt.NextDouble() * (max - min) + min);
      }

      /// <summary>
      /// Generates a new double random number, being restricted to some constraints.
      /// </summary>
      /// <exception cref="InvalidOperationException">Thrown when no valid value could be found.</exception>
      /// <param name="data">The double object which the new value is assigned to and whose constraints
      /// must be fulfilled.</param>
      /// <param name="mt">The random number generator.</param>
      /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
      /// <param name="max">The right border (exclusive) of the interval in which the next random number
      /// has to lie.</param>
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
      /// <summary>
      /// Generates a new int random number, being restricted to some constraints.
      /// </summary>
      /// <exception cref="InvalidOperationException">Thrown when no valid value could be found.</exception>
      /// <param name="data">The int object which the new value is assigned to and whose constraints
      /// must be fulfilled.</param>
      /// <param name="mt">The random number generator.</param>
      /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
      /// <param name="max">The right border (exclusive) of the interval in which the next random number
      /// has to lie.</param>
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
