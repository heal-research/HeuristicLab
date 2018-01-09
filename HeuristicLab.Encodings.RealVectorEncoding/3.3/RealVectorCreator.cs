#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// A base class for operators creating real-valued vectors.
  /// </summary>
  [Item("RealVectorCreator", "A base class for operators creating real-valued vectors.")]
  [StorableClass]
  public abstract class RealVectorCreator : InstrumentedOperator, IRealVectorCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected RealVectorCreator(bool deserializing) : base(deserializing) { }
    protected RealVectorCreator(RealVectorCreator original, Cloner cloner) : base(original, cloner) { }
    protected RealVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled."));
    }

    public sealed override IOperation InstrumentedApply() {
      RealVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue, BoundsParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract RealVector Create(IRandom random, IntValue length, DoubleMatrix bounds);
  }
}
