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
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Drawing;

namespace HeuristicLab.CEDMA.Core {
  public class Results : ItemBase {
    private string[] categoricalVariables = null;
    public string[] CategoricalVariables {
      get {
        if (categoricalVariables == null) {
          LoadModelAttributes();
        }
        return categoricalVariables;
      }
    }

    private string[] ordinalVariables = null;
    public string[] OrdinalVariables {
      get {
        if (ordinalVariables == null) {
          LoadModelAttributes();
        }
        return ordinalVariables;
      }
    }

    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
      }
    }

    private Entity dataSetEntity;

    public Results(IStore store) {
      this.store = store;
    }

    internal void FilterDataSet(Entity entity) {
      this.dataSetEntity = entity;
    }

    private List<ResultsEntry> entries = null;
    private bool cached = false;
    public IEnumerable<ResultsEntry> GetEntries() {
      if (!cached)
        return SelectRows();
      return entries.AsEnumerable();
    }

    private IEnumerable<ResultsEntry> SelectRows() {
      if (store == null) yield break;
      entries = new List<ResultsEntry>();
      var results = store.Query("<" + dataSetEntity + "> <" + Ontology.PredicateHasModel + "> ?Model ." + Environment.NewLine +
        "?Model ?Attribute ?Value .")
        .GroupBy(x => (Entity)x.Get("Model"));
      foreach (var modelBindings in results) {
        ResultsEntry entry = new ResultsEntry(modelBindings.Key.Uri);
        foreach (var binding in modelBindings) {
          if (binding.Get("Value") is Literal) {
            string name = ((Entity)binding.Get("Attribute")).Uri.Replace(Ontology.CedmaNameSpace, "");
            if (entry.Get(name) == null) {
              object value = ((Literal)binding.Get("Value")).Value;
              entry.Set(name, value);
            }
          }
        }
        entries.Add(entry);
        yield return entry;
      }

      FireChanged();
      cached = true;
    }

    internal IEnumerable<string> SelectModelAttributes() {
      return CategoricalVariables.Concat(OrdinalVariables);
    }

    private void LoadModelAttributes() {
      this.ordinalVariables =
        store.Query("?ModelAttribute <" + Ontology.PredicateInstanceOf + "> <" + Ontology.TypeOrdinalAttribute + "> .")
        .Select(s => ((Entity)s.Get("ModelAttribute")).Uri.Replace(Ontology.CedmaNameSpace, ""))
        .ToArray();
      this.categoricalVariables =
        store.Query("?ModelAttribute <" + Ontology.PredicateInstanceOf + "> <" + Ontology.TypeCategoricalAttribute + "> .")
        .Select(s => ((Entity)s.Get("ModelAttribute")).Uri.Replace(Ontology.CedmaNameSpace, ""))
        .ToArray();
    }
  }
}
