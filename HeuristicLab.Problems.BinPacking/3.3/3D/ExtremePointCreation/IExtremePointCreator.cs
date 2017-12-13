using HeuristicLab.Problems.BinPacking3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  public interface IExtremePointCreator {

    /// <summary>
    /// Updates the extreme points for a given bin packing
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="item"></param>
    /// <param name="position"></param>
    void UpdateBinPacking(BinPacking3D binPacking, PackingItem item, PackingPosition position);

    
  }
}
