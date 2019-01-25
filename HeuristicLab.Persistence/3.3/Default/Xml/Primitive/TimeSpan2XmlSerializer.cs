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

using System;
using HEAL.Attic;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class TimeSpan2XmlSerializer : PrimitiveXmlSerializerBase<TimeSpan> {

    public override XmlString Format(TimeSpan o) {
      return new XmlString(o.ToString());
    }

    public override TimeSpan Parse(XmlString t) {
      try {
        return TimeSpan.Parse(t.Data);
      }
      catch (FormatException x) {
        throw new PersistenceException("Cannot parse TimeSpan string representation.", x);
      }
      catch (OverflowException x) {
        throw new PersistenceException("Overflow during TimeSpan parsing.", x);
      }
    }
  }
}