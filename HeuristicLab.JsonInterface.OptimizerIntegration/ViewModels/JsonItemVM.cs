using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class JsonItemVM {
    public JsonItem Item { get; set; }

    public bool Selected { get; set; } = true;

    public JsonItemVM(JsonItem item) {
      this.Item = item;
    }
  }
}
