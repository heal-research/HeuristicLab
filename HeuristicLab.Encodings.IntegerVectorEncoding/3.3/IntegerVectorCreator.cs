#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// A base class for operators creating int-valued vectors.
  /// </summary>
  [Item("IntegerVectorCreator", "A base class for operators creating int-valued vectors.")]
  [StorableClass]
  public abstract class IntegerVectorCreator : SingleSuccessorOperator, IIntegerVectorCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }
    public IValueLookupParameter<IntValue> MinimumParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Minimum"]; }
    }
    public IValueLookupParameter<IntValue> MaximumParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Maximum"]; }
    }

    [StorableConstructor]
    protected IntegerVectorCreator(bool deserializing) : base(deserializing) { }
    protected IntegerVectorCreator(IntegerVectorCreator original, Cloner cloner) : base(original, cloner) { }
    protected IntegerVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Minimum", "The inclusive lower bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Maximum", "The exclusive upper bound for each element in the vector."));
    }

    public sealed override IOperation Apply() {
      IntegerVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue, MinimumParameter.ActualValue, MaximumParameter.ActualValue);
      return base.Apply();
    }

    protected abstract IntegerVector Create(IRandom random, IntValue length, IntValue minimum, IntValue maximum);
  }
}
