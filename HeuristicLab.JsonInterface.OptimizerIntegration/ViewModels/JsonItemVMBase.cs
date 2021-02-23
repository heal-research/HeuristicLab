using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public abstract class JsonItemVMBase<JsonItemType> : IJsonItemVM<JsonItemType>
    where JsonItemType : class, IJsonItem 
  {
    IJsonItem IJsonItemVM.Item { 
      get => Item; 
      set => Item = (JsonItemType)value; 
    }

    private JsonItemType item;
    public JsonItemType Item {
      get => item;
      set {
        item = value;
        ItemChanged?.Invoke();
      }
    }
    public TreeNode TreeNode { get; set; }
    public TreeView TreeView { get; set; }
    public bool Selected {
      get => Item.Active;
      set {
        Item.Active = value;
        if(TreeNode != null) {
          TreeNode.ForeColor = (Selected ? Color.Green : Color.Black);
          TreeNode.Checked = value;
        }
        if (TreeView != null)
          TreeView.Refresh();
        SelectedChanged?.Invoke();
        OnPropertyChange(this, nameof(Selected));
      }
    }
    public string Name {
      get => Item.Name;
      set {
        Item.Name = value;
        OnPropertyChange(this, nameof(Name));
      }
    }
    public string Description {
      get => Item.Description;
      set {
        Item.Description = value;
        OnPropertyChange(this, nameof(Description));
      }
    }

    public virtual Type TargetedJsonItemType => typeof(JsonItemType);
    public abstract UserControl Control { get; }

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action ItemChanged;
    public event Action SelectedChanged;

    protected void OnPropertyChange(object sender, string propertyName) {
      // Make a temporary copy of the event to avoid possibility of
      // a race condition if the last subscriber unsubscribes
      // immediately after the null check and before the event is raised.
      // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
      
      PropertyChangedEventHandler tmp = PropertyChanged;
      tmp?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
