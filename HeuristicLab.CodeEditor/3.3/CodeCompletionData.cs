/*
 * Erstellt mit SharpDevelop.
 * Benutzer: grunwald
 * Datum: 27.08.2007
 * Zeit: 14:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace HeuristicLab.CodeEditor {
  /// <summary>
  /// Represents an item in the code completion window.
  /// </summary>
  class CodeCompletionData : DefaultCompletionData, ICompletionData {

    List<IMember> members;
    IClass c;
    static CSharpAmbience csharpAmbience = new CSharpAmbience();

    public CodeCompletionData(IMember member)
      : base(member.Name, null, GetMemberImageIndex(member)) {
      this.members = new List<IMember>() { member };
    }

    public CodeCompletionData(IClass c)
      : base(c.Name, null, GetClassImageIndex(c)) {
      this.c = c;
    }

    int overloads = 0;

    public void AddOverload(IMember m) {
      overloads++;
      members.Add(m);
    }

    static int GetMemberImageIndex(IMember member) {
      // Missing: different icons for private/public member
      if (member is IMethod)
        return 1;
      if (member is IProperty)
        return 2;
      if (member is IField)
        return 3;
      if (member is IEvent)
        return 6;
      return 3;
    }

    static int GetClassImageIndex(IClass c) {
      switch (c.ClassType) {
        case ClassType.Enum:
          return 4;
        default:
          return 0;
      }
    }

    string description;

    // DefaultCompletionData.Description is not virtual, but we can reimplement
    // the interface to get the same effect as overriding.
    string ICompletionData.Description {
      get {
        if (description == null) {
          if (members != null)
            description = string.Join("\n\n\n", members.Select(m => GetDocumentation(m)).ToArray());
          else
            description = GetDocumentation(c);
        }
        return description;
      }
    }

    public static string GetDocumentation(IEntity entity) {
      return string.Format("{0}\n{1}",
        GetText(entity),
        XmlDocumentationToText(entity.Documentation));
    }

    /// <summary>
    /// Converts a member to text.
    /// Returns the declaration of the member as C# or VB code, e.g.
    /// "public void MemberName(string parameter)"
    /// </summary>
    static string GetText(IEntity entity) {
      IAmbience ambience = csharpAmbience;
      if (entity is IMethod)
        return ambience.Convert(entity as IMethod);
      if (entity is IProperty)
        return ambience.Convert(entity as IProperty);
      if (entity is IEvent)
        return ambience.Convert(entity as IEvent);
      if (entity is IField)
        return ambience.Convert(entity as IField);
      if (entity is IClass)
        return ambience.Convert(entity as IClass);
      // unknown entity:
      return entity.ToString();
    }

    public static string XmlDocumentationToText(string xmlDoc) {
      System.Diagnostics.Debug.WriteLine(xmlDoc);
      StringBuilder b = new StringBuilder();
      try {
        using (XmlTextReader reader = new XmlTextReader(new StringReader("<root>" + xmlDoc + "</root>"))) {
          reader.XmlResolver = null;
          while (reader.Read()) {
            switch (reader.NodeType) {
              case XmlNodeType.Text:
                b.Append(reader.Value);
                break;
              case XmlNodeType.Element:
                switch (reader.Name) {
                  case "filterpriority":
                    reader.Skip();
                    break;
                  case "returns":
                    b.AppendLine();
                    b.Append("Returns: ");
                    break;
                  case "param":
                    b.AppendLine();
                    b.Append(reader.GetAttribute("name") + ": ");
                    break;
                  case "remarks":
                    b.AppendLine();
                    b.Append("Remarks: ");
                    break;
                  case "see":
                    if (reader.IsEmptyElement) {
                      b.Append(reader.GetAttribute("cref"));
                    } else {
                      reader.MoveToContent();
                      if (reader.HasValue) {
                        b.Append(reader.Value);
                      } else {
                        b.Append(reader.GetAttribute("cref"));
                      }
                    }
                    break;
                }
                break;
            }
          }
        }
        return b.ToString();
      }
      catch (XmlException) {
        return xmlDoc;
      }
    }
  }
}
