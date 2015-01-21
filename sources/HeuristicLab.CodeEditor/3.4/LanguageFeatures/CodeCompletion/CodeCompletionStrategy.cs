#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace HeuristicLab.CodeEditor {
  internal abstract class CodeCompletionStrategy : ICodeCompletionStrategy {
    protected readonly CodeEditor codeEditor;
    protected readonly Task backgroundParser;

    protected CodeCompletionStrategy(CodeEditor codeEditor) {
      this.codeEditor = codeEditor;
      backgroundParser = new Task(DoBackgroundParsing);
    }

    public virtual void DoCodeCompletion(bool controlSpace) {
      var codeCompletionResult = GetCodeCompletionResult(controlSpace);
      ApplyCodeCompletionData(codeCompletionResult);
    }

    public virtual void RunBackgroundParser() {
      backgroundParser.Start();
    }

    protected abstract CodeCompletionResult GetCodeCompletionResult(bool controlSpace);
    protected abstract void DoParseStep();

    protected virtual void ApplyCodeCompletionData(CodeCompletionResult codeCompletionResult) {
      var textArea = codeEditor.TextEditor.TextArea;
      var document = codeEditor.TextEditor.Document;
      int offset = codeEditor.TextEditor.CaretOffset;

      if (codeEditor.OverloadInsightWindow == null && codeCompletionResult.OverloadProvider != null) {
        var iw = codeEditor.OverloadInsightWindow = new OverloadInsightWindow(textArea);
        iw.Provider = codeCompletionResult.OverloadProvider;
        iw.Show();
        iw.Closed += (sender, args) => codeEditor.OverloadInsightWindow = null;
      }

      if (codeEditor.CompletionWindow == null && codeCompletionResult.CompletionData.Any()) {
        var cw = codeEditor.CompletionWindow = new CompletionWindow(textArea);
        cw.CloseWhenCaretAtBeginning = true;
        cw.StartOffset -= codeCompletionResult.TriggerWordLength;
        cw.Closed += (sender, args) => codeEditor.CompletionWindow = null;

        var data = cw.CompletionList.CompletionData;
        var newData = codeCompletionResult.CompletionData.OrderBy(x => x.Text).ToArray();
        foreach (var completion in newData)
          data.Add(completion);

        if (codeCompletionResult.TriggerWordLength > 0)
          cw.CompletionList.SelectItem(codeCompletionResult.TriggerWord);

        cw.Show();
      }

      if (codeEditor.OverloadInsightWindow != null) {
        var iw = codeEditor.OverloadInsightWindow;
        var provider = iw.Provider as IUpdatableOverloadProvider;
        if (provider != null) {
          provider.Update(document, offset);
          if (provider.RequestClose) {
            iw.Close();
          }
        }
      }
    }

    protected virtual void DoBackgroundParsing() {
      while (!codeEditor.IsDisposed) {
        DoParseStep();
        Thread.Sleep(1000);
      }
    }
  }
}
