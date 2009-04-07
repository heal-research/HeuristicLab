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
  public class DataSet : IViewable {
    public IStore Store { get; set; }

    private Guid guid;
    public Guid Guid {
      get { return guid; }
    }

    private string name;
    public string Name {
      get { return name; }
      set { name = value; }
    }

    private Problem problem;
    public Problem Problem {
      get {
        // lazy loading of problem from DB
        if (problem == null) {
          var persistedData = Store.Query(
            "<" + Ontology.CedmaNameSpace + Guid + "> <" + Ontology.PredicateSerializedData.Uri + "> ?SerializedData .", 0, 10);
          if (persistedData.Count() == 1) {
            Literal persistedLiteral = (Literal)persistedData.First().Get("SerializedData");
            this.problem = (Problem)PersistenceManager.RestoreFromGZip(Convert.FromBase64String((string)persistedLiteral.Value));
          } else
            this.problem = new Problem(); // no entry in the DB => create a new problem
        }
        return problem;
      }
    }

    public DataSet()
      : base() {
      guid = Guid.NewGuid();
      name = "Data set";
    }

    public DataSet(IStore store, Entity dataSetEntity)
      : this() {
      Store = store;
      guid = new Guid(dataSetEntity.Uri.Remove(0, Ontology.CedmaNameSpace.Length));
      name = guid.ToString();
    }

    public void Activate() {
      Entity myEntity = new Entity(Ontology.CedmaNameSpace + Guid);
      Store.Add(new Statement(myEntity, Ontology.PredicateInstanceOf, Ontology.TypeDataSet));
      Store.Add(new Statement(myEntity, Ontology.PredicateSerializedData, new Literal(Convert.ToBase64String(PersistenceManager.SaveToGZip(problem)))));
      Store.Add(new Statement(myEntity, Ontology.PredicateName, new Literal(name)));
    }

    public IView CreateView() {
      return new DataSetView(this);
    }

    internal Results GetResults() {
      Results results = new Results(Store);
      results.FilterDataSet(new Entity(Ontology.CedmaNameSpace + Guid));
      return results;
    }
  }
}
