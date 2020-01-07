using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class LookupParameterConverter : ParameterBaseConverter {
    public override int Priority => 3;
    public override Type ConvertableType => typeof(ILookupParameter);

    public override void Populate(IParameter value, JsonItem item, IJsonItemConverter root) {
      item.Name = value.Name;
      item.ActualName = ((ILookupParameter)value).ActualName;
    } 

    public override void InjectData(IParameter parameter, JsonItem data, IJsonItemConverter root) =>
      ((ILookupParameter)parameter).ActualName = data.ActualName as string;
  }
}
