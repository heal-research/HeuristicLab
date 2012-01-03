#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public class EventArgs<T> : EventArgs {
    private T myValue;
    public T Value {
      get { return myValue; }
    }

    public EventArgs(T value) {
      myValue = value;
    }
  }

  public class EventArgs<T, U> : EventArgs<T> {
    private U myValue2;
    public U Value2 {
      get { return myValue2; }
    }

    public EventArgs(T value, U value2)
      : base(value) {
      myValue2 = value2;
    }
  }
}
