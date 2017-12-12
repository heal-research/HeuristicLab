#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting {
  class SolutionParser {
    private string file;

    private List<List<int>> routes;

    public List<List<int>> Routes {
      get {
        return routes;
      }
    }

    public SolutionParser(string file) {
      this.file = file;
      this.routes = new List<List<int>>();
    }

    public void Parse() {
      this.routes.Clear();
      using (StreamReader reader = new StreamReader(file)) {
        String line;
        while ((line = reader.ReadLine()) != null) {
          string[] token = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

          if (token.Length > 2) {
            List<int> route = new List<int>();

            for (int i = 2; i < token.Length; i++) {
              route.Add(int.Parse(token[i]));
            }

            routes.Add(route);
          }
        }
      }
    }
  }
}
