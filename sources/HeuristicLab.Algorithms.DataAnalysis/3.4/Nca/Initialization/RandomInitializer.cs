#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Random", "Initializes the matrix randomly.")]
  [StorableClass]
  public class RandomInitializer : ParameterizedNamedItem, INCAInitializer {
    private IValueParameter<IntValue> RandomParameter {
      get { return (IValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }

    public int Seed {
      get { return RandomParameter.Value.Value; }
      set { RandomParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected RandomInitializer(bool deserializing) : base(deserializing) { }
    protected RandomInitializer(RandomInitializer original, Cloner cloner) : base(original, cloner) { }
    public RandomInitializer()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The seed for the random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "Whether the seed should be randomized for each call.", new BoolValue(true)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomInitializer(this, cloner);
    }

    public double[] Initialize(IClassificationProblemData data, int dimensions) {
      var instances = data.TrainingIndices.Count();
      var attributes = data.AllowedInputVariables.Count();

      var random = new MersenneTwister();
      if (SetSeedRandomly) Seed = random.Next();
      random.Reset(Seed);

      var range = data.AllowedInputVariables.Select(x => data.Dataset.GetDoubleValues(x).Max() - data.Dataset.GetDoubleValues(x).Min()).ToArray();
      var matrix = new double[attributes * dimensions];
      for (int i = 0; i < matrix.Length; i++)
        matrix[i] = random.NextDouble() / range[i / dimensions];

      return matrix;
    }
  }
}
