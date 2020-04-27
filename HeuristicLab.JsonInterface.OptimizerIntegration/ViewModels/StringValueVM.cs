using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class StringValueVM : JsonItemVMBase<StringJsonItem> {
    public override UserControl Control =>
       new JsonItemValidValuesControl(this);

    public string Value {
      get => Item.Value?.ToString();
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    public IEnumerable<string> Range {
      get => Item.ConcreteRestrictedItems;
      set {
        Item.ConcreteRestrictedItems = value;
        //check if value is still in range
        if (!Range.Any(x => x == Value)) {
          Value = Range.FirstOrDefault();
          if (Range.Count() == 0)
            //if no elements exists -> deselect item
            base.Selected = false;
          OnPropertyChange(this, nameof(Value));
        }
        
        OnPropertyChange(this, nameof(Range));
      }
    }
  }

  public class StringArrayVM : JsonItemVMBase<StringArrayJsonItem> {
    public override UserControl Control =>
       new JsonItemConcreteItemArrayControl(this);

    public string[] Value {
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    public IEnumerable<string> Range {
      get => Item.ConcreteRestrictedItems;
      set {
        Item.ConcreteRestrictedItems = value;
        OnPropertyChange(this, nameof(Range));
      }
    }
  }
}
