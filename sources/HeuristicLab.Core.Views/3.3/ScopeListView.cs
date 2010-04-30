using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ScopeList View")]
  [Content(typeof(ScopeList), true)]
  [Content(typeof(IItemList<IScope>), false)]
  public partial class ScopeListView : ItemListView<IScope> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ScopeListView() {
      InitializeComponent();
      Caption = "ScopeList";
      itemsGroupBox.Text = "Scopes";
    }
  }
}
