#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Normally distributed random number generator.
  /// </summary>
  [StorableClass]
  [Item("NormalRandomizer", "Initializes the value of variable 'Value' to a random value normally distributed with parameters 'Mu' and 'Sigma'")]
  public class NormalRandomizer : SingleSuccessorOperator {
    #region Parameter Properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<DoubleValue> MuParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Mu"]; }
    }
    public IValueLookupParameter<DoubleValue> SigmaParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Sigma"]; }
    }
    public ILookupParameter<DoubleValue> ValueParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    #endregion

    #region Properties
    public DoubleValue Mu {
      get { return MuParameter.ActualValue; }
      set { MuParameter.ActualValue = value; }
    }
    public DoubleValue Max {
      get { return SigmaParameter.ActualValue; }
      set { SigmaParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected NormalRandomizer(bool deserializing) : base(deserializing) { }
    protected NormalRandomizer(NormalRandomizer original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="NormalRandomizer"/> with four variable infos
    /// (<c>Mu</c>, <c>Sigma</c>, <c>Value</c> and <c>Random</c>).
    /// </summary>
    public NormalRandomizer() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random generator that supplies uniformly distributed values."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Mu", "Mu parameter of the normal distribution (N(mu,sigma))."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Sigma", "Sigma parameter of the normal distribution (N(mu,sigma))."));
      Parameters.Add(new LookupParameter<DoubleValue>("Value", "The value that should be set to a random value."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NormalRandomizer(this, cloner);
    }

    /// <summary>
    /// Generates a new normally distributed random variable and assigns it to the specified variable.
    /// </summary>
    public override IOperation Apply() {
      IRandom random = RandomParameter.ActualValue;
      double mu = MuParameter.ActualValue.Value;
      double sigma = SigmaParameter.ActualValue.Value;

      NormalDistributedRandom normalRandom = new NormalDistributedRandom(random, mu, sigma);
      ValueParameter.ActualValue = new DoubleValue(normalRandom.NextDouble());
      return base.Apply();
    }
  }
}
