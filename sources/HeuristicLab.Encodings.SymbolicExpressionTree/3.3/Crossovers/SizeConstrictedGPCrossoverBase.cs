#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.Operators {
  public abstract class SizeConstrictedGPCrossoverBase : GPCrossoverBase {

    private int MaxRecombinationTries { get { return 20; } }

    public SizeConstrictedGPCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
    }

    internal override IFunctionTree Cross(IScope scope, TreeGardener gardener, IRandom random, IFunctionTree tree0, IFunctionTree tree1) {
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;

      // when tree0 is terminal then try to cross into tree1, when tree1 is also terminal just return tree0 unchanged.
      IFunctionTree newTree;
      int tries = 0;
      do {
        if (tries++ > MaxRecombinationTries)
          // if after so many tries it was not possible to create a solution within the given size constraints just return the tree without changes.
          return tree0;
        if (tree0.SubTrees.Count > 0) {
          newTree = Cross(gardener, random, (IFunctionTree)tree0.Clone(), (IFunctionTree)tree1.Clone(), maxTreeSize, maxTreeHeight);
        } else if (tree1.SubTrees.Count > 0) {
          newTree = Cross(gardener, random, (IFunctionTree)tree1.Clone(), (IFunctionTree)tree0.Clone(), maxTreeSize, maxTreeHeight);
        } else newTree = tree0;
      } while (newTree.GetSize() > maxTreeSize || newTree.GetHeight() > maxTreeHeight);
      return newTree;
    }

    internal abstract IFunctionTree Cross(TreeGardener gardener, IRandom random, IFunctionTree tree0, IFunctionTree tree1, int maxTreeSize, int maxTreeHeight);
  }
}
