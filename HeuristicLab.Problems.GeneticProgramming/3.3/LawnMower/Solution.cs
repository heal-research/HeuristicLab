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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.LawnMower {
  [StorableType("99ADBE85-803D-4463-8BF2-5F825E14F7BD")]
  public sealed class Solution : NamedItem {
    [Storable]
    public int Width { get; private set; }
    [Storable]
    public int Length { get; private set; }
    [Storable]
    public ISymbolicExpressionTree Tree { get; private set; }
    [Storable]
    public double Quality { get; private set; }

    #region item cloning and persistence
    [StorableConstructor]
    private Solution(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    private Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
      this.Length = original.Length;
      this.Width = original.Width;
      this.Tree = cloner.Clone(original.Tree);
      this.Quality = original.Quality;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
    #endregion

    public Solution(ISymbolicExpressionTree tree, int length, int width, double quality)
      : base("Solution", "A lawn mower solution.") {
      this.Tree = tree;
      this.Length = length;
      this.Width = width;
      this.Quality = quality;
    }
  }
}
