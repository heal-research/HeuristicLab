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
using System.Globalization;
using System.IO;

namespace HeuristicLab.Problems.Instances.CordeauGQAP {
  public class CordeauGQAPParser {
    public int Equipments { get; private set; }
    public int Locations { get; private set; }
    public double TransportationCosts { get; private set; }
    public double BestKnownQuality { get; private set; }
    public double[,] Weights { get; private set; }
    public double[,] Distances { get; private set; }
    public double[,] InstallationCosts { get; private set; }
    public double[] Demands { get; private set; }
    public double[] Capacities { get; private set; }

    public CordeauGQAPParser() {
      Reset();
    }

    public void Reset() {
      Equipments = 0;
      Locations = 0;
      Weights = null;
      Distances = null;
      InstallationCosts = null;
      Demands = null;
      Capacities = null;
    }

    public void Parse(string file) {
      using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the Cordeau's GQAP format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    public void Parse(Stream stream) {
      var separator = new char[] { ' ', '\t' };
      var reader = new StreamReader(stream);
      string line = Continue(reader);

      var info = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      Equipments = int.Parse(info[0]);
      Locations = int.Parse(info[1]);
      TransportationCosts = double.Parse(info[2]);

      line = Continue(reader);
      double bkq;
      if (!double.TryParse(line, out bkq))
        BestKnownQuality = double.NaN;
      else {
        BestKnownQuality = bkq;
        line = Continue(reader);
      }

      Weights = new double[Equipments, Equipments];
      for (int i = 0; i < Equipments; i++) {
        var weightsPerEquipment = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < Equipments; j++) {
          Weights[i, j] = double.Parse(weightsPerEquipment[j], CultureInfo.InvariantCulture.NumberFormat);
        }
        line = Continue(reader);
      }

      Distances = new double[Locations, Locations];
      for (int i = 0; i < Locations; i++) {
        var distancePerLocation = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < Locations; j++) {
          Distances[i, j] = double.Parse(distancePerLocation[j], CultureInfo.InvariantCulture.NumberFormat);
        }
        line = Continue(reader);
      }

      InstallationCosts = new double[Equipments, Locations];
      for (int i = 0; i < Equipments; i++) {
        var costsPerEquipment = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < Locations; j++)
          InstallationCosts[i, j] = double.Parse(costsPerEquipment[j], CultureInfo.InvariantCulture.NumberFormat);
        line = Continue(reader);
      }

      Demands = new double[Equipments];
      var demandsPerEquipment = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < Equipments; i++)
        Demands[i] = double.Parse(demandsPerEquipment[i], CultureInfo.InvariantCulture.NumberFormat);

      line = Continue(reader);

      Capacities = new double[Locations];
      string[] capacityPerLocation = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < Locations; i++)
        Capacities[i] = double.Parse(capacityPerLocation[i], CultureInfo.InvariantCulture.NumberFormat);
    }

    private static string Continue(StreamReader reader) {
      string line = String.Empty;
      while (String.IsNullOrEmpty(line) && !reader.EndOfStream) {
        line = reader.ReadLine();
      }
      return line;
    }
  }
}
