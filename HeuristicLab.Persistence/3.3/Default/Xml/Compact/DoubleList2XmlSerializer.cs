#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml.Primitive;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  internal sealed class DoubleList2XmlSerializer : CompactXmlSerializerBase<List<double>> {

    public override XmlString Format(List<double> list) {
      StringBuilder sb = new StringBuilder(list.Count * 3);
      foreach (var d in list) {
        sb.Append(Double2XmlSerializer.FormatG17(d)).Append(';');
      }
      return new XmlString(sb.ToString());
    }

    public override List<double> Parse(XmlString data) {
      try {
        List<double> list = new List<double>();
        foreach (var value in data.Data.EnumerateSplit(';')) {
          list.Add(Double2XmlSerializer.ParseG17(value));
        }
        return list;
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data during reconstruction of List<double>.", e);
      }
      catch (OverflowException e) {
        throw new PersistenceException("Overflow during element parsing while trying to reconstruct List<double>.", e);
      }
    }
  }
}