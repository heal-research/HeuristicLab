using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class JsonItemVMBase : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action ItemChanged;

    private IJsonItem item;
    public IJsonItem Item {
      get => item;
      set {
        item = value;
        ItemChanged?.Invoke();
      }
    }

    public TreeNode TreeNode { get; set; }
    public TreeView TreeView { get; set; }

    protected void OnPropertyChange(object sender, string propertyName) {
      // Make a temporary copy of the event to avoid possibility of
      // a race condition if the last subscriber unsubscribes
      // immediately after the null check and before the event is raised.
      // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
      
      PropertyChangedEventHandler tmp = PropertyChanged;
      tmp?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual Type JsonItemType => typeof(JsonItem);

    private bool selected = true;
    public bool Selected {
      get => selected;
      set {
        selected = value;
        if(TreeNode != null) {
          TreeNode.ForeColor = (selected ? Color.Black : Color.Red);
          TreeNode.Checked = value;
        }
        if (TreeView != null)
          TreeView.Refresh();
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

    public string ActualName {
      get => Item.ActualName;
      set {
        Item.ActualName = value;
        OnPropertyChange(this, nameof(ActualName));
      }
    }

    public virtual JsonItemBaseControl GetControl() {
      return new JsonItemBaseControl(this);
    }

  }
}
