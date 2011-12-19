#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting {
  class ORLIBParser {
    private StreamReader source;
    CultureInfo culture = new CultureInfo("en-US");
    
    private string name;
    public string Name {
      get {
        return name;
      }
    }

    private double[,] vertices;
    public double[,] Vertices {
      get { 
        return vertices; 
      }
    }

    private double capacity;
    public double Capacity {
      get {
        return capacity;
      }
    }

    private double maxRouteTime;
    public double MaxRouteTime {
      get {
        return maxRouteTime;
      }
    }

    private double serviceTime;
    public double ServiceTime {
      get {
        return serviceTime;
      }
    }

    private double[] demands;
    public double[] Demands {
      get {
        return demands;
      }
    }

    public ORLIBParser(String path) {
      source = null;
      
      name = path;
      vertices = null;
      capacity = -1;
      maxRouteTime = -1;
      serviceTime = -1;
      demands = null;
    }

    public void Parse() {
      using (source = new StreamReader(name)) {
        //Read header
        string line = source.ReadLine();
        string[] tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        int customers = int.Parse(tokens[0]);
        demands = new double[customers + 1];
        demands[0] = 0;
        vertices = new double[customers + 1, 2];

        capacity = int.Parse(tokens[1]);
        maxRouteTime = int.Parse(tokens[2]);
        serviceTime = int.Parse(tokens[3]);

        //Read depot
        line = source.ReadLine();
        tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        vertices[0, 0] = double.Parse(tokens[0], culture.NumberFormat);
        vertices[0, 1] = double.Parse(tokens[1], culture.NumberFormat);

        for (int i = 0; i < customers; i++) {
          line = source.ReadLine();
          tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
          vertices[i + 1, 0] = double.Parse(tokens[0], culture.NumberFormat);
          vertices[i + 1, 1] = double.Parse(tokens[1], culture.NumberFormat);
          demands[i + 1] = double.Parse(tokens[2], culture.NumberFormat);
        }
      }

      source = null;
    }
  }
}
