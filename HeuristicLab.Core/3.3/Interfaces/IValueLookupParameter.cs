using HEAL.Attic;
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [StorableType("d125528a-dfaa-46b1-9a5f-9a6443b7229c")]
  public interface IValueLookupParameter : IValueParameter, ILookupParameter { }

  [StorableType("ed351d54-ee70-4f9f-8187-7507dcaeb919")]
  public interface IValueLookupParameter<T> : IValueLookupParameter, IValueParameter<T>, ILookupParameter<T> where T : class, IItem { }
}
