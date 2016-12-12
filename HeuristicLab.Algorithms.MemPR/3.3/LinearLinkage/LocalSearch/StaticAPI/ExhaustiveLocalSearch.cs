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
using System.Linq;
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Collections;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.MemPR.LinearLinkage.LocalSearch {
  public static class ExhaustiveLocalSearch {
    public static Tuple<int, int> Optimize(IRandom random, Encodings.LinearLinkageEncoding.LinearLinkage solution, ref double quality, bool maximization, Func<Encodings.LinearLinkageEncoding.LinearLinkage, IRandom, double> eval, CancellationToken token, bool[] subspace = null) {
      var evaluations = 0;
      var current = solution;
      if (double.IsNaN(quality)) {
        quality = eval(current, random);
        evaluations++;
      }
      var steps = 0;
      // this dictionary holds the last relevant links
      var links = new BidirectionalDictionary<int, int>();
      for (var iter = 0; iter < int.MaxValue; iter++) {
        var change = false;
        // clear the dictionary before a new pass through the array is made
        links.Clear();
        for (var i = 0; i < current.Length; i++) {
          if (subspace != null && !subspace[i]) {
            links.RemoveBySecond(i);
            links.Add(i, current[i]);
            continue;
          }
          var pred = -1;
          var isFirst = !links.TryGetBySecond(i, out pred);
          var keepLink = false;
          if (!isFirst) {
            keepLink = subspace != null && !subspace[pred];
          }
          var next = current[i];
          var isLast = next == i;

          if (!keepLink) {
            // try to insert current into each previous group
            // first remove i from its group
            var linksList = links.Where(x => x.Value != i).ToList();
            if (linksList.Count > 0 && !isFirst) current[pred] = isLast ? pred : next;
            for (var k = 0; k < linksList.Count; k++) {
              var l = linksList[k];
              current[l.Key] = i;
              current[i] = Math.Max(i, l.Value);
              var moveF = eval(current, random);
              evaluations++;
              if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
                steps++;
                quality = moveF;
                change = true;
                links.RemoveBySecond(i);
                links.SetByFirst(l.Key, i); // otherwise the link won't be removed
                if (!isFirst) links.SetByFirst(pred, isLast ? pred : next);
                next = current[i];
                if (next == i) { isLast = true; }
                pred = l.Key;
                isFirst = false;
                break;
              } else { // undo
                current[l.Key] = l.Value;
                if (k == linksList.Count - 1) {
                  // all attempts unsuccessful
                  if (!isFirst) current[pred] = i; // undo - readd i to its group
                  current[i] = next;
                }
              }
            }
          }

          if (!isLast) {
            // try to split group at this point
            // this is safe even if keepLink was true
            current[i] = i;
            var moveF = eval(current, random);
            evaluations++;
            if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
              steps++;
              quality = moveF;
              change = true;
              isLast = true;
              next = i;
              links.SetBySecond(i, i);
              continue;
            } else current[i] = next; // undo
          }

          if (isFirst && !isLast) {
            // try merge with all terminated groups
            foreach (var l in links.Where(x => x.Key == x.Value && (subspace == null || subspace[x.Key]))) {
              current[l.Key] = i;
              var moveF = eval(current, random);
              evaluations++;
              if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
                steps++;
                quality = moveF;
                change = true;
                isFirst = false;
                pred = l.Key;
                links.SetByFirst(l.Key, i);
                break;
              } else {
                current[l.Key] = l.Value;
              }
            }
          } else if (!isFirst && !keepLink) {
            // try to extract current into own group
            current[pred] = isLast ? pred : next;
            current[i] = i;
            var moveF = eval(current, random);
            evaluations++;
            if (FitnessComparer.IsBetter(maximization, moveF, quality)) {
              steps++;
              links.SetByFirst(pred, current[pred]);
              quality = moveF;
              change = true;
            } else { // undo
              current[pred] = i;
              current[i] = next;
            }
          }
          links.RemoveBySecond(i);
          links.Add(i, current[i]);
          if (token.IsCancellationRequested) break;
        }
        if (!change || token.IsCancellationRequested) break;
      }

      return Tuple.Create(evaluations, steps);
    }

    public static Tuple<int, int> OptimizeSwapOnly(IRandom random, Encodings.LinearLinkageEncoding.LinearLinkage solution, ref double quality, bool maximization, Func<Encodings.LinearLinkageEncoding.LinearLinkage, IRandom, double> eval, CancellationToken token) {
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
