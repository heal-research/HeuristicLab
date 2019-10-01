using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class LookupParameterConverter : ParameterBaseConverter {
    public override JsonItem ExtractData(IParameter value) {
      return new JsonItem() { Name = value.Name };
    }

    public override void InjectData(IParameter parameter, JsonItem data) {
      
    }
  }
}
