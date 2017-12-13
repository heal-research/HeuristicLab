using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  public abstract class ExtremePointCreator : IExtremePointCreator {


    public void UpdateBinPacking(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      UpdateExtremePoints(binPacking, item, position);
      UpdateResidualSpace(binPacking, item, position);
    }

    protected abstract void UpdateExtremePoints(BinPacking3D binPacking, PackingItem item, PackingPosition position);

    /// <summary>
    /// Updates the residual space for a given bin packing
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="item"></param>
    /// <param name="position"></param>
    protected abstract void UpdateResidualSpace(BinPacking3D binPacking, PackingItem item, PackingPosition position);
    protected abstract bool AddExtremePoint(BinPacking3D binPacking, PackingPosition position);

    /// <summary>
    /// Generates new extreme points for a given item and its position.
    /// It also recalcualtes the residual space and removes the extreme points which are not needed any more.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="newItem"></param>
    /// <param name="position"></param>
    protected virtual void GenerateNewExtremePointsForItem(BinPacking3D binPacking, PackingItem newItem, PackingPosition position) {
      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;
      var binShape = binPacking.BinShape;

      var itemPlacement = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] }).ToList();
      // Find ExtremePoints beginning from sourcepointX
      var sourcePoint = new Vector3D() { X = position.X + newWidth, Y = position.Y, Z = position.Z };
      if (sourcePoint.X < binShape.Width && sourcePoint.Y < binShape.Height && sourcePoint.Z < binShape.Depth) {
        // Projecting onto the XZ-plane
        var below = ProjectDown(binPacking, sourcePoint);
        var residualSpace = CalculateResidualSpace(binPacking, below);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(binPacking, belowPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(binPacking, sourcePoint);
        residualSpace = CalculateResidualSpace(binPacking, back);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, back, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var backPoint = new PackingPosition(position.AssignedBin, back.X, back.Y, back.Z);
          AddExtremePoint(binPacking, backPoint);
        }
      }

      //Find ExtremePoints beginning from sourcepointY
      sourcePoint = new Vector3D(position.X, position.Y + newItem.Height, position.Z);
      if (sourcePoint.X < binShape.Width && sourcePoint.Y < binShape.Height && sourcePoint.Z < binShape.Depth) {
        // Projecting onto the YZ-plane
        var left = ProjectLeft(binPacking, sourcePoint);
        var residualSpace = CalculateResidualSpace(binPacking, left);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(binPacking, leftPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(binPacking, sourcePoint);
        residualSpace = CalculateResidualSpace(binPacking, back);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, back, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var backPoint = new PackingPosition(position.AssignedBin, back.X, back.Y, back.Z);
          AddExtremePoint(binPacking, backPoint);
        }
      }

      //Find ExtremePoints beginning from sourcepointZ
      sourcePoint = new Vector3D(position.X, position.Y, position.Z + newDepth);
      if (sourcePoint.X < binShape.Width && sourcePoint.Y < binShape.Height && sourcePoint.Z < binShape.Depth) {
        // Projecting onto the XZ-plane
        var below = ProjectDown(binPacking, sourcePoint);
        var residualSpace = CalculateResidualSpace(binPacking, below);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(binPacking, belowPoint);
        }
        // Projecting onto the YZ-plane
        var left = ProjectLeft(binPacking, sourcePoint);
        residualSpace = CalculateResidualSpace(binPacking, left);
        if (!residualSpace.IsZero()
          && !IsWithinResidualSpaceOfAnotherExtremePoint(binPacking, left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(binPacking, leftPoint);
        }
      }
    }

    #region Get items

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsAbove(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Bottom))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsInfront(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Back))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsOnRight(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Left))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBelow(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Top))
      }).Where(x => x.Intersection != null && x.Intersection.Y <= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBehind(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Front))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsOnLeft(BinPacking3D binPacking, PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(binPacking.Positions[x.Key], x.Value, Side.Right))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    #endregion

    #region Residual space


    /// <summary>
    /// 
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="pos"></param>
    /// <param name="residualSpace"></param>
    /// <returns></returns>
    protected virtual bool IsWithinResidualSpaceOfAnotherExtremePoint(BinPacking3D binPacking, Vector3D pos, ResidualSpace residualSpace) {
      var eps = binPacking.ExtremePoints.Where(x => !pos.Equals(x) && pos.IsInside(x, binPacking.ResidualSpaces[x]));
      return eps.Any(x => IsWithinResidualSpaceOfAnotherExtremePoint(pos, residualSpace, x, binPacking.ResidualSpaces[x]));
    }

    /// <summary>
    /// Returns true, if the given poisition and the related residual space is within the residual space of the given extreme point
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rsPos"></param>
    /// <param name="ep"></param>
    /// <param name="rsEp"></param>
    /// <returns></returns>
    protected virtual bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, ResidualSpace rsPos, PackingPosition ep, ResidualSpace rsEp) {
      return rsEp.Width >= pos.X - ep.X + rsPos.Width
          && rsEp.Height >= pos.Y - ep.Y + rsPos.Height
          && rsEp.Depth >= pos.Z - ep.Z + rsPos.Depth;
    }

    /// <summary>
    /// Calculates the residual space for a given bin packing and position.
    /// It returns the residual space as a tuple which contains the dimensions
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="pos"></param>
    /// <returns>A Tuple(width, Height, Depth)</width></returns>
    protected virtual ResidualSpace CalculateResidualSpace(BinPacking3D binPacking, Vector3D pos) {
      //return ResidualSpaceCalculator.Create().CalculateResidualSpace(binPacking, pos).First();
      return new ResidualSpace(CalculateResidualSpaceOld(binPacking, pos));
    }

    protected virtual Tuple<int, int, int> CalculateResidualSpaceOld(BinPacking3D binPacking, Vector3D pos) {

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

    #endregion

    #region Projections

    protected Vector3D ProjectBackward(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, -1));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                             .Select(x => new Plane3D(x.Position, x.Item, Side.Front))
                             .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Back) })
                             .Select(x => x.Intersect(line))
                             .Where(x => x != null && x.Z <= pos.Z);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }

    protected Vector3D ProjectLeft(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(-1, 0, 0));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Right))
                  .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Left) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.X <= pos.X);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }

    protected Vector3D ProjectDown(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, -1, 0));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Top))
                  .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Bottom) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Y <= pos.Y);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }

    protected Vector3D ProjectForward(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Back))
                  .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Front) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Z >= pos.Z);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }

    protected Vector3D ProjectRight(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Left))
                  .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Right) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.X >= pos.X);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }

    protected Vector3D ProjectUp(BinPacking3D binPacking, Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      var m = binPacking.Items.Select(x => new { Item = x.Value, Position = binPacking.Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Bottom))
                  .Concat(new[] { new Plane3D(binPacking.BinShape, Side.Top) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Y >= pos.Y);
      if (m.Where(x => x != null).Any()) {
        return m.MaxItems(x => x.Y).First();
      }
      return null;
    }
    #endregion
  }
}
