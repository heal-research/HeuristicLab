#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("SolutionMessageBuilder", "Holds and uses a number of converters to translate HeuristicLab objects into appropriate fields of a solution message.")]
  [StorableClass]
  public class SolutionMessageBuilder : NamedItem {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }
    private Dictionary<Type, Action<IItem, string, SolutionMessage.Builder>> dispatcher;

    [Storable]
    private CheckedItemCollection<IItemToSolutionMessageConverter> converters;
    public CheckedItemCollection<IItemToSolutionMessageConverter> Converters {
      get { return converters; }
    }

    [StorableConstructor]
    protected SolutionMessageBuilder(bool deserializing) : base(deserializing) { }
    public SolutionMessageBuilder()
      : base() {
      name = ItemName;
      description = ItemDescription;
      converters = new CheckedItemCollection<IItemToSolutionMessageConverter>();
      AttachEventHandlers();

      converters.Add(new BoolConverter());
      converters.Add(new DateTimeValueConverter());
      converters.Add(new DoubleConverter());
      converters.Add(new IntegerConverter());
      converters.Add(new StringConverter());
      converters.Add(new TimeSpanValueConverter());
    }

    public void AddToMessage(IItem item, string name, SolutionMessage.Builder builder) {
      if (dispatcher == null) FillDispatcher();
      Type itemType = item.GetType();
      while (!dispatcher.ContainsKey(itemType)) {
        if (itemType.BaseType != null) itemType = itemType.BaseType;
        else break;
      }
      if (itemType.BaseType == null && !dispatcher.ContainsKey(itemType)) {
        IEnumerable<Type> interfaces = item.GetType().GetInterfaces().Where(x => dispatcher.ContainsKey(x));
        if (interfaces.Count() != 1) throw new ArgumentException(Name + ": No converter for type + " + itemType.FullName + " defined.", "item");
        else itemType = interfaces.Single();
      }
      dispatcher[itemType](item, name, builder);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AttachEventHandlers() {
      converters.ItemsAdded += new CollectionItemsChangedEventHandler<IItemToSolutionMessageConverter>(converters_ItemsAdded);
      converters.ItemsRemoved += new CollectionItemsChangedEventHandler<IItemToSolutionMessageConverter>(converters_ItemsRemoved);
      converters.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IItemToSolutionMessageConverter>(converters_CheckedItemsChanged);
    }

    private void converters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IItemToSolutionMessageConverter> e) {
      AddToDispatcher(e.Items);
    }

    private void converters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IItemToSolutionMessageConverter> e) {
      RemoveFromDispatcher(e.Items);
    }

    private void converters_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IItemToSolutionMessageConverter> e) {
      FillDispatcher();
    }

    private void FillDispatcher() {
      dispatcher = new Dictionary<Type, Action<IItem, string, SolutionMessage.Builder>>();
      foreach (IItemToSolutionMessageConverter c in converters.CheckedItems) {
        Type[] types = c.ItemTypes;
        foreach (Type t in types) {
          if (dispatcher.ContainsKey(t)) dispatcher.Remove(t);
          dispatcher.Add(t, new Action<IItem, string, SolutionMessage.Builder>(c.AddItemToBuilder));
        }
      }
    }

    private void AddToDispatcher(IEnumerable<IItemToSolutionMessageConverter> items) {
      if (dispatcher == null) FillDispatcher();
      foreach (IItemToSolutionMessageConverter c in items) {
        Type[] types = c.ItemTypes;
        foreach (Type t in types) {
          if (dispatcher.ContainsKey(t)) dispatcher.Remove(t);
          dispatcher.Add(t, new Action<IItem, string, SolutionMessage.Builder>(c.AddItemToBuilder));
        }
      }
    }

    private void RemoveFromDispatcher(IEnumerable<IItemToSolutionMessageConverter> items) {
      if (dispatcher == null) FillDispatcher();
      foreach (IItemToSolutionMessageConverter c in items) {
        Type[] types = c.ItemTypes;
        foreach (Type t in types) {
          if (dispatcher.ContainsKey(t)) dispatcher.Remove(t);
        }
      }
    }
  }
}
