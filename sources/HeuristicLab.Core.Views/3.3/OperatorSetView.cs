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
  [Content(typeof(OperatorSet), true)]
  [Content(typeof(IObservableSet<IOperator>), true)]
  public partial class OperatorSetView : ItemSetView<IOperator> {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OperatorSetView() {
      InitializeComponent();
      Caption = "Operator Set";
      itemsGroupBox.Text = "&Operators";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public OperatorSetView(IObservableSet<IOperator> operatorSet)
      : this() {
      Content = operatorSet;
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
