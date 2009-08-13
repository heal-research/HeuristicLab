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
using SVM;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class Predictor : ItemBase, IPredictor {
    private SVMModel svmModel;
    private string targetVariable;

    public Predictor() : base() { } // for persistence

    public Predictor(SVMModel model, string targetVariable)
      : base() {
      this.svmModel = model;
      this.targetVariable = targetVariable;
    }

    public double[] Predict(Dataset input, int start, int end) {
      if (start < 0 || end <= start) throw new ArgumentException("start must be larger than zero and strictly smaller than end");
      if (end > input.Rows) throw new ArgumentOutOfRangeException("number of rows in input is smaller then end");
      RangeTransform transform = svmModel.RangeTransform;
      Model model = svmModel.Model;

      Problem p = SVMHelper.CreateSVMProblem(input, input.GetVariableIndex(targetVariable), start, end);
      Problem scaledProblem = SVM.Scaling.Scale(p, transform);

      int rows = end - start;
      int columns = input.Columns;
      double[] result = new double[rows];
      for (int row = 0; row < rows; row++) {
        result[row] = SVM.Prediction.Predict(model, scaledProblem.X[row]);
      }
      return result;
    }

    public override IView CreateView() {
      return svmModel.CreateView();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Predictor clone = (Predictor)base.Clone(clonedObjects);
      clone.svmModel = (SVMModel)Auxiliary.Clone(svmModel, clonedObjects);
      clone.targetVariable = targetVariable;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute targetVarAttr = document.CreateAttribute("TargetVariable");
      targetVarAttr.Value = targetVariable;
      node.Attributes.Append(targetVarAttr);
      node.AppendChild(PersistenceManager.Persist(svmModel, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      targetVariable = node.Attributes["TargetVariable"].Value;
      svmModel = (SVMModel)PersistenceManager.Restore(node.ChildNodes[0], restoredObjects);
    }
  }
}
