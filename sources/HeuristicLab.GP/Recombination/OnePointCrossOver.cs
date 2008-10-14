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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Constraints;
using System.Diagnostics;

namespace HeuristicLab.GP {
  /// <summary>
  /// Implementation of a homologous one point crossover operator as described in: 
  /// W. B. Langdon and R. Poli.  Foundations of Genetic Programming. Springer-Verlag, 2002.
  /// </summary>
  public class OnePointCrossOver : OperatorBase {
    public override string Description {
      get {
        return @"";
      }
    }
    public OnePointCrossOver()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, opLibrary);

      if((scope.SubScopes.Count % 2) != 0)
        throw new InvalidOperationException("Number of parents is not even");

      CompositeOperation initOperations = new CompositeOperation();

      int children = scope.SubScopes.Count / 2;
      for(int i = 0; i < children; i++) {
        IScope parent1 = scope.SubScopes[0];
        scope.RemoveSubScope(parent1);
        IScope parent2 = scope.SubScopes[0];
        scope.RemoveSubScope(parent2);
        IScope child = new Scope(i.ToString());
        IOperation childInitOperation = Cross(gardener, maxTreeSize, maxTreeHeight, scope, random, parent1, parent2, child);
        initOperations.AddOperation(childInitOperation);
        scope.AddSubScope(child);
      }

      return initOperations;
    }

    private IOperation Cross(TreeGardener gardener, int maxTreeSize, int maxTreeHeight,
      IScope scope, MersenneTwister random, IScope parent1, IScope parent2, IScope child) {
      IFunctionTree newTree = Cross(gardener, parent1, parent2,
        random, maxTreeSize, maxTreeHeight);

      int newTreeSize = newTree.Size;
      int newTreeHeight = newTree.Height;
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTreeSize)));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTreeHeight)));

      // check if the new tree is valid and if the height of is still in the allowed bounds (we are not so strict for the max-size)
      Debug.Assert(gardener.IsValidTree(newTree) && newTreeHeight <= maxTreeHeight && newTreeSize <= maxTreeSize);
      return null;
    }


    private IFunctionTree Cross(TreeGardener gardener, IScope f, IScope g, MersenneTwister random, int maxTreeSize, int maxTreeHeight) {
      IFunctionTree tree0 = f.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IFunctionTree tree1 = g.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

      if(tree0Size == 1 && tree1Size == 1) {
        return CombineTerminals(gardener, tree0, tree1, random);
      } else {
        // we are going to insert tree1 into tree0 at a random place so we have to make sure that tree0 is not a terminal
        // in case both trees are higher than 1 we swap the trees with probability 50%
        if(tree0Height == 1 || (tree1Height > 1 && random.Next(2) == 0)) {
          IFunctionTree tmp = tree0; tree0 = tree1; tree1 = tmp;
          int tmpHeight = tree0Height; tree0Height = tree1Height; tree1Height = tmpHeight;
          int tmpSize = tree0Size; tree0Size = tree1Size; tree1Size = tmpSize;
        }

        List<IFunctionTree[]> allowedCrossOverPoints = GetCrossOverPoints(null, tree0, tree1);
        if(allowedCrossOverPoints.Count == 0) {
          if(random.NextDouble() < 0.5) return tree0; else return tree1;
        }
        IFunctionTree[] crossOverPoints = allowedCrossOverPoints[random.Next(allowedCrossOverPoints.Count)];
        IFunctionTree parent = crossOverPoints[0];
        IFunctionTree replacedBranch = crossOverPoints[1];
        IFunctionTree insertedBranch = crossOverPoints[2];
        if(parent == null) return insertedBranch;
        else {
          int i = 0;
          while(parent.SubTrees[i] != replacedBranch) i++;
          parent.RemoveSubTree(i);
          parent.InsertSubTree(i, insertedBranch);
          return tree0;
        }
      }
    }

    private List<IFunctionTree[]> GetCrossOverPoints(IFunctionTree parent, IFunctionTree tree0, IFunctionTree tree1) {
      List<IFunctionTree[]> results = new List<IFunctionTree[]>();
      if(tree0.Function != tree1.Function) return results;

      int maxArity = Math.Min(tree0.SubTrees.Count, tree1.SubTrees.Count);
      for(int i = 0; i < maxArity; i++) {
        results.AddRange(GetCrossOverPoints(tree0, tree0.SubTrees[i], tree1.SubTrees[i]));
      }
      if(results.Count == 0) results.Add(new IFunctionTree[] { parent, tree0, tree1 });
      return results;
    }

    private IFunctionTree CombineTerminals(TreeGardener gardener, IFunctionTree f, IFunctionTree g, MersenneTwister random) {
      if(random.NextDouble() < 0.5) return f; else return g;
    }
  }
}
