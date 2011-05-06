
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
namespace HeuristicLab.Problems.ExternalEvaluation {

  [View("EvaluationCacheView")]
  [Content(typeof(EvaluationCache), IsDefaultView = true)]
  public sealed partial class EvaluationCacheView : NamedItemView {

    public new EvaluationCache Content {
      get { return (EvaluationCache)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationCacheView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CacheSizeChanged -= Content_CacheSizeChanged;
      Content.CacheHitsChanged -= Content_CacheHitsChanged;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CacheSizeChanged += Content_CacheSizeChanged;
      Content.CacheHitsChanged += Content_CacheHitsChanged;
    }


    #region Event Handlers (Content)
    void Content_CacheSizeChanged(object sender, System.EventArgs e) {
      sizeTextBox.Text = Content.CacheSize.ToString();
    }

    void Content_CacheHitsChanged(object sender, System.EventArgs e) {
      hitsTextBox.Text = Content.CacheHits.ToString();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        sizeTextBox.Text = "";
        hitsTextBox.Text = "";
      } else {
        sizeTextBox.Text = Content.CacheSize.ToString();
        hitsTextBox.Text = Content.CacheHits.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      clearButton.Enabled = !ReadOnly && Content != null;
    }

    #region Event Handlers (child controls)
    private void clearButton_Click(object sender, System.EventArgs e) {
      Content.Reset();
    }
    #endregion
  }

}
