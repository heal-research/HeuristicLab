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
using System.Drawing;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public static class TextEditor {

    private static TextEditorControl editor = null;
    private static ITextProvider myTextProvider;
    private static IDiagramControl diagramControl;

    private static EventHandler<MouseEventArgs> onescape =
           new EventHandler<MouseEventArgs>(Controller_OnMouseDown);

    private static ITextProvider TextProvider {
      get { return myTextProvider; }
    }

    private static TextEditorControl Editor {
      get {
        if (editor == null)
          editor = new TextEditorControl();
        return editor;
      }
    }

    public static void Init(DiagramControlBase parent) {
      diagramControl = parent;
      parent.Controls.Add(Editor);
      Editor.Visible = false;
      Editor.BackColor = Color.White;
    }

    public static TextEditorControl GetEditor(ITextProvider textProvider) {
      if (textProvider == null) {
        throw new InconsistencyException(
            "Cannot assign an editor to a 'null' text provider.");
      }

      // Adjust the editor's location and size by the current scroll 
      // position and zoom factor.
      Point location = Point.Round(diagramControl.View.WorldToView(
          textProvider.TextArea.Location));

      Size size = Size.Round(diagramControl.View.WorldToView(
          textProvider.TextArea.Size));

      myTextProvider = textProvider;
      Editor.Location = location;
      Editor.Width = size.Width;
      Editor.Height = size.Height;
      Editor.Font = textProvider.TextStyle.Font;
      Editor.Visible = false;
      return Editor;
    }

    public static void Show() {
      if (myTextProvider == null)
        return;
      diagramControl.Controller.Model.Selection.Clear();
      diagramControl.View.ResetTracker();
      diagramControl.Controller.SuspendAllTools();
      diagramControl.Controller.Enabled = false;
      diagramControl.Controller.OnMouseDown += onescape;
      Editor.Visible = true;
      Editor.Text = myTextProvider.Text;

      if (myTextProvider.TextStyle.HorizontalAlignment == StringAlignment.Center)
        Editor.TextAlign = HorizontalAlignment.Center;
      else if (myTextProvider.TextStyle.HorizontalAlignment == StringAlignment.Far)
        Editor.TextAlign = HorizontalAlignment.Right;
      else
        Editor.TextAlign = HorizontalAlignment.Left;

      Editor.SelectionLength = Editor.Text.Length;
      Editor.ScrollToCaret();
      Editor.Focus();
    }

    static void Controller_OnMouseDown(object sender, MouseEventArgs e) {
      Hide();
      diagramControl.Controller.OnMouseDown -= onescape;
    }

    public static void Hide() {
      if (myTextProvider == null)
        return;
      diagramControl.Controller.Enabled = true;
      diagramControl.Focus();
      Editor.Visible = false;
      myTextProvider.Text = Editor.Text;
      diagramControl.Controller.UnsuspendAllTools();
      myTextProvider = null;
    }

    public class TextEditorControl : TextBox {
      public TextEditorControl()
        : base() {
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Multiline = true;
        this.ScrollBars = ScrollBars.None;
        this.WordWrap = true;
        this.BackColor = Color.White;
      }

    }

  }
}
