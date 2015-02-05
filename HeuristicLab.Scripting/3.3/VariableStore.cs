#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.IO;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Scripting {
  [Item("VariableStore", "Represents a variable store.")]
  [StorableClass]
  public class VariableStore : ObservableDictionary<string, object>, IItem {
    #region Properties
    public virtual string ItemName {
      get { return ItemAttribute.GetName(GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(GetType()); }
    }
    public static Image StaticItemImage {
      get { return VSImageLibrary.Class; }
    }
    public virtual Image ItemImage {
      get { return ItemAttribute.GetImage(GetType()); }
    }
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    protected VariableStore(bool deserializing) : base(deserializing) { }
    protected VariableStore(VariableStore original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      foreach (var kvp in original.dict) {
        var clonedValue = kvp.Value as IDeepCloneable;
        if (clonedValue != null) {
          dict[kvp.Key] = cloner.Clone(clonedValue);
        } else {
          try {
            dict[kvp.Key] = CloneByPersistence(kvp.Value);
          } catch (PersistenceException pe) {
            throw new NotSupportedException(string.Format(@"VariableStore: Variable ""{0}"" could not be cloned.", kvp.Key), pe);
          }
        }
      }
    }
    public VariableStore() : base() { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public IDeepCloneable Clone(Cloner cloner) {
      return new VariableStore(this, cloner);
    }

    protected T CloneByPersistence<T>(T value) {
      using (var serializerStream = new MemoryStream()) {
        XmlGenerator.Serialize(value, serializerStream);
        var bytes = serializerStream.GetBuffer();
        using (var deserializerStream = new MemoryStream(bytes)) {
          return XmlParser.Deserialize<T>(deserializerStream);
        }
      }
    }
    #endregion

    #region Overrides
    public override string ToString() {
      return ItemName;
    }
    #endregion

    #region Events
    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      var handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      var handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
