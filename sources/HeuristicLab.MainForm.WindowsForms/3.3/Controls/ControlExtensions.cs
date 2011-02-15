#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public static class ControlExtensions {
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

    private const int WM_SETREDRAW = 11;

    public static void SuspendRepaint(this Control control) {
      if (control.InvokeRequired)
        control.Invoke(new Action<Control>(c => c.SuspendRepaint()), control);
      else
        SendMessage(control.Handle, WM_SETREDRAW, false, 0);
    }
    public static void ResumeRepaint(this Control control, bool refresh) {
      if (control.InvokeRequired)
        control.Invoke(new Action<Control, bool>((c, b) => c.ResumeRepaint(b)), control, refresh);
      else {
        SendMessage(control.Handle, WM_SETREDRAW, true, 0);
        if (refresh) control.Refresh();
      }
    }
  }
}
