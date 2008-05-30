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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using System;
using HeuristicLab.Random;
using HeuristicLab.Functions;

namespace HeuristicLab.StructureIdentification {
  public class RandomTreeCreator : OperatorBase {
    public override string Description {
      get { return @"Generates a new random operator tree."; }
    }

    public RandomTreeCreator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BalancedTreesRate", "Determines how many trees should be balanced", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The created tree", typeof(IFunctionTree), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      double balancedTreesRate = GetVariableValue<DoubleData>("BalancedTreesRate", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, opLibrary);

      int treeHeight = random.Next(1, maxTreeHeight + 1);
      int treeSize = random.Next(1, maxTreeSize + 1);
      IFunctionTree root;
      if(random.NextDouble() <= balancedTreesRate) {
        root = gardener.CreateBalancedRandomTree(treeSize, treeHeight);
      } else {
        root = gardener.CreateUnbalancedRandomTree(treeSize, treeHeight);
      }

      int actualTreeSize = gardener.GetTreeSize(root);
      int actualTreeHeight = gardener.GetTreeHeight(root);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), root));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(actualTreeSize)));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(actualTreeHeight)));

      if(!gardener.IsValidTree(root)) { throw new InvalidProgramException(); }

      if(actualTreeSize > maxTreeSize ||
        actualTreeHeight > maxTreeHeight) {
        throw new InvalidProgramException();
      }

      return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(root), scope);
    }
  }
}
