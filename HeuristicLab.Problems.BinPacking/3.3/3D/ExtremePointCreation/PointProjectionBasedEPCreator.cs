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
    public override void UpdateExtremePoints(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      GenerateNewExtremePointsForNewItem(binPacking, item, position);

      // if an item is fit in below, before, or left of another its extreme points have to be regenerated
      foreach (var above in GetItemsAbove(binPacking, position)) {
        GenerateNewExtremePointsForNewItem(binPacking, above.Item2, above.Item1);
      }
      foreach (var front in GetItemsInfront(binPacking, position)) {
        GenerateNewExtremePointsForNewItem(binPacking, front.Item2, front.Item1);
      }
      foreach (var right in GetItemsOnRight(binPacking, position)) {
        GenerateNewExtremePointsForNewItem(binPacking, right.Item2, right.Item1);
      }
    }
    
    public override void UpdateResidualSpace(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      foreach (var ep in binPacking.ExtremePoints.ToList()) {
        var rs = binPacking.ResidualSpace[ep];
        var depth = position.Rotated ? item.Width : item.Depth;
        var width = position.Rotated ? item.Depth : item.Width;
        var changed = false;
        if (ep.Z >= position.Z && ep.Z < position.Z + depth) {
          if (ep.X <= position.X && ep.Y >= position.Y && ep.Y < position.Y + item.Height) {
            var diff = position.X - ep.X;
            var newRSX = Math.Min(rs.Item1, diff);
            rs = Tuple.Create(newRSX, rs.Item2, rs.Item3);
            changed = true;
          }
          if (ep.Y <= position.Y && ep.X >= position.X && ep.X < position.X + width) {
            var diff = position.Y - ep.Y;
            var newRSY = Math.Min(rs.Item2, diff);
            rs = Tuple.Create(rs.Item1, newRSY, rs.Item3);
            changed = true;
          }
        }

        if (ep.Z <= position.Z &&
            ep.Y >= position.Y && ep.Y < position.Y + item.Height &&
            ep.X >= position.X && ep.X < position.X + width) {
          var diff = position.Z - ep.Z;
          var newRSZ = Math.Min(rs.Item3, diff);
          rs = Tuple.Create(rs.Item1, rs.Item2, newRSZ);
          changed = true;
        }

        if (changed) {
          if (IsNonZero(rs) && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, new Vector3D(ep), rs)) {
            binPacking.ResidualSpace[ep] = rs;
          } else {
            binPacking.ExtremePoints.Remove(ep);
            binPacking.ResidualSpace.Remove(ep);
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
      if (binPacking.ExtremePoints.Add(position)) {
        var rs = CalculateResidualSpace(binPacking, new Vector3D(position));
        binPacking.ResidualSpace.Add(position, rs);
        // Check if the existing extreme points are shadowed by the new point
        // This is, their residual space fit entirely into the residual space of the new point
        foreach (var ep in binPacking.ExtremePoints.Where(x => x != position && new Vector3D(x).IsInside(position, rs)).ToList()) {
          if (IsWithinResidualSpaceOfAnotherExtremePoint(new Vector3D(ep), binPacking.ResidualSpace[ep], position, rs)) {
            binPacking.ExtremePoints.Remove(ep);
            binPacking.ResidualSpace.Remove(ep);
          }
        }
        return true;
      }
      return false;
    }
  }
}
