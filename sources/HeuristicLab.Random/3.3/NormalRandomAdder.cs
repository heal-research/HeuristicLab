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
  /// Normally distributed random number generator that adds the generated value to the existing value
  /// in the specified scope.
  /// </summary>
  public class NormalRandomAdder : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Samples a normally distributed (mu, sigma * shakingFactor) random variable and adds the result to variable 'Value'.
        
If a constraint for the allowed range of 'Value' is defined and the result of the operation would be smaller then 
the smallest allowed value then 'Value' is set to the lower bound and vice versa for the upper bound.";
      }
    }

    /// <summary>
    /// Gets or sets the value for µ.
    /// </summary>
    /// <remarks>Gets or sets the variable with the name <c>Mu</c> through the method 
    /// <see cref="OperatorBase.GetVariable"/> of class <see cref="OperatorBase"/>.</remarks>
    public double Mu {
      get { return ((DoubleData)GetVariable("Mu").Value).Data; }
      set { ((DoubleData)GetVariable("Mu").Value).Data = value; }
    }
    /// <summary>
    /// Gets or sets the value for sigma.
    /// </summary>
    /// <remarks>Gets or sets the variable with the name <c>Sigma</c> through the method 
    /// <see cref="OperatorBase.GetVariable"/> of class <see cref="OperatorBase"/>.</remarks>
    public double Sigma {
      get { return ((DoubleData)GetVariable("Sigma").Value).Data; }
      set { ((DoubleData)GetVariable("Sigma").Value).Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalRandomAdder"/> with five variable infos
    /// (<c>Mu</c>, <c>Sigma</c>, <c>Value</c>, <c>ShakingFactor</c> and <c>Random</c>).
    /// </summary>
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

    /// <summary>
    /// Generates a new normally distributed random number and adds it to the specified value in the
    /// given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to add the generated random number.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IObjectData value = GetVariableValue<IObjectData>("Value", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      double factor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      double mu = GetVariableValue<DoubleData>("Mu", scope, true).Data;
      double sigma = GetVariableValue<DoubleData>("Sigma", scope, true).Data;
      NormalDistributedRandom normal = new NormalDistributedRandom(mt, mu, sigma * factor);

      AddNormal(value, normal);
      return null;
    }

    private void AddNormal(IObjectData value, NormalDistributedRandom normal) {
      // dispatch manually based on dynamic type
      if (value is IntData)
        AddNormal((IntData)value, normal);
      else if (value is ConstrainedIntData)
        AddNormal((ConstrainedIntData)value, normal);
      else if (value is ConstrainedDoubleData)
        AddNormal((ConstrainedDoubleData)value, normal);
      else if (value is DoubleData)
        AddNormal((DoubleData)value, normal);
      else throw new InvalidOperationException("Can't handle type " + value.GetType().Name);
    }
    /// <summary>
    /// Generates a new double random number and adds it to the value of 
    /// the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The double object where to add the random number.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void AddNormal(DoubleData data, NormalDistributedRandom normal) {
      data.Data += normal.NextDouble();
    }

    /// <summary>
    /// Generates a new double random number and adds it to the value of the given <paramref name="data"/>
    /// checking its constraints.
    /// </summary>
    /// <exception cref="InvalidProgramException">Thrown when with the current settings no valid value
    /// could be found.</exception>
    /// <param name="data">The double object where to add the random number and whose constraints
    /// to fulfill.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void AddNormal(ConstrainedDoubleData data, NormalDistributedRandom normal) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        double newValue = data.Data + normal.NextDouble();
        if (IsIntegerConstrained(data)) {
          newValue = Math.Round(newValue);
        }
        if (data.TrySetData(newValue)) {
          return;
        }
      }
      throw new InvalidProgramException("Coudn't find a valid value");
    }

    /// <summary>
    /// Generates a new int random number and adds it to value of the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The int object where to add the random number.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void AddNormal(IntData data, NormalDistributedRandom normal) {
      data.Data = (int)Math.Round(data.Data + normal.NextDouble());
    }

    /// <summary>
    /// Generates a new int random number and adds it to the value of the given <paramref name="data"/>
    /// checking its constraints.
    /// </summary>
    /// <exception cref="InvalidProgramException">Thrown when with the current settings no valid value 
    /// could be found.</exception>
    /// <param name="data">The int object where to add the generated value and whose contraints to check.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void AddNormal(ConstrainedIntData data, NormalDistributedRandom normal) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        if (data.TrySetData((int)Math.Round(data.Data + normal.NextDouble())))
          return;
      }
      throw new InvalidProgramException("Couldn't find a valid value.");
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
