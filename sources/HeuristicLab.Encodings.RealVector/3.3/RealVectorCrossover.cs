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
  /// A base class for operators that perform a crossover of real-valued vectors.
  /// </summary>
  [Item("RealVectorCrossover", "A base class for operators that perform a crossover of real-valued vectors.")]
  [StorableClass]
  public abstract class RealVectorCrossover : SingleSuccessorOperator, IRealVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<DoubleArray>> ParentsParameter {
      get { return (SubScopesLookupParameter<DoubleArray>)Parameters["Parents"]; }
    }
    public ILookupParameter<DoubleArray> ChildParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Child"]; }
    }

    protected RealVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new SubScopesLookupParameter<DoubleArray>("Parents", "The parent vectors which should be crossed."));
      Parameters.Add(new LookupParameter<DoubleArray>("Child", "The child vector resulting from the crossover."));
    }

    public sealed override IOperation Apply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.Apply();
    }

    protected abstract DoubleArray Cross(IRandom random, ItemArray<DoubleArray> parents);
  }
}
