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
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ScrambleMove", "Item that describes a scramble move.")]
  [StorableType("DBE9C007-10AA-4B8A-8092-23C9ACBA411A")]
  public sealed class ScrambleMove : Item {
    [Storable]
    public int StartIndex { get; private set; }
    [Storable]
    public int[] ScrambledIndices { get; private set; }

    [StorableConstructor]
    private ScrambleMove(StorableConstructorFlag _) : base(_) { }
    private ScrambleMove(ScrambleMove original, Cloner cloner)
      : base(original, cloner) {
      this.StartIndex = original.StartIndex;
      if (original.ScrambledIndices != null)
        this.ScrambledIndices = (int[])original.ScrambledIndices.Clone();
    }
    public ScrambleMove() : this(-1, null) { }
    public ScrambleMove(int startIndex, int[] scrambledIndices)
      : base() {
      this.StartIndex = startIndex;
      this.ScrambledIndices = scrambledIndices;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScrambleMove(this, cloner);
    }
  }
}
