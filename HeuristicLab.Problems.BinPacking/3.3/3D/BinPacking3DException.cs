using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D {
  public class BinPacking3DException : Exception {
    public BinPacking3DException() : base() {

    }
    public BinPacking3DException(string message) : base(message) {
    }
  }
}
