using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public interface IJsonItemVM : INotifyPropertyChanged, IDisposable {
    event Action ItemChanged;

    Type JsonItemType { get; }

    JsonItemBaseControl Control { get; }

    IJsonItem Item { get; set; }

    bool Selected { get; set; }

    string Name { get; set; }

    string Description { get; set; }

    string ActualName { get; set; }

    TreeNode TreeNode { get; set; }

    TreeView TreeView { get; set; }
  }
}
