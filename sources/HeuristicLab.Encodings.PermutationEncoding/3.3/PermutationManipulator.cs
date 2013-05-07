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

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// A base class for permutation manipulation operators.
  /// </summary>
  [Item("PermutationManipulator", "A base class for permutation manipulation operators.")]
  [StorableClass]
  public abstract class PermutationManipulator : SingleSuccessorOperator, IPermutationManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    protected PermutationManipulator(bool deserializing) : base(deserializing) { }
    protected PermutationManipulator(PermutationManipulator original, Cloner cloner) : base(original, cloner) { }
    protected PermutationManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation which should be manipulated."));
    }

    public sealed override IOperation Apply() {
      Manipulate(RandomParameter.ActualValue, PermutationParameter.ActualValue);
      return base.Apply();
    }

    protected abstract void Manipulate(IRandom random, Permutation permutation);
  }
}
