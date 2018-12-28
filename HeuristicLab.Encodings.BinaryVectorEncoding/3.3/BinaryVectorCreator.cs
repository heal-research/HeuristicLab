#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// A base class for operators creating bool-valued vectors.
  /// </summary>
  [Item("BinaryVectorCreator", "A base class for operators creating bool-valued vectors.")]
  [StorableType("5CE4B676-CE68-4338-AF85-A289D3C8BA69")]
  public abstract class BinaryVectorCreator : InstrumentedOperator, IBinaryVectorCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }

    [StorableConstructor]
    protected BinaryVectorCreator(StorableConstructorFlag _) : base(_) { }
    protected BinaryVectorCreator(BinaryVectorCreator original, Cloner cloner) : base(original, cloner) { }
    protected BinaryVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector."));
    }

    public sealed override IOperation InstrumentedApply() {
      BinaryVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract BinaryVector Create(IRandom random, IntValue length);
  }
}
