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

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// A base class for operators creating real-valued vectors.
  /// </summary>
  [Item("RealVectorCreator", "A base class for operators creating real-valued vectors.")]
  [StorableClass(StorableClassType.Empty)]
  public abstract class RealVectorCreator : SingleSuccessorOperator, IRealVectorCreator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<DoubleArrayData> RealVectorParameter {
      get { return (ILookupParameter<DoubleArrayData>)Parameters["RealVector"]; }
    }
    public IValueLookupParameter<IntData> LengthParameter {
      get { return (IValueLookupParameter<IntData>)Parameters["Length"]; }
    }
    public IValueLookupParameter<DoubleData> MinimumParameter {
      get { return (IValueLookupParameter<DoubleData>)Parameters["Minimum"]; }
    }
    public IValueLookupParameter<DoubleData> MaximumParameter {
      get { return (IValueLookupParameter<DoubleData>)Parameters["Maximum"]; }
    }

    protected RealVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<DoubleArrayData>("RealVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntData>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Minimum", "The lower bound for each element in the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Maximum", "The upper bound for each element in the vector."));
    }

    public sealed override IOperation Apply() {
      RealVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue, MinimumParameter.ActualValue, MaximumParameter.ActualValue);
      return base.Apply();
    }

    protected abstract DoubleArrayData Create(IRandom random, IntData length, DoubleData minimum, DoubleData maximum);
  }
}
