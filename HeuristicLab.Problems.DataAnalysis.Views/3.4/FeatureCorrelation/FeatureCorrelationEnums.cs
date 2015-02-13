#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HeuristicLab.Data;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  // see http://blog.spontaneouspublicity.com/associating-strings-with-enums-in-c
  [NonDiscoverableType]
  public class FeatureCorrelationEnums {
    public enum Partitions {
      [Description("All Samples")]
      AllSamples,
      [Description("Training Samples")]
      TrainingSamples,
      [Description("Test Samples")]
      TestSamples
    }

    public enum CorrelationCalculators {
      [Description("Pearsons R")]
      PearsonsR,
      [Description("Pearsons R Squared")]
      PearsonsRSquared,
      [Description("Hoeffdings Dependence")]
      HoeffdingsDependence,
      [Description("Spearmans Rank")]
      SpearmansRank
    }

    public static readonly Dictionary<CorrelationCalculators, DoubleRange> calculatorInterval = new Dictionary<CorrelationCalculators, DoubleRange>(){ 
      { CorrelationCalculators.PearsonsR, new DoubleRange(1.0, -1.0)},
      { CorrelationCalculators.SpearmansRank, new DoubleRange(1.0, -1.0)},
      { CorrelationCalculators.HoeffdingsDependence, new DoubleRange(1.0, -0.5)},
      { CorrelationCalculators.PearsonsRSquared, new DoubleRange(1.0, 0.0)}
    };

    public static string GetEnumDescription(object value) {
      FieldInfo fi = value.GetType().GetField(value.ToString());

      DescriptionAttribute[] attributes =
          (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

      if (attributes != null && attributes.Length > 0)
        return attributes[0].Description;
      else
        return value.ToString();
    }

    public static IEnumerable<T> EnumToList<T>() {
      Type enumType = typeof(T);

      // Can't use generic type constraints on value types,
      // so have to do check like this
      if (enumType.BaseType != typeof(Enum)) {
        throw new ArgumentException("T must be of type System.Enum");
      }

      Array enumValArray = Enum.GetValues(enumType);
      return Enum.GetValues(enumType).Cast<int>().Select(x => (T)Enum.Parse(enumType, x.ToString())).ToList();
    }

    public static T GetEnumOfDescription<T>(string desc) {
      Type enumType = typeof(T);

      if (enumType.BaseType != typeof(Enum))
        throw new ArgumentException("T must be of type System.Enum");

      Array enumValArray = Enum.GetValues(enumType);
      foreach (int val in enumValArray) {

        T e = (T)Enum.Parse(enumType, val.ToString());
        if (GetEnumDescription(e).Equals(desc)) {
          return (T)e;
        }
      }
      throw new ArgumentException("Description is not in the given type T");
    }
  }
}
