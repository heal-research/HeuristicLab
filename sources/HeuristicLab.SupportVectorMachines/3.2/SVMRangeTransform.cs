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
using HeuristicLab.Core;
using System.Globalization;
using System.IO;

namespace HeuristicLab.Data {
  public class SVMRangeTransform : ObjectData {
    /// <summary>
    /// Gets or sets the SVM range transformation.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/> of base class <see cref="ObjectData"></see>. 
    /// No own data storage present.</remarks>
    public new SVM.RangeTransform Data {
      get { return (SVM.RangeTransform)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Clones the current instance and adds it to the dictionary <paramref name="clonedObjects"/>.
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="DoubleData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SVMRangeTransform clone = new SVMRangeTransform();
      clonedObjects.Add(Guid, clone);
      // beware we are only using a shallow copy here! (gkronber)
      clone.Data = Data;
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The actual range transformation is saved in the node's inner text as string, 
    /// its format depending on the local culture info and its number format.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode data = document.CreateElement("Data");

      using (MemoryStream stream = new MemoryStream()) {
        SVM.RangeTransform.Write(stream, Data);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        StreamReader reader = new StreamReader(stream);
        data.InnerText = reader.ReadToEnd();
        node.AppendChild(data);
      }
      return node;
    }
    /// <summary>
    /// Loads the persisted SVM range transformation from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The serialized SVM range transformation must be saved in the node's inner text as a string  
    /// (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the SVM range transformation is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode data = node.SelectSingleNode("Data");
      using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(data.InnerText))) {
        Data = SVM.RangeTransform.Read(stream);
      }
    }
  }
}
