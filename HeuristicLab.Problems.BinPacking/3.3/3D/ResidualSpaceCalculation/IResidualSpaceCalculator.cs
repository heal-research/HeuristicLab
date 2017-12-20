using HeuristicLab.Problems.BinPacking3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation {
  public interface IResidualSpaceCalculator {

    /// <summary>
    /// Calculates all available residual spaces for a given point and returns them packed in a collection.
    /// The returned collection has zero elements if the residual space is zero
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    IEnumerable<ResidualSpace> CalculateResidualSpaces(BinPacking3D binPacking, Vector3D point);
  }
}
