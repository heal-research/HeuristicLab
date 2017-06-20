#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for all visual representations.
  /// </summary>
  public partial class ItemView : AsynchronousContentView {
    public new IItem Content {
      get { return (IItem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ViewBase"/> with the caption "View".
    /// </summary>
    public ItemView() {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);

      if (CountParentControls() > Properties.Settings.Default.MaximumNestedControls) {
        //capture content, needed because it is set at a later time
        NestingLevelErrorControl errorControl = new NestingLevelErrorControl(() => Content, this.GetType());
        errorControl.Dock = DockStyle.Fill;

        Controls.Clear();
        Controls.Add(errorControl);
      }
    }

    private int CountParentControls() {
      int cnt = 0;
      Control parent = Parent;
      while (parent != null) {
        parent = parent.Parent;
        cnt++;
      }
      return cnt;
    }
  }
}
