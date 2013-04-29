#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.MainForm {
  public abstract class ActionUserInterfaceItem : PositionableUserInterfaceItem, IActionUserInterfaceItem {
    protected ActionUserInterfaceItem() {
      MainFormManager.MainForm.ActiveViewChanged += this.OnActiveViewChanged;
      MainFormManager.MainForm.Changed += this.OnMainFormChanged;
      MainFormManager.MainForm.ViewShown += this.OnViewShown;
      MainFormManager.MainForm.ViewClosed += this.OnViewClosed;
    }

    public abstract string Name { get; }

    public virtual System.Drawing.Image Image {
      get { return null; }
    }

    public virtual string ToolTipText {
      get { return string.Empty; }
    }

    public abstract void Execute();

    protected virtual void OnActiveViewChanged(object sender, EventArgs e) {
    }

    protected virtual void OnMainFormChanged(object sender, EventArgs e) {
    }

    protected virtual void OnViewChanged(object sender, EventArgs e) {
    }

    private void OnViewShown(object sender, ViewShownEventArgs e) {
      if (e.FirstTimeShown)
        e.View.Changed += this.OnViewChanged;
    }

    private void OnViewClosed(object sender, ViewEventArgs e) {
      e.View.Changed -= this.OnViewChanged;
    }
  }
}
