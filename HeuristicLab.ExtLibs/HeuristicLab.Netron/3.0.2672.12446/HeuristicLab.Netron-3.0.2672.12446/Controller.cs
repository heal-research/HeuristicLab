#region License Information
//This end-user license agreement applies to the following software;

//The Netron Diagramming Library
//Cobalt.IDE
//Xeon webserver
//Neon UI Library

//Copyright (C) 2007, Francois M.Vanderseypen, The Netron Project & The Orbifold

//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA


//http://www.fsf.org/licensing/licenses/gpl.html

//http://www.fsf.org/licensing/licenses/gpl-faq.html
#endregion

using System;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public class Controller : ControllerBase {

    public const string TextToolName = "Text Tool";
    public const string TextEditorToolName = "Text Editor Tool";

    public Controller(IDiagramControl surface)
      : base(surface) {
    }

    public override bool ActivateTextEditor(ITextProvider textProvider) {
      TextEditor.GetEditor(textProvider);
      TextEditor.Show();
      return true;
    }

    public ILayout StandardTreeLayout {
      get {
        return (ILayout)this.FindActivity("Standard TreeLayout");
      }
    }

    public void RemoveTool(ITool tool) {
      if (tool == null)
        return;
      tool.Controller = null;
      registeredTools.Remove(tool);

      IMouseListener mouseTool = tool as IMouseListener;
      if (mouseTool != null)
        mouseListeners.Remove(mouseTool);
      IKeyboardListener keyboardTool = tool as IKeyboardListener;
      if (keyboardTool != null)
        keyboardListeners.Remove(keyboardTool);
      IDragDropListener dragdropTool = tool as IDragDropListener;
      if (dragdropTool != null)
        dragdropListeners.Remove(dragdropTool);

      tool.OnToolActivate -= new EventHandler<ToolEventArgs>(AddedTool_OnToolActivate);
      tool.OnToolDeactivate -= new EventHandler<ToolEventArgs>(AddedTool_OnToolDeactivate);
    }
  }
}
