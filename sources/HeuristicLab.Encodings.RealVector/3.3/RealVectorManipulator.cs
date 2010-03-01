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
  /// A base class for operators that manipulate real-valued vectors.
  /// </summary>
  [Item("RealVectorManipulator", "A base class for operators that manipulate real-valued vectors.")]
  [EmptyStorableClass]
  public abstract class RealVectorManipulator : SingleSuccessorOperator, IRealVectorManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<DoubleArrayData> RealVectorParameter {
      get { return (ILookupParameter<DoubleArrayData>)Parameters["RealVector"]; }
    }

    protected RealVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<DoubleArrayData>("RealVector", "The vector which should be manipulated."));
    }

    public sealed override IOperation Apply() {
      Manipulate(RandomParameter.ActualValue, RealVectorParameter.ActualValue);
      return base.Apply();
    }

    protected abstract void Manipulate(IRandom random, DoubleArrayData permutation);
  }
}
