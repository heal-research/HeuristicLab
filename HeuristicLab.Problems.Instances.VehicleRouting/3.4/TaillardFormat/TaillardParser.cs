#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  class TaillardParser {
    private string file;
    private Stream stream;
    private string problemName;
    private int cities;
    private double capacity;
    private List<double> xCoord;
    private List<double> yCoord;
    private List<double> demand;

    public int Cities {
      get {
        return cities;
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

    public String ProblemName {
      get {
        return problemName;
      }
    }

    public TaillardParser() {
      xCoord = new List<double>();
      yCoord = new List<double>();
      demand = new List<double>();
    }

    public TaillardParser(string file)
      : this() {
      this.file = file;
    }

    public TaillardParser(Stream stream)
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
        line = reader.ReadLine();
        m = reg.Matches(line);
        if (m.Count != 2)
          throw new InvalidDataException("File has wrong format!");

        cities = int.Parse(m[0].Value);

        line = reader.ReadLine();
        m = reg.Matches(line);
        capacity = int.Parse(m[0].Value);

        line = reader.ReadLine();
        m = reg.Matches(line);
        xCoord.Add(double.Parse(m[0].Value, System.Globalization.CultureInfo.InvariantCulture));
        yCoord.Add(double.Parse(m[1].Value, System.Globalization.CultureInfo.InvariantCulture));
        demand.Add(0);

        for (int i = 0; i < cities; i++) {
          line = reader.ReadLine();
          m = reg.Matches(line);
          xCoord.Add(double.Parse(m[1].Value, System.Globalization.CultureInfo.InvariantCulture));
          yCoord.Add(double.Parse(m[2].Value, System.Globalization.CultureInfo.InvariantCulture));
          demand.Add(int.Parse(m[3].Value));
        }
      }
    }
  }
}
