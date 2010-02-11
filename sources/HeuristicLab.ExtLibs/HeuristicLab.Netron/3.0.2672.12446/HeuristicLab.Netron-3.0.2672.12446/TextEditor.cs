using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netron.Diagramming.Core;
using System.Windows.Forms;
using System.Drawing;

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
      Selection.Clear();
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
