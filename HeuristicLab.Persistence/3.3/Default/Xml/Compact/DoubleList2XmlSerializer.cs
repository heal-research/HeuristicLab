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

using System.Collections;
using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml.Primitive;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  internal sealed class DoubleList2XmlSerializer : NumberEnumeration2XmlSerializerBase<List<double>> {

    protected override void Add(IEnumerable enumeration, object o) {
      ((List<double>)enumeration).Add((double)o);
    }

    protected override IEnumerable Instantiate() {
      return new List<double>();
    }

    protected override string FormatValue(object o) {
      return Double2XmlSerializer.FormatG17((double)o);
    }

    protected override object ParseValue(string o) {
      return Double2XmlSerializer.ParseG17(o);
    }

  }
}