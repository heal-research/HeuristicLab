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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class CordeauFormatInstanceProvider<TData> : VRPInstanceProvider<TData> where TData : MDCVRPData {
    protected override TData LoadData(Stream stream) {
      return LoadInstance(new CordeauParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override TData ImportData(string path) {
      return LoadInstance(new CordeauParser(path));
    }

    internal abstract TData LoadInstance(CordeauParser parser);

    protected override void LoadSolution(Stream stream, TData instance) {
      using (var reader = new StreamReader(stream)) {
        double costs = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);

        var toursPerDepotQuery =
          from line in reader.ReadAllLines()
          where !string.IsNullOrEmpty(line)
          let tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
          let depot = int.Parse(tokens[0])
          //let vehicle = int.Parse(tokens[1])
          //let duration = double.Parse(tokens[2], CultureInfo.InvariantCulture)
          //let load = double.Parse(tokens[3], new CultureInfo.InvariantCulture)
          let customers = tokens.Skip(4).Where(t => !t.StartsWith("(")).Select(int.Parse)
          let numberOfCustomers = customers.Count()
          //let serviceTimes = tokens.Skip(5).Where(t => t.StartsWith("(")).Select(t => double.Parse(t.Trim('(', ')'), CultureInfo.InvariantCulture))
          let stops = customers.Skip(1).Take(numberOfCustomers - 2).Select(s => s - 1)
          select new { depot, /*vehicle,*/ stops } into assignment
          group assignment by assignment.depot;

        var toursPerDepot = toursPerDepotQuery.Select(d => d.Select(v => v.stops.ToArray()).ToArray()).ToArray();

        instance.BestKnownTour = toursPerDepot.SelectMany(depot => depot).ToArray();

        instance.BestKnownTourVehicleAssignment = DetermineTourToVehicleAssignment(toursPerDepot, instance.VehicleDepotAssignment);
      }
    }

    private static int[] DetermineTourToVehicleAssignment(int[][][] toursPerDepot, int[] vehicleDepotAssignments) {
      var usedVehiclesPerDepot = toursPerDepot.Select((d, i) => new { i, d }).ToDictionary(k => k.i, v => v.d.Length);
      var availableVehiclesPerDepot = vehicleDepotAssignments.GroupBy(a => a).ToDictionary(k => k.Key, v => v.Count());

      var tourToVehicle = new List<int>(vehicleDepotAssignments.Length);
      var unusedVehicles = new List<int>();

      int vehicle = 0;
      for (int depot = 0; depot < toursPerDepot.Length; depot++) {
        int used = usedVehiclesPerDepot[depot];
        tourToVehicle.AddRange(Enumerable.Range(vehicle, used));
        vehicle += used;

        int unused = availableVehiclesPerDepot[depot] - used;
        unusedVehicles.AddRange(Enumerable.Range(vehicle, unused));
        vehicle += unused;
      }
      tourToVehicle.AddRange(unusedVehicles);

      return tourToVehicle.ToArray();
    }
  }
}
