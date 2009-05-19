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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Random {
  /// <summary>
  /// Normally distributed random number generator.
  /// </summary>
  [EmptyStorableClass]
  public class NormalRandomizer : OperatorBase {
    private static int MAX_NUMBER_OF_TRIES = 100;

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Initializes the value of variable 'Value' to a random value normally distributed with 'Mu' and 'Sigma'."; }
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
    /// Initializes a new instance of <see cref="NormalRandomizer"/> with four variable infos
    /// (<c>Mu</c>, <c>Sigma</c>, <c>Value</c> and <c>Random</c>).
    /// </summary>
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

    /// <summary>
    /// Generates a new normally distributed random variable and assigns it to the specified variable
    /// in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to assign the new random value to.</param>
    /// <returns><c>null</c>.</returns>
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

    /// <summary>
    /// Generates a new double random variable based on a continuous, normally distributed random number generator
    /// <paramref name="normal"/> and checks some contraints.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when with the given settings no valid value in
    /// 100 tries could be found.
    /// </exception>
    /// <param name="data">The double object where to assign the new number to and whose constraints
    /// must be fulfilled.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
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

    /// <summary>
    /// Generates a new int random variable based on a continuous, normally distributed random number 
    /// generator <paramref name="normal"/> and checks some constraints.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when with the given settings no valid
    /// value could be found.</exception>
    /// <param name="data">The int object where to assign the new value to and whose constraints must
    /// be fulfilled.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void RandomizeNormal(ConstrainedIntData data, NormalDistributedRandom normal) {
      for (int tries = MAX_NUMBER_OF_TRIES; tries >= 0; tries--) {
        double r = normal.NextDouble();
        if (data.TrySetData((int)Math.Round(r))) // since r is a continuous, normally distributed random variable rounding should be OK
          return;
      }
      throw new InvalidOperationException("Couldn't find a valid value");
    }

    /// <summary>
    /// Generates a new double random number based on a continuous, normally distributed random number
    /// generator <paramref name="normal"/>.
    /// </summary>
    /// <param name="data">The double object where to assign the new value to.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
    public void RandomizeNormal(DoubleData data, NormalDistributedRandom normal) {
      data.Data = normal.NextDouble();
    }

    /// <summary>
    /// Generates a new int random number based on a continuous, normally distributed random number 
    /// generator <paramref name="normal"/>.
    /// </summary>
    /// <param name="data">The int object where to assign the new value to.</param>
    /// <param name="normal">The continuous, normally distributed random variable.</param>
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
