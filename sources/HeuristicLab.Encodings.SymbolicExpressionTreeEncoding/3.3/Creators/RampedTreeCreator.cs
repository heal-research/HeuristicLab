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
using System;
using HeuristicLab.Random;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public class RampedTreeCreator : OperatorBase {
    public override string Description {
      get { return @"Generates a new random operator tree."; }
    }

    public RampedTreeCreator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionLibrary", "The function library containing all available functions", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTreeHeight", "The minimal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BalancedTreesRate", "Determines how many trees should be balanced", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The created tree", typeof(IGeneticProgrammingModel), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      FunctionLibrary opLibrary = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      int minTreeHeight = GetVariableValue<IntData>("MinTreeHeight", scope, true).Data;
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      double balancedTreesRate = GetVariableValue<DoubleData>("BalancedTreesRate", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, opLibrary);

      int treeHeight = random.Next(minTreeHeight, maxTreeHeight + 1);
      IFunctionTree root;
      if(random.NextDouble() <= balancedTreesRate) {
        root = gardener.CreateBalancedRandomTree(Int32.MaxValue, treeHeight);
      } else {
        root = gardener.CreateUnbalancedRandomTree(Int32.MaxValue, treeHeight);
      }

      Debug.Assert(gardener.IsValidTree(root) && root.GetHeight() <= maxTreeHeight);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), new GeneticProgrammingModel(root)));
      return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(root), scope);
    }
  }
}
