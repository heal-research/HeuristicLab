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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Random", "Initializes the matrix randomly.")]
  [StorableClass]
  public sealed class RandomInitializer : NcaInitializer, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private RandomInitializer(bool deserializing) : base(deserializing) { }
    private RandomInitializer(RandomInitializer original, Cloner cloner) : base(original, cloner) { }
    public RandomInitializer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomInitializer(this, cloner);
    }

    public override double[,] Initialize(IClassificationProblemData data, int dimensions) {
      var attributes = data.AllowedInputVariables.Count();

      var random = RandomParameter.ActualValue;
      var matrix = new double[attributes, dimensions];
      for (int i = 0; i < attributes; i++)
        for (int j = 0; j < dimensions; j++)
          matrix[i, j] = random.NextDouble();

      return matrix;
    }
  }
}
