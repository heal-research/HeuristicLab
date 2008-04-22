#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using System.IO;

namespace HeuristicLab.Core {
  public static class PersistenceManager {
    public static XmlDocument CreateXmlDocument() {
      XmlDocument document = new XmlDocument();
      document.AppendChild(document.CreateXmlDeclaration("1.0", null, null));
      return document;
    }
    public static XmlNode Persist(IStorable instance, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      string name = instance.GetType().Name;
      name = name.Replace('`', '_');
      return Persist(name, instance, document, persistedObjects);
    }
    public static XmlNode Persist(string name, IStorable instance, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      if (persistedObjects.ContainsKey(instance.Guid)) {
        XmlNode node = document.CreateNode(XmlNodeType.Element, name, null);
        XmlAttribute guidAttribute = document.CreateAttribute("GUID");
        guidAttribute.Value = instance.Guid.ToString();
        node.Attributes.Append(guidAttribute);
        return node;
      } else {
        persistedObjects.Add(instance.Guid, instance);
        XmlNode node = instance.GetXmlNode(name, document, persistedObjects);
        return node;
      }
    }
    public static IStorable Restore(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      Guid guid = new Guid(node.Attributes["GUID"].Value);
      if (restoredObjects.ContainsKey(guid)) {
        return restoredObjects[guid];
      } else {
        Type type = Type.GetType(node.Attributes["Type"].Value, true);
        IStorable instance = (IStorable)Activator.CreateInstance(type);
        restoredObjects.Add(guid, instance);
        instance.Populate(node, restoredObjects);
        return instance;
      }
    }
    public static void Save(IStorable instance, string filename) {
      FileStream stream = File.Create(filename);
      Save(instance, stream);
      stream.Close();
    }
    public static void Save(IStorable instance, Stream stream) {
      XmlDocument document = PersistenceManager.CreateXmlDocument();

      document.AppendChild(Persist(instance, document, new Dictionary<Guid, IStorable>()));
      document.Save(stream);
    }
    public static IStorable Load(string filename) {
      FileStream stream = File.OpenRead(filename);
      IStorable storable = Load(stream);
      stream.Close();
      return storable;
    }
    public static IStorable Load(Stream stream) {
      XmlDocument doc = new XmlDocument();
      doc.Load(stream);
      return PersistenceManager.Restore(doc.ChildNodes[1], new Dictionary<Guid, IStorable>());
    }

    public static string BuildTypeString(Type type) {
      string assembly = type.Assembly.FullName;
      assembly = assembly.Split(new string[] { ", " }, StringSplitOptions.None)[0];

      StringBuilder builder = new StringBuilder();
      builder.Append(type.Namespace);
      builder.Append(".");
      builder.Append(type.Name);
      Type[] args = type.GetGenericArguments();
      if (args.Length > 0) {
        builder.Append("[[");
        builder.Append(BuildTypeString(args[0]));
        builder.Append("]");
        for (int i = 1; i < args.Length; i++) {
          builder.Append(",[");
          builder.Append(BuildTypeString(args[i]));
          builder.Append("]");
        }
        builder.Append("]");
      }
      builder.Append(", ");
      builder.Append(assembly);
      return builder.ToString();
    }
  }
}
