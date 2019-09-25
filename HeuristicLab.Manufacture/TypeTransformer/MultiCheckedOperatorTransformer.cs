using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class MultiCheckedOperatorTransformer : ParameterizedItemTransformer {
    public override ParameterData ExtractData(IItem value) {
      ParameterData data = base.ExtractData(value);

      data.Operators = new List<ParameterData>();
      dynamic val = value.Cast<dynamic>();
      foreach (var op in val.Operators) {
        data.Operators.Add(new ParameterData() {
          Name = op.Name,
          Default = val.Operators.ItemChecked(op),
          Range = new object[] { false, true }
        });
      }
      return data;
    }

    public override void InjectData(IItem item, ParameterData data) {
      base.InjectData(item, data);

      dynamic val = item.Cast<dynamic>();
      foreach (var op in val.Operators) {
        val.Operators.SetItemCheckedState(op, GetOperatorState(op.Name, data));
      }
    }

    private bool GetOperatorState(string name, ParameterData data) {
      foreach(var op in data.Operators) {
        if (op.Name == name) return op.Default.Cast<bool>();
      }
      return false;
    }
  }
}
