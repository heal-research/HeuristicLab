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

namespace HeuristicLab.Encodings.BinaryVectors {
  /// <summary>
  /// A base class for operators creating bool-valued vectors.
  /// </summary>
  [Item("BoolVectorCreator", "A base class for operators creating bool-valued vectors.")]
  [StorableClass]
  public abstract class BinaryVectorCreator : SingleSuccessorOperator, IBinaryVectorCreator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<BoolArrayData> BinaryVectorParameter {
      get { return (ILookupParameter<BoolArrayData>)Parameters["BinaryVector"]; }
    }
    public IValueLookupParameter<IntData> LengthParameter {
      get { return (IValueLookupParameter<IntData>)Parameters["Length"]; }
    }

    protected BinaryVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<IntArrayData>("BinaryVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntData>("Length", "The length of the vector."));
    }

    public sealed override IOperation Apply() {
      BinaryVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue);
      return base.Apply();
    }

    protected abstract BoolArrayData Create(IRandom random, IntData length);
  }
}
