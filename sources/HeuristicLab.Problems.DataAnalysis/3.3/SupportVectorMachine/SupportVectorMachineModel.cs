#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  /// <summary>
  /// Represents a support vector machine model.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineModel", "Represents a support vector machine model.")]
  public class SupportVectorMachineModel : NamedItem {
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

    #region persistence
    [Storable]
    private string ModelAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          SVM.Model.Write(stream, Model);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          model = SVM.Model.Read(stream);
        }
      }
    }
    [Storable]
    private string RangeTransformAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          SVM.RangeTransform.Write(stream, RangeTransform);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          RangeTransform = SVM.RangeTransform.Read(stream);
        }
      }
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      SupportVectorMachineModel clone = (SupportVectorMachineModel)base.Clone(cloner);
      // beware we are only using a shallow copy here! (gkronber)
      clone.model = model;
      clone.rangeTransform = rangeTransform;
      return clone;
    }

    /// <summary>
    ///  Exports the <paramref name="model"/> in string representation to output stream <paramref name="s"/>
    /// </summary>
    /// <param name="model">The support vector regression model to export</param>
    /// <param name="s">The output stream to export the model to</param>
    public static void Export(SupportVectorMachineModel model, Stream s) {
      StreamWriter writer = new StreamWriter(s);
      writer.WriteLine("RangeTransform:");
      writer.Flush();
      using (MemoryStream memStream = new MemoryStream()) {
        SVM.RangeTransform.Write(memStream, model.RangeTransform);
        memStream.Seek(0, SeekOrigin.Begin);
        memStream.WriteTo(s);
      }
      writer.WriteLine("Model:");
      writer.Flush();
      using (MemoryStream memStream = new MemoryStream()) {
        SVM.Model.Write(memStream, model.Model);
        memStream.Seek(0, SeekOrigin.Begin);
        memStream.WriteTo(s);
      }
      s.Flush();
    }

    /// <summary>
    /// Imports a support vector machine model given as string representation.
    /// </summary>
    /// <param name="reader">The reader to retrieve the string representation from</param>
    /// <returns>The imported support vector machine model.</returns>
    public static SupportVectorMachineModel Import(TextReader reader) {
      SupportVectorMachineModel model = new SupportVectorMachineModel();
      while (reader.ReadLine().Trim() != "RangeTransform:") ; // read until line "RangeTransform";
      model.RangeTransform = SVM.RangeTransform.Read(reader);
      // read until "Model:"
      while (reader.ReadLine().Trim() != "Model:") ;
      model.Model = SVM.Model.Read(reader);
      return model;
    }
  }
}
