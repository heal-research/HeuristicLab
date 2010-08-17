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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Random {
  /// <summary>
  /// An operator which creates a new "FastRandom" pseudo random number generator.
  /// </summary>
  [Item("FastRandomCreator", "An operator which creates a new \'FastRandom\' pseudo random number generator.")]
  [StorableClass]
  public sealed class FastRandomCreator : SingleSuccessorOperator {
    public ValueLookupParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public ValueLookupParameter<IntValue> SeedParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Seed"]; }
    }
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }

    public FastRandomCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The new pseudo random number generator which is initialized with the given seed."));
    }

    public override IOperation Apply() {
      if (SetSeedRandomlyParameter.ActualValue == null) SetSeedRandomlyParameter.ActualValue = new BoolValue(true);
      bool setSeedRandomly = SetSeedRandomlyParameter.ActualValue.Value;
      if (SeedParameter.ActualValue == null) SeedParameter.ActualValue = new IntValue(0);
      IntValue seed = SeedParameter.ActualValue;

      if (setSeedRandomly) seed.Value = new System.Random().Next();
      RandomParameter.ActualValue = new FastRandom(seed.Value);

      return base.Apply();
    }
  }
}
