using HeuristicLab.Problems.BinPacking3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation {
  internal class ResidualSpaceCalculator : IResidualSpaceCalculator {

    internal ResidualSpaceCalculator() {}

    public IEnumerable<ResidualSpace> CalculateResidualSpaces(BinPacking3D binPacking, Vector3D point) {
      IList<ResidualSpace> residualSpaces = new List<ResidualSpace>();
      var rs1 = CalculateXZY(binPacking, point);
      var rs2 = CalculateZYX(binPacking, point);
      var rs3 = CalculateYXZ(binPacking, point);

      residualSpaces.Add(rs1);


      if (!residualSpaces.Any(rs => rs.Equals(rs2))) {
        residualSpaces.Add(rs2);
      }
      if (!residualSpaces.Any(rs => rs.Equals(rs3))) {
        residualSpaces.Add(rs3);
      }
      return residualSpaces;
    }

    private ResidualSpace CalculateXZY(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceOnRight(binPacking, point, rs);
      LimitResidualSpaceInFront(binPacking, point, rs);
      LimitResidualSpaceAbove(binPacking, point, rs);
      return rs;
    }

    private ResidualSpace CalculateZYX(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceInFront(binPacking, point, rs);
      LimitResidualSpaceAbove(binPacking, point, rs);
      LimitResidualSpaceOnRight(binPacking, point, rs);
      return rs;
    }

    private ResidualSpace CalculateYXZ(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceAbove(binPacking, point, rs);
      LimitResidualSpaceOnRight(binPacking, point, rs);
      LimitResidualSpaceInFront(binPacking, point, rs);
      return rs;
    }
    
    private bool OverlapsX(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.X > position.X && point.X >= position.X + item.Width) {
        return false;
      }

      if (point.X <= position.X && position.X >= point.X + residualSpace.Width) {
        return false;
      }
      return true;
    }
    
    private bool OverlapsY(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.Y > position.Y && point.Y >= position.Y + item.Height) {
        return false;
      }

      if (point.Y <= position.Y && position.Y >= point.Y + residualSpace.Height) {
        return false;
      }
      return true;
    }

    private bool OverlapsZ(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.Z > position.Z && point.Z >= position.Z + item.Depth) {
        return false;
      }

      if (point.Z <= position.Z && position.Z >= point.Z + residualSpace.Depth) {
        return false;
      }
      return true;
    }

    private bool OverlapsOnRight(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.X >= position.X) {
        return false;
      }
      var y = OverlapsY(point, residualSpace, position, item);
      var z = OverlapsZ(point, residualSpace, position, item);
      return y && z;
    }

    private bool OverlapsInFront(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.Z >= position.Z) {
        return false;
      }
      var x = OverlapsX(point, residualSpace, position, item);
      var y = OverlapsY(point, residualSpace, position, item);

      return x && y;
    }

    private bool OverlapsAbove(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item ) {
      if (point.Y >= position.Y) {
        return false;
      }
      var x = OverlapsX(point, residualSpace, position, item);
      var z = OverlapsZ(point, residualSpace, position, item);

      return x && z;
    }
    
    private void LimitResidualSpaceOnRight(BinPacking3D binPacking, Vector3D point, ResidualSpace residualSpace) {
      if (residualSpace.IsZero()) {
        return;
      }
      
      var items = binPacking.Items.Select(item => new { Dimension = item.Value, Position = binPacking.Positions[item.Key] })
                                  .Where(item => OverlapsOnRight(point, residualSpace, item.Position, item.Dimension));
      if (items.Count() > 0) {
        foreach (var item in items) {
          int newWidth = item.Position.X - point.X;
          if (newWidth <= 0) {
            residualSpace.SetZero();
            return;
          } else if (residualSpace.Width > newWidth) {
            residualSpace.Width = newWidth;
          }
        }
      }      
    }

    private void LimitResidualSpaceInFront(BinPacking3D binPacking, Vector3D point, ResidualSpace residualSpace) {
      if (residualSpace.IsZero()) {
        return;
      }

      var items = binPacking.Items.Select(item => new { Dimension = item.Value, Position = binPacking.Positions[item.Key] })
                                  .Where(item => OverlapsInFront(point, residualSpace, item.Position, item.Dimension));
      if (items.Count() > 0) {
        foreach (var item in items) {
          int newDepth = item.Position.Z - point.Z;
          if (newDepth <= 0) {
            residualSpace.SetZero();
            return;
          } else if (residualSpace.Depth > newDepth) {
            residualSpace.Depth = newDepth;
          }
        }
      }
    }

    private void LimitResidualSpaceAbove(BinPacking3D binPacking, Vector3D point, ResidualSpace residualSpace) {
      if (residualSpace.IsZero()) {
        return;
      }

      var items = binPacking.Items.Select(item => new { Dimension = item.Value, Position = binPacking.Positions[item.Key] })
                                  .Where(item => OverlapsAbove(point, residualSpace, item.Position, item.Dimension));
      if (items.Count() > 0) {
        foreach (var item in items) {
          int newHeight = item.Position.Y - point.Y;
          if (newHeight <= 0) {
            residualSpace.SetZero();
            return;
          } else if (residualSpace.Height > newHeight) {
            residualSpace.Height = newHeight;
          }
        }
      }
    }
  }
}
