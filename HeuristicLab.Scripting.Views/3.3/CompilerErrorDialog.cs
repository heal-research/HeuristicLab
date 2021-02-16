using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Scripting.Views {
  public partial class CompilerErrorDialog : Form {
    private const string ErrorLinkFormatString = "https://docs.microsoft.com/en-us/search/?search={0}&scope=.NET";

    private static readonly Bitmap WarningImage = VSImageLibrary.Warning;
    private static readonly Bitmap ErrorImage = VSImageLibrary.Error;

    public CompilerErrorDialog(CompilerError error) {
      InitializeComponent();

      var image = error.IsWarning ? WarningImage : ErrorImage;

      Icon = Icon.FromHandle(image.GetHicon());
      Text = error.IsWarning ? "Warning" : "Error";
      iconLabel.Image = image;
      infoTextBox.Text = string.Format(infoTextBox.Text, error.IsWarning ? "A warning" : "An error");
      lineValueLabel.Text = error.Line.ToString();
      columnValueLabel.Text = error.Column.ToString();
      codeValueLinkLabel.Text = error.ErrorNumber;
      codeValueLinkLabel.Links.Add(new LinkLabel.Link(0, error.ErrorNumber.Length, string.Format(ErrorLinkFormatString, error.ErrorNumber)));
      messageValueTextBox.Text = error.ErrorText;
    }

    private void codeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start((string)e.Link.LinkData);
    }
  }
}
