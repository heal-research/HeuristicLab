#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("SolutionMessageBuilder", "Holds and uses a number of converters to translate HeuristicLab objects into appropriate fields of a solution message.")]
  [StorableType("8F406464-C1F6-4BBD-8791-C836846B473B")]
  public class SolutionMessageBuilder : NamedItem {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }
    private Dictionary<Type, Action<IItem, string, SolutionMessage.Builder>> dispatcher;

    [Storable]
    private CheckedItemList<IItemToSolutionMessageConverter> convertersList;
    public CheckedItemList<IItemToSolutionMessageConverter> Converters {
      get { return convertersList; }
    }

    [StorableConstructor]
    protected SolutionMessageBuilder(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected SolutionMessageBuilder(SolutionMessageBuilder original, Cloner cloner)
      : base(original, cloner) {
      convertersList = cloner.Clone(original.convertersList);
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SolutionMessageBuilder(this, cloner);
    }
    public SolutionMessageBuilder()
      : base() {
      name = ItemName;
      description = ItemDescription;
      convertersList = new CheckedItemList<IItemToSolutionMessageConverter>();
      convertersList.Add(new BoolConverter());
      convertersList.Add(new DateTimeValueConverter());
      convertersList.Add(new DoubleConverter());
      convertersList.Add(new IntegerConverter());
      convertersList.Add(new StringConverter());
      convertersList.Add(new TimeSpanValueConverter());

      RegisterEventHandlers();
    }

    public void AddToMessage(IItem item, string name, SolutionMessage.Builder builder) {
      if (dispatcher == null) BuildDispatcher();
      Type itemType = item.GetType();
      while (!dispatcher.ContainsKey(itemType)) {
        if (itemType.BaseType != null) itemType = itemType.BaseType;
        else break;
      }
      if (itemType.BaseType == null && !dispatcher.ContainsKey(itemType)) {
        IEnumerable<Type> interfaces = item.GetType().GetInterfaces().Where(x => dispatcher.ContainsKey(x));
        if (interfaces.Count() != 1) throw new ArgumentException(Name + ": No converter for type " + itemType.FullName + " defined.", "item");
        else itemType = interfaces.Single();
      }
      dispatcher[itemType](item, name, builder);
    }

    private void RegisterEventHandlers() {
      convertersList.ItemsAdded += convertersList_Changed;
      convertersList.ItemsRemoved += convertersList_Changed;
      convertersList.CheckedItemsChanged += convertersList_Changed;
      convertersList.ItemsMoved += convertersList_Changed;
      convertersList.ItemsReplaced += convertersList_Changed;
    }

    private void convertersList_Changed(object sender, CollectionItemsChangedEventArgs<IndexedItem<IItemToSolutionMessageConverter>> e) {
      BuildDispatcher();
    }

    private void BuildDispatcher() {
      dispatcher = new Dictionary<Type, Action<IItem, string, SolutionMessage.Builder>>();
      foreach (var c in convertersList.CheckedItems.OrderBy(x => x.Index).Select(x => x.Value)) {
        var types = c.ItemTypes;
        foreach (var t in types) {
          if (!dispatcher.ContainsKey(t))
            dispatcher.Add(t, c.AddItemToBuilder);
        }
      }
    }
  }
}
