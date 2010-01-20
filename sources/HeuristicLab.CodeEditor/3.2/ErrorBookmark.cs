using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;

namespace HeuristicLab.CodeEditor {
  public class ErrorBookmark : Bookmark {

    public override bool CanToggle { get { return false; } }

    public ErrorBookmark(IDocument document, TextLocation location)
      : base(document, location) {
    }

    public override void Draw(IconBarMargin margin, System.Drawing.Graphics g, System.Drawing.Point p) {      
      int delta = margin.TextArea.TextView.FontHeight / 4;
			Rectangle rect = new Rectangle(
        2,
        p.Y + delta,
        margin.DrawingPosition.Width - 6,
        margin.TextArea.TextView.FontHeight - delta * 2);
      g.FillRectangle(Brushes.Red, rect);
      g.DrawRectangle(Pens.White, rect);
    }
  }
}
