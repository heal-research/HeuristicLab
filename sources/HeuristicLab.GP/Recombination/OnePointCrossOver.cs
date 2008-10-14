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
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);

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
        IOperation childInitOperation = Cross(scope, random, parent1, parent2, child);
        initOperations.AddOperation(childInitOperation);
        scope.AddSubScope(child);
      }

      return initOperations;
    }

    private IOperation Cross(IScope scope, MersenneTwister random, IScope parent1, IScope parent2, IScope child) {
      IFunctionTree newTree = Cross(random, parent1, parent2);

      int newTreeSize = newTree.Size;
      int newTreeHeight = newTree.Height;
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTreeSize)));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTreeHeight)));

      return null;
    }


    private IFunctionTree Cross(MersenneTwister random, IScope f, IScope g) {
      IFunctionTree tree0 = f.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IFunctionTree tree1 = g.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

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

    private List<IFunctionTree[]> GetCrossOverPoints(IFunctionTree parent, IFunctionTree tree0, IFunctionTree tree1) {
      List<IFunctionTree[]> results = new List<IFunctionTree[]>();
      if(tree0.SubTrees.Count != tree1.SubTrees.Count) return results;
      // invariant arity - number of subtrees is equal in both trees

      results.Add(new IFunctionTree[] { parent, tree0, tree1 });
      for(int i = 0; i < tree0.SubTrees.Count; i++) {
        results.AddRange(GetCrossOverPoints(tree0, tree0.SubTrees[i], tree1.SubTrees[i]));
      }
      return results;
    }
  }
}
