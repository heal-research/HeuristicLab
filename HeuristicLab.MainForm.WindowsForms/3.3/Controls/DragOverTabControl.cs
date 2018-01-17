#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class DragOverTabControl : TabControl {
    private int destinationTabIndex;

    public DragOverTabControl() {
      InitializeComponent();
    }

    private int GetTabIndex() {
      Point position = PointToClient(Control.MousePosition);
      int tabIndex = -1;
      for (int i = 0; i < TabPages.Count; i++) {
        if (GetTabRect(i).Contains(position))
          tabIndex = i;
      }
      return tabIndex;
    }

    private void DragOverTabControl_DragOver(object sender, DragEventArgs e) {
      int tabIndex = GetTabIndex();

      if (!timer.Enabled && tabIndex != SelectedIndex && tabIndex != -1) {
        destinationTabIndex = tabIndex;
        timer.Start();
      }
    }

    private void timer_Tick(object sender, System.EventArgs e) {
      timer.Stop();
      int tabIndex = GetTabIndex();

      if (tabIndex == destinationTabIndex && tabIndex != SelectedIndex && tabIndex != -1) {
        SelectedIndex = tabIndex;
      }
    }
  }
}
