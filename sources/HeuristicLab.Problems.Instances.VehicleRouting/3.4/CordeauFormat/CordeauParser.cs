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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  class CordeauParser {
    private string file;
    private Stream stream;
    private string problemName;
    private int cities;
    private int depots;
    private int vehicles;
    private List<double> capacity;
    private List<double> xCoord;
    private List<double> yCoord;
    private List<double> demand;
    private List<double> readyTime;
    private List<double> dueTime;
    private List<double> serviceTime;

    public int Cities {
      get {
        return cities;
      }
    }

    public int Depots {
      get {
        return depots;
      }
    }

    public int Vehicles {
      get {
        return vehicles;
      }
    }

    public double[] Capacity {
      get {
        return capacity.ToArray();
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

    public String ProblemName {
      get {
        return problemName;
      }
    }

    public CordeauParser() {
      capacity = new List<double>();
      xCoord = new List<double>();
      yCoord = new List<double>();
      demand = new List<double>();
      readyTime = new List<double>();
      dueTime = new List<double>();
      serviceTime = new List<double>();
    }

     public CordeauParser(string file): this() {
      this.file = file;
    }

     public CordeauParser(Stream stream)
       : this() {
      this.stream = stream;
    }

     public void Parse() {
       string line;
       Regex reg = new Regex(@"-?\d+(\.\d+)?");
       MatchCollection m;

       StreamReader reader;
       if (stream != null) {
         reader = new StreamReader(stream);
       } else {
         reader = new StreamReader(file);
         problemName = Path.GetFileNameWithoutExtension(file);
       }

       using (reader) {
         List<double> depotXcoord = new List<double>();
         List<double> depotYcoord = new List<double>();
         List<double> depotReadyTime = new List<double>();
         List<double> depotDueTime = new List<double>();
         
         List<double> routeDueTime = new List<double>();

         line = reader.ReadLine();

         m = reg.Matches(line);
         if (m.Count != 4)
           throw new InvalidDataException("File has wrong format!");

         int type = int.Parse(m[0].Value);
         if(type != 2 && type != 6)
           throw new InvalidDataException("Unsupported instance type");

         bool timeWindows = type == 6;
         vehicles = int.Parse(m[1].Value);
         cities = int.Parse(m[2].Value);
         depots = int.Parse(m[3].Value);
         line = reader.ReadLine();

         for (int i = 0; i < depots; i++) {
           m = reg.Matches(line);
           if (m.Count != 2) { continue; }

           routeDueTime.Add(double.Parse(m[0].Value, System.Globalization.CultureInfo.InvariantCulture));
           capacity.Add(double.Parse(m[1].Value, System.Globalization.CultureInfo.InvariantCulture));

           line = reader.ReadLine();
         }

         while ((line != null)) {
           m = reg.Matches(line);

           if (demand.Count < cities) {
             xCoord.Add(double.Parse(m[1].Value, System.Globalization.CultureInfo.InvariantCulture));
             yCoord.Add(double.Parse(m[2].Value, System.Globalization.CultureInfo.InvariantCulture));
             demand.Add((double)int.Parse(m[4].Value, System.Globalization.CultureInfo.InvariantCulture));
             serviceTime.Add(int.Parse(m[3].Value));

             if (timeWindows) {
               readyTime.Add(int.Parse(m[m.Count - 2].Value));
               dueTime.Add(int.Parse(m[m.Count - 1].Value));
             } else {
               readyTime.Add(0);
               dueTime.Add(double.MaxValue);
             }
           } else {
             depotXcoord.Add(double.Parse(m[1].Value, System.Globalization.CultureInfo.InvariantCulture));
             depotYcoord.Add(double.Parse(m[2].Value, System.Globalization.CultureInfo.InvariantCulture));

             if (timeWindows) {
               depotReadyTime.Add(int.Parse(m[m.Count - 2].Value));
               depotDueTime.Add(int.Parse(m[m.Count - 1].Value));
             } else {
               depotReadyTime.Add(0);
               depotDueTime.Add(double.MaxValue);
             }
           }

           line = reader.ReadLine();
         }

         for (int i = 0; i < depotDueTime.Count; i++) {
           if (!timeWindows) {
             depotDueTime[i] = routeDueTime[i];
           }
           if (depotDueTime[i] < double.Epsilon)
             depotDueTime[i] = double.MaxValue;
         }

         xCoord.InsertRange(0, depotXcoord);
         yCoord.InsertRange(0, depotYcoord);
         readyTime.InsertRange(0, depotReadyTime);
         dueTime.InsertRange(0, depotDueTime);

         List<double> originalCapacities = new List<double>(capacity);
         capacity.Clear();
         for (int i = 0; i < depots; i++) {
           for (int j = 0; j < vehicles; j++) {
             capacity.Add(originalCapacities[i]);
           }
         }
         vehicles *= depots;
       }
     }
  }
}
