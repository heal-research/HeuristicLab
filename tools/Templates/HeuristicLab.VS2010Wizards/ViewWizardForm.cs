using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ViewWizardForm : Form {
    public string BaseClass {
      get;
      private set;
    }
    public string ViewContentType {
      get;
      private set;
    }
    public bool IsDefaultView {
      get;
      private set;
    }

    public ViewWizardForm() {
      InitializeComponent();
      BaseClass = baseClassTextBox.Text;
      ViewContentType = viewContentTypeTextBox.Text;
      IsDefaultView = isDefaultViewCheckBox.Checked;
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void baseClassTextBox_TextChanged(object sender, System.EventArgs e) {
      BaseClass = baseClassTextBox.Text;
    }

    private void viewContentTypeTextBox_TextChanged(object sender, System.EventArgs e) {
      ViewContentType = viewContentTypeTextBox.Text;
    }

    private void isDefaultViewCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      IsDefaultView = isDefaultViewCheckBox.Checked;
    }
  }
}
