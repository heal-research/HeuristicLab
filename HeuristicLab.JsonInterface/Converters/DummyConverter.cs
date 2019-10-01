using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class DummyConverter : BaseConverter {
    public override void InjectData(IItem item, JsonItem data) {
      // do nothing because the instance already exists and 
      // there are no values to inject
    }

    public override JsonItem ExtractData(IItem value) => 
      new JsonItem() { Default = value.GetType().Name };
  }
}
