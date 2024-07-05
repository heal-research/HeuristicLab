﻿#region License Information
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
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;
//using ICSharpCode.NRefactory.Editor;
using Timer = System.Timers.Timer;

namespace HeuristicLab.CodeEditor {
  internal abstract class CodeCompletionStrategy /*: ICodeCompletionStrategy */ {
    /*
    protected readonly CodeEditor codeEditor;
    protected readonly Timer parserTimer;
    protected IDocument document;

    protected CodeCompletionStrategy(CodeEditor codeEditor) {
      this.codeEditor = codeEditor;
      this.codeEditor.TextEditorTextChanged += codeEditor_TextEditorTextChanged;
      parserTimer = new Timer(1000);
      parserTimer.Elapsed += (sender, args) => Task.Run(() => {
        DoParseStep();
        if (codeEditor.IsDisposed && parserTimer.Enabled) {
          parserTimer.Stop();
          parserTimer.Dispose();
        }
      });
    }

    public virtual void DoCodeCompletion(bool controlSpace) {
      var codeCompletionResult = GetCodeCompletionResult(controlSpace);
      ApplyCodeCompletionData(codeCompletionResult);
    }

    public virtual void Initialize() {
      parserTimer.Enabled = true;
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

    private void codeEditor_TextEditorTextChanged(object sender, EventArgs e) {
      var doc = codeEditor.TextEditor.Document;
      document = new ReadOnlyDocument(doc, doc.FileName);
    }
    */
  }
}
