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
  /// Uniformly distributed random number generator that adds the generated value to the existing one.
  /// </summary>
  public class UniformRandomAdder : OperatorBase {

    private static int MAX_NUMBER_OF_TRIES = 100;

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Samples a uniformly distributed random variable 'U' with range = [min,max] and E(u) = (max-min)/2
and adds the result to the variable 'Value'. ShakingFactor influences the effective range of U. 
If r=(max-min) then the effective range of U is [E(u) - shakingFactor * r/2, E(u) + shakingFactor * r/2].

If a constraint for the allowed range of 'Value' is defined and the result of the operation would be smaller then 
the smallest allowed value then 'Value' is set to the lower bound and vice versa for the upper bound.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformRandomAdder"/> with five variable infos
    /// (<c>Value</c>, <c>ShakingFactor</c>, <c>Random</c>, <c>Min</c> and <c>Max</c>), having an interval
    /// of -1.0 to 1.0.
    /// </summary>
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

    /// <summary>
    /// Generates a new uniformly distributed random number and adds it to the value in the given
    /// <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The current scope where to add the generated random number.</param>
    /// <returns>null.</returns>
    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double factor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;

      double ex = (max - min) / 2.0 + min;
      double newRange = (max - min) * factor;
      min = ex - newRange / 2;
      max = ex + newRange / 2;

      AddUniform(value, mt, min, max);
      return null;
    }

    private void AddUniform(IObjectData value, MersenneTwister mt, double min, double max) {
      // dispatch manually on dynamic type
      if (value is IntData)
        AddUniform((IntData)value, mt, min, max);
      else if (value is ConstrainedIntData)
        AddUniform((ConstrainedIntData)value, mt, min, max);
      else if (value is DoubleData)
        AddUniform((DoubleData)value, mt, min, max);
      else if (value is ConstrainedDoubleData)
        AddUniform((ConstrainedDoubleData)value, mt, min, max);
      else throw new InvalidOperationException("Can't handle type " + value.GetType().Name);
    }

    /// <summary>
    /// Adds a new double random variable being restricted to some constraints to the value of the given 
    /// <paramref name="data"/>.
    /// </summary>
    /// <exception cref="InvalidProgramException">Thrown when no valid value can be found.</exception>
    /// <param name="data">The data where to add the new variable and where to assign the new value to.</param>
    /// <param name="mt">The random number generator.</param>
    /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
    /// <param name="max">The right border (exclusive) of the interval in which the next random number
    /// has to lie.</param>
    public void AddUniform(ConstrainedDoubleData data, MersenneTwister mt, double min, double max) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        double newValue = data.Data + mt.NextDouble() * (max - min) + min;
        if (IsIntegerConstrained(data)) {
          newValue = Math.Floor(newValue);
        }
        if (data.TrySetData(newValue)) {
          return;
        }
      }
      throw new InvalidProgramException("Couldn't find a valid value");
    }

    /// <summary>
    /// Adds a new int random variable being restricted to some constraints to the value of the given
    /// <paramref name="data"/>.
    /// </summary>
    /// <exception cref="InvalidProgramException">Thrown when no valid value could be found.</exception>
    /// <param name="data">The data where to add the random value and where to assign the new value to.</param>
    /// <param name="mt">The random number generator.</param>
    /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
    /// <param name="max">The right border (exclusive) of the interval in which the next random number
    /// has to lie.</param>
    public void AddUniform(ConstrainedIntData data, MersenneTwister mt, double min, double max) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        int newValue = (int)Math.Floor(data.Data + mt.NextDouble() * (max - min) + min);
        if (data.TrySetData(newValue)) {
          return;
        }
      }
      throw new InvalidProgramException("Couldn't find a valid value");
    }

    /// <summary>
    /// Generates a new double random variable and adds it to the value of 
    /// the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The double object where to add the random value.</param>
    /// <param name="mt">The random number generator.</param>
    /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
    /// <param name="max">The right border (exclusive) of the interval in which the next random number
    /// has to lie.</param>
    public void AddUniform(DoubleData data, MersenneTwister mt, double min, double max) {
      data.Data = data.Data + mt.NextDouble() * (max - min) + min;
    }

    /// <summary>
    /// Generates a new int random variable and adds it to the value of the given
    /// <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The int object where to add the random value.</param>
    /// <param name="mt">The random number generator.</param>
    /// <param name="min">The left border of the interval in which the next random number has to lie.</param>
    /// <param name="max">The right border (exclusive) of the interval in which the next random number
    /// has to lie.</param>
    public void AddUniform(IntData data, MersenneTwister mt, double min, double max) {
      data.Data = (int)Math.Floor(data.Data + mt.NextDouble() * (max - min) + min);
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
