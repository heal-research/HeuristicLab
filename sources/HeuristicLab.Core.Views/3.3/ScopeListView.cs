using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ScopeList View")]
  [Content(typeof(ScopeList), true)]
  [Content(typeof(IObservableList<IScope>), false)]
  public partial class ScopeListView : ItemListView<IScope> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ScopeListView() {
      InitializeComponent();
      Caption = "ScopeList";
      itemsGroupBox.Text = "Scopes";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public ScopeListView(IObservableList<IScope> content)
      : this() {
      Content = content;
    }
  }
}
