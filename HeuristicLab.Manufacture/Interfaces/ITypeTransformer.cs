using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace ParameterTest {
  public interface ITypeTransformer {
    ParameterData ToData(IItem value);
    //IItem FromData(ParameterData obj, Type targetType);

    void SetValue(IItem item, ParameterData obj);
    //void SetParameter(ParameterData data, IParameter parameter, Type )
  }
}
