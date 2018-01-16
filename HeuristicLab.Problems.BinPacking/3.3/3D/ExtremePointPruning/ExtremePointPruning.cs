#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointPruning {
  internal class ExtremePointPruning : IExtremePointPruning {
    public void PruneExtremePoints(ExtremePointPruningMethod pruningMethod, PackingShape bin, Dictionary<BinPacking3D, List<KeyValuePair<int, PackingPosition>>> positions) {
      if (pruningMethod == ExtremePointPruningMethod.PruneBehind) {
        PruneExtremePointsBehind(bin, positions);
      }
    }

    public void PruneExtremePoints(ExtremePointPruningMethod pruningMethod, IList<BinPacking3D> binPackings) {
      if (binPackings.Count <= 0) {
        return;
      }
      var fixedPositions = new Dictionary<BinPacking3D, List<KeyValuePair<int, PackingPosition>>>();
      foreach (BinPacking3D bp in binPackings) {
        var list = new List<KeyValuePair<int, PackingPosition>>();
        fixedPositions.Add(bp, list);
        foreach (var p in bp.Positions) {
          list.Add(p);
        }
      }
      PruneExtremePointsBehind(binPackings[0].BinShape, fixedPositions);
    }


    /// <summary>
    /// Prunes all extreme point behind the given positions.
    /// </summary>
    /// <param name="bin"></param>
    /// <param name="positions"></param>
    private static void PruneExtremePointsBehind(PackingShape bin, Dictionary<BinPacking3D, List<KeyValuePair<int, PackingPosition>>> positions) {
      int binHeight = bin.Height;
      foreach (var kvp in positions) {
        var bp = kvp.Key;
        foreach (var item in kvp.Value.OrderByDescending(x => x.Value.Z).ThenByDescending(x => x.Value.X)) {
          // everything behind the item
          var limit = new {
            X = item.Value.X + bp.Items[item.Key].Width,
            Y = item.Value.Y + binHeight,
            Z = item.Value.Z
          };
          var eps = bp.ExtremePoints.Where(x => x.Key.X < limit.X && x.Key.Y < limit.Y && x.Key.Z < limit.Z).ToList();
          foreach (var ep in eps) {
            bp.ExtremePoints.Remove(ep);
          }
        }
      }
    }
    
  }
}
