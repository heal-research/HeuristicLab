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
using System.Diagnostics;

namespace HeuristicLab.StructureIdentification {
  public class ProbabilisticTreeCreator : OperatorBase {
    public override string Description {
      get { return @"Generates a new random operator tree."; }
    }

    public ProbabilisticTreeCreator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTreeSize", "The minimal allowed size of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The created tree", typeof(IFunctionTree), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int minTreeSize = GetVariableValue<IntData>("MinTreeSize", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, opLibrary);

      int treeSize = random.Next(minTreeSize, maxTreeSize + 1);
      IFunctionTree root = gardener.PTC2(random, treeSize, maxTreeHeight);

      int actualTreeSize = root.Size;
      int actualTreeHeight = root.Height;

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), root));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(actualTreeSize)));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(actualTreeHeight)));

      Debug.Assert(gardener.IsValidTree(root) && actualTreeHeight<=maxTreeHeight);

      return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(root), scope);
    }
  }
}