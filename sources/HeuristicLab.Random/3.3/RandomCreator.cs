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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which creates a new Mersenne Twister pseudo random number generator.
  /// </summary>
  [Item("RandomCreator", "An operator which creates a new Mersenne Twister pseudo random number generator.")]
  [StorableClass(StorableClassType.Empty)]
  [Creatable("Test")]
  public sealed class RandomCreator : SingleSuccessorOperator {
    public ValueLookupParameter<BoolData> SetSeedRandomlyParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["SetSeedRandomly"]; }
    }
    public ValueLookupParameter<IntData> SeedParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["Seed"]; }
    }
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public BoolData SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntData Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }

    public RandomCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueLookupParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The new pseudo random number generator which is initialized with the given seed."));
    }

    public override IOperation Apply() {
      if (SetSeedRandomlyParameter.ActualValue == null) SetSeedRandomlyParameter.ActualValue = new BoolData(true);
      bool setSeedRandomly = SetSeedRandomlyParameter.ActualValue.Value;
      if (SeedParameter.ActualValue == null) SeedParameter.ActualValue = new IntData(0);
      IntData seed = SeedParameter.ActualValue;

      if (setSeedRandomly) seed.Value = new System.Random().Next();
      RandomParameter.ActualValue = new MersenneTwister((uint)seed.Value);

      return base.Apply();
    }
  }
}
