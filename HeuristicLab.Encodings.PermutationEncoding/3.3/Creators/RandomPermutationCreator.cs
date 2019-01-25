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
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// An operator which creates a new random permutation of integer values.
  /// </summary>
  [Item("RandomPermutationCreator", "An operator which creates a new random permutation of integer values.")]
  [StorableType("527B14A9-DB58-4507-8C3F-1DE1EF4C4A2B")]
  public sealed class RandomPermutationCreator : InstrumentedOperator, IPermutationCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public IValueParameter<PermutationType> PermutationTypeParameter {
      get { return (IValueParameter<PermutationType>)Parameters["PermutationType"]; }
    }

    public PermutationTypes PermutationType {
      get { return PermutationTypeParameter.Value.Value; }
      set { PermutationTypeParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private RandomPermutationCreator(StorableConstructorFlag _) : base(_) { }
    private RandomPermutationCreator(RandomPermutationCreator original, Cloner cloner) : base(original, cloner) { }
    public RandomPermutationCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used to initialize the new random permutation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the new random permutation."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The new random permutation."));
      Parameters.Add(new ValueParameter<PermutationType>("PermutationType", "The type of the permutation.", new PermutationType(PermutationTypes.RelativeUndirected)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomPermutationCreator(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      PermutationParameter.ActualValue = new Permutation(PermutationType, LengthParameter.ActualValue.Value, RandomParameter.ActualValue);
      return base.InstrumentedApply();
    }
  }
}
