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
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Functions;
using HeuristicLab.Charting.Data;
using System.Drawing;

namespace HeuristicLab.CEDMA.Charting {

  public class RecordAddedEventArgs : EventArgs {
    private Record record;
    public Record Record { get { return record; } }
    public RecordAddedEventArgs(Record r)
      : base() {
      this.record = r;
    }
  }

  public class ResultList : ItemBase {
    private const string cedmaNS = "http://www.heuristiclab.com/cedma/";
    private readonly Entity targetVariablePredicate = new Entity(cedmaNS + "TargetVariable");
    private readonly Entity trainingMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTraining");
    private readonly Entity validationMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorValidation");
    private readonly Entity testMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTest");
    private readonly Entity trainingR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTraining");
    private readonly Entity validationR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationValidation");
    private readonly Entity testR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTest");
    private readonly Entity treeSizePredicate = new Entity(cedmaNS + "TreeSize");
    private readonly Entity treeHeightPredicate = new Entity(cedmaNS + "TreeHeight");
    private readonly Entity selectionPressurePredicate = new Entity(cedmaNS + "SelectionPressure");
    private readonly Entity rawDataPredicate = new Entity(cedmaNS + "RawData");
    private readonly Entity hasModelPredicate = new Entity(cedmaNS + "Model");
    private readonly Entity anyEntity = new Entity(null);
    private Dictionary<Record, Dataset> datasets;

    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
        Action reloadList = ReloadList;
        reloadList.BeginInvoke(null, null);
      }
    }

    private List<string> variableNames = new List<string>() { Record.TARGET_VARIABLE, Record.TREE_SIZE, Record.TREE_HEIGHT, Record.SELECTIONPRESSURE,
    Record.MAPE_TRAINING, Record.MAPE_VALIDATION, Record.MAPE_TEST,
    Record.R2_TRAINING, Record.R2_VALIDATION, Record.R2_TEST};
    public string[] VariableNames {
      get {
        return variableNames.ToArray();
      }
    }

    private Dictionary<Entity, string> predicateToVariableName;

    public event EventHandler<RecordAddedEventArgs> OnRecordAdded;

    private List<Record> records;
    public List<Record> Records {
      get {
        List<Record> result = new List<Record>();
        lock(records) {
          result.AddRange(records);
        }
        return result;
      }
    }
    private void ReloadList() {
      var results = store.Select(new Statement(anyEntity, new Entity(cedmaNS + "instanceOf"), new Literal("class:GpFunctionTree")))
      .Select(x => store.Select(new SelectFilter(
        new Entity[] { new Entity(x.Subject.Uri) },
        new Entity[] { targetVariablePredicate, treeSizePredicate, treeHeightPredicate, selectionPressurePredicate,
          trainingMAPEPredicate, validationMAPEPredicate, testMAPEPredicate,
          trainingR2Predicate, validationR2Predicate, testR2Predicate },
          new Resource[] { anyEntity })));

      Random random = new Random();
      foreach(Statement[] ss in results) {
        if(ss.Length > 0) {
          Record r = new Record(this, ss[0].Subject.Uri);
          r.Set(Record.X_JITTER, random.NextDouble() * 2.0 - 1.0);
          r.Set(Record.Y_JITTER, random.NextDouble() * 2.0 - 1.0);
          foreach(Statement s in ss) {
            string varName;
            predicateToVariableName.TryGetValue(s.Predicate, out varName);
            if(varName != null) {
              if(varName == Record.TREE_HEIGHT || varName == Record.TREE_SIZE || varName == Record.TARGET_VARIABLE) {
                r.Set(varName, (double)(int)((Literal)s.Property).Value);
              } else {
                r.Set(varName, (double)((Literal)s.Property).Value);
              }
            }
          }
          lock(records) {
            records.Add(r);
          }
          FireRecordAdded(r);
        }
      }
      FireChanged();
    }

    private void FireRecordAdded(Record r) {
      if(OnRecordAdded != null) OnRecordAdded(this, new RecordAddedEventArgs(r));
    }

    public ResultList()
      : base() {
      records = new List<Record>();
      datasets = new Dictionary<Record, Dataset>();
      predicateToVariableName = new Dictionary<Entity, string>();
      predicateToVariableName[targetVariablePredicate] = Record.TARGET_VARIABLE;
      predicateToVariableName[treeSizePredicate] = Record.TREE_SIZE;
      predicateToVariableName[treeHeightPredicate] = Record.TREE_HEIGHT;
      predicateToVariableName[selectionPressurePredicate] = Record.SELECTIONPRESSURE;
      predicateToVariableName[trainingMAPEPredicate] = Record.MAPE_TRAINING;
      predicateToVariableName[validationMAPEPredicate] = Record.MAPE_VALIDATION;
      predicateToVariableName[testMAPEPredicate] = Record.MAPE_TEST;
      predicateToVariableName[trainingR2Predicate] = Record.R2_TRAINING;
      predicateToVariableName[validationR2Predicate] = Record.R2_VALIDATION;
      predicateToVariableName[testR2Predicate] = Record.R2_TEST;
    }

    public override IView CreateView() {
      return new ResultListView(this);
    }

    internal void OpenModel(Record record) {
      IList<Statement> modelResults = store.Select(new Statement(new Entity(record.Uri), rawDataPredicate, anyEntity));
      if(modelResults.Count == 1) {
        string rawData = ((SerializedLiteral)modelResults[0].Property).RawData;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(rawData);
        IFunctionTree tree = (IFunctionTree)PersistenceManager.Restore(doc.ChildNodes[1], new Dictionary<Guid, IStorable>());
        int targetVariable = (int)record.Get(Record.TARGET_VARIABLE);
        Dataset dataset = GetDataset(record);

        ModelView modelView = new ModelView(dataset, tree, targetVariable);
        PluginManager.ControlManager.ShowControl(modelView);
      }
    }

    private Dataset GetDataset(Record record) {
      if(!datasets.ContainsKey(record)) {
        IList<Statement> result = store.Select(new Statement(anyEntity, hasModelPredicate, new Entity(record.Uri)));
        if(result.Count == 1) {
          IList<Statement> datasetResult = store.Select(new Statement(result[0].Subject, rawDataPredicate, anyEntity));
          if(datasetResult.Count == 1) {
            string rawData = ((SerializedLiteral)datasetResult[0].Property).RawData;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(rawData);
            Dataset dataset = (Dataset)PersistenceManager.Restore(doc.ChildNodes[1], new Dictionary<Guid, IStorable>());
            datasets.Add(record, dataset);
          }
        }
      }
      return datasets[record];
    }
  }
}
