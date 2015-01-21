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

using System;
using System.Windows.Input;

namespace HeuristicLab.CodeEditor {
  internal abstract class LanguageFeatures : ILanguageFeatures {
    protected readonly CodeEditor codeEditor;
    protected readonly CommandBinding commandBinding;

    private readonly ICodeFoldingStrategy codeFoldingStrategy;
    public ICodeFoldingStrategy CodeFoldingStrategy { get { return codeFoldingStrategy; } }

    private readonly ICodeCompletionStrategy codeCompletionStrategy;
    public ICodeCompletionStrategy CodeCompletionStrategy { get { return codeCompletionStrategy; } }

    protected LanguageFeatures(CodeEditor codeEditor, ICodeFoldingStrategy codeFoldingStrategy, ICodeCompletionStrategy codeCompletionStrategy) {
      this.codeEditor = codeEditor;
      this.codeFoldingStrategy = codeFoldingStrategy;
      this.codeCompletionStrategy = codeCompletionStrategy;

      commandBinding = new CommandBinding(codeEditor.CompletionCommand, CodeCompletionRequestedHandler);
      if (codeFoldingStrategy != null) AddCodeFoldingStrategy();
      if (codeCompletionStrategy != null) AddCodeComplectionStrategy();
    }

    #region CodeFoldingStrategy
    protected virtual void AddCodeFoldingStrategy() {
      codeEditor.TextEditor.TextChanged += CodeFoldingHandler;
    }

    private void CodeFoldingHandler(object sender, EventArgs e) {
      codeFoldingStrategy.DoCodeFolding();
    }
    #endregion

    #region CodeCompletionStrategy
    protected virtual void AddCodeComplectionStrategy() {
      codeEditor.Load += LoadedHandler;
      codeEditor.TextEditor.TextArea.TextEntered += CodeCompletionHandler;
      codeEditor.TextEditor.TextArea.TextEntering += InsertionRequestedHandler;
      codeEditor.TextEditor.CommandBindings.Add(commandBinding);
    }

    private void LoadedHandler(object sender, EventArgs e) {
      codeCompletionStrategy.RunBackgroundParser();
    }

    private void CodeCompletionRequestedHandler(object sender, EventArgs e) {
      var readOnlySectionProvider = codeEditor.TextEditor.TextArea.ReadOnlySectionProvider;
      if (readOnlySectionProvider.CanInsert(codeEditor.TextEditor.CaretOffset))
        codeCompletionStrategy.DoCodeCompletion(true);
    }

    private void CodeCompletionHandler(object sender, EventArgs e) {
      var readOnlySectionProvider = codeEditor.TextEditor.TextArea.ReadOnlySectionProvider;
      if (readOnlySectionProvider.CanInsert(codeEditor.TextEditor.CaretOffset))
        codeCompletionStrategy.DoCodeCompletion(false);
    }

    private void InsertionRequestedHandler(object sender, TextCompositionEventArgs e) {
      var cw = codeEditor.CompletionWindow;
      if (cw != null && e.Text.Length > 0 && !char.IsLetterOrDigit(e.Text[0])) {
        cw.CompletionList.RequestInsertion(e);
      }
    }
    #endregion
  }
}
