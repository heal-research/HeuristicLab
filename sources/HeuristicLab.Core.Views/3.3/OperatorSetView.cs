using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("OperatorSet View")]
  [Content(typeof(OperatorSet), true)]
  [Content(typeof(IItemSet<IOperator>), false)]
  public partial class OperatorSetView : ItemSetView<IOperator> {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OperatorSetView() {
      InitializeComponent();
      Caption = "Operator Set";
      itemsGroupBox.Text = "Operators";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public OperatorSetView(IItemSet<IOperator> content)
      : this() {
      Content = content;
    }

    protected override IOperator CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.TypeSelector.Caption = "Available Operators";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOperator), false, false);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK)
        return (IOperator)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      else
        return null;
    }
  }
}
