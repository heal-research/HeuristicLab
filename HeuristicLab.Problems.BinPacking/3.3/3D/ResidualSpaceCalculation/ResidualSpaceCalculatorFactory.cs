using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation {
  public static class ResidualSpaceCalculatorFactory {
    public static IResidualSpaceCalculator CreateCalculator() {
      return new ResidualSpaceCalculator();
    }
  }
}
