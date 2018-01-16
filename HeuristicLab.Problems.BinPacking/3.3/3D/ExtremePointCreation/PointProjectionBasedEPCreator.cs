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

using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {

  /// <summary>
  /// This extreme point creation class uses the point projection based method by Crainic, T. G., Perboli, G., & Tadei, R. for creating extreme points.
  /// Each extreme point of an item is being projectet backwards, downwards and to the left. 
  /// A new extreme point will be created where this projections instersects with another item or the bin boundins.
  /// </summary>
  public class PointProjectionBasedEPCreator : ExtremePointCreator {
    protected override void UpdateExtremePoints(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      GenerateNewExtremePointsForItem(binPacking, item, position);

      // if an item is fit in below, before, or left of another its extreme points have to be regenerated
      foreach (var above in GetItemsAbove(binPacking, position)) {
        GenerateNewExtremePointsForItem(binPacking, above.Item2, above.Item1);
      }
      foreach (var front in GetItemsInfront(binPacking, position)) {
        GenerateNewExtremePointsForItem(binPacking, front.Item2, front.Item1);
      }
      foreach (var right in GetItemsOnRight(binPacking, position)) {
        GenerateNewExtremePointsForItem(binPacking, right.Item2, right.Item1);
      }
    }
    
    protected override void UpdateResidualSpace(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      foreach (var extremePoint in binPacking.ExtremePoints.ToList()) {
        var ep = extremePoint.Key;
        var residualSpaces = extremePoint.Value as IList<ResidualSpace>;
        for (int i = 0; i < residualSpaces.Count(); i++) {
          var rs = residualSpaces[i];
          var depth = item.Depth;
          var width = item.Width;
          var changed = false;
          if (ep.Z >= position.Z && ep.Z < position.Z + depth) {
            if (ep.X <= position.X && ep.Y >= position.Y && ep.Y < position.Y + item.Height) {
              var diff = position.X - ep.X;
              var newRSX = Math.Min(rs.Width, diff);
              rs = ResidualSpace.Create(newRSX, rs.Height, rs.Depth);
              changed = true;
            }
            if (ep.Y <= position.Y && ep.X >= position.X && ep.X < position.X + width) {
              var diff = position.Y - ep.Y;
              var newRSY = Math.Min(rs.Height, diff);
              rs = ResidualSpace.Create(rs.Width, newRSY, rs.Depth);
              changed = true;
            }
          }

          if (ep.Z <= position.Z &&
              ep.Y >= position.Y && ep.Y < position.Y + item.Height &&
              ep.X >= position.X && ep.X < position.X + width) {
            var diff = position.Z - ep.Z;
            var newRSZ = Math.Min(rs.Depth, diff);
            rs = ResidualSpace.Create(rs.Width, rs.Height, newRSZ);
            changed = true;
          }

          if (changed) {
            if (!rs.IsZero() && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, new Vector3D(ep), rs)) {
              residualSpaces[i] = rs;
            } else {
              binPacking.ExtremePoints.Remove(ep);
              break;
            }
          }
        }        
      }
      return;
    }
    
    /// <summary>
    /// Adds an extreme point to a given bin packing.
    /// It also adds the residual space for the new extreme point 
    /// and removes the extreme point and the related residual space for the given position which are not needed anymore.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected override bool AddExtremePoint(BinPacking3D binPacking, PackingPosition position) {
      if (binPacking.ExtremePoints.ContainsKey(position)) {
        return false;
      }

      var residualSpaces = CalculateResidualSpace(binPacking, new Vector3D(position));
      binPacking.ExtremePoints.Add(position, residualSpaces);
      // Check if the existing extreme points are shadowed by the new point
      // This is, their residual space fit entirely into the residual space of the new point
      foreach (var ep in binPacking.ExtremePoints.Where(x => x.Key != position && new Vector3D(x.Key).IsInside(position, residualSpaces)).ToList()) {
        foreach (var residualSpace in ep.Value) {
          if (IsWithinResidualSpaceOfAnotherExtremePoint(new Vector3D(ep.Key), residualSpace, position, residualSpaces)) {
            binPacking.ExtremePoints.Remove(ep);
            break;
          }
        }          
      }
      return true;
    }

    /// <summary>
    /// Calculates the residual space for a given bin packing and position.
    /// It returns the residual space as a tuple which contains the dimensions
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="pos"></param>
    /// <returns>A Tuple(width, Height, Depth)</width></returns>
    protected override IEnumerable<ResidualSpace> CalculateResidualSpace(BinPacking3D binPacking, Vector3D pos) {
      var residualSpaces = new List<ResidualSpace>();
      var residualSpace = new ResidualSpace(CalculateResidualSpaceOld(binPacking, pos));
      if (!residualSpace.IsZero()) {
        residualSpaces.Add(residualSpace);
      }
      return residualSpaces;
    }

    protected Tuple<int, int, int> CalculateResidualSpaceOld(BinPacking3D binPacking, Vector3D pos) {

      if (pos == null) {
        return Tuple.Create(0, 0, 0);
      }
      var itemPos = binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key]
      });
      Vector3D limit = new Vector3D() {
        X = binPacking.BinShape.Width,
        Y = binPacking.BinShape.Height,
        Z = binPacking.BinShape.Depth
      };

      if (pos.X < limit.X && pos.Y < limit.Y && pos.Z < limit.Z) {
        var rightLimit = ProjectRight(binPacking, pos);
        var upLimit = ProjectUp(binPacking, pos);
        var forwardLimit = ProjectForward(binPacking, pos);
        if (rightLimit.X > 0) {
          limit.X = rightLimit.X;
        }
        if (upLimit.Y > 0) {
          limit.Y = upLimit.Y;
        }
        if (forwardLimit.Z > 0) {
          limit.Z = forwardLimit.Z;
        }
      }

      if (limit.X - pos.X <= 0 || limit.Y - pos.Y <= 0 || limit.Z - pos.Z <= 0) {
        return Tuple.Create(0, 0, 0);
      }
      return Tuple.Create(limit.X - pos.X, limit.Y - pos.Y, limit.Z - pos.Z);
    }
  }
}
