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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("StochasticEMSSMultiMoveGenerator", "Randomly samples n from all possible EMSS moves (extract, merge, shift, and split) from a given lle grouping.")]
  [StorableClass]
  public class StochasticEMSSMultiMoveGenerator : EMSSMoveGenerator, IMultiMoveGenerator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public IntValue SampleSize {
      get { return SampleSizeParameter.Value; }
      set { SampleSizeParameter.Value = value; }
    }

    [StorableConstructor]
    protected StochasticEMSSMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticEMSSMultiMoveGenerator(StochasticEMSSMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticEMSSMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticEMSSMultiMoveGenerator(this, cloner);
    }

    protected override EMSSMove[] GenerateMoves(LinearLinkage lle) {
      int length = lle.Length;
      if (length == 1) throw new ArgumentException("StochasticEMSSMultiMoveGenerator: There cannot be a move given only one item.", "lle");

      var random = RandomParameter.ActualValue;
      var sampleSize = SampleSizeParameter.ActualValue.Value;
      return ExhaustiveEMSSMoveGenerator.Generate(lle).Shuffle(random).Take(sampleSize).ToArray();
    }
  }
}
