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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Instances {
  public class ProblemInstanceManager {
    public static IEnumerable<IProblemInstanceProvider> GetProviders(object item) {
      var consumerTypes = item.GetType().GetInterfaces()
        .Where(x => x.IsGenericType
          && x.GetGenericTypeDefinition() == typeof(IProblemInstanceConsumer<>));

      if (consumerTypes.Any()) {
        var instanceTypes = consumerTypes
          .Select(x => x.GetGenericArguments().First())
          .Select(x => typeof(IProblemInstanceProvider<>).MakeGenericType(x));

        var interfaceTypes = instanceTypes.Where(x => x.GetGenericArguments().First().IsInterface);
        if (interfaceTypes.Any()) {
          var concreteTypes = interfaceTypes.SelectMany(x => ApplicationManager.Manager.GetTypes(x.GetGenericArguments().First()));

          if (concreteTypes.Any())
            instanceTypes = instanceTypes.Union(concreteTypes.Select(x => typeof(IProblemInstanceProvider<>).MakeGenericType(x))).Distinct();
        }

        foreach (var type in instanceTypes) {
          foreach (var provider in ApplicationManager.Manager.GetInstances(type))
            yield return (IProblemInstanceProvider)provider;
        }
      }
    }

    public static IEnumerable<IDataDescriptor> GetDataDescriptors(IProblemInstanceProvider provider) {
      IEnumerable<IDataDescriptor> descriptors = ((dynamic)provider).GetDataDescriptors();
      return descriptors;
    }

    public static void LoadData(IProblemInstanceProvider provider, IDataDescriptor descriptor, IProblemInstanceConsumer consumer) {
      ((dynamic)consumer).Load(((dynamic)provider).LoadData(descriptor));
    }
  }
}
