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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("Single Bitflip Manipulator", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public sealed class SingleBitflipManipulator<TContext> : BitflipManipulator<TContext>
      where TContext : ISingleObjectiveSolutionContext<BinaryVector>, IStochasticContext {

    [StorableConstructor]
    private SingleBitflipManipulator(bool deserializing) : base(deserializing) { }
    private SingleBitflipManipulator(SingleBitflipManipulator<TContext> original, Cloner cloner)
      : base(original, cloner) { }
    public SingleBitflipManipulator() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleBitflipManipulator<TContext>(this, cloner);
    }

    public override void Manipulate(TContext context) {
      var len = context.Solution.Solution.Length;
      var idx = context.Random.Next(len);
      context.Solution.Solution[idx] = !context.Solution.Solution[idx];
    }
  }
}
