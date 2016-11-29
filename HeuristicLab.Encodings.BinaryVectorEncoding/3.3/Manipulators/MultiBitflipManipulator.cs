#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("Multi Bitflip Manipulator", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public sealed class MultiBitflipManipulator<TContext> : BitflipManipulator<TContext>
      where TContext : ISingleObjectiveSolutionContext<BinaryVector>, IStochasticContext {

    private IValueParameter<PercentValue> RatePerBitParameter {
      get { return (IValueParameter<PercentValue>)Parameters["RatePerBit"]; }
    }

    public double RatePerBit {
      get { return RatePerBitParameter.Value.Value; }
      set { RatePerBitParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private MultiBitflipManipulator(bool deserializing) : base(deserializing) { }
    private MultiBitflipManipulator(MultiBitflipManipulator<TContext> original, Cloner cloner)
      : base(original, cloner) { }
    public MultiBitflipManipulator() {
      Parameters.Add(new ValueParameter<PercentValue>("RatePerBit", "The probability of mutating each bit (independent of each other).", new PercentValue(0.05)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiBitflipManipulator<TContext>(this, cloner);
    }

    public override void Manipulate(TContext context) {
      var len = context.Solution.Solution.Length;
      var rate = RatePerBit;

      for (var idx = 0; idx < len; idx++)
        if (context.Random.NextDouble() < rate)
          context.Solution.Solution[idx] = !context.Solution.Solution[idx];
    }
  }
}
