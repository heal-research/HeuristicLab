#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory.Editor;

namespace HeuristicLab.CodeEditor {
  internal class MethodDefinitionReadOnlySectionProvider : IReadOnlySectionProvider {
    private readonly CodeEditor codeEditor;

    private IDocument Doc { get { return codeEditor.TextEditor.Document; } }
    private string Prefix { get { return codeEditor.Prefix; } }
    private string Suffix { get { return codeEditor.Suffix; } }

    public MethodDefinitionReadOnlySectionProvider(CodeEditor codeEditor) {
      this.codeEditor = codeEditor;
    }

    public bool CanInsert(int offset) {
      return offset >= Prefix.Length && offset <= Doc.TextLength - Suffix.Length;
    }

    public IEnumerable<ISegment> GetDeletableSegments(ISegment segment) {
      if (segment == null)
        throw new ArgumentNullException("segment");

      int offset = segment.Offset;
      int length = segment.Length;
      if (offset >= Prefix.Length && offset + length <= Doc.TextLength - Suffix.Length)
        yield return segment;
    }
  }
}
