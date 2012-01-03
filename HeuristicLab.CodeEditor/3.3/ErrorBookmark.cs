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
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
