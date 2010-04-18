using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("VariableCollection View")]
  [Content(typeof(VariableCollection), true)]
  [Content(typeof(IKeyedItemCollection<string, IVariable>), false)]
  public partial class VariableCollectionView : NamedItemCollectionView<IVariable> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public VariableCollectionView() {
      InitializeComponent();
      Caption = "VariableCollection";
      itemsGroupBox.Text = "Variables";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public VariableCollectionView(IKeyedItemCollection<string, IVariable> content)
      : this() {
      Content = content;
    }

    protected override IVariable CreateItem() {
      IVariable item = new Variable();
      item.Name = GetUniqueName(item.Name);
      return item;
    }
  }
}
