using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class LookupParameterTransformer : ParameterBaseTransformer {
    public override ParameterData ExtractData(IParameter value) {
      return new ParameterData() { Name = value.Name };
    }

    public override void InjectData(IParameter parameter, ParameterData data) {
      
    }
  }
}
