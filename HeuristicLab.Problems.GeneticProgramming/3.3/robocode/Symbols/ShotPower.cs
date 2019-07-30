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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("95B53DAD-A47C-49CD-8157-6DF34A750F68")]
  public class ShotPower : Symbol {
    public override int MinimumArity { get { return 0; } }
    public override int MaximumArity { get { return 0; } }

    [StorableConstructor]
    protected ShotPower(StorableConstructorFlag _) : base(_) { }
    protected ShotPower(ShotPower original, Cloner cloner) : base(original, cloner) { }

    public ShotPower() : base("ShotPower", "The power of a shot between 0.1 and 3.") { }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new ShotPowerTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShotPower(this, cloner);
    }
  }
}