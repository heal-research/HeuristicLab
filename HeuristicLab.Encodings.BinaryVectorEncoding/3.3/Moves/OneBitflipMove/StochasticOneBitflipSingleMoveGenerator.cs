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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("StochasticOneBitflipSingleMoveGenerator", "Randomly samples a single from all possible one bitflip moves from a given BinaryVector.")]
  [StorableType("9EFDBD19-B9AF-4820-8DA9-EE04370D7D8B")]
  public class StochasticOneBitflipSingleMoveGenerator : OneBitflipMoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticOneBitflipSingleMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected StochasticOneBitflipSingleMoveGenerator(StochasticOneBitflipSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticOneBitflipSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticOneBitflipSingleMoveGenerator(this, cloner);
    }

    public static OneBitflipMove Apply(BinaryVector binaryVector, IRandom random) {
      int length = binaryVector.Length;
      int index = random.Next(length);
      return new OneBitflipMove(index);
    }

    protected override OneBitflipMove[] GenerateMoves(BinaryVector binaryVector) {
      IRandom random = RandomParameter.ActualValue;
      return new OneBitflipMove[] { Apply(binaryVector, random) };
    }
  }
}
