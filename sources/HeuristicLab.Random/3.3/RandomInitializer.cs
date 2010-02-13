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

using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which initializes a pseudo random number generator.
  /// </summary>
  [Item("RandomInitializer", "An operator which initializes a pseudo random number generator.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class RandomInitializer : SingleSuccessorOperator {
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

    public RandomInitializer()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueLookupParameter<IntData>("Seed", "The random seed used to initialize the pseudo random number generator.", new IntData(0)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be initialized with the given seed."));
    }

    public override IExecutionSequence Apply() {
      bool setSeedRandomly = SetSeedRandomlyParameter.ActualValue == null ? true : SetSeedRandomlyParameter.ActualValue.Value;
      IntData seed = SeedParameter.ActualValue;
      IRandom random = RandomParameter.ActualValue;

      if (seed == null) seed = new IntData(0);
      if (setSeedRandomly) seed.Value = new System.Random().Next();
      RandomParameter.ActualValue = new MersenneTwister((uint)seed.Value);

      return base.Apply();
    }
  }
}
