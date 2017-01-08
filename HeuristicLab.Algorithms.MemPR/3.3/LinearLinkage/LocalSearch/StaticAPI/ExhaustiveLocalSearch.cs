#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Core;
using HeuristicLab.Encodings.LinearLinkageEncoding;

namespace HeuristicLab.Algorithms.MemPR.Grouping.LocalSearch {
  public static class ExhaustiveLocalSearch {
    public static Tuple<int, int> Optimize(IRandom random, LinearLinkage solution, ref double quality, bool maximization, Func<LinearLinkage, CancellationToken, double> eval, CancellationToken token, bool[] subspace = null) {
      var evaluations = 0;
      if (double.IsNaN(quality)) {
        quality = eval(solution, token);
        evaluations++;
      }
      var steps = 0;
      var lleb = solution.ToBackLinks();
      for (var iter = 0; iter < int.MaxValue; iter++) {
        var change = false;
        var groupItems = new List<int>();
        for (var i = 0; i < solution.Length; i++) {
          foreach (var move in MoveGenerator.GenerateForItem(i, groupItems, solution, lleb).ToList()) {
            move.Apply(solution);
            var moveF = eval(solution, token);
            evaluations++;
            if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
              move.ApplyToLLEb(lleb);
              steps++;
              quality = moveF;
              change = true;
              break;
            } else {
              move.Undo(solution);
            }
            if (token.IsCancellationRequested) break;
          }
          if (lleb[i] != i)
            groupItems.Remove(lleb[i]);
          groupItems.Add(i);
          if (token.IsCancellationRequested) break;
        }
        if (!change || token.IsCancellationRequested) break;
      }

      return Tuple.Create(evaluations, steps);
    }

    public static Tuple<int, int> OptimizeSwap(IRandom random, LinearLinkage solution, ref double quality, bool maximization, Func<LinearLinkage, IRandom, double> eval, CancellationToken token) {
      var evaluations = 0;
      var current = solution;
      if (double.IsNaN(quality)) {
        quality = eval(current, random);
        evaluations++;
      }
      var groups = current.GetGroups().ToList();
      var exclude = new bool[groups.Count, groups.Count];
      var steps = 0;
      for (var iter = 0; iter < int.MaxValue; iter++) {
        var change = false;
        for (var i = 0; i < groups.Count - 1; i++) {
          for (var j = i + 1; j < groups.Count; j++) {
            if (exclude[i, j]) continue;
            var groupsChanged = false;
            if (groups[i].Count == 1 && groups[j].Count == 1) continue; // groups are anonymous, swap has no effect
            for (var x = 0; x < groups[i].Count; x++) {
              for (var y = 0; y < groups[j].Count; y++) {
                var a = groups[i][x];
                var b = groups[j][y];
                groups[i][x] = b;
                groups[j][y] = a;
                current.SetGroups(groups);
                var moveF = eval(current, random);
                evaluations++;
                if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
                  steps++;
                  quality = moveF;
                  groupsChanged = true;
                } else {
                  // undo
                  groups[i][x] = a;
                  groups[j][y] = b;
                  current.SetGroups(groups);
                }
              }
            }
            if (!groupsChanged) {
              exclude[i, j] = true;
            } else {
              change = true;
              for (var k = 0; k < groups.Count; k++) {
                exclude[k, j] = false;
                exclude[i, k] = false;
              }
            }
            if (token.IsCancellationRequested) break;
          }
          if (token.IsCancellationRequested) break;
        }
        if (!change || token.IsCancellationRequested) break;
      }
      return Tuple.Create(evaluations, steps);
    }
  }
}
