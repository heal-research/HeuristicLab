#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("StochasticEMSSSingleMoveGenerator", "Randomly samples a single from all possible EMSS moves (extract, merge, shift, and split) from a given lle grouping.")]
  [StorableClass]
  public class StochasticEMSSSingleMoveGenerator : EMSSMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticEMSSSingleMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticEMSSSingleMoveGenerator(StochasticEMSSSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticEMSSSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticEMSSSingleMoveGenerator(this, cloner);
    }

    protected override EMSSMove[] GenerateMoves(LinearLinkage lle) {
      int length = lle.Length;
      if (length == 1) throw new ArgumentException("StochasticEMSSSingleMoveGenerator: There cannot be a move given only one item.", "lle");

      var random = RandomParameter.ActualValue;
      return new[] { ExhaustiveEMSSMoveGenerator.Generate(lle).Shuffle(random).First() };
    }
  }
}
