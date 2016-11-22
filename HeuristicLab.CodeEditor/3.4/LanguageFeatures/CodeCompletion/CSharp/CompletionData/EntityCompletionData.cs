#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;

namespace HeuristicLab.CodeEditor {
  internal class EntityCompletionData : CompletionData, IEntityCompletionData {
    private static readonly CSharpAmbience Ambience = new CSharpAmbience();

    #region IEntityCompletionData Members
    public IEntity Entity { get; private set; }
    #endregion

    public EntityCompletionData(IEntity entity)
      : base() {
      if (entity == null) throw new ArgumentException("entity");

      Entity = entity;
      Ambience.ConversionFlags = entity is ITypeDefinition ? ConversionFlags.ShowTypeParameterList : ConversionFlags.None;
      CompletionText = DisplayText = entity.Name;
      Image = CompletionImage.GetImage(entity);
    }

    protected override object CreateFancyDescription() {
      var ambience = new CSharpAmbience();
      ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
      string header = ambience.ConvertSymbol(Entity);
      var documentation = XmlDocumentationElement.Get(Entity);

      ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
      var b = new CSharpDocumentationBuilder(ambience);
      b.AddCodeBlock(header, keepLargeMargin: true);
      if (documentation != null) {
        foreach (var child in documentation.Children) {
          b.AddDocumentationElement(child);
        }
      }

      var flowDocument = b.CreateFlowDocument();
      flowDocument.PagePadding = new Thickness(0); // default is NOT Thickness(0), but Thickness(Auto), which adds unnecessary space

      return new FlowDocumentScrollViewer {
        Document = flowDocument,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
      };
    }
  }
}
