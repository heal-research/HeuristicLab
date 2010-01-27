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
  [Content(typeof(ScopeList), true)]
  [Content(typeof(IObservableList<IScope>), true)]
  public partial class ScopeListView : ItemListView<IScope> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ScopeListView() {
      InitializeComponent();
      Caption = "ScopeList";
      itemsGroupBox.Text = "&Scopes";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public ScopeListView(IObservableList<IScope> scopeList)
      : this() {
      ItemList = scopeList;
    }
  }
}
