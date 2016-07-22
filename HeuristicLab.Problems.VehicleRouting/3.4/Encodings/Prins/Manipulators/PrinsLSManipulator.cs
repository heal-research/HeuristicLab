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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsLSManipulator", "An operator which manipulates a VRP representation by using the Prins local search.  It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableClass]
  public abstract class PrinsLSManipulator : PrinsManipulator, IVRPLocalSearchManipulator {
    public IValueParameter<IntValue> Iterations {
      get { return (IValueParameter<IntValue>)Parameters["Iterations"]; }
    }

    [StorableConstructor]
    protected PrinsLSManipulator(bool deserializing) : base(deserializing) { }

    public PrinsLSManipulator()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Iterations", "The number of max iterations.", new IntValue(5)));
    }

    protected PrinsLSManipulator(PrinsLSManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected double GetQuality(PrinsEncoding individual) {
      return ProblemInstance.Evaluate(individual).Quality;
    }

    private int FindCity(PrinsEncoding individual, int city) {
      int index = -1;

      int i = 0;
      while (i < individual.Length && index == -1) {
        if (individual[i] == city)
          index = i;

        i++;
      }

      return index;
    }

    protected const int depot = -1;

    private Tour FindTour(PrinsEncoding individual, int city) {
      Tour found = null;

      List<Tour> tours = individual.GetTours();
      int i = 0;

      while (found == null && i < tours.Count) {
        if (tours[i].Stops.Contains(city))
          found = tours[i];

        i++;
      }

      return found;
    }

    //inserts u after v in the child
    private void InsertAfter(int u, int v, PrinsEncoding parent, PrinsEncoding child) {
      int pi = 0;
      int ci = 0;

      while (ci != child.Length) {
        if (parent[pi] != u) {
          child[ci] = parent[pi];
          ci++;
        }
        if (parent[pi] == v) {
          child[ci] = u;
          ci++;
        }

        pi++;
      }
    }

    //inserts (u, x) after v in the child
    private void InsertAfter(int u, int x, int v, PrinsEncoding parent, PrinsEncoding child) {
      int pi = 0;
      int ci = 0;

      while (ci != child.Length) {
        if (parent[pi] != u && parent[pi] != x) {
          child[ci] = parent[pi];
          ci++;
        }
        if (parent[pi] == v) {
          child[ci] = u;
          ci++;

          child[ci] = x;
          ci++;
        }

        pi++;
      }
    }

    //inserts u before v in the child
    private void InsertBefore(int u, int v, PrinsEncoding parent, PrinsEncoding child) {
      int pi = 0;
      int ci = 0;

      while (ci != child.Length) {
        if (parent[pi] == v) {
          child[ci] = u;
          ci++;
        }
        if (parent[pi] != u) {
          child[ci] = parent[pi];
          ci++;
        }

        pi++;
      }
    }

    //inserts (u, x) before v in the child
    private void InsertBefore(int u, int x, int v, PrinsEncoding parent, PrinsEncoding child) {
      int pi = 0;
      int ci = 0;

      while (ci != child.Length) {
        if (parent[pi] == v) {
          child[ci] = u;
          ci++;

          child[ci] = x;
          ci++;
        }
        if (parent[pi] != u && parent[pi] != x) {
          child[ci] = parent[pi];
          ci++;
        }

        pi++;
      }
    }

    //swaps u and v
    private void Swap(int u, int v, PrinsEncoding parent, PrinsEncoding child) {
      for (int i = 0; i < child.Length; i++) {
        if (parent[i] == u)
          child[i] = v;
        else if (parent[i] == v)
          child[i] = u;
        else
          child[i] = parent[i];
      }
    }

    //swaps (u, x) and v
    private void Swap(int u, int x, int v, PrinsEncoding parent, PrinsEncoding child) {
      int childIndex = 0;
      int parentIndex = 0;

      while (childIndex < child.Length) {
        if (parent[parentIndex] == u) {
          child[childIndex] = v;
          parentIndex++;
        } else if (parent[parentIndex] == v) {
          child[childIndex] = u;
          childIndex++;
          child[childIndex] = x;
        } else {
          child[childIndex] = parent[parentIndex];
        }

        childIndex++;
        parentIndex++;
      }
    }

    //swaps (u, x) and (v, y)
    private void Swap(int u, int x, int v, int y, PrinsEncoding parent, PrinsEncoding child) {
      int i = 0;

      while (i < child.Length) {
        if (child[i] == u) {
          child[i] = v;
          i++;
          child[i] = y;
        } else if (child[i] == v) {
          child[i] = u;
          i++;
          child[i] = x;
        } else {
          child[i] = parent[i];
        }

        i++;
      }
    }

    //swaps (u, x) and (v, y) by (u, y) and (x, v)
    private void Swap2(int u, int x, int v, int y, PrinsEncoding parent, PrinsEncoding child) {
      int i = 0;

      while (i < child.Length) {
        if (parent[i] == x) {
          child[i] = y;
        } else if (parent[i] == v) {
          child[i] = x;
        } else if (parent[i] == y) {
          child[i] = v;
        } else {
          child[i] = parent[i];
        }

        i++;
      }
    }

    private void M1(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot) {
        if (v == depot) {
          Tour tour = FindTour(child, u + 1);
          v = tour.Stops[0] - 1;
          InsertBefore(u, v, parent, child);
        } else {
          InsertAfter(u, v, parent, child);
        }
      }
    }

    private void M2(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot) {
        Tour tour = FindTour(child, u + 1);
        int iu = tour.Stops.IndexOf(u + 1);
        if (iu < tour.Stops.Count - 1) {
          int x = tour.Stops[iu + 1] - 1;

          if (v == depot) {
            tour = FindTour(child, u + 1);
            v = tour.Stops[0] - 1;
            InsertBefore(u, x, v, parent, child);
          } else {
            InsertAfter(u, x, v, parent, child);
          }
        }
      }
    }

    private void M3(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot) {
        Tour tour = FindTour(child, u + 1);
        int iu = tour.Stops.IndexOf(u + 1);
        if (iu < tour.Stops.Count - 1) {
          int x = tour.Stops[iu + 1] - 1;

          if (v == depot) {
            tour = FindTour(child, u + 1);
            v = tour.Stops[0] - 1;
            InsertBefore(x, u, v, parent, child);
          } else {
            InsertAfter(x, u, v, parent, child);
          }
        }
      }
    }

    private void M4(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Swap(u, v, parent, child);
      }
    }

    private void M5(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Tour tour = FindTour(child, u + 1);
        int iu = tour.Stops.IndexOf(u + 1);
        if (iu < tour.Stops.Count - 1) {
          int x = tour.Stops[iu + 1] - 1;

          if (x != v)
            Swap(u, x, v, parent, child);
        }
      }
    }

    private void M6(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Tour tour = FindTour(child, u + 1);
        int iu = tour.Stops.IndexOf(u + 1);
        if (iu < tour.Stops.Count - 1) {
          int x = tour.Stops[iu + 1] - 1;

          tour = FindTour(child, v + 1);
          int iv = tour.Stops.IndexOf(v + 1);
          if (iv < tour.Stops.Count - 1) {
            int y = tour.Stops[iv + 1] - 1;

            if (x != v && y != u)
              Swap(u, x, v, y, parent, child);
          }
        }
      }
    }

    private void M7(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Tour tu = FindTour(child, u + 1);
        Tour tv = FindTour(child, v + 1);

        if (tu.Stops[0] == tv.Stops[0]) {
          int iu = tu.Stops.IndexOf(u + 1);
          if (iu < tu.Stops.Count - 1) {
            int x = tu.Stops[iu + 1] - 1;

            int iv = tv.Stops.IndexOf(v + 1);
            if (iv < tv.Stops.Count - 1) {
              int y = tv.Stops[iv + 1] - 1;

              if (x != v && y != u)
                Swap(x, v, parent, child);
            }
          }
        }
      }
    }

    private void M8(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Tour tu = FindTour(child, u + 1);
        Tour tv = FindTour(child, v + 1);

        if (tu.Stops[0] != tv.Stops[0]) {
          int iu = tu.Stops.IndexOf(u + 1);
          if (iu < tu.Stops.Count - 1) {
            int x = tu.Stops[iu + 1] - 1;

            int iv = tv.Stops.IndexOf(v + 1);
            if (iv < tv.Stops.Count - 1) {
              int y = tv.Stops[iv + 1] - 1;

              if (x != v && y != u)
                Swap(x, v, parent, child);
            }
          }
        }
      }
    }

    private void M9(PrinsEncoding parent, PrinsEncoding child, int u, int v) {
      if (u != depot && v != depot) {
        Tour tu = FindTour(child, u + 1);
        Tour tv = FindTour(child, v + 1);

        if (tu.Stops[0] != tv.Stops[0]) {
          int iu = tu.Stops.IndexOf(u + 1);
          if (iu < tu.Stops.Count - 1) {
            int x = tu.Stops[iu + 1] - 1;

            int iv = tv.Stops.IndexOf(v + 1);
            if (iv < tv.Stops.Count - 1) {
              int y = tv.Stops[iv + 1] - 1;

              if (x != v && y != u)
                Swap2(u, x, v, y, parent, child);
            }
          }
        }
      }
    }

    protected PrinsEncoding Manipulate(PrinsEncoding individual,
      double originalQuality, int u, int v) {
      PrinsEncoding child = null;
      bool improvement = false;

      if (u != v) {
        child = individual.Clone() as PrinsEncoding;
        M1(individual, child, u, v);
        improvement = GetQuality(child) < originalQuality;

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M2(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M3(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M4(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M5(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M6(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M7(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M8(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }

        if (!improvement) {
          child = individual.Clone() as PrinsEncoding;
          M9(individual, child, u, v);
          improvement = GetQuality(child) < originalQuality;
        }
      }

      if (improvement)
        return child;
      else
        return null;
    }
  }
}
