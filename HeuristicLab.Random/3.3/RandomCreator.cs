#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Random {
  /// <summary>
  /// An operator which creates a new Mersenne Twister pseudo random number generator.
  /// </summary>
  [Item("RandomCreator", "An operator which creates a new Mersenne Twister pseudo random number generator.")]
  [StorableType("78A995DA-CE6C-4693-A494-6ABBF1849CEB")]
  public sealed class RandomCreator : SingleSuccessorOperator {
    #region Parameter Properties
    public ValueLookupParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public ValueLookupParameter<IntValue> SeedParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Seed"]; }
    }
    public ValueParameter<IRandom> RandomTypeParameter {
      get { return (ValueParameter<IRandom>)Parameters["RandomType"]; }
    }
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    #region Properties
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public IRandom RandomType {
      get { return RandomTypeParameter.Value; }
      set { RandomTypeParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private RandomCreator(StorableConstructorFlag _) : base(_) { }
    private RandomCreator(RandomCreator original, Cloner cloner) : base(original, cloner) { }
    public RandomCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IRandom>("RandomType", "The type of pseudo random number generator which is created.", new MersenneTwister()));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The new pseudo random number generator which is initialized with the given seed."));
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code (remove with 3.4)
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("RandomType"))
        Parameters.Add(new ValueParameter<IRandom>("RandomType", "The type of pseudo random number generator which is created.", new MersenneTwister()));
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomCreator(this, cloner);
    }

    public override IOperation Apply() {
      if (SetSeedRandomlyParameter.ActualValue == null) SetSeedRandomlyParameter.ActualValue = new BoolValue(true);
      bool setSeedRandomly = SetSeedRandomlyParameter.ActualValue.Value;
      if (SeedParameter.ActualValue == null) SeedParameter.ActualValue = new IntValue(0);
      IntValue seed = SeedParameter.ActualValue;

      if (setSeedRandomly) seed.Value = RandomSeedGenerator.GetSeed();
      IRandom random = (IRandom)RandomType.Clone();
      random.Reset(seed.Value);
      RandomParameter.ActualValue = random;

      return base.Apply();
    }
  }
}
