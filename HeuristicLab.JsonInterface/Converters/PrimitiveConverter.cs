using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface.Converters {
  public class PrimitiveConverter<T> : BaseConverter where T : struct {
    public override JsonItem ExtractData(IItem value) {
      throw new NotImplementedException();
    }

    public override void InjectData(IItem item, JsonItem data) {
      throw new NotImplementedException();
    }
  }
}
