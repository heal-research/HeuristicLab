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
  /// Implementation of a homologous uniform crossover operator as described in: 
  /// R. Poli and W. B. Langdon.  On the Search Properties of Different Crossover Operators in Genetic Programming.
  /// In Proceedings of Genetic Programming '98, Madison, Wisconsin, 1998.
  /// </summary>
  public class UniformCrossover : OperatorBase {
    public override string Description {
      get {
        return @"Uniform crossover as defined by Poli and Langdon";
      }
    }
    public UniformCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      TreeGardener gardener = new TreeGardener(random, opLibrary);

      if ((scope.SubScopes.Count % 2) != 0)
        throw new InvalidOperationException("Number of parents is not even");

      CompositeOperation initOperations = new CompositeOperation();

      int children = scope.SubScopes.Count / 2;
      for (int i = 0; i < children; i++) {
        IScope parent1 = scope.SubScopes[0];
        scope.RemoveSubScope(parent1);
        IScope parent2 = scope.SubScopes[0];
        scope.RemoveSubScope(parent2);
        IScope child = new Scope(i.ToString());
        IOperation childInitOperation = Cross(scope, random, gardener, parent1, parent2, child);
        initOperations.AddOperation(childInitOperation);
        scope.AddSubScope(child);
      }

      return initOperations;
    }

    private IOperation Cross(IScope scope, MersenneTwister random, TreeGardener gardener, IScope parent1, IScope parent2, IScope child) {
      IFunctionTree newTree = Cross(random, gardener, parent1, parent2);

      int newTreeSize = newTree.Size;
      int newTreeHeight = newTree.Height;
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTreeSize)));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTreeHeight)));

      return null;
    }


    private IFunctionTree Cross(MersenneTwister random, TreeGardener gardener, IScope f, IScope g) {
      IFunctionTree tree0 = f.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IFunctionTree tree1 = g.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

      List<CrossoverPoint> allowedCrossOverPoints = GetCrossOverPoints(gardener, tree0, tree1);

      // iterate through the list of crossover points and swap nodes with p=0.5
      foreach (CrossoverPoint crossoverPoint in allowedCrossOverPoints) {
        if (random.NextDouble() < 0.5) {
          IFunctionTree parent0 = crossoverPoint.parent0;
          IFunctionTree parent1 = crossoverPoint.parent1;
          IFunctionTree branch0 = crossoverPoint.parent0.SubTrees[crossoverPoint.childIndex];
          IFunctionTree branch1 = crossoverPoint.parent1.SubTrees[crossoverPoint.childIndex];

          if (branch0.SubTrees.Count == branch1.SubTrees.Count) {
            // if we are at an internal node of the common region swap only the node but not the subtrees
            if (parent0 != null) {
              Debug.Assert(parent1 != null); Debug.Assert(branch0 != null); Debug.Assert(branch0 != null);
              // we are not at the root => exchange the branches in the parent
              parent0.RemoveSubTree(crossoverPoint.childIndex);
              parent1.RemoveSubTree(crossoverPoint.childIndex);
              parent0.InsertSubTree(crossoverPoint.childIndex, branch1);
              parent1.InsertSubTree(crossoverPoint.childIndex, branch0);
            }
            // always exchange all children
            List<IFunctionTree> branch0Children = new List<IFunctionTree>(branch0.SubTrees); // create backup lists
            List<IFunctionTree> branch1Children = new List<IFunctionTree>(branch1.SubTrees);
            while (branch0.SubTrees.Count > 0) branch0.RemoveSubTree(0); // remove all children
            while (branch1.SubTrees.Count > 0) branch1.RemoveSubTree(0);
            foreach (IFunctionTree subTree in branch1Children) branch0.AddSubTree(subTree); // append children of branch1 to branch0
            foreach (IFunctionTree subTree in branch0Children) branch1.AddSubTree(subTree); // and vice versa                                          
          } else {
            // If we are at a node at the border of the common region then exchange the whole branch.
            // If we are at the root node and the number of children is already different we can't do anything now but
            // at the end either tree0 or tree1 must be returned with p=0.5.

            // However if we are not at the root => exchange the branches in the parent
            if (parent0 != null) {
              Debug.Assert(parent1 != null); Debug.Assert(branch0 != null); Debug.Assert(branch1 != null);              
              parent0.RemoveSubTree(crossoverPoint.childIndex);
              parent1.RemoveSubTree(crossoverPoint.childIndex);
              parent0.InsertSubTree(crossoverPoint.childIndex, branch1);
              parent1.InsertSubTree(crossoverPoint.childIndex, branch0);
            }
          }
        }
      }
      if (random.NextDouble() < 0.5) return tree0; else return tree1;
    }

    class CrossoverPoint {
      public IFunctionTree parent0;
      public IFunctionTree parent1;
      public int childIndex;
    }

    private List<CrossoverPoint> GetCrossOverPoints(TreeGardener gardener, IFunctionTree branch0, IFunctionTree branch1) {
      List<CrossoverPoint> results = new List<CrossoverPoint>();
      if (branch0.SubTrees.Count != branch1.SubTrees.Count) return results;

      for (int i = 0; i < branch0.SubTrees.Count; i++) {
        if (gardener.GetAllowedSubFunctions(branch0.Function, i).Contains(branch1.SubTrees[i].Function) &&
          gardener.GetAllowedSubFunctions(branch1.Function, i).Contains(branch0.SubTrees[i].Function)) {
          CrossoverPoint p = new CrossoverPoint();
          p.childIndex = i;
          p.parent0 = branch0;
          p.parent1 = branch1;
          results.Add(p);
        }
        results.AddRange(GetCrossOverPoints(gardener, branch0.SubTrees[i], branch1.SubTrees[i]));
      }
      return results;
    }
  }
}
