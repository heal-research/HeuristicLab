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
 
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Extreme-point Permutation Decoder (3d) Base", "Base class for decoders")]
  [StorableType("CF1A3362-B650-419D-A5C0-ADB9AFFCCC56")]
  public abstract class ExtremePointPermutationDecoderBase : Item, IDecoder<Permutation> {

    [StorableConstructor]
    protected ExtremePointPermutationDecoderBase(StorableConstructorFlag _) : base(_) { }
    protected ExtremePointPermutationDecoderBase(ExtremePointPermutationDecoderBase original, Cloner cloner)
      : base(original, cloner) {
    }
    public ExtremePointPermutationDecoderBase() : base() { }

    public abstract Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints);
  }
}
