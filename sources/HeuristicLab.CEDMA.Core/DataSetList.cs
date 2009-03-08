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
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class DataSetList : ItemBase, IEnumerable<DataSet> {
    private List<DataSet> dataSetList;
    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
        Action reload = ReloadList;
        lock (dataSetList) {
          dataSetList.Clear();
        }
        reload.BeginInvoke(null, null);
      }
    }

    public DataSetList()
      : base() {
      dataSetList = new List<DataSet>();
    }

    public override IView CreateView() {
      return new DataSetListView(this);
    }

    private void ReloadList() {
      HeuristicLab.CEDMA.DB.Interfaces.Variable dataSetVar = new HeuristicLab.CEDMA.DB.Interfaces.Variable("DataSet");
      HeuristicLab.CEDMA.DB.Interfaces.Variable dataSetNameVar = new HeuristicLab.CEDMA.DB.Interfaces.Variable("Name");
      var bindings = store.Query(
        "?DataSet <"+Ontology.PredicateInstanceOf.Uri+"> <"+Ontology.TypeDataSet.Uri+"> ." + Environment.NewLine +
        "?DataSet <"+Ontology.PredicateName.Uri+"> ?Name .");
      //  new Statement[] { 
      //    new Statement(dataSetVar, Ontology.PredicateInstanceOf, Ontology.TypeDataSet),
      //    new Statement(dataSetVar, Ontology.PredicateName, dataSetNameVar)
      //});
      foreach (var binding in bindings) {
        DataSet d = new DataSet(store, (Entity)binding.Get("DataSet"));
        d.Name = (string)((Literal)binding.Get("Name")).Value;
        lock (dataSetList) {
          dataSetList.Add(d);
        }
        FireChanged();
      }
    }

    public IEnumerator<DataSet> GetEnumerator() {
      List<DataSet> dataSets = new List<DataSet>();
      lock (dataSets) {
        dataSets.AddRange(dataSetList);
      }
      return dataSets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void Add(DataSet dataSet) {
      dataSetList.Add(dataSet);
    }
  }
}
