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

using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  internal class CSharpCodeCompletionContext {
    private readonly IDocument document;
    public IDocument Document { get { return document; } }

    private readonly int offset;
    public int Offset { get { return offset; } }

    private readonly IProjectContent projectContent;
    public IProjectContent ProjectContent { get { return projectContent; } }

    private readonly CSharpTypeResolveContext typeResolveContextAtCaret;
    public CSharpTypeResolveContext TypeResolveContextAtCaret { get { return typeResolveContextAtCaret; } }

    private readonly ICompletionContextProvider completionContextProvider;
    public ICompletionContextProvider CompletionContextProvider { get { return completionContextProvider; } }

    public CSharpCodeCompletionContext(IDocument document, int offset, IProjectContent projectContent) {
      this.document = new ReadOnlyDocument(document, document.FileName);
      this.offset = offset;

      var unresolvedFile = CSharpParsingHelpers.CreateCSharpUnresolvedFile(this.document);
      this.projectContent = projectContent.AddOrUpdateFiles(unresolvedFile);

      completionContextProvider = new DefaultCompletionContextProvider(this.document, unresolvedFile);

      var compilation = this.projectContent.CreateCompilation();
      var location = this.document.GetLocation(this.offset);
      typeResolveContextAtCaret = unresolvedFile.GetTypeResolveContext(compilation, location);
    }
  }
}
