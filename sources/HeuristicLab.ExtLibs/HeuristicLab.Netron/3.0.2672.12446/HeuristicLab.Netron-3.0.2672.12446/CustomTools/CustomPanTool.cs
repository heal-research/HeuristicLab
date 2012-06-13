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

using System.Drawing;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron.CustomTools {
  public class CustomPanTool : AbstractTool, IMouseListener, IKeyboardListener {
    public const string ToolName = "Custom Pan Tool";

    private Point initialLocation = Point.Empty;
    private Point previousMouseLocation = Point.Empty;

    public CustomPanTool() : base(ToolName) { }

    public CustomPanTool(string toolName) : base(toolName) { }

    protected override void OnActivateTool() {
      base.OnActivateTool();
      Cursor = CursorPalette.Pan;
    }

    #region IMouseListener Members
    public bool MouseDown(MouseEventArgs e) {
      this.previousMouseLocation = Point.Round(Controller.View.WorldToView(e.Location));
      this.initialLocation = previousMouseLocation;
      return true;
    }

    public void MouseMove(MouseEventArgs e) {
      Point currentLocation = Point.Round(Controller.View.WorldToView(e.Location));
      if (!IsSuspended && IsActive) {
        IDiagramControl control = Controller.ParentControl;
        Point origin = Controller.View.Origin;

        Point offset = new Point(previousMouseLocation.X - currentLocation.X, previousMouseLocation.Y - currentLocation.Y);

        origin.Offset(offset);

        if (origin.X < 0) origin.X = 0;


        if (origin.Y < 0) origin.Y = 0;

        control.AutoScrollPosition = origin;
        Controller.View.Origin = origin;
        previousMouseLocation = currentLocation;
      }
    }

    public void MouseUp(MouseEventArgs e) {
      //if (IsActive && !IsSuspended) this.previousMouseLocation = Point.Empty;
    }
    #endregion

    #region IKeyboardListener Members
    public void KeyDown(KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape)
        DeactivateTool();
      else if (e.KeyCode == Keys.Space)
        ActivateTool();
    }

    public void KeyUp(KeyEventArgs e) {
      if (e.KeyCode == Keys.Space)
        DeactivateTool();
    }

    public void KeyPress(KeyPressEventArgs e) { }
    #endregion
  }
}
