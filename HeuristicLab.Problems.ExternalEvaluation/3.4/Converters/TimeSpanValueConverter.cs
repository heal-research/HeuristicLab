﻿#region License Information
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("TimeSpanValueConverter", "Converts a TimeSpanValue and adds it to the SolutionMessage's StringVars. The format is d.hh:mm:ss, e.g. 1113.10:55:00 (1113 days 10 hours, 55 minutes, 0 seconds).")]
  [StorableType("4617C8B5-89CF-42B5-82B5-C57E9B924C0F")]
  public class TimeSpanValueConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(TimeSpanValue) };

    [StorableConstructor]
    protected TimeSpanValueConverter(StorableConstructorFlag _) : base(_) { }
    protected TimeSpanValueConverter(TimeSpanValueConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeSpanValueConverter(this, cloner);
    }
    public TimeSpanValueConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage builder) {
      DateTimeValue date = (item as DateTimeValue);
      if (date != null) {
        SolutionMessage.Types.StringVariable var = new SolutionMessage.Types.StringVariable();
        var.Name = name;
        var.Data = date.Value.ToString(@"d\.hh\:mm\:ss");
        builder.StringVars.Add(var);
      } else {
        throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
      }
    }

    #endregion

  }
}
