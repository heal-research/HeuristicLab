#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;

namespace HeuristicLab.CodeEditor {
  internal class CSharpParsingHelpers {
    public static SyntaxTree CreateSyntaxTree(IDocument document) {
      var parser = new CSharpParser();
      var syntaxTree = parser.Parse(document, document.FileName);
      return syntaxTree;
    }

    public static CSharpUnresolvedFile CreateCSharpUnresolvedFile(IDocument document) {
      var syntaxTree = CreateSyntaxTree(document);
      syntaxTree.Freeze();
      var unresolvedFile = syntaxTree.ToTypeSystem();
      return unresolvedFile;
    }
  }
}
