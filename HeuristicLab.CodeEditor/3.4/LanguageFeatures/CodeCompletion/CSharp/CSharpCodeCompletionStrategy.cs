#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  internal class CSharpCodeCompletionStrategy : CodeCompletionStrategy {
    private IProjectContent projectContent = new CSharpProjectContent();

    public CSharpCodeCompletionStrategy(CodeEditor codeEditor)
      : base(codeEditor) {
      codeEditor.InternalAssembliesLoaded += (sender, args) => {
        projectContent = projectContent.AddAssemblyReferences(args.Value);
      };
      codeEditor.InternalAssembliesUnloaded += (sender, args) => {
        projectContent = projectContent.RemoveAssemblyReferences(args.Value);
      };
    }

    protected override CodeCompletionResult GetCodeCompletionResult(bool controlSpace) {
      var document = codeEditor.TextEditor.Document;
      int offset = codeEditor.TextEditor.CaretOffset;
      var result = new CodeCompletionResult();

      try {
        var completionContext = new CSharpCodeCompletionContext(document, offset, projectContent);
        var completionFactory = new CSharpCodeCompletionDataFactory(completionContext);
        var cce = new CSharpCompletionEngine(
          completionContext.Document,
          completionContext.CompletionContextProvider,
          completionFactory,
          completionContext.ProjectContent,
          completionContext.TypeResolveContextAtCaret
          );

        char completionChar = completionContext.Document.GetCharAt(completionContext.Offset - 1);
        int startPos, triggerWordLength;
        IEnumerable<ICompletionData> completionData;

        if (controlSpace) {
          if (!cce.TryGetCompletionWord(completionContext.Offset, out startPos, out triggerWordLength)) {
            startPos = completionContext.Offset;
            triggerWordLength = 0;
          }
          completionData = cce.GetCompletionData(startPos, true);
        } else {
          startPos = completionContext.Offset;
          if (char.IsLetterOrDigit(completionChar) || completionChar == '_') {
            if (startPos > 1 && char.IsLetterOrDigit(completionContext.Document.GetCharAt((startPos - 2))))
              return result;
            completionData = cce.GetCompletionData(startPos, false);
            triggerWordLength = 1;
          } else {
            completionData = cce.GetCompletionData(startPos, false);
            triggerWordLength = 0;
          }
        }

        result.TriggerWordLength = triggerWordLength;
        result.TriggerWord = completionContext.Document.GetText(completionContext.Offset - triggerWordLength, triggerWordLength);

        if (completionData.Any() && cce.AutoCompleteEmptyMatch) {
          foreach (var completion in completionData) {
            var cast = completion as CompletionData;
            if (cast != null) {
              cast.TriggerWord = result.TriggerWord;
              cast.TriggerWordLength = result.TriggerWordLength;
              result.CompletionData.Add(cast);
            }
          }
        }

        if (!controlSpace) {
          var pce = new CSharpParameterCompletionEngine(
            completionContext.Document,
            completionContext.CompletionContextProvider,
            completionFactory,
            completionContext.ProjectContent,
            completionContext.TypeResolveContextAtCaret
            );

          var parameterDataProvider = pce.GetParameterDataProvider(completionContext.Offset, completionChar);
          result.OverloadProvider = parameterDataProvider as IUpdatableOverloadProvider;
        }
      } catch {
        // ignore exceptions thrown during code completion
      }

      return result;
    }

    protected override void DoParseStep() {
      if (document == null) return;
      var unresolvedFile = CSharpParsingHelpers.CreateCSharpUnresolvedFile(document);
      projectContent = projectContent.AddOrUpdateFiles(unresolvedFile);
    }
  }
}
