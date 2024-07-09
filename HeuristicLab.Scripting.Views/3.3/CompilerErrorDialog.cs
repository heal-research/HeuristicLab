using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using Microsoft.CodeAnalysis;

namespace HeuristicLab.Scripting.Views {
  public partial class CompilerErrorDialog : Form {
    private const string ErrorLinkFormatString = "https://docs.microsoft.com/en-us/search/?search={0}&scope=.NET";

    private static readonly Bitmap WarningImage = VSImageLibrary.Warning;
    private static readonly Bitmap ErrorImage = VSImageLibrary.Error;

    public CompilerErrorDialog(Diagnostic error) {
      InitializeComponent();

      var image = error.Severity == DiagnosticSeverity.Warning ? WarningImage : ErrorImage;

      Icon = Icon.FromHandle(image.GetHicon());
      Text = error.Severity == DiagnosticSeverity.Warning ? "Warning" : "Error";
      iconLabel.Image = image;
      infoTextBox.Text = string.Format(infoTextBox.Text, error.Severity == DiagnosticSeverity.Warning ? "A warning" : "An error");
      lineValueLabel.Text = error.Location.GetLineSpan().StartLinePosition.Line.ToString();
      columnValueLabel.Text = error.Location.GetLineSpan().StartLinePosition.Character.ToString();
      codeValueLinkLabel.Text = error.Id;
      codeValueLinkLabel.Links.Add(new LinkLabel.Link(0, error.Id.Length, string.Format(ErrorLinkFormatString, error.Id)));
      messageValueTextBox.Text = error.GetMessage();
    }

    private void codeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      var startInfo = new ProcessStartInfo((string)e.Link.LinkData) {
        UseShellExecute = true
      };
      Process.Start(startInfo);
    }
  }
}
