#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntVector {
  /// <summary>
  /// A base class for operators creating int-valued vectors.
  /// </summary>
  [Item("IntVectorCreator", "A base class for operators creating int-valued vectors.")]
  [StorableClass]
  public abstract class IntVectorCreator : SingleSuccessorOperator, IIntVectorCreator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<IntArrayData> IntVectorParameter {
      get { return (ILookupParameter<IntArrayData>)Parameters["IntVector"]; }
    }
    public IValueLookupParameter<IntData> LengthParameter {
      get { return (IValueLookupParameter<IntData>)Parameters["Length"]; }
    }
    public IValueLookupParameter<IntData> MinimumParameter {
      get { return (IValueLookupParameter<IntData>)Parameters["Minimum"]; }
    }
    public IValueLookupParameter<IntData> MaximumParameter {
      get { return (IValueLookupParameter<IntData>)Parameters["Maximum"]; }
    }

    protected IntVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<IntArrayData>("IntVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntData>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<IntData>("Minimum", "The lower bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<IntData>("Maximum", "The upper bound for each element in the vector."));
    }

    public sealed override IOperation Apply() {
      IntVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue, MinimumParameter.ActualValue, MaximumParameter.ActualValue);
      return base.Apply();
    }

    protected abstract IntArrayData Create(IRandom random, IntData length, IntData minimum, IntData maximum);
  }
}
