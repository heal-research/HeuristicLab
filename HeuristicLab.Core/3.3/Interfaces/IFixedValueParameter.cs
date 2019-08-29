using HEAL.Attic;
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

namespace HeuristicLab.Core {
  [StorableType("f4b80054-f730-44c0-a0b9-35ab27cf29a2")]
  public interface IFixedValueParameter : IValueParameter {
    new IItem Value { get; }
    new IItem ActualValue { get; }
  }

  [StorableType("93b6ff11-134d-4486-b9bf-9c6802ef3885")]
  public interface IFixedValueParameter<T> : IFixedValueParameter, IValueParameter<T> where T : class, IItem {
    new T Value { get; }
  }
}
