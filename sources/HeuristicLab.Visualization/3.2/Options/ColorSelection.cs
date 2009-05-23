using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public delegate void ColorChangedHandler(ColorSelection sender);

  [DefaultEvent("ColorChanged")]
  public partial class ColorSelection : UserControl {
    public ColorSelection() {
      InitializeComponent();
    }

    public event ColorChangedHandler ColorChanged;

    public Color Color {
      get { return ColorPreviewTB.BackColor; }
      set {
        ColorPreviewTB.BackColor = value;
        FireColorChanged();
      }
    }

    private void OptionsDialogSelectColorBtn_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      if (dlg.ShowDialog() == DialogResult.OK) {
        ColorPreviewTB.BackColor = dlg.Color;
        FireColorChanged();
      }
    }

    private void FireColorChanged() {
      if (ColorChanged != null)
        ColorChanged(this);
    }
  }
}