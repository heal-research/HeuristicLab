using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  public static class ExtremePointCreatorFactory {
    public static IExtremePointCreator CreateExtremePointCreator(ExtremePointCreationMethod epGenerationMethod, bool useStackingConstraints) {
      if (epGenerationMethod == ExtremePointCreationMethod.PointProjection) {
        return new PointProjectionBasedEPCreator();
      } else if (epGenerationMethod == ExtremePointCreationMethod.LineProjection) {
        return new LineProjectionBasedEPCreator();
      }
      return new PointProjectionBasedEPCreator();
    }
  }
}
