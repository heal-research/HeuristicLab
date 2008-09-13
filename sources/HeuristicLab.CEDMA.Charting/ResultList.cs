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

namespace HeuristicLab.CEDMA.Charting {
  public class Record {
    private Dictionary<string, double> values = new Dictionary<string, double>();
    public Dictionary<string,double> Values {
      get {
        return values;
      }
    }

    private bool selected = false;
    public bool Selected { get { return selected; } }

    private string uri;
    public string Uri { get { return uri; } }
    public Record(string uri) {
      this.uri = uri;
    }

    public void Set(string name, double value) {
      Values.Add(name, value);
    }

    public double Get(string name) {
      if(name==null || !Values.ContainsKey(name)) return double.NaN;
      return Values[name];
    }

    public void ToggleSelected() {
      selected = !selected;
      if(OnSelectionChanged != null) OnSelectionChanged(this, new EventArgs());
    }

    public event EventHandler OnSelectionChanged;
  }

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

    private const string MAPE_TRAINING = "MAPE (training)";
    private const string MAPE_VALIDATION = "MAPE (validation)";
    private const string MAPE_TEST = "MAPE (test)";
    private const string R2_TRAINING = "R2 (training)";
    private const string R2_VALIDATION = "R2 (validation)";
    private const string R2_TEST = "R2 (test)";
    private const string TARGET_VARIABLE = "Target variable";
    private const string TREE_SIZE = "Tree size";
    private const string TREE_HEIGHT = "Tree height";

    private readonly Entity targetVariablePredicate = new Entity(cedmaNS + "TargetVariable");
    private readonly Entity trainingMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTraining");
    private readonly Entity validationMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorValidation");
    private readonly Entity testMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTest");
    private readonly Entity trainingR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTraining");
    private readonly Entity validationR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationValidation");
    private readonly Entity testR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTest");
    private readonly Entity treeSizePredicate = new Entity(cedmaNS + "TreeSize");
    private readonly Entity treeHeightPredicate = new Entity(cedmaNS + "TreeHeight");
    private readonly Entity rawDataPredicate = new Entity(cedmaNS + "rawData");
    private readonly Entity anyEntity = new Entity(null);

    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
        Action reloadList = ReloadList;
        reloadList.BeginInvoke(null, null);
      }
    }

    private List<string> variableNames = new List<string>() { TARGET_VARIABLE, TREE_SIZE, TREE_HEIGHT,
    MAPE_TRAINING, MAPE_VALIDATION, MAPE_TEST,
    R2_TRAINING, R2_VALIDATION, R2_TEST};
    public string[] VariableNames {
      get {
        return variableNames.ToArray();
      }
    }

    public event EventHandler<RecordAddedEventArgs> OnRecordAdded;

    private List<Record> records;

    private void ReloadList() {
      var results = store.Select(new Statement(anyEntity, new Entity(cedmaNS + "instanceOf"), new Literal("class:GpFunctionTree")))
      .Select(x => store.Select(new SelectFilter(
        new Entity[] { new Entity(x.Subject.Uri) },
        new Entity[] { targetVariablePredicate, treeSizePredicate, treeHeightPredicate,
          trainingMAPEPredicate, validationMAPEPredicate, testMAPEPredicate,
          trainingR2Predicate, validationR2Predicate, testR2Predicate },
          new Resource[] { anyEntity })));

      foreach(Statement[] ss in results) {
        if(ss.Length > 0) {
          Record r = new Record(ss[0].Subject.Uri);
          foreach(Statement s in ss) {
            if(s.Predicate.Equals(targetVariablePredicate)) {
              r.Set(TARGET_VARIABLE, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(treeSizePredicate)) {
              r.Set(TREE_SIZE, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(treeHeightPredicate)) {
              r.Set(TREE_HEIGHT, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(trainingMAPEPredicate)) {
              r.Set(MAPE_TRAINING, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(validationMAPEPredicate)) {
              r.Set(MAPE_VALIDATION, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(testMAPEPredicate)) {
              r.Set(MAPE_TEST, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(trainingR2Predicate)) {
              r.Set(R2_TRAINING, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(validationR2Predicate)) {
              r.Set(R2_VALIDATION, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(testR2Predicate)) {
              r.Set(R2_TEST, (double)((Literal)s.Property).Value);
            }
          }
          records.Add(r);
          FireRecordAdded(r);
        }
      }
    }

    private void FireRecordAdded(Record r) {
      if(OnRecordAdded != null) OnRecordAdded(this, new RecordAddedEventArgs(r));
    }

    public ResultList()
      : base() {
      records = new List<Record>();
    }

    public override IView CreateView() {
      return new ResultListView(this);
    }

    internal void OpenModel(Record r) {
      IList<Statement> s = store.Select(new Statement(new Entity(r.Uri), rawDataPredicate, anyEntity));
      if(s.Count == 1) {
        string rawData = ((SerializedLiteral)s[0].Property).RawData;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(rawData);
        IItem item = (IItem)PersistenceManager.Restore(doc.ChildNodes[1], new Dictionary<Guid, IStorable>());
        PluginManager.ControlManager.ShowControl(item.CreateView());
      }
    }
  }
}
