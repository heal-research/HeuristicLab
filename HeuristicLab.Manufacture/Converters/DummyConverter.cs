using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class DummyConverter : BaseConverter {
    public override void InjectData(IItem item, Component data) {
      // do nothing because the instance already exists and 
      // there are no values to inject
    }

    public override Component ExtractData(IItem value) => 
      new Component() { Default = value.GetType().Name };
  }
}
