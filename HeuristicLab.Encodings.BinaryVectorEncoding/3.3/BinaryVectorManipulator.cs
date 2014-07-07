#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// A base class for operators that manipulate bool-valued vectors.
  /// </summary>
  [Item("BinaryVectorManipulator", "A base class for operators that manipulate bool-valued vectors.")]
  [StorableClass]
  public abstract class BinaryVectorManipulator : InstrumentedOperator, IBinaryVectorManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }

    [StorableConstructor]
    protected BinaryVectorManipulator(bool deserializing) : base(deserializing) { }
    protected BinaryVectorManipulator(BinaryVectorManipulator original, Cloner cloner) : base(original, cloner) { }
    protected BinaryVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The vector which should be manipulated."));
    }

    public sealed override IOperation InstrumentedApply() {
      Manipulate(RandomParameter.ActualValue, BinaryVectorParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract void Manipulate(IRandom random, BinaryVector binaryVector);
  }
}
