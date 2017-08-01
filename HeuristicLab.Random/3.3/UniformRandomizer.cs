#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Random {
  /// <summary>
  /// Uniformly distributed random number generator.
  /// </summary>
  [StorableClass]
  [Item("UniformRandomizer", "Initializes the value of variable 'Value' to a random value uniformly distributed between 'Min' and 'Max'")]
  public class UniformRandomizer : SingleSuccessorOperator {
    #region Parameter Properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<DoubleValue> MinParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Min"]; }
    }
    public IValueLookupParameter<DoubleValue> MaxParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Max"]; }
    }
    public ILookupParameter<DoubleValue> ValueParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    #endregion

    #region Properties
    public DoubleValue Min {
      get { return MinParameter.ActualValue; }
      set { MinParameter.ActualValue = value; }
    }
    public DoubleValue Max {
      get { return MaxParameter.ActualValue; }
      set { MaxParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected UniformRandomizer(bool deserializing) : base(deserializing) { }
    protected UniformRandomizer(UniformRandomizer original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="UniformRandomizer"/> with four variable infos 
    /// (<c>Value</c>, <c>Random</c>, <c>Max</c> and <c>Min</c>), being a random number generator 
    /// between 0.0 and 1.0.
    /// </summary>
    public UniformRandomizer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random generator that supplies uniformly distributed values."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Min", "The minimal allowed value (inclusive)"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Max", "The maximal allowed value (exclusive)"));
      Parameters.Add(new LookupParameter<DoubleValue>("Value", "The value that should be set to a random value."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformRandomizer(this, cloner);
    }

    /// <summary>
    /// Generates a new uniformly distributed random variable.
    /// </summary>
    public override IOperation Apply() {
      IRandom random = RandomParameter.ActualValue;
      double min = MinParameter.ActualValue.Value;
      double max = MaxParameter.ActualValue.Value;

      ValueParameter.ActualValue = new DoubleValue(random.NextDouble() * (max - min) + min);
      return base.Apply();
    }
  }
}
