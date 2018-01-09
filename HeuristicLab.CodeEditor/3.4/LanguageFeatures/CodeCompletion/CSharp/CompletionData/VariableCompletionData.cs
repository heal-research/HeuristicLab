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

using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  internal class VariableCompletionData : CompletionData, IVariableCompletionData {
    private static readonly CSharpAmbience Ambience = new CSharpAmbience();

    #region IVariableCompletionData Members
    public IVariable Variable { get; private set; }
    #endregion

    public VariableCompletionData(IVariable variable)
      : base() {
      if (variable == null) throw new ArgumentException("variable");

      Variable = variable;
      Ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
      DisplayText = CompletionText = variable.Name;
      Description = Ambience.ConvertVariable(variable);
      Image = CompletionImage.Field.BaseImage;
    }
  }
}
