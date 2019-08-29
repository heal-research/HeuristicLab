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

using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace HeuristicLab.CodeEditor {
  internal class CodeCompletionResult {
    public IList<ICompletionData> CompletionData { get; set; }
    public int TriggerWordLength { get; set; }
    public string TriggerWord { get; set; }
    public IOverloadProvider OverloadProvider { get; set; }

    public CodeCompletionResult() {
      CompletionData = new List<ICompletionData>();
    }
  }
}
