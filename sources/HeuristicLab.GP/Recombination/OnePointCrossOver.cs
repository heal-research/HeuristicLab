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

      int crossoverEvents = scope.SubScopes.Count / 2;
      for (int i = 0; i < crossoverEvents; i++) {
        IScope parent1 = scope.SubScopes[0];
        scope.RemoveSubScope(parent1);
        IScope parent2 = scope.SubScopes[0];
        scope.RemoveSubScope(parent2);
        IScope child0 = new Scope((i * 2).ToString());
        IScope child1 = new Scope((i * 2 + 1).ToString());
        Cross(scope, random, gardener, parent1, parent2, child0, child1);
        scope.AddSubScope(child0);
        scope.AddSubScope(child1);
      }
      return null;
    }

    private void Cross(IScope scope, MersenneTwister random, TreeGardener gardener, IScope parent1, IScope parent2, IScope child0, IScope child1) {
      IFunctionTree newTree0;
      IFunctionTree newTree1;
      Cross(random, gardener, parent1, parent2, out newTree0, out newTree1);

      child0.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree0));
      child0.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTree0.Size)));
      child0.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTree0.Height)));
      child1.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree1));
      child1.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTree1.Size)));
      child1.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTree1.Height)));
    }


    private void Cross(MersenneTwister random, TreeGardener gardener, IScope f, IScope g, out IFunctionTree child0, out IFunctionTree child1) {
      IFunctionTree tree0 = f.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IFunctionTree tree1 = g.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

      List<CrossoverPoint> allowedCrossOverPoints = GetCrossOverPoints(gardener, tree0, tree1);
      if (allowedCrossOverPoints.Count > 0) {
        CrossoverPoint crossOverPoint = allowedCrossOverPoints[random.Next(allowedCrossOverPoints.Count)];
        IFunctionTree parent0 = crossOverPoint.parent0;
        IFunctionTree parent1 = crossOverPoint.parent1;
        IFunctionTree branch0 = crossOverPoint.parent0.SubTrees[crossOverPoint.childIndex];
        IFunctionTree branch1 = crossOverPoint.parent1.SubTrees[crossOverPoint.childIndex];
        parent0.RemoveSubTree(crossOverPoint.childIndex);
        parent1.RemoveSubTree(crossOverPoint.childIndex);
        parent0.InsertSubTree(crossOverPoint.childIndex, branch1);
        parent1.InsertSubTree(crossOverPoint.childIndex, branch0);
      }

      if (random.NextDouble() < 0.5) {
        child0 = tree0;
        child1 = tree1;
      } else {
        child0 = tree1;
        child1 = tree0;
      }
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
        // if the branches fit to the parent
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
