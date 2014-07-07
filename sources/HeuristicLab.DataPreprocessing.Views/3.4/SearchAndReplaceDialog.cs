using System;
using System.Windows.Forms;

namespace HeuristicLab.DataPreprocessing.Views {
  public enum ReplaceAction {
    Value,
    Average,
    Median,
    Random,
    MostCommon,
    Interpolation
  }

  public enum ComparisonOperation {
    Equal,
    Less,
    LessOrEqual,
    Greater,
    GreaterOrEqual,
    NotEqual
  }



  public partial class SearchAndReplaceDialog : Form {
    private string[] cmbItemsText = { "Value", "Average", "Median", "Random", "Most Common", "Interpolation" };
    private string[] cmbComparisonOperatorText = { "==", "<", "<=", ">", ">=", "!=" };

    public SearchAndReplaceDialog() {
      InitializeComponent();
      cmbReplaceWith.Items.AddRange(cmbItemsText);
      cmbReplaceWith.SelectedIndex = (int)ReplaceAction.Value;
      cmbComparisonOperator.Items.AddRange(cmbComparisonOperatorText);
      cmbComparisonOperator.SelectedIndex = (int)ComparisonOperation.Equal;
    }

    public void ActivateSearch() {
      tabSearchReplace.SelectTab(tabSearch);
      AddControlsToCurrentTab();
    }

    public void ActivateReplace() {
      tabSearchReplace.SelectTab(tabReplace);
      AddControlsToCurrentTab();
    }

    public void DisableReplace() {
      tabSearchReplace.SelectTab(tabSearch);
      tabReplace.Enabled = false;
    }

    public void EnableReplace() {
      tabReplace.Enabled = true;
    }

    private void tabSearchReplace_SelectedIndexChanged(object sender, System.EventArgs e) {
      AddControlsToCurrentTab();
    }

    private void cmbReplaceWith_SelectedIndexChanged(object sender, System.EventArgs e) {
      lblValue.Visible = txtValue.Visible = cmbReplaceWith.SelectedIndex == (int)ReplaceAction.Value;
    }

    private void AddControlsToCurrentTab() {
      tabSearchReplace.SelectedTab.Controls.Add(btnFindAll);
      tabSearchReplace.SelectedTab.Controls.Add(btnFindNext);
      tabSearchReplace.SelectedTab.Controls.Add(lblSearch);
      tabSearchReplace.SelectedTab.Controls.Add(txtSearchString);
      tabSearchReplace.SelectedTab.Controls.Add(cmbComparisonOperator);
      ActiveControl = txtSearchString;
      AcceptButton = btnFindNext;
    }

    public String GetSearchText() {
      return txtSearchString.Text;
    }

    public string GetReplaceText() {
      return txtValue.Text;
    }

    public ReplaceAction GetReplaceAction() {
      return (ReplaceAction)cmbReplaceWith.SelectedIndex;
    }

    public ComparisonOperation GetComparisonOperation() {
      return (ComparisonOperation)cmbComparisonOperator.SelectedIndex;
    }

    public event EventHandler FindAllEvent {
      add { btnFindAll.Click += value; }
      remove { btnFindAll.Click -= value; }
    }

    public event EventHandler FindNextEvent {
      add { btnFindNext.Click += value; }
      remove { btnFindNext.Click -= value; }
    }

    public event EventHandler ReplaceAllEvent {
      add { btnReplaceAll.Click += value; }
      remove { btnReplaceAll.Click -= value; }
    }

    public event EventHandler ReplaceNextEvent {
      add { btnReplace.Click += value; }
      remove { btnReplace.Click -= value; }
    }
  }
}
