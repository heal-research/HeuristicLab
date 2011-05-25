using System;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace HeuristicLab.Problems.ExternalEvaluation.Views {

  [View("EvaluationCacheView")]
  [Content(typeof(EvaluationCache), IsDefaultView = true)]
  public sealed partial class EvaluationCacheView : ParameterizedNamedItemView {

    public new EvaluationCache Content {
      get { return (EvaluationCache)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationCacheView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= new System.EventHandler(Content_StatusChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new System.EventHandler(Content_StatusChanged);
    }

    #region Event Handlers (Content)
    void Content_StatusChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StatusChanged), sender, e);
      else
        hits_sizeTextBox.Text = string.Format("{0}/{1} ({2} active)", Content.Hits, Content.Size, Content.ActiveEvaluations);
    }

    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        hits_sizeTextBox.Text = "#/#";
      } else {
        Content_StatusChanged(this, EventArgs.Empty);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      clearButton.Enabled = !ReadOnly && Content != null;
      saveButton.Enabled = !ReadOnly && Content != null;
    }

    #region Event Handlers (child controls)
    private void clearButton_Click(object sender, EventArgs e) {
      Content.Reset();
    }
    #endregion
    
    private void saveButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        saveButton.Enabled = false;
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += (s, a) => {
          Content.Save((string)a.Argument);
        };
        worker.RunWorkerCompleted += (s, a) => {
          SetEnabledStateOfControls();
        };
        worker.RunWorkerAsync(saveFileDialog.FileName);
      }
    }    
  }
}
