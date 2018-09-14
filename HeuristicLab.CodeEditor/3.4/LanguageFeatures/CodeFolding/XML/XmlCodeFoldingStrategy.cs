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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace HeuristicLab.CodeEditor {
  internal class XmlCodeFoldingStrategy : CodeFoldingStrategy {
    /// <summary>
    /// Holds information about the start of a fold in an xml string.
    /// </summary>
    sealed class XmlFoldStart : NewFolding {
      internal int StartLine;
    }

    /// <summary>
    /// Flag indicating whether attributes should be displayed on folded
    /// elements.
    /// </summary>
    public bool ShowAttributesWhenFolded { get; set; }

    public XmlCodeFoldingStrategy(CodeEditor codeEditor) : base(codeEditor) { }

    protected override CodeFoldingResult GetCodeFoldingResult(out int firstErrorOffset) {
      var document = codeEditor.TextEditor.Document;
      var result = new CodeFoldingResult();

      try {
        var reader = new XmlTextReader(document.CreateReader()) { XmlResolver = null };
        var foldings = CreateNewFoldings(document, reader, out firstErrorOffset);
        result.FoldingData = foldings.ToList();
      } catch (XmlException) {
        firstErrorOffset = 0;
        result.FoldingData = new List<NewFolding>();
      }

      return result;
    }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document.
    /// </summary>
    public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, XmlReader reader, out int firstErrorOffset) {
      Stack<XmlFoldStart> stack = new Stack<XmlFoldStart>();
      List<NewFolding> foldMarkers = new List<NewFolding>();
      try {
        while (reader.Read()) {
          switch (reader.NodeType) {
            case XmlNodeType.Element:
              if (!reader.IsEmptyElement) {
                XmlFoldStart newFoldStart = CreateElementFoldStart(document, reader);
                stack.Push(newFoldStart);
              }
              break;

            case XmlNodeType.EndElement:
              XmlFoldStart foldStart = stack.Pop();
              CreateElementFold(document, foldMarkers, reader, foldStart);
              break;

            case XmlNodeType.Comment:
              CreateCommentFold(document, foldMarkers, reader);
              break;
          }
        }
        firstErrorOffset = -1;
      } catch (XmlException ex) {
        // ignore errors at invalid positions (prevent ArgumentOutOfRangeException)
        if (ex.LineNumber >= 1 && ex.LineNumber <= document.LineCount)
          firstErrorOffset = document.GetOffset(ex.LineNumber, ex.LinePosition);
        else
          firstErrorOffset = 0;
      }
      foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
      return foldMarkers;
    }

    static int GetOffset(TextDocument document, XmlReader reader) {
      IXmlLineInfo info = reader as IXmlLineInfo;
      if (info != null && info.HasLineInfo()) {
        return document.GetOffset(info.LineNumber, info.LinePosition);
      } else {
        throw new ArgumentException("XmlReader does not have positioning information.");
      }
    }

    /// <summary>
    /// Creates a comment fold if the comment spans more than one line.
    /// </summary>
    /// <remarks>The text displayed when the comment is folded is the first
    /// line of the comment.</remarks>
    static void CreateCommentFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader) {
      string comment = reader.Value;
      if (comment != null) {
        int firstNewLine = comment.IndexOf('\n');
        if (firstNewLine >= 0) {

          // Take off 4 chars to get the actual comment start (takes
          // into account the <!-- chars.

          int startOffset = GetOffset(document, reader) - 4;
          int endOffset = startOffset + comment.Length + 7;

          string foldText = String.Concat("<!--", comment.Substring(0, firstNewLine).TrimEnd('\r'), "-->");
          foldMarkers.Add(new NewFolding(startOffset, endOffset) { Name = foldText });
        }
      }
    }

    /// <summary>
    /// Creates an XmlFoldStart for the start tag of an element.
    /// </summary>
    XmlFoldStart CreateElementFoldStart(TextDocument document, XmlReader reader) {
      // Take off 1 from the offset returned
      // from the xml since it points to the start
      // of the element name and not the beginning
      // tag.
      //XmlFoldStart newFoldStart = new XmlFoldStart(reader.Prefix, reader.LocalName, reader.LineNumber - 1, reader.LinePosition - 2);
      XmlFoldStart newFoldStart = new XmlFoldStart();

      IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
      newFoldStart.StartLine = lineInfo.LineNumber;
      newFoldStart.StartOffset = document.GetOffset(newFoldStart.StartLine, lineInfo.LinePosition - 1);

      if (this.ShowAttributesWhenFolded && reader.HasAttributes) {
        newFoldStart.Name = String.Concat("<", reader.Name, " ", GetAttributeFoldText(reader), ">");
      } else {
        newFoldStart.Name = String.Concat("<", reader.Name, ">");
      }

      return newFoldStart;
    }

    /// <summary>
    /// Create an element fold if the start and end tag are on
    /// different lines.
    /// </summary>
    static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader, XmlFoldStart foldStart) {
      IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
      int endLine = lineInfo.LineNumber;
      if (endLine > foldStart.StartLine) {
        int endCol = lineInfo.LinePosition + reader.Name.Length + 1;
        foldStart.EndOffset = document.GetOffset(endLine, endCol);
        foldMarkers.Add(foldStart);
      }
    }

    /// <summary>
    /// Gets the element's attributes as a string on one line that will
    /// be displayed when the element is folded.
    /// </summary>
    /// <remarks>
    /// Currently this puts all attributes from an element on the same
    /// line of the start tag.  It does not cater for elements where attributes
    /// are not on the same line as the start tag.
    /// </remarks>
    static string GetAttributeFoldText(XmlReader reader) {
      StringBuilder text = new StringBuilder();

      for (int i = 0; i < reader.AttributeCount; ++i) {
        reader.MoveToAttribute(i);

        text.Append(reader.Name);
        text.Append("=");
        text.Append(reader.QuoteChar.ToString());
        text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
        text.Append(reader.QuoteChar.ToString());

        // Append a space if this is not the
        // last attribute.
        if (i < reader.AttributeCount - 1) {
          text.Append(" ");
        }
      }

      return text.ToString();
    }

    /// <summary>
    /// Xml encode the attribute string since the string returned from
    /// the XmlTextReader is the plain unencoded string and .NET
    /// does not provide us with an xml encode method.
    /// </summary>
    static string XmlEncodeAttributeValue(string attributeValue, char quoteChar) {
      StringBuilder encodedValue = new StringBuilder(attributeValue);

      encodedValue.Replace("&", "&amp;");
      encodedValue.Replace("<", "&lt;");
      encodedValue.Replace(">", "&gt;");

      if (quoteChar == '"') {
        encodedValue.Replace("\"", "&quot;");
      } else {
        encodedValue.Replace("'", "&apos;");
      }

      return encodedValue.ToString();
    }
  }
}
