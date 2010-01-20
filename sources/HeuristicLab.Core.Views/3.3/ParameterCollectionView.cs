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
  [Content(typeof(ParameterCollection), true)]
  [Content(typeof(IObservableKeyedCollection<string, IParameter>), true)]
  public partial class ParameterCollectionView : NamedItemCollectionView<IParameter> {
    CreateParameterDialog createParameterDialog;
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ParameterCollectionView() {
      InitializeComponent();
      Caption = "ParameterCollection";
      itemsGroupBox.Text = "&Parameters";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public ParameterCollectionView(IObservableKeyedCollection<string, IParameter> parameterCollection)
      : this() {
      NamedItemCollection = parameterCollection;
    }

    protected override IParameter CreateItem() {
      if (createParameterDialog == null) createParameterDialog = new CreateParameterDialog();

      if (createParameterDialog.ShowDialog(this) == DialogResult.OK) {
        IParameter param = createParameterDialog.Parameter;
        if (NamedItemCollection.ContainsKey(param.Name))
          param = (IParameter)Activator.CreateInstance(param.GetType(), GetUniqueName(param.Name), param.Description);
        return param;
      } else
        return null;
    }
  }
}
