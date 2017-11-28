using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  /// <summary>
  /// This extreme point creation class uses the line projection based method for creating extreme points.
  /// </summary>
  public class LineProjectionBasedEPCreator : ExtremePointCreator {


    

    public override void UpdateExtremePoints(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      // Before any extreme point for an item can be created, each residual space must be resized if the new item is in the residual space.
      // If the residual spaces were not updated, it could be that an extreme point won't be generated because it lies in a residual space which 
      // size isn't correct anymore.
      RecalculateResidualSpaces(binPacking, item, position);

      GenerateNewExtremePointsForNewItem(binPacking, item, position);

      foreach (var ep in GetEpsOnLeft(binPacking, item, position)) {
        AddExtremePoint(binPacking, ep);
      }

      foreach (var ep in GetEpsBelow(binPacking, item, position)) {
        AddExtremePoint(binPacking, ep);
      }

      foreach (var ep in GetEpsBehind(binPacking, item, position)) {
        AddExtremePoint(binPacking, ep);
      }
    }

    /// <summary>
    /// Returns true, if the given poisition and the related residual space is within the residual space of the given extreme point
    /// </summary>
    /// <param name="pos">New Position</param>
    /// <param name="rsPos"></param>
    /// <param name="ep"></param>
    /// <param name="rsEp"></param>
    /// <returns></returns>
    protected override bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> rsPos, PackingPosition ep, Tuple<int, int, int> rsEp) {
      bool x = pos.X >= ep.X && rsPos.Item1 + pos.X <= rsEp.Item1 + ep.X;
      bool y = (pos.Y >= ep.Y && pos.Y == 0 || pos.Y > ep.Y && pos.Y > 0) && rsPos.Item2 + pos.Y <= rsEp.Item2 + ep.Y;
      bool z = pos.Z >= ep.Z && rsPos.Item3 + pos.Z <= rsEp.Item3 + ep.Z;
      return x && y && z;
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

    /// <summary>
    /// Returns true if an item is in the residual space of a given extrem point
    /// </summary>
    /// <param name="rs">KeyValuePair with the position of the extreme point and the size of the residual space</param>
    /// <param name="item">Given Item</param>
    /// <param name="position">Given position</param>
    /// <returns></returns>
    private bool ItemIsInRs(KeyValuePair<PackingPosition, Tuple<int, int, int>> rs, PackingItem item, PackingPosition position) {
      return GetVertices(item, position).Where(pos => pos.IsInside(rs.Key, rs.Value)).Any();
    }

    /// <summary>
    /// Recalculates the residual spaces if needed.
    /// It checks if an item is in an residual space and if so the residual space will be recalculated
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="item"></param>
    /// <param name="position"></param>
    private void RecalculateResidualSpaces(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      var recalculatedSpaces = new Dictionary<PackingPosition, Tuple<int, int, int>>();
      foreach (var rs in binPacking.ResidualSpace) {
        int width = rs.Value.Item1;
        int height = rs.Value.Item2;
        int depth = rs.Value.Item3;

        if (ItemIsInRs(rs, item, position)) {
          if (rs.Key.X + rs.Value.Item1 > position.X) {
            width = position.X - rs.Key.X;
          } else if (rs.Key.Y + rs.Value.Item2 > position.Y) {
            height = position.Y - rs.Key.Y;
          } else if (rs.Key.Z + rs.Value.Item3 > position.Z) {
            depth = position.Z - rs.Key.Z;
          }
        }

        var newRs = new Tuple<int, int, int>(width, height, depth);
        if (IsNonZero(newRs)) {
          recalculatedSpaces.Add(rs.Key, newRs);
        } else {
          recalculatedSpaces.Add(rs.Key, rs.Value);
        }
      }
      binPacking.ResidualSpace = recalculatedSpaces;
    }

    /// <summary>
    /// Returns the extremepoints on the left side of an given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IList<PackingPosition> GetEpsOnLeft(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      IList<PackingPosition> eps = new List<PackingPosition>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsOnLeft(binPacking, position);
      var edges = GetProjectionEdgesOnLeft(item, position);

      foreach (var i in items) {
        foreach (var edge in edges) {
          var newEps = IntersectionsForItem(edge, GetEdgesOnRight(i.Item2, i.Item1));
          foreach (var ep in newEps) {
            try {
              if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
                var point = ProjectLeft(binPacking, ep);
                var residualSpace = CalculateResidualSpace(binPacking, point);
                if (IsNonZero(residualSpace)) {
                  eps.Add(new PackingPosition(position.AssignedBin, point.X, point.Y, point.Z));
                }
              }
            } catch {
              var s = ep;
            }
          }
        }
      }
      return eps;
    }

    /// <summary>
    /// Returns the extremepoints below of an given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IList<PackingPosition> GetEpsBelow(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      IList<PackingPosition> eps = new List<PackingPosition>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsBelow(binPacking, position);
      var edges = GetProjectionEdgesBelow(item, position);

      foreach (var i in items) {
        foreach (var edge in edges) {
          var newEps = IntersectionsForItem(edge, GetEdgesOnTop(i.Item2, i.Item1));
          foreach (var ep in newEps) {
            try {
              var point = ProjectDown(binPacking, ep);
              var residualSpace = CalculateResidualSpace(binPacking, point);
              if (IsNonZero(residualSpace)) {
                eps.Add(new PackingPosition(position.AssignedBin, point.X, point.Y, point.Z));
              }
            } catch {
              var s = ep;
            }
          }
        }
      }
      return eps;
    }

    /// <summary>
    /// Returns the extremepoints on the left side of an given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IList<PackingPosition> GetEpsBehind(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      IList<PackingPosition> eps = new List<PackingPosition>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsBehind(binPacking, position);
      var edges = GetProjectionEdgesBehind(item, position);

      foreach (var i in items) {
        foreach (var edge in edges) {
          var newEps = IntersectionsForItem(edge, GetEdgesInFront(i.Item2, i.Item1));
          foreach (var ep in newEps) {
            try {
              var point = ProjectBackward(binPacking, ep);
              var residualSpace = CalculateResidualSpace(binPacking, point);
              if (IsNonZero(residualSpace)) {
                eps.Add(new PackingPosition(position.AssignedBin, point.X, point.Y, point.Z));
              }
            } catch {
              var s = ep;
            }
          }
        }
      }
      return eps;
    }

    #region Methods for getting the edges for items

    /// <summary>
    /// Returns an array of packing position which represents the vertices of an item.
    /// The position of a vertex in the array is mapped to an item as followed:
    ///      4----------5
    ///     /|         /|
    ///    / |        / |
    ///   /  0-------/--1
    ///  6--/-------7  /
    ///  | /        | /
    ///  |/         |/
    ///  2----------3
    ///  
    ///  0 = (0,0,0)
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3D[] GetVertices(PackingItem item, PackingPosition position) {
      int width = position.Rotated ? item.Depth : item.Width;
      int depth = position.Rotated ? item.Width : item.Depth;
      return new Vector3D[] {
        new Vector3D(position.X + 0,     position.Y + 0,           position.Z + 0), // (0,0,0)
        new Vector3D(position.X + width, position.Y + 0,           position.Z + 0), // (x,0,0)
        new Vector3D(position.X + 0,     position.Y + 0,           position.Z + depth), // (0,0,z)
        new Vector3D(position.X + width, position.Y + 0,           position.Z + depth), // (x,0,z)

        new Vector3D(position.X + 0,     position.Y + item.Height, position.Z + 0), // (0,y,0)
        new Vector3D(position.X + width, position.Y + item.Height, position.Z + 0), // (x,y,0)
        new Vector3D(position.X + 0,     position.Y + item.Height, position.Z + depth), // (0,y,z)
        new Vector3D(position.X + width, position.Y + item.Height, position.Z + depth), //(x,y,z)
      };
    }

    private Edge3D[] GetProjectionEdgesOnLeft(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[6]),
        new Edge3D(points[6], points[4])
      };
    }

    private Edge3D[] GetProjectionEdgesBelow(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[3]),
        new Edge3D(points[3], points[1])
      };
    }

    private Edge3D[] GetProjectionEdgesBehind(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[1], points[5]),
        new Edge3D(points[5], points[4])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges of the rigth side of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesOnRight(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[1], points[5]),
        new Edge3D(points[5], points[7]),
        new Edge3D(points[7], points[3]),
        new Edge3D(points[3], points[1])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges on the top of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesOnTop(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[4], points[5]),
        new Edge3D(points[5], points[7]),
        new Edge3D(points[7], points[6]),
        new Edge3D(points[6], points[4])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges in front of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesInFront(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[3]),
        new Edge3D(points[3], points[7]),
        new Edge3D(points[7], points[6]),
        new Edge3D(points[6], points[2])
      };
    }

    #endregion


    #region Intersections

    /// <summary>
    /// Returns a list of extreme points.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="projectedEdge3D"></param>
    /// <returns></returns>
    private IEnumerable<Vector3D> IntersectionsForItem(Edge3D projectedEdge, Edge3D[] edges) {
      IList<Vector3D> eps = new List<Vector3D>();
      foreach (var edge in edges) {
        var ep = edge.Intersects(projectedEdge);
        if (ep != null) {
          eps.Add(ep);
        }
      }
      return eps as IEnumerable<Vector3D>;
    }


    #endregion

  }
}
