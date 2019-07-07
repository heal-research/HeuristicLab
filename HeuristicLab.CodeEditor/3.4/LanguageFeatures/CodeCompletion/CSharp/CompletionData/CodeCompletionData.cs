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

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory.Editor;
using ACC = ICSharpCode.AvalonEdit.CodeCompletion;
using NCC = ICSharpCode.NRefactory.Completion;

namespace HeuristicLab.CodeEditor {
  internal class CompletionData : NCC.ICompletionData, ACC.ICompletionData {
    public string TriggerWord { get; set; }
    public int TriggerWordLength { get; set; }

    #region Constructors
    protected CompletionData() { }

    public CompletionData(string text) {
      DisplayText = CompletionText = Description = text;
    }
    #endregion

    #region ICSharpCode.NRefactory.Completion.ICompletionData Members
    public NCC.CompletionCategory CompletionCategory { get; set; }
    public string DisplayText { get; set; }
    public virtual string Description { get; set; }
    public string CompletionText { get; set; }
    public NCC.DisplayFlags DisplayFlags { get; set; }
    public bool HasOverloads { get { return overloadedData.Count > 0; } }

    private readonly List<NCC.ICompletionData> overloadedData = new List<NCC.ICompletionData>();
    public IEnumerable<NCC.ICompletionData> OverloadedData {
      get { return overloadedData; }
      set { throw new InvalidOperationException(); }
    }

    public void AddOverload(NCC.ICompletionData data) {
      if (overloadedData.Count == 0)
        overloadedData.Add(this);
      overloadedData.Add(data);
    }
    #endregion

    #region ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData Members
    public object Content { get { return DisplayText; } }

    object fancyDescription;
    object ACC.ICompletionData.Description {
      get {
        if (fancyDescription == null) {
          fancyDescription = CreateFancyDescription();
        }
        return fancyDescription;
      }
    }

    public virtual ImageSource Image { get; set; }
    public double Priority { get; set; }
    public string Text { get { return CompletionText; } }

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
      textArea.Document.Replace(completionSegment, CompletionText);
    }
    #endregion

    protected virtual object CreateFancyDescription() {
      return Description;
    }
  }
}
