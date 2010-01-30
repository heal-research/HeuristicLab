using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Collections;

namespace HeuristicLab.Core.Views {
  [Content(typeof(OperatorCollection), true)]
  [Content(typeof(IObservableCollection<IOperator>), false)]
  public partial class OperatorCollectionView : ItemCollectionView<IOperator> {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OperatorCollectionView() {
      InitializeComponent();
      Caption = "Operator Collection";
      itemsGroupBox.Text = "&Operators";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public OperatorCollectionView(IObservableCollection<IOperator> content)
      : this() {
      Content = content;
    }

    protected override IOperator CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.TypeSelector.Caption = "&Available Operators";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOperator), false, false);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK)
        return (IOperator)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      else
        return null;
    }
  }
}
