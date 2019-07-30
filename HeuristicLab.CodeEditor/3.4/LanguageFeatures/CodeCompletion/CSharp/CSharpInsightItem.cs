#region License Information
// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;

namespace HeuristicLab.CodeEditor {
  sealed class CSharpInsightItem {
    public readonly IParameterizedMember Method;

    public CSharpInsightItem(IParameterizedMember method) {
      this.Method = method;
    }

    FlowDocumentScrollViewer header;

    public object Header {
      get {
        if (header == null) {
          header = GenerateHeader();
        }
        return header;
      }
    }

    int highlightedParameterIndex = -1;

    public void HighlightParameter(int parameterIndex) {
      if (highlightedParameterIndex == parameterIndex)
        return;
      this.highlightedParameterIndex = parameterIndex;
      if (header != null)
        header = GenerateHeader();
    }

    FlowDocumentScrollViewer GenerateHeader() {
      var ambience = new CSharpAmbience();
      ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
      var stringBuilder = new StringBuilder();
      var formatter = new ParameterHighlightingOutputFormatter(stringBuilder, highlightedParameterIndex);
      ambience.ConvertSymbol(Method, formatter, FormattingOptionsFactory.CreateSharpDevelop());

      var documentation = XmlDocumentationElement.Get(Method);
      ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;

      var b = new CSharpDocumentationBuilder(ambience);
      string parameterName = null;
      if (Method.Parameters.Count > highlightedParameterIndex)
        parameterName = Method.Parameters[highlightedParameterIndex].Name;
      b.AddSignatureBlock(stringBuilder.ToString(), formatter.parameterStartOffset, formatter.parameterLength, parameterName);

      var b2 = new CSharpDocumentationBuilder(ambience);
      b2.ParameterName = parameterName;
      b2.ShowAllParameters = false;

      if (documentation != null) {
        foreach (var child in documentation.Children) {
          b2.AddDocumentationElement(child);
        }
      }

      content = new FlowDocumentScrollViewer {
        Document = b2.CreateFlowDocument(),
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
      };

      var flowDocument = b.CreateFlowDocument();
      flowDocument.PagePadding = new Thickness(0); // default is NOT Thickness(0), but Thickness(Auto), which adds unnecessary space

      return new FlowDocumentScrollViewer {
        Document = flowDocument,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
      };
    }

    FlowDocumentScrollViewer content;

    public object Content {
      get {
        if (content == null) {
          GenerateHeader();
        }
        return content;
      }
    }

    sealed class ParameterHighlightingOutputFormatter : TextWriterTokenWriter {
      StringBuilder b;
      int highlightedParameterIndex;
      int parameterIndex;
      internal int parameterStartOffset;
      internal int parameterLength;

      public ParameterHighlightingOutputFormatter(StringBuilder b, int highlightedParameterIndex)
        : base(new StringWriter(b)) {
        this.b = b;
        this.highlightedParameterIndex = highlightedParameterIndex;
      }

      public override void StartNode(AstNode node) {
        if (parameterIndex == highlightedParameterIndex && node is ParameterDeclaration) {
          parameterStartOffset = b.Length;
        }
        base.StartNode(node);
      }

      public override void EndNode(AstNode node) {
        base.EndNode(node);
        if (node is ParameterDeclaration) {
          if (parameterIndex == highlightedParameterIndex)
            parameterLength = b.Length - parameterStartOffset;
          parameterIndex++;
        }
      }
    }
  }
}
