#region License Information
/* HeuristicLab
 * Copyright (C) 2002-$year$ Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace $rootnamespace$ {
  [Item("$hlItemName$", "$hlItemDescription$")]
  [StorableClass]
  public class $safeitemname$ : Item {
    
    [StorableConstructor]
    protected $safeitemname$(bool deserializing) : base(deserializing) { }
    protected $safeitemname$($safeitemname$ original, Cloner cloner)
      : base(original, cloner) {
      // TODO: All fields, especially storable fields, need to be deep-cloned here
      // myField = cloner.Clone(original.myField);
    }
    public $safeitemname$()
      : base() {
      // TODO: Add construction logic here
    }
    
    public override IDeepCloneable Clone(Cloner cloner) {
      return new $safeitemname$(this, cloner);
    }
  }
}
