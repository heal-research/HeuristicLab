using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {

  [View("RunCollection Calculator View")]
  [Content(typeof(RunCollectionModificationEvaluator), IsDefaultView = true)]
  public sealed partial class RunCollectionModificationEvaluatorView : NamedItemView {

    public new RunCollectionModificationEvaluator Content {
      get { return (RunCollectionModificationEvaluator)base.Content; }
      set { base.Content = value; }
    }

    public RunCollectionModificationEvaluatorView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.RunCollectionParameter.ValueChanged -= new EventHandler(RunCollection_Changed);
      Content.ModifiersParameter.ValueChanged -= new EventHandler(Modifiers_Changed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.RunCollectionParameter.ValueChanged += new EventHandler(RunCollection_Changed);
      Content.ModifiersParameter.ValueChanged += new EventHandler(Modifiers_Changed);
    }

    #region Event Handlers (Content)
    private void RunCollection_Changed(object sender, EventArgs args) {
      if (InvokeRequired)
        Invoke(new EventHandler(RunCollection_Changed), sender, args);
      else
        runCollectionViewHost.Content = Content.RunCollection;
    }
    private void Modifiers_Changed(object sender, EventArgs args) {
      if (InvokeRequired)
        Invoke(new EventHandler(Modifiers_Changed), sender, args);
      else
        modifiersViewHost.Content = Content.Modifiers;
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        runCollectionViewHost.Content = null;
        modifiersViewHost.Content = null;
      } else {
        runCollectionViewHost.Content = Content.RunCollection;
        modifiersViewHost.Content = Content.Modifiers;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      evaluateButton.Enabled = Content != null;
    }

    #region Event Handlers (child controls)
    private void evaluateButton_Click(object sender, EventArgs e) {
      evaluateButton.Enabled = false;
      var worker = new BackgroundWorker();
      worker.DoWork += (s, a) => Content.Evaluate();
      worker.RunWorkerCompleted += (s, a) => { evaluateButton.Enabled = Content != null; };
      worker.RunWorkerAsync();
    }        
    #endregion

  }
}
