#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Drawing;
using HeuristicLab.MainForm;
using System.Windows.Forms;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal class ZoomInToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItem, HeuristicLab.Optimizer.IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Zoom In"; }
    }

    public override string ToolTipText {
      get { return "Zoom In"; }
    }
    public override int Position {
      get { return 141; }
    }
    public override Image Image {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Sort; }
    }

    protected override void OnActiveViewChanged(object sender, System.EventArgs e) {
      base.OnActiveViewChanged(sender, e);
      this.ToolStripItem.Visible = MainFormManager.MainForm.ActiveView is OperatorGraphVisualizationView;
    }

    public override void Execute() {
      OperatorGraphVisualizationView view = MainFormManager.MainForm.ActiveView as OperatorGraphVisualizationView;
      if (view != null)
        view.ActivateZoomInTool();
    }
  }
}
