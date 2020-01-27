using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public delegate void OnSelectionChangeHandler(JsonItemVM sender, bool selection);
  public delegate void OnChildAddedHandler(JsonItemVM sender, JsonItemVM child);

  public class JsonItemVM {
    public IJsonItem Item { get; set; }

    private IList<JsonItemVM> children = new List<JsonItemVM>();
    public IEnumerable<JsonItemVM> Children { 
      get => children; 
    }

    public JsonItemVM Parent { get; set; }

    private bool selected = true;
    public bool Selected {
      get => selected; 
      set {
        selected = value;
        OnSelectionChange?.Invoke(this, Selected);
      } 
    }

    public event OnSelectionChangeHandler OnSelectionChange;
    public event OnChildAddedHandler OnChildAdded;

    public JsonItemVM(IJsonItem item) {
      this.Item = item;
    }

    public void AddChild(JsonItemVM vm) {
      children.Add(vm);
      vm.Parent = this;
      OnChildAdded?.Invoke(this, vm);
    }
  }
}
