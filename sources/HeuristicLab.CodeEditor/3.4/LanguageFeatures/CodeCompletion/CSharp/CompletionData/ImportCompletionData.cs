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

using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  internal class ImportCompletionData : EntityCompletionData {
    public ImportCompletionData(ITypeDefinition typeDefinition, CSharpTypeResolveContext contextAtCaret, bool useFullName)
      : base(typeDefinition) {
      Description = string.Format("using {0};", typeDefinition.Namespace);
      if (useFullName) {
        var builder = new TypeSystemAstBuilder(new CSharpResolver(contextAtCaret));
        CompletionText = builder.ConvertType(typeDefinition).ToString();
      } else {
        CompletionText = typeDefinition.Namespace;
      }
    }
  }
}
