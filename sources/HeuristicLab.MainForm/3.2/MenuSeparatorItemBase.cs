using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm {
  public abstract class MenuSeparatorItemBase : IMenuSeparatorItem {
    public abstract IEnumerable<string> Structure { get; }
    public abstract int Position { get; }


    public string Name {
      get { return string.Empty; }
    }

    public System.Drawing.Image Image {
      get { return null; }
    }

    public string ToolTipText {
      get { return string.Empty; }
    }

    public void Execute() {
      throw new NotImplementedException();
    }

    public void ActiveViewChanged(object sender, EventArgs e) {
      throw new NotImplementedException();
    }

    public void ViewChanged(object sender, EventArgs e) {
      throw new NotImplementedException();
    }

    public void MainFormChanged(object sender, EventArgs e) {
      throw new NotImplementedException();
    }

    public void MainFormInitialized(object sender, EventArgs e) {
      throw new NotImplementedException();
    }
  }
}
