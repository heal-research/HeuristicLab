#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  public static class LinkageCrossover {
    // In the GECCO paper, Figure 3
    public static double ImproveUsingTree(LinkageTree tree, IList<BinaryVector> donors, BinaryVector solution, double fitness, ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector> problem, IRandom rand) {
      var options = Enumerable.Range(0, donors.Count).ToArray();
      foreach (var cluster in tree.Clusters) {
        // Find a donor which has at least one gene value different
        // from the current solution for this cluster of genes
        bool donorFound = false;
        foreach (var donorIndex in options.ShuffleList(rand)) {
          // Attempt the donation
          fitness = Donate(solution, fitness, donors[donorIndex], cluster, problem, rand, out donorFound);
          if (donorFound) break;
        }
      }
      return fitness;
    }

    private static double Donate(BinaryVector solution, double fitness, BinaryVector source, IEnumerable<int> cluster, ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector> problem, IRandom rand, out bool changed) {
      // keep track of which bits flipped to make the donation
      List<int> flipped = new List<int>();
      foreach (var index in cluster) {
        if (solution[index] != source[index]) {
          flipped.Add(index);
          solution[index] = !solution[index];
        }
      }
      changed = flipped.Count > 0;
      if (changed) {
        double newFitness = problem.Evaluate(solution, rand);
        // if the original is strictly better, revert the change
        if (problem.IsBetter(fitness, newFitness)) {
          foreach (var index in flipped) {
            solution[index] = !solution[index];
          }
        } else {
          // new solution is no worse than original, keep change to solution
          fitness = newFitness;
        }
      }
      return fitness;
    }
  }
}
