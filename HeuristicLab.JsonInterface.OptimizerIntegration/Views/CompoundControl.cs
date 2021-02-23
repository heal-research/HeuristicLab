using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class CompoundControl : UserControl {
    private UserControl _topControl;
    public UserControl TopControl {
      get => _topControl;
      set {
        value.Margin = new Padding() { All = 0 };
        tableLayoutPanel1.Controls.Add(value, 0, 0);
        value.Dock = DockStyle.Fill;
        _topControl = value;
      } 
    }

    private UserControl _bottomControl;
    public UserControl BottomControl {
      get => _bottomControl;
      set {
        value.Margin = new Padding() { All = 0 };
        tableLayoutPanel1.Controls.Add(value, 0, 0);
        value.Dock = DockStyle.Fill;
        _bottomControl = value;
      }
    }

    protected CompoundControl() {
      InitializeComponent();
    }

    public static CompoundControl Create(UserControl top, UserControl bottom) 
      => new CompoundControl() { TopControl = top, BottomControl = bottom };
  }
}
