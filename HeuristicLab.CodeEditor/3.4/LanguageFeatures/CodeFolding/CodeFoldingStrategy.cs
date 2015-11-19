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

using ICSharpCode.AvalonEdit.Folding;

namespace HeuristicLab.CodeEditor {
  internal abstract class CodeFoldingStrategy : ICodeFoldingStrategy {
    protected readonly CodeEditor codeEditor;
    protected readonly FoldingManager foldingManager;

    protected CodeFoldingStrategy(CodeEditor codeEditor) {
      this.codeEditor = codeEditor;
      foldingManager = FoldingManager.Install(codeEditor.TextEditor.TextArea);
    }

    public virtual void DoCodeFolding() {
      int firstErrorOffset;
      var codeFoldingResult = GetCodeFoldingResult(out firstErrorOffset);
      ApplyCodeFoldingData(codeFoldingResult, firstErrorOffset);
    }

    protected abstract CodeFoldingResult GetCodeFoldingResult(out int firstErrorOffset);

    protected virtual void ApplyCodeFoldingData(CodeFoldingResult codeFoldingResult, int firstErrorOffset) {
      var foldings = codeFoldingResult.FoldingData;
      foldingManager.UpdateFoldings(foldings, firstErrorOffset);
    }
  }
}
