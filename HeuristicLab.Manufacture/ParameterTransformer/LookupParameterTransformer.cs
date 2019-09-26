using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class LookupParameterTransformer : ParameterBaseTransformer {
    public override Component ExtractData(IParameter value) {
      return new Component() { Name = value.Name };
    }

    public override void InjectData(IParameter parameter, Component data) {
      
    }
  }
}
