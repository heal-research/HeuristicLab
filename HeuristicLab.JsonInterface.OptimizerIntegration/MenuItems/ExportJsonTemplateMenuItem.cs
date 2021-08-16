using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimizer;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  internal class ExportJsonTemplateMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Json-Template..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File", "&Export" }; }
    }
    public override int Position {
      get { return 1601; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.Content != null) &&
                              (activeView.Content is IStorableContent) && !activeView.Locked && activeView.Enabled;
    }

    public override void Execute() {
      FileManager.ExportJsonTemplate();
    }
  }
}
