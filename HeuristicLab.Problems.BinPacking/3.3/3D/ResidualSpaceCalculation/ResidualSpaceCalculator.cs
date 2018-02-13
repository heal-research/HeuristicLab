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

using HeuristicLab.Core;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation {
  internal class ResidualSpaceCalculator : Item, IResidualSpaceCalculator {

    internal ResidualSpaceCalculator() {}

    [StorableConstructor]
    protected ResidualSpaceCalculator(bool deserializing) : base(deserializing) { }

    protected ResidualSpaceCalculator(ResidualSpaceCalculator original, Cloner cloner) 
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResidualSpaceCalculator(this, cloner);
    }

    public IEnumerable<ResidualSpace> CalculateResidualSpaces(BinPacking3D binPacking, Vector3D point) {
      IList<ResidualSpace> residualSpaces = new List<ResidualSpace>();
      var rs1 = CalculateXZY(binPacking, point);
      var rs2 = CalculateZYX(binPacking, point);
      var rs3 = CalculateYXZ(binPacking, point);

      if (!rs1.IsZero()) {
        residualSpaces.Add(rs1);
      }

      if (!rs2.IsZero() && !residualSpaces.Any(rs => rs.Equals(rs2))) {
        residualSpaces.Add(rs2);
      }
      if (!rs3.IsZero() && !residualSpaces.Any(rs => rs.Equals(rs3))) {
        residualSpaces.Add(rs3);
      }
      return residualSpaces;
    }

    /// <summary>
    /// Calculates a resiual space by expanding to the limits of the axis in the following order:
    /// 1. x
    /// 2. z
    /// 3. y
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private ResidualSpace CalculateXZY(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceOnRight(binPacking, point, rs);
      LimitResidualSpaceInFront(binPacking, point, rs);
      LimitResidualSpaceAbove(binPacking, point, rs);
      return rs;
    }

    /// <summary>
    /// Calculates a resiual space by expanding to the limits of the axis in the following order:
    /// 1. z
    /// 2. y
    /// 3. x
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private ResidualSpace CalculateZYX(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceInFront(binPacking, point, rs);
      LimitResidualSpaceAbove(binPacking, point, rs);
      LimitResidualSpaceOnRight(binPacking, point, rs);
      return rs;
    }

    /// <summary>
    /// Calculates a resiual space by expanding to the limits of the axis in the following order:
    /// 1. y
    /// 2. x
    /// 3. z
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private ResidualSpace CalculateYXZ(BinPacking3D binPacking, Vector3D point) {
      ResidualSpace rs = new ResidualSpace(binPacking, point);

      LimitResidualSpaceAbove(binPacking, point, rs);
      LimitResidualSpaceOnRight(binPacking, point, rs);
      LimitResidualSpaceInFront(binPacking, point, rs);
      return rs;
    }
    
    /// <summary>
    /// Returnst true if a given residual space and item overlaps at the x-axis
    /// </summary>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
    /// <param name="position"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool OverlapsX(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.X > position.X && point.X >= position.X + item.Width) {
        return false;
      }

      if (point.X <= position.X && position.X >= point.X + residualSpace.Width) {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Returnst true if a given residual space and item overlaps at the y-axis
    /// </summary>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
    /// <param name="position"></param>
    /// <param name="item"></param>
    private bool OverlapsY(Vector3D point, ResidualSpace residualSpace, PackingPosition position, PackingItem item) {
      if (point.Y > position.Y && point.Y >= position.Y + item.Height) {
        return false;
      }

      if (point.Y <= position.Y && position.Y >= point.Y + residualSpace.Height) {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Returnst true if a given residual space and item overlaps at the z-axis
    /// </summary>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
    /// <param name="position"></param>
    /// <param name="item"></param>
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
      // if point.x >= position.x => the residual space is being located on the left side!
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

    /// <summary>
    /// Recalculates the width of a given residual space.
    /// The new width is being limited by any item right of the residual space or the dimension of the bin shape.
    /// If the new width is zero, the whole residual space is being set to zero.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
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

    /// <summary>
    /// Recalculates the depth of a given residual space.
    /// The new depth is being limited by any item in front of the residual space or the dimension of the bin shape.
    /// If the new depth is zero, the whole residual space is being set to zero.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
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

    /// <summary>
    /// Recalculates the height of a given residual space.
    /// The new height is being limited by any item above the residual space or the dimension of the bin shape.
    /// If the new height is zero, the whole residual space is being set to zero.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <param name="residualSpace"></param>
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
