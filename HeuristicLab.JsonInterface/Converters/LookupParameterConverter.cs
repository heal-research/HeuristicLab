using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class LookupParameterConverter : ParameterBaseConverter {
    public override JsonItem ExtractData(IParameter value) => 
      new JsonItem() { 
        Name = value.Name,
        ActualName = value.Cast<ILookupParameter>().ActualName
      };

    public override void InjectData(IParameter parameter, JsonItem data) =>
      parameter.Cast<ILookupParameter>().ActualName = data.ActualName.Cast<string>();
  }
}
