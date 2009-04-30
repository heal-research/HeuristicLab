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
using System.IO.Compression;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core {
  /// <summary>
  /// Static class for serializing and deserializing objects.
  /// </summary>
  public static class PersistenceManager {
    /// <summary>
    /// Creates an <see cref="XmlDocument"/> to persist an object with xml declaration.
    /// </summary>
    /// <returns>The created <see cref="XmlDocument"/>.</returns>
    public static XmlDocument CreateXmlDocument() {
      XmlDocument document = new XmlDocument();
      document.AppendChild(document.CreateXmlDeclaration("1.0", null, null));
      return document;
    }
    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified <paramref name="document"/>
    /// if it has not already been serialized.
    /// </summary>
    /// <remarks>The tag name of the saved instance is its type name.<br/>
    /// The guid is saved as an <see cref="XmlAttribute"/> with tag name <c>GUID</c>.</remarks>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public static XmlNode Persist(IStorable instance, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      string name = instance.GetType().Name;
      name = name.Replace('`', '_');
      return Persist(name, instance, document, persistedObjects);
    }
    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified <paramref name="document"/>
    /// if it has not already been serialized.
    /// </summary>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public static XmlNode Persist(string name, IStorable instance, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      if(persistedObjects.ContainsKey(instance.Guid)) {
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
    /// <summary>
    /// Loads a persisted object from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The guid is saved as an attribute with tag name <c>GUID</c>. The type of the 
    /// persisted object is saved as attribute with tag name <c>Type</c>.<br/>
    /// Calls <c>instance.Populate</c>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the object is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The loaded object.</returns>
    public static IStorable Restore(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      Guid guid = new Guid(node.Attributes["GUID"].Value);
      if(restoredObjects.ContainsKey(guid)) {
        return restoredObjects[guid];
      } else {
        Type type = Type.GetType(node.Attributes["Type"].Value, true);
        IStorable instance = (IStorable)Activator.CreateInstance(type);
        restoredObjects.Add(guid, instance);
        instance.Populate(node, restoredObjects);
        return instance;
      }
    }
    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified file through creating an 
    /// <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="filename">The name of the file where the <paramref name="object"/> should be saved.</param>
    public static void Save(IStorable instance, string filename) {
      using(FileStream stream = File.Create(filename)) {
        Save(instance, stream);
        stream.Close();
      }
    }
    /// <summary>
    /// Saves the specified <paramref name="instance"/> in the specified <paramref name="stream"/> 
    /// through creating an <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="instance">The object that should be saved.</param>
    /// <param name="stream">The (file) stream where the object should be saved.</param>
    public static void Save(IStorable instance, Stream stream) {
      XmlDocument document = PersistenceManager.CreateXmlDocument();
      Dictionary<Guid, IStorable> dictionary = new Dictionary<Guid, IStorable>();
      XmlNode rootNode = document.CreateElement("Root");
      document.AppendChild(rootNode);
      rootNode.AppendChild(Persist(instance, document, dictionary));
      document.Save(stream);
    }
    /// <summary>
    /// Loads an object from a file with the specified <paramref name="filename"/>.
    /// </summary>
    /// <remarks>The object must be saved as an <see cref="XmlDocument"/>. <br/>
    /// Calls <see cref="Restore"/>.</remarks>
    /// <param name="filename">The filename of the file where the data is saved.</param>
    /// <returns>The loaded object.</returns>
    public static IStorable Load(string filename) {
      using(FileStream stream = File.OpenRead(filename)) {
        IStorable storable = Load(stream);
        stream.Close();
        return storable;
      }
    }
    /// <summary>
    /// Loads an object from the specified <paramref name="stream"/>.
    /// </summary>
    /// <remarks>The object must be saved as an <see cref="XmlDocument"/>. <br/>
    /// Calls <see cref="Restore"/>.</remarks>
    /// <param name="stream">The stream from where to load the data.</param>
    /// <returns>The loaded object.</returns>
    public static IStorable Load(Stream stream) {
      XmlDocument doc = new XmlDocument();
      doc.Load(stream);
      XmlNode rootNode = doc.ChildNodes[1];
      if(rootNode.Name == "Root" && rootNode.ChildNodes.Count == 2) {
        // load documents that have a list of necessary plugins at the top
        return PersistenceManager.Restore(rootNode.ChildNodes[1], new Dictionary<Guid, IStorable>());
      } else {
        // compatibility to load documents without list of necessary plugins 
        return PersistenceManager.Restore(rootNode, new Dictionary<Guid, IStorable>());
      }
    }

    /// <summary>
    /// Loads an object from a zip file.
    /// </summary>
    /// <param name="serializedStorable">The zip file from where to load as byte array.</param>
    /// <returns>The loaded object.</returns>
    public static IStorable RestoreFromGZip(byte[] serializedStorable) {
      GZipStream stream = new GZipStream(new MemoryStream(serializedStorable), CompressionMode.Decompress);
      return Load(stream);
    }

    /// <summary>
    /// Saves the specified <paramref name="storable"/> in a zip file.
    /// </summary>
    /// <remarks>Calls <see cref="Save(HeuristicLab.Core.IStorable, Stream)"/>.</remarks>
    /// <param name="storable">The object to save.</param>
    /// <returns>The zip stream as byte array.</returns>
    public static byte[] SaveToGZip(IStorable storable) {
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      Save(storable, stream);
      stream.Close();
      return memStream.ToArray();
    }

    /// <summary>
    /// Builds a meaningful string for the given <paramref name="type"/> with the namespace information, 
    /// all its arguments, the assembly name...
    /// </summary>
    /// <param name="type">The type for which a string should be created.</param>
    /// <returns>A string value of this type containing different additional information.</returns>
    public static string BuildTypeString(Type type) {
      string assembly = type.Assembly.FullName;
      assembly = assembly.Substring(0, assembly.IndexOf(", "));

      StringBuilder builder = new StringBuilder();
      builder.Append(type.Namespace);
      builder.Append(".");
      builder.Append(type.Name);
      Type[] args = type.GetGenericArguments();
      if(args.Length > 0) {
        builder.Append("[[");
        builder.Append(BuildTypeString(args[0]));
        builder.Append("]");
        for(int i = 1; i < args.Length; i++) {
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
