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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableClass]
  [Item("Solution", "Robocode program and configuration.")]
  public sealed class Solution : Item {
    [Storable]
    public ISymbolicExpressionTree Tree { get; set; }

    [Storable]
    public string Path { get; set; }

    [Storable]
    public int NrOfRounds { get; set; }

    [Storable]
    public EnemyCollection Enemies { get; set; }

    [StorableConstructor]
    private Solution(bool deserializing) : base(deserializing) { }
    private Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
      Tree = cloner.Clone(original.Tree);
      Path = (string)original.Path.Clone();
      NrOfRounds = original.NrOfRounds;
      Enemies = cloner.Clone(original.Enemies);
    }

    public Solution(ISymbolicExpressionTree tree, string path, int nrOfRounds, EnemyCollection enemies)
      : base() {
      this.Tree = tree;
      this.Path = path;
      this.NrOfRounds = nrOfRounds;
      this.Enemies = enemies;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
  }
}