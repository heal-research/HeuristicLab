using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.JsonInterface;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class JsonItemVM: INotifyPropertyChanged
  {
    private JsonItem Item { get; }

    public TreeNode TreeNode { get; }

    public TreeView TreeView { get; }

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

    public IEnumerable<KeyValuePair<string, string>> Properties { get; }


    //public virtual Type TargetedJsonItemType => typeof(JsonItemType);
    //public abstract UserControl Control { get; }

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action ItemChanged;
    public event Action SelectedChanged;

    public JsonItemVM(JsonItem item, TreeView treeView, TreeNode treeNode) {
      Item = item;
      Properties = Item.Properties
        .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.GetType().Name));
      TreeView = treeView;
      TreeNode = treeNode;
    }

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
