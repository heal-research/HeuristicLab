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
using HeuristicLab.Modeling;

namespace HeuristicLab.SupportVectorMachines {
  public class SVMModel : ItemBase {
    private SVM.Model model;
    /// <summary>
    /// Gets or sets the SVM model.
    /// </summary>
    public SVM.Model Model {
      get { return model; }
      set { model = value; }
    }

    /// <summary>
    /// Gets or sets the range transformation for the model.
    /// </summary>
    private SVM.RangeTransform rangeTransform;
    public SVM.RangeTransform RangeTransform {
      get { return rangeTransform; }
      set { rangeTransform = value; }
    }

    public override IView CreateView() {
      return new SVMModelView(this);
    }

    /// <summary>
    /// Clones the current instance and adds it to the dictionary <paramref name="clonedObjects"/>.
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="DoubleData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SVMModel clone = new SVMModel();
      clonedObjects.Add(Guid, clone);
      // beware we are only using a shallow copy here! (gkronber)
      clone.Model = Model;
      clone.RangeTransform = RangeTransform;
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The actual model is saved in the node's inner text as string, 
    /// its format depending on the local culture info and its number format.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode model = document.CreateElement("Model");
      using (MemoryStream stream = new MemoryStream()) {
        SVM.Model.Write(stream, Model);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        StreamReader reader = new StreamReader(stream);
        model.InnerText = reader.ReadToEnd();
        node.AppendChild(model);
      }

      XmlNode rangeTransform = document.CreateElement("RangeTransform");
      using (MemoryStream stream = new MemoryStream()) {
        SVM.RangeTransform.Write(stream, RangeTransform);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        StreamReader reader = new StreamReader(stream);
        rangeTransform.InnerText = reader.ReadToEnd();
        node.AppendChild(rangeTransform);
      }

      return node;
    }
    /// <summary>
    /// Loads the persisted SVM model from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The serialized SVM model must be saved in the node's inner text as a string  
    /// (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the SVM model is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode model = node.SelectSingleNode("Model");
      using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(model.InnerText))) {
        Model = SVM.Model.Read(stream);
      }
      XmlNode rangeTransform = node.SelectSingleNode("RangeTransform");
      using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(rangeTransform.InnerText))) {
        RangeTransform = SVM.RangeTransform.Read(stream);
      }
    }

    public static void Export(SVMModel model, Stream s) {
      StreamWriter writer = new StreamWriter(s);
      writer.WriteLine("RangeTransform:");
      using (MemoryStream memStream = new MemoryStream()) {
        SVM.RangeTransform.Write(memStream, model.RangeTransform);
        memStream.Seek(0, SeekOrigin.Begin);
        memStream.WriteTo(s);
      }
      writer.WriteLine("Model:");

      using (MemoryStream memStream = new MemoryStream()) {
        SVM.Model.Write(memStream, model.Model);
        memStream.Seek(0, SeekOrigin.Begin);
        memStream.WriteTo(s);
      }
      s.Flush();
    }

    public static SVMModel Import(Stream s) {
      SVMModel model = new SVMModel();
      StreamReader reader = new StreamReader(s);
      while (reader.ReadLine().Trim() != "RangeTransform:") ; // read until line "RangeTransform";
      model.RangeTransform = SVM.RangeTransform.Read(s);

      // read until "Model:"
      while (reader.ReadLine().Trim() != "Model:") ;
      model.Model = SVM.Model.Read(s);
      return model;
    }
  }
}
