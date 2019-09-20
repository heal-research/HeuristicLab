using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class DummyTransformer : BaseTransformer {
    public override void InjectData(IItem item, ParameterData data) {
      // do nothing because the instance already exists and 
      // there are no values to inject
    }

    public override ParameterData ExtractData(IItem value) => new ParameterData();
  }
}
