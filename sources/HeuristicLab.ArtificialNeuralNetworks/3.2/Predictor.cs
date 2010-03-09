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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Modeling;
using System;
using System.Xml;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.ArtificialNeuralNetworks {
  public class Predictor : PredictorBase {
    private MultiLayerPerceptron perceptron;
    public Predictor() : base() { } // for persistence
    public Predictor(MultiLayerPerceptron perceptron, double lowerPredictionLimit, double upperPredictionLimit)
      : base(lowerPredictionLimit, upperPredictionLimit) {
      this.perceptron = perceptron;
    }

    public override IEnumerable<double> Predict(Dataset input, int start, int end) {

      if (start < 0 || end <= start) throw new ArgumentException("start must be larger than zero and strictly smaller than end");
      if (end > input.Rows) throw new ArgumentOutOfRangeException("number of rows in input is smaller then end");

      for (int i = 0; i < end - start; i++) {
        double[] output = new double[1];
        double[] inputRow = new double[GetInputVariables().Count()];
        int c = 0;
        foreach (var inputVariable in GetInputVariables()) {
          int inputVariableIndex = input.GetVariableIndex(inputVariable);
          inputRow[c++] = input.GetValue(i + start, inputVariableIndex);
        }
        double estimatedValue;
        try {
          alglib.mlpbase.multilayerperceptron p = perceptron.Perceptron;
          alglib.mlpbase.mlpprocess(ref p, ref inputRow, ref output);
          perceptron.Perceptron = p;
          estimatedValue = output[0];
        }
        catch (ArithmeticException) {
          estimatedValue = UpperPredictionLimit;
        }
        yield return Math.Max(Math.Min(estimatedValue, UpperPredictionLimit), LowerPredictionLimit);
      }
    }

    public override IEnumerable<string> GetInputVariables() {
      return perceptron.InputVariables;
    }


    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Predictor clone = (Predictor)base.Clone(clonedObjects);
      clone.perceptron = (MultiLayerPerceptron)Auxiliary.Clone(perceptron, clonedObjects);
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Perceptron", perceptron, document, persistedObjects));
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      perceptron = (MultiLayerPerceptron)PersistenceManager.Restore(node.SelectSingleNode("Perceptron"), restoredObjects);
    }
  }
}
