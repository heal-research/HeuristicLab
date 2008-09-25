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
using System.Data;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class OdbcDatabaseDriverConfiguration : ItemBase, IDriverConfiguration {
    private StringData odbcDriver;
    public StringData OdbcDriver {
      get { return odbcDriver; }
      set {
        odbcDriver = value;
        OnChanged();
      }
    }
    private StringData ipAddress;
    public StringData IPAddress {
      get { return ipAddress; }
      set {
        ipAddress = value;
        OnChanged();
      }
    }
    private IntData port;
    public IntData Port {
      get { return port; }
      set {
        port = value;
        OnChanged();
      }
    }
    private StringData dbName;
    public StringData DBName {
      get { return dbName; }
      set {
        dbName = value;
        OnChanged();
      }
    }
    private StringData dbUser;
    public StringData DBUser {
      get { return dbUser; }
      set {
        dbUser = value;
        OnChanged();
      }
    }
    private StringData dbPassword;
    public StringData DBPassword {
      get { return dbPassword; }
      set {
        dbPassword = value;
        OnChanged();
      }
    }
    private StringData dbTable;
    public StringData DBTable {
      get { return dbTable; }
      set {
        dbTable = value;
        OnChanged();
      }
    }
    private StringData dbIDcolumnName;
    public StringData DBIDColumnName {
      get { return dbIDcolumnName; }
      set {
        dbIDcolumnName = value;
        OnChanged();
      }
    }
    private StringData dbCommunicationColumnName;
    public StringData DBCommunciationColumnName {
      get { return dbCommunicationColumnName; }
      set {
        dbCommunicationColumnName = value;
        OnChanged();
      }
    }
    private StringData dbSynchronizationColumnName;
    public StringData DBSynchronizationColumnName {
      get { return dbSynchronizationColumnName; }
      set {
        dbSynchronizationColumnName = value;
        OnChanged();
      }
    }
    
    public OdbcDatabaseDriverConfiguration() {
      odbcDriver = new StringData("");
      ipAddress = new StringData("127.0.0.1");
      port = new IntData(0);
      dbName = new StringData("");
      dbUser = new StringData("");
      dbPassword = new StringData("");
      dbTable = new StringData("");
      dbIDcolumnName = new StringData("");
      dbCommunicationColumnName = new StringData("");
      dbSynchronizationColumnName = new StringData("");
    }

    public override IView CreateView() {
      return new OdbcDatabaseDriverConfigurationView(this);
    }

    #region persistence & clone
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode driverNode = PersistenceManager.Persist("Driver", OdbcDriver, document, persistedObjects);
      node.AppendChild(driverNode);
      XmlNode ipNode = PersistenceManager.Persist("IP", IPAddress, document, persistedObjects);
      node.AppendChild(ipNode);
      XmlNode portNode = PersistenceManager.Persist("Port", Port, document, persistedObjects);
      node.AppendChild(portNode);
      XmlNode nameNode = PersistenceManager.Persist("DBName", DBName, document, persistedObjects);
      node.AppendChild(nameNode);
      XmlNode userNode = PersistenceManager.Persist("DBUser", DBUser, document, persistedObjects);
      node.AppendChild(userNode);
      XmlNode pwdNode = PersistenceManager.Persist("DBPassword", DBPassword, document, persistedObjects);
      node.AppendChild(pwdNode);
      XmlNode tableNode = PersistenceManager.Persist("DBTable", DBTable, document, persistedObjects);
      node.AppendChild(tableNode);
      XmlNode idNode = PersistenceManager.Persist("DBIDColumnName", DBIDColumnName, document, persistedObjects);
      node.AppendChild(idNode);
      XmlNode commNode = PersistenceManager.Persist("DBCommunicationColumnName", DBCommunciationColumnName, document, persistedObjects);
      node.AppendChild(commNode);
      XmlNode syncNode = PersistenceManager.Persist("DBSynchronizationColumnName", DBSynchronizationColumnName, document, persistedObjects);
      node.AppendChild(syncNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      odbcDriver = (StringData)PersistenceManager.Restore(node.SelectSingleNode("Driver"), restoredObjects);
      ipAddress = (StringData)PersistenceManager.Restore(node.SelectSingleNode("IP"), restoredObjects);
      port = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Port"), restoredObjects);
      dbName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBName"), restoredObjects);
      dbUser = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBUser"), restoredObjects);
      dbPassword = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBPassword"), restoredObjects);
      dbTable = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBTable"), restoredObjects);
      dbIDcolumnName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBIDColumnName"), restoredObjects);
      dbCommunicationColumnName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBCommunicationColumnName"), restoredObjects);
      dbSynchronizationColumnName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("DBSynchronizationColumnName"), restoredObjects);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OdbcDatabaseDriverConfiguration clone = new OdbcDatabaseDriverConfiguration();
      clonedObjects.Add(Guid, clone);
      clone.odbcDriver = (StringData)Auxiliary.Clone(odbcDriver, clonedObjects);
      clone.ipAddress = (StringData)Auxiliary.Clone(ipAddress, clonedObjects);
      clone.port = (IntData)Auxiliary.Clone(port, clonedObjects);
      clone.dbName = (StringData)Auxiliary.Clone(dbName, clonedObjects);
      clone.dbUser = (StringData)Auxiliary.Clone(dbUser, clonedObjects);
      clone.dbPassword = (StringData)Auxiliary.Clone(dbPassword, clonedObjects);
      clone.dbTable = (StringData)Auxiliary.Clone(dbTable, clonedObjects);
      clone.dbIDcolumnName = (StringData)Auxiliary.Clone(dbIDcolumnName, clonedObjects);
      clone.dbCommunicationColumnName = (StringData)Auxiliary.Clone(dbCommunicationColumnName, clonedObjects);
      clone.dbSynchronizationColumnName = (StringData)Auxiliary.Clone(dbSynchronizationColumnName, clonedObjects);
      return clone;
    }
    #endregion
  }
}
