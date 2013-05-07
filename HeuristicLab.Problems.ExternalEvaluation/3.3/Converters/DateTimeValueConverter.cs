#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("DateTimeValueConverter", "Converts a DateTimeValue and adds it to the SolutionMessage's StringVars. The format is yyyy-MM-dd HH:mm:sszzz, e.g. 2010-05-31 19:15:33+01:00.")]
  [StorableClass]
  public class DateTimeValueConverter : Item, IItemToSolutionMessageConverter {
    private static readonly Type[] itemTypes = new Type[] { typeof(DateTimeValue) };
    [StorableConstructor]
    protected DateTimeValueConverter(bool deserializing) : base(deserializing) { }
    protected DateTimeValueConverter(DateTimeValueConverter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DateTimeValueConverter(this, cloner);
    }
    public DateTimeValueConverter() : base() { }

    #region IItemToSolutionMessageConverter Members

    public Type[] ItemTypes {
      get { return itemTypes; }
    }

    public void AddItemToBuilder(IItem item, string name, SolutionMessage.Builder builder) {
      DateTimeValue date = (item as DateTimeValue);
      if (date != null) {
        SolutionMessage.Types.StringVariable.Builder var = SolutionMessage.Types.StringVariable.CreateBuilder();
        var.SetName(name).SetData(date.Value.ToString("yyyy-MM-dd HH:mm:sszzz"));
        builder.AddStringVars(var.Build());
      } else {
        throw new ArgumentException(ItemName + ": Item is not of a supported type.", "item");
      }
    }

    #endregion

  }
}
