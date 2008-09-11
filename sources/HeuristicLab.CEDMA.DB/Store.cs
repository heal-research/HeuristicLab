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
using System.Data.SQLite;
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
    public Store(string connectionString) {
      this.connectionString = connectionString;
      store = SemWeb.Store.Create(connectionString);
    }

    public void Add(Statement statement) {
      store.Add(Translate(statement));
    }

    public IList<Statement> Select(Statement template) {
      SemWeb.SelectResult result = store.Select(Translate(template));
      List<Statement> r = new List<Statement>();
      foreach(SemWeb.Statement resultStatement in result) {
        r.Add(Translate(resultStatement));
      }
      return r;
    }

    public IList<Statement> Select(SelectFilter filter) {
      SemWeb.SelectResult result = store.Select(Translate(filter));
      List<Statement> r = new List<Statement>();
      foreach(SemWeb.Statement resultStatement in result) {
        r.Add(Translate(resultStatement));
      }
      return r;
    }

    private SemWeb.SelectFilter Translate(SelectFilter filter) {
      SemWeb.SelectFilter f = new SemWeb.SelectFilter();
      f.Subjects = Array.ConvertAll(filter.Subjects, s => Translate(s));
      f.Predicates = Array.ConvertAll(filter.Predicates, p => Translate(p));
      f.Objects = Array.ConvertAll(filter.Properties, prop => Translate(prop));
      return f;
    }

    private SemWeb.Entity Translate(Entity e) {
      return e.Uri == null ? null : new SemWeb.Entity(e.Uri);
    }

    private SemWeb.Resource Translate(Resource prop) {
      if(prop is Literal) {
        return TranslateLiteral((Literal)prop);
      } else if(prop is SerializedLiteral) {
        return TranslateLiteral((SerializedLiteral)prop);
      } else {
        return Translate((Entity)prop);
      }
    }

    private Statement Translate(SemWeb.Statement statement) {
      if(statement.Object is SemWeb.Literal) {
        return new Statement(
          new Entity(statement.Subject.Uri),
          new Entity(statement.Predicate.Uri),
          TranslateLiteral((SemWeb.Literal)statement.Object));
      } else {
        return new Statement(
          new Entity(statement.Subject.Uri),
          new Entity(statement.Predicate.Uri),
          new Entity(((SemWeb.Entity)statement.Object).Uri));
      }
    }

    private SemWeb.Statement Translate(Statement statement) {
      return new SemWeb.Statement(
        Translate(statement.Subject),
        Translate(statement.Predicate),
        Translate(statement.Property));
    }

    private SemWeb.Literal TranslateLiteral(SerializedLiteral l) {
      if(l.RawData == null) return null;
      return new SemWeb.Literal(l.RawData, null, "serializedItem");
    }

    private SemWeb.Literal TranslateLiteral(Literal l) {
      if(l.Value == null) return null;
      if(l.Value is double) return SemWeb.Literal.FromValue((double)l.Value);
      else if(l.Value is bool) return SemWeb.Literal.FromValue((bool)l.Value);
      else if(l.Value is int) return SemWeb.Literal.FromValue((int)l.Value);
      else if(l.Value is long) return SemWeb.Literal.FromValue((long)l.Value);
      else if(l.Value is string) return SemWeb.Literal.FromValue((string)l.Value);
      else return new SemWeb.Literal(l.Value.ToString());
    }

    private Resource TranslateLiteral(SemWeb.Literal l) {
      if(l.DataType == "serializedItem") {
        return new SerializedLiteral(l.Value);
      } else if(l.DataType != null) {
        return new Literal(l.ParseValue());
      } else {
        return new Literal(l.Value);
      }
    }
  }
}