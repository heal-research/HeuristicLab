using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {
  //IDataProcessor?
  public interface ITypeTransformer {
    ParameterData Extract(IItem value);
    void Inject(IItem item, ParameterData data);
  }
}

