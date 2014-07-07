#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Text.RegularExpressions;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  class LiLimParser {
    private string file;
    private Stream stream;
    private string problemName;
    private int cities;
    private int vehicles;
    private double capacity;
    private List<double> xCoord;
    private List<double> yCoord;
    private List<double> demand;
    private List<double> readyTime;
    private List<double> dueTime;
    private List<double> serviceTime;
    private List<int> pickipDeliveryLocation;

    public int Cities {
      get {
        return cities;
      }
    }

    public int Vehicles {
      get {
        return vehicles;
      }
    }

    public double Capacity {
      get {
        return capacity;
      }
    }

    public double[,] Coordinates {
      get {
        double[] x = xCoord.ToArray();
        double[] y = yCoord.ToArray();
        double[,] coord = new double[x.Length, 2];
        for (int i = 0; i < x.Length; i++) {
          coord[i, 0] = x[i];
          coord[i, 1] = y[i];
        }
        return coord;
      }
    }

    public double[] Demands {
      get {
        return demand.ToArray();
      }
    }

    public double[] Readytimes {
      get {
        return readyTime.ToArray();
      }
    }

    public double[] Duetimes {
      get {
        return dueTime.ToArray();
      }
    }

    public double[] Servicetimes {
      get {
        return serviceTime.ToArray();
      }
    }

    public int[] PickupDeliveryLocations {
      get {
        return pickipDeliveryLocation.ToArray();
      }
    }

    public String ProblemName {
      get {
        return problemName;
      }
    }

    public LiLimParser() {
      xCoord = new List<double>();
      yCoord = new List<double>();
      demand = new List<double>();
      readyTime = new List<double>();
      dueTime = new List<double>();
      serviceTime = new List<double>();
      pickipDeliveryLocation = new List<int>();
    }

    public LiLimParser(string file)
      : this() {
      this.file = file;
    }

    public LiLimParser(Stream stream)
      : this() {
      this.stream = stream;
    }

    public void Parse() {
      string line;
      Regex reg = new Regex(@"-?\d+");
      MatchCollection m;

      StreamReader reader;
      if (stream != null) {
        reader = new StreamReader(stream);
      } else {
        reader = new StreamReader(file);
        problemName = Path.GetFileNameWithoutExtension(file);
      }

      using (reader) {
        line = reader.ReadLine();

        m = reg.Matches(line);
        if (m.Count != 3)
          throw new InvalidDataException("File has wrong format!");

        vehicles = int.Parse(m[0].Value);
        capacity = double.Parse(m[1].Value);

        line = reader.ReadLine();
        while ((line != null) && (line.Length > 5)) {
          m = reg.Matches(line);
          if (m.Count != 9) { continue; }
          xCoord.Add((double)int.Parse(m[1].Value));
          yCoord.Add((double)int.Parse(m[2].Value));
          demand.Add((double)int.Parse(m[3].Value));
          readyTime.Add((double)int.Parse(m[4].Value));
          double st = (double)int.Parse(m[6].Value);
          dueTime.Add((double)int.Parse(m[5].Value));
          serviceTime.Add(st);

          int location = int.Parse(m[7].Value);
          if (location == 0)
            location = int.Parse(m[8].Value);
          pickipDeliveryLocation.Add(location);

          line = reader.ReadLine();
        }
        cities = serviceTime.Count;
      }
    }
  }
}
