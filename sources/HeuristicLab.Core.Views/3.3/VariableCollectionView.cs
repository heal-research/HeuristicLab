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
  [Content(typeof(VariableCollection), true)]
  [Content(typeof(IObservableKeyedCollection<string, Variable>), true)]
  public partial class VariableCollectionView : NamedItemCollectionView<Variable> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public VariableCollectionView() {
      InitializeComponent();
      Caption = "VariableCollection";
      itemsGroupBox.Text = "&Variables";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public VariableCollectionView(IObservableKeyedCollection<string, Variable> variableCollection)
      : this() {
      NamedItemCollection = variableCollection;
    }
  }
}
