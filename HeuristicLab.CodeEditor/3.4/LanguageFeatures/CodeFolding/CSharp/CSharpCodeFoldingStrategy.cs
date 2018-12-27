#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using CSharpBinding.Parser;

namespace HeuristicLab.CodeEditor {
  internal class CSharpCodeFoldingStrategy : CodeFoldingStrategy {
    public CSharpCodeFoldingStrategy(CodeEditor codeEditor) : base(codeEditor) { }

    protected override CodeFoldingResult GetCodeFoldingResult(out int firstErrorOffset) {
      var document = codeEditor.TextEditor.Document;
      var result = new CodeFoldingResult();

      try {
        var foldingContext = new CSharpCodeFoldingContext(document);
        var v = new FoldingVisitor();
        v.document = foldingContext.Document;
        foldingContext.SyntaxTree.AcceptVisitor(v);
        result.FoldingData = v.foldings.OrderBy(x => x.StartOffset).ToList();

        var firstError = foldingContext.SyntaxTree.Errors.FirstOrDefault();
        firstErrorOffset = firstError != null
          ? foldingContext.Document.GetOffset(firstError.Region.Begin)
          : int.MaxValue;
      } catch {
        // ignore exceptions thrown during code folding
        firstErrorOffset = int.MaxValue;
      }

      return result;
    }
  }
}
