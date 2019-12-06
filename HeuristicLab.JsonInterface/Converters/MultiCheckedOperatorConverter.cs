using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class MultiCheckedOperatorConverter : ParameterizedItemConverter {
    public override JsonItem ExtractData(IItem value) {
      JsonItem data = base.ExtractData(value);

      if(data.Parameters == null)
        data.Parameters = new List<JsonItem>();
      dynamic val = value.Cast<dynamic>();
      foreach (var op in val.Operators) {
        data.Parameters.Add(new JsonItem() {
          Name = op.Name,
          Value = val.Operators.ItemChecked(op),
          Range = new object[] { false, true }/*,
          Path = data.Path + "." + op.Name*/
        });
      }
      return data;
    }

    public override void InjectData(IItem item, JsonItem data) {
      base.InjectData(item, data);

      dynamic val = item.Cast<dynamic>();
      foreach (var op in val.Operators)
        val.Operators.SetItemCheckedState(op, GetOperatorState(op.Name, data));
    }

    private bool GetOperatorState(string name, JsonItem data) {
      foreach(var op in data.Operators) {
        if (op.Name == name) return op.Value.Cast<bool>();
      }
      return false;
    }
  }
}
