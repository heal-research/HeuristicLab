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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// A base class for operators that manipulate int-valued vectors.
  /// </summary>
  [Item("IntegerVectorManipulator", "A base class for operators that manipulate int-valued vectors.")]
  [StorableClass]
  public abstract class IntegerVectorManipulator : IntegerVectorOperator, IIntegerVectorManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }

    [StorableConstructor]
    protected IntegerVectorManipulator(bool deserializing) : base(deserializing) { }
    protected IntegerVectorManipulator(IntegerVectorManipulator original, Cloner cloner) : base(original, cloner) { }
    protected IntegerVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The vector which should be manipulated."));
    }

    public sealed override IOperation InstrumentedApply() {
      Manipulate(RandomParameter.ActualValue, IntegerVectorParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract void Manipulate(IRandom random, IntegerVector integerVector);
  }
}
