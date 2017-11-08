using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {

  /// <summary>
  /// This class can be used for creating bin packer insances
  /// </summary>
  public static class BinPackerFactory {

    /// <summary>
    /// Returns the bin packer depending on the given fitting method
    /// </summary>
    /// <param name="fittingMethod"></param>
    /// <returns></returns>
    public static BinPacker CreateBinPacker(FittingMethod fittingMethod) {
      BinPacker binPacker = null;
      switch (fittingMethod) {
        case FittingMethod.FirstFit:
          binPacker = new BinPackerFirstFit();
          break;
        case FittingMethod.FreeVolumeBestFit:
          binPacker = new BinPackerFreeVolumeBestFit();
          break;
        case FittingMethod.ResidualSpaceBestFit:
          binPacker = new BinPackerResidualSpaceBestFit();
          break;
        default:
          throw new ArgumentException("Unknown fitting method: " + fittingMethod);
      }
      return binPacker;
    }
  }
}
