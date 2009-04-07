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
using System.Data.Linq;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using HeuristicLab.Data;
using HeuristicLab.Core;
using System.Xml;
using System.IO;

namespace HeuristicLab.CEDMA.DB {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class Store : IStore {
    private string connectionString;
    private SemWeb.Store store;
    private object bigLock = new object();
    public Store(string connectionString) {
      lock (bigLock) {
        this.connectionString = connectionString;
        store = SemWeb.Store.Create(connectionString);
        InitStore();
      }
    }

    private void InitStore() {
      foreach (Statement s in Ontology.InitialStatements) {
        Add(s);
      }
    }

    public void Add(Statement statement) {
      lock (bigLock) {
        store.Add(Translate(statement));
      }
    }


    public ICollection<VariableBindings> Query(string query, int page, int pageSize) {
      MyQueryResultSink resultSink = new MyQueryResultSink();
      SemWeb.N3Reader n3Reader = new SemWeb.N3Reader(new StringReader(query));
      SemWeb.Query.GraphMatch matcher = new SemWeb.Query.GraphMatch(n3Reader);
      matcher.Run(store, resultSink);
      return resultSink.Bindings.Skip(page*pageSize).Take(pageSize).ToList();
    }

    public ICollection<VariableBindings> Query(ICollection<Statement> query, int page, int pageSize) {
      MyQueryResultSink resultSink = new MyQueryResultSink();
      Translate(query).Run(store, resultSink);
      return resultSink.Bindings.Skip(page * pageSize).Take(pageSize).ToList();
    }

    private SemWeb.Query.Query Translate(ICollection<Statement> query) {
      Dictionary<object, object> translatedObjects = new Dictionary<object, object>();
      SemWeb.MemoryStore queryStore = new SemWeb.MemoryStore(query.Select(st => Translate(st, translatedObjects)).ToArray());

      return new SemWeb.Query.GraphMatch(queryStore);
    }

    private static SemWeb.Entity Translate(Entity e) {
      return e.Uri == null ? null : new SemWeb.Entity(e.Uri);
    }

    private static SemWeb.Resource Translate(Resource prop) {
      if (prop is Literal) {
        return TranslateLiteral((Literal)prop);
      } else if (prop is SerializedLiteral) {
        return TranslateLiteral((SerializedLiteral)prop);
      } else {
        return Translate((Entity)prop);
      }
    }

    private static Statement Translate(SemWeb.Statement statement) {
      return Translate(statement, new Dictionary<object, object>());
    }

    private static Statement Translate(SemWeb.Statement statement, Dictionary<object, object> translatedObjects) {
      if (!translatedObjects.ContainsKey(statement.Subject)) {
        translatedObjects[statement.Subject] = new Entity(statement.Subject.Uri);
      }
      if (!translatedObjects.ContainsKey(statement.Predicate)) {
        translatedObjects[statement.Predicate] = new Entity(statement.Predicate.Uri);
      }
      if (!translatedObjects.ContainsKey(statement.Object)) {
        if (statement.Object is SemWeb.Literal) {
          translatedObjects[statement.Object] = TranslateLiteral((SemWeb.Literal)statement.Object);
        } else {
          translatedObjects[statement.Object] = new Entity(((SemWeb.Entity)statement.Object).Uri);
        }
      }

      Entity subjectEntity = (Entity)translatedObjects[statement.Subject];
      Entity predicateEntity = (Entity)translatedObjects[statement.Predicate];
      Resource property = (Resource)translatedObjects[statement.Object];

      return new Statement(
        subjectEntity,
        predicateEntity,
        property);
    }

    private static SemWeb.Statement Translate(Statement statement) {
      return Translate(statement, new Dictionary<object, object>());
    }

    private static SemWeb.Statement Translate(Statement statement, Dictionary<object, object> translatedObjects) {
      if (!translatedObjects.ContainsKey(statement.Subject)) {
        translatedObjects[statement.Subject] = Translate(statement.Subject);
      }
      if (!translatedObjects.ContainsKey(statement.Predicate)) {
        translatedObjects[statement.Predicate] = Translate(statement.Predicate);
      }
      if (!translatedObjects.ContainsKey(statement.Property)) {
        translatedObjects[statement.Property] = Translate(statement.Property);
      }

      SemWeb.Entity subject = (SemWeb.Entity)translatedObjects[statement.Subject];
      SemWeb.Entity predicate = (SemWeb.Entity)translatedObjects[statement.Predicate];
      SemWeb.Resource property = (SemWeb.Resource)translatedObjects[statement.Property];

      return new SemWeb.Statement(
        subject,
        predicate,
        property);
    }

    private static SemWeb.Literal TranslateLiteral(SerializedLiteral l) {
      if (l.RawData == null) return null;
      return new SemWeb.Literal(l.RawData, null, "serializedItem");
    }

    private static SemWeb.Literal TranslateLiteral(Literal l) {
      if (l.Value == null) return null;
      if (l.Value is double) return SemWeb.Literal.FromValue((double)l.Value);
      else if (l.Value is bool) return SemWeb.Literal.FromValue((bool)l.Value);
      else if (l.Value is int) return SemWeb.Literal.FromValue((int)l.Value);
      else if (l.Value is long) return SemWeb.Literal.FromValue((long)l.Value);
      else if (l.Value is string) return SemWeb.Literal.FromValue((string)l.Value);
      else return new SemWeb.Literal(l.Value.ToString());
    }

    private static Resource TranslateLiteral(SemWeb.Literal l) {
      if (l.DataType == "serializedItem") {
        return new SerializedLiteral(l.Value);
      } else if (l.DataType != null) {
        return new Literal(l.ParseValue());
      } else {
        return new Literal(l.Value);
      }
    }

    private class MyQueryResultSink : SemWeb.Query.QueryResultSink {

      private List<VariableBindings> bindings = new List<VariableBindings>();
      public ICollection<VariableBindings> Bindings {
        get { return bindings.AsReadOnly(); }
      }

      public override bool Add(SemWeb.Query.VariableBindings result) {
        VariableBindings varBindings = new VariableBindings();
        foreach (SemWeb.Variable var in result.Variables) {
          if (var.LocalName != null && result[var] != null) {
            if (result[var] is SemWeb.Literal) {
              varBindings.Add(var.LocalName, TranslateLiteral((SemWeb.Literal)result[var]));
            } else {
              varBindings.Add(var.LocalName, new Entity(((SemWeb.Entity)result[var]).Uri));
            }
          }
          bindings.Add(varBindings);
        }
        return true;
      }
    }
  }
}