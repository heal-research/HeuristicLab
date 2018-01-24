using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Material {
  public class FrictionalCoefficientTable {
    private FrictionalCoefficientTable _instance;

    //private MaterialType 

    private FrictionalCoefficientTable() {

    }

    public FrictionalCoefficientTable Instance {
      get {
        if (_instance == null) {
          _instance = new FrictionalCoefficientTable();
        }
        return _instance;
      }
    }




  }
}
