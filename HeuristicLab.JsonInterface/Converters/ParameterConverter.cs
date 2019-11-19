using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ParameterConverter : ParameterBaseConverter {
    public override JsonItem ExtractData(IParameter value) {
      JsonItem item = new JsonItem() { 
        Name = value.Name  
      };
      if(value.ActualValue != null) {
        item.Parameters = new List<JsonItem>();
        item.Parameters.Add(JsonItemConverter.Extract(value.ActualValue));
      }
      return item;
    }

    public override void InjectData(IParameter parameter, JsonItem data) {
      throw new NotImplementedException();
    }
  }
}
