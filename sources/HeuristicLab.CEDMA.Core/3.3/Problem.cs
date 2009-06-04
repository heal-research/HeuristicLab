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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {

  public enum LearningTask {
    Classification,
    Regression,
    TimeSeries,
    Clustering
  }

  /// <summary>
  /// Problem describes the data mining task.
  /// Contains the actual data and meta-data:
  ///  * which variables should be modelled 
  ///  * regression, time-series or classification problem
  /// </summary>
  public class Problem : ItemBase {
    private HeuristicLab.DataAnalysis.Dataset dataset;
    public HeuristicLab.DataAnalysis.Dataset Dataset {
      get { return dataset; }
    }

    private int trainingSamplesStart;
    public int TrainingSamplesStart {
      get { return trainingSamplesStart; }
      set { trainingSamplesStart = value; }
    }

    private int trainingSamplesEnd;
    public int TrainingSamplesEnd {
      get { return trainingSamplesEnd; }
      set { trainingSamplesEnd = value; }
    }

    private int validationSamplesStart;
    public int ValidationSamplesStart {
      get { return validationSamplesStart; }
      set { validationSamplesStart = value; }
    }

    private int validationSamplesEnd;
    public int ValidationSamplesEnd {
      get { return validationSamplesEnd; }
      set { validationSamplesEnd = value; }
    }

    private int testSamplesStart;
    public int TestSamplesStart {
      get { return testSamplesStart; }
      set { testSamplesStart = value; }
    }

    private int testSamplesEnd;
    public int TestSamplesEnd {
      get { return testSamplesEnd; }
      set { testSamplesEnd = value; }
    }

    private List<int> allowedInputVariables;
    public List<int> AllowedInputVariables {
      get { return allowedInputVariables; }
    }

    private List<int> allowedTargetVariables;
    public List<int> AllowedTargetVariables {
      get { return allowedTargetVariables; }
    }

    private bool autoRegressive;
    public bool AutoRegressive {
      get { return autoRegressive; }
      set { autoRegressive = value; }
    }

    private int minTimeOffset;
    public int MinTimeOffset {
      get { return minTimeOffset; }
      set { minTimeOffset = value; }
    }

    private int maxTimeOffset;
    public int MaxTimeOffset {
      get { return maxTimeOffset; }
      set { maxTimeOffset = value; }
    }

    private LearningTask learningTask;
    public LearningTask LearningTask {
      get { return learningTask; }
      set { learningTask = value; }
    }

    public Problem()
      : base() {
      dataset = new DataAnalysis.Dataset();
      allowedInputVariables = new List<int>();
      allowedTargetVariables = new List<int>();
    }


    public string GetVariableName(int index) {
      return dataset.GetVariableName(index);
    }

    public override IView CreateView() {
      return new ProblemView(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("DataSet", dataset, document, persistedObjects));
      XmlAttribute trainingSamplesStartAttr = document.CreateAttribute("TrainingSamplesStart");
      trainingSamplesStartAttr.Value = TrainingSamplesStart.ToString();
      XmlAttribute trainingSamplesEndAttr = document.CreateAttribute("TrainingSamplesEnd");
      trainingSamplesEndAttr.Value = TrainingSamplesEnd.ToString();
      XmlAttribute validationSamplesStartAttr = document.CreateAttribute("ValidationSamplesStart");
      validationSamplesStartAttr.Value = ValidationSamplesStart.ToString();
      XmlAttribute validationSamplesEndAttr = document.CreateAttribute("ValidationSamplesEnd");
      validationSamplesEndAttr.Value = ValidationSamplesEnd.ToString();
      XmlAttribute testSamplesStartAttr = document.CreateAttribute("TestSamplesStart");
      testSamplesStartAttr.Value = TestSamplesStart.ToString();
      XmlAttribute testSamplesEndAttr = document.CreateAttribute("TestSamplesEnd");
      testSamplesEndAttr.Value = TestSamplesEnd.ToString();
      XmlAttribute learningTaskAttr = document.CreateAttribute("LearningTask");
      learningTaskAttr.Value = LearningTask.ToString();
      XmlAttribute autoRegressiveAttr = document.CreateAttribute("AutoRegressive");
      autoRegressiveAttr.Value = AutoRegressive.ToString();
      XmlAttribute minTimeOffsetAttr = document.CreateAttribute("MinTimeOffset");
      minTimeOffsetAttr.Value = MinTimeOffset.ToString();
      XmlAttribute maxTimeOffsetAttr = document.CreateAttribute("MaxTimeOffset");
      maxTimeOffsetAttr.Value = MaxTimeOffset.ToString();

      node.Attributes.Append(trainingSamplesStartAttr);
      node.Attributes.Append(trainingSamplesEndAttr);
      node.Attributes.Append(validationSamplesStartAttr);
      node.Attributes.Append(validationSamplesEndAttr);
      node.Attributes.Append(testSamplesStartAttr);
      node.Attributes.Append(testSamplesEndAttr);
      node.Attributes.Append(learningTaskAttr);
      node.Attributes.Append(autoRegressiveAttr);
      node.Attributes.Append(minTimeOffsetAttr);
      node.Attributes.Append(maxTimeOffsetAttr);

      XmlElement targetVariablesElement = document.CreateElement("AllowedTargetVariables");
      targetVariablesElement.InnerText = SemiColonSeparatedList(AllowedTargetVariables);
      XmlElement inputVariablesElement = document.CreateElement("AllowedInputVariables");
      inputVariablesElement.InnerText = SemiColonSeparatedList(AllowedInputVariables);
      node.AppendChild(targetVariablesElement);
      node.AppendChild(inputVariablesElement);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      dataset = (HeuristicLab.DataAnalysis.Dataset)PersistenceManager.Restore(node.SelectSingleNode("DataSet"), restoredObjects);
      TrainingSamplesStart = int.Parse(node.Attributes["TrainingSamplesStart"].Value);
      TrainingSamplesEnd = int.Parse(node.Attributes["TrainingSamplesEnd"].Value);
      ValidationSamplesStart = int.Parse(node.Attributes["ValidationSamplesStart"].Value);
      ValidationSamplesEnd = int.Parse(node.Attributes["ValidationSamplesEnd"].Value);
      TestSamplesStart = int.Parse(node.Attributes["TestSamplesStart"].Value);
      TestSamplesEnd = int.Parse(node.Attributes["TestSamplesEnd"].Value);
      LearningTask = (LearningTask)Enum.Parse(typeof(LearningTask), node.Attributes["LearningTask"].Value);
      AutoRegressive = bool.Parse(node.Attributes["AutoRegressive"].Value);
      if (node.Attributes["MinTimeOffset"] != null)
        MinTimeOffset = XmlConvert.ToInt32(node.Attributes["MinTimeOffset"].Value);
      else MinTimeOffset = 0;
      if (node.Attributes["MaxTimeOffset"] != null)
        MaxTimeOffset = XmlConvert.ToInt32(node.Attributes["MaxTimeOffset"].Value);
      else MaxTimeOffset = 0;

      allowedTargetVariables.Clear();
      foreach (string tok in node.SelectSingleNode("AllowedTargetVariables").InnerText.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
        allowedTargetVariables.Add(int.Parse(tok));
      allowedInputVariables.Clear();
      foreach (string tok in node.SelectSingleNode("AllowedInputVariables").InnerText.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
        allowedInputVariables.Add(int.Parse(tok));
    }

    private string SemiColonSeparatedList(List<int> xs) {
      StringBuilder b = new StringBuilder();
      foreach (int x in xs) {
        b = b.Append(x).Append(";");
      }
      if (xs.Count > 0) b.Remove(b.Length - 1, 1); // remove last ';'
      return b.ToString();
    }
  }
}
