#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using Google.OrTools.LinearSolver;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  public static class LinearSolverExtensions {

    public static bool[] GetSolutionBoolVarArray(this Solver solver, int count, string name) {
      var array = new bool[count];
      for (var d1 = 0; d1 < count; d1++) {
        var varName = $"{name}{d1}";
        array[d1] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
      }

      return array;
    }

    public static bool[,] GetSolutionBoolVarArray(this Solver solver, int rows, int cols, string name) {
      var array = new bool[rows, cols];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          var varName = $"{name}[{d1}, {d2}]";
          array[d1, d2] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
        }
      }

      return array;
    }

    public static bool[,,] GetSolutionBoolVarArray(this Solver solver, int rows, int cols, int dim3, string name) {
      var array = new bool[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
          }
        }
      }

      return array;
    }

    public static bool[,,,] GetSolutionBoolVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, string name) {
      var array = new bool[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
            }
          }
        }
      }

      return array;
    }

    public static bool[,,,,] GetSolutionBoolVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, int dim5, string name) {
      var array = new bool[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
              }
            }
          }
        }
      }

      return array;
    }

    public static bool[,,,,,] GetSolutionBoolVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, int dim5, int dim6, string name) {
      var array = new bool[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = (int)solver.LookupVariableOrNull(varName).SolutionValue() != 0;
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static bool[,] GetSolutionBoolVarMatrix(this Solver solver, int rows, int cols, string name) =>
      solver.GetSolutionBoolVarArray(rows, cols, name);

    public static int[] GetSolutionIntVarArray(this Solver solver, int count, string name) {
      var array = new int[count];
      for (var d1 = 0; d1 < count; d1++) {
        var varName = $"{name}{d1}";
        array[d1] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
      }

      return array;
    }

    public static int[,] GetSolutionIntVarArray(this Solver solver, int rows, int cols, string name) {
      var array = new int[rows, cols];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          var varName = $"{name}[{d1}, {d2}]";
          array[d1, d2] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
        }
      }

      return array;
    }

    public static int[,,] GetSolutionIntVarArray(this Solver solver, int rows, int cols, int dim3, string name) {
      var array = new int[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
          }
        }
      }

      return array;
    }

    public static int[,,,] GetSolutionIntVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, string name) {
      var array = new int[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
            }
          }
        }
      }

      return array;
    }

    public static int[,,,,] GetSolutionIntVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, int dim5, string name) {
      var array = new int[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
              }
            }
          }
        }
      }

      return array;
    }

    public static int[,,,,,] GetSolutionIntVarArray(this Solver solver,
        int rows, int cols, int dim3, int dim4, int dim5, int dim6, string name) {
      var array = new int[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = (int)solver.LookupVariableOrNull(varName).SolutionValue();
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static int[,] GetSolutionIntVarMatrix(this Solver solver, int rows, int cols, string name) =>
      solver.GetSolutionIntVarArray(rows, cols, name);

    public static double[] GetSolutionNumVarArray(this Solver solver, int count, string name) {
      var array = new double[count];
      for (var d1 = 0; d1 < count; d1++) {
        var varName = $"{name}{d1}";
        array[d1] = solver.LookupVariableOrNull(varName).SolutionValue();
      }

      return array;
    }

    public static double[,] GetSolutionNumVarArray(this Solver solver, int rows, int cols, string name) {
      var array = new double[rows, cols];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          var varName = $"{name}[{d1}, {d2}]";
          array[d1, d2] = solver.LookupVariableOrNull(varName).SolutionValue();
        }
      }

      return array;
    }

    public static double[,,] GetSolutionNumVarArray(this Solver solver, int rows, int cols, int dim3, string name) {
      var array = new double[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = solver.LookupVariableOrNull(varName).SolutionValue();
          }
        }
      }

      return array;
    }

    public static double[,,,] GetSolutionNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, string name) {
      var array = new double[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = solver.LookupVariableOrNull(varName).SolutionValue();
            }
          }
        }
      }

      return array;
    }

    public static double[,,,,] GetSolutionNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, string name) {
      var array = new double[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = solver.LookupVariableOrNull(varName).SolutionValue();
              }
            }
          }
        }
      }

      return array;
    }

    public static double[,,,,,] GetSolutionNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, string name) {
      var array = new double[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = solver.LookupVariableOrNull(varName).SolutionValue();
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static double[,] GetSolutionNumVarMatrix(this Solver solver, int rows, int cols, string name) =>
      solver.GetSolutionNumVarArray(rows, cols, name);

    public static Variable[,] MakeBoolVarArray(this Solver solver, int rows, int cols) =>
      solver.MakeBoolVarMatrix(rows, cols);

    public static Variable[,] MakeBoolVarArray(this Solver solver, int rows, int cols, string name) =>
      solver.MakeBoolVarMatrix(rows, cols, name);

    public static Variable[,,] MakeBoolVarArray(this Solver solver, int rows, int cols, int dim3) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            array[d1, d2, d3] = solver.MakeBoolVar("");
          }
        }
      }

      return array;
    }

    public static Variable[,,] MakeBoolVarArray(this Solver solver, int rows, int cols, int dim3, string name) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = solver.MakeBoolVar(varName);
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              array[d1, d2, d3, d4] = solver.MakeBoolVar("");
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, string name) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = solver.MakeBoolVar(varName);
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                array[d1, d2, d3, d4, d5] = solver.MakeBoolVar("");
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = solver.MakeBoolVar(varName);
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeBoolVar("");
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeBoolVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeBoolVar(varName);
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,] MakeIntVarArray(this Solver solver,
      int rows, int cols, double lb, double ub) => solver.MakeIntVarMatrix(rows, cols, lb, ub);

    public static Variable[,] MakeIntVarArray(this Solver solver,
      int rows, int cols, double lb, double ub, string name) => solver.MakeIntVarMatrix(rows, cols, lb, ub, name);

    public static Variable[,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            array[d1, d2, d3] = solver.MakeIntVar(lb, ub, "");
          }
        }
      }

      return array;
    }

    public static Variable[,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = solver.MakeIntVar(lb, ub, varName);
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              array[d1, d2, d3, d4] = solver.MakeIntVar(lb, ub, "");
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = solver.MakeIntVar(lb, ub, varName);
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                array[d1, d2, d3, d4, d5] = solver.MakeIntVar(lb, ub, "");
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = solver.MakeIntVar(lb, ub, varName);
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeIntVar(lb, ub, "");
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeIntVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeIntVar(lb, ub, varName);
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,] MakeNumVarArray(this Solver solver,
      int rows, int cols, double lb, double ub) => solver.MakeNumVarMatrix(rows, cols, lb, ub);

    public static Variable[,] MakeNumVarArray(this Solver solver,
      int rows, int cols, double lb, double ub, string name) =>
      solver.MakeNumVarMatrix(rows, cols, lb, ub, name);

    public static Variable[,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            array[d1, d2, d3] = solver.MakeNumVar(lb, ub, "");
          }
        }
      }

      return array;
    }

    public static Variable[,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = solver.MakeNumVar(lb, ub, varName);
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              array[d1, d2, d3, d4] = solver.MakeNumVar(lb, ub, "");
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = solver.MakeNumVar(lb, ub, varName);
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                array[d1, d2, d3, d4, d5] = solver.MakeNumVar(lb, ub, "");
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = solver.MakeNumVar(lb, ub, varName);
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeNumVar(lb, ub, "");
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeNumVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeNumVar(lb, ub, varName);
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,] MakeVarArray(this Solver solver,
      int rows, int cols, double lb, double ub, bool integer) =>
      solver.MakeVarMatrix(rows, cols, lb, ub, integer);

    public static Variable[,] MakeVarArray(this Solver solver,
      int rows, int cols, double lb, double ub, bool integer, string name) =>
      solver.MakeVarMatrix(rows, cols, lb, ub, integer, name);

    public static Variable[,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub, bool integer) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            array[d1, d2, d3] = solver.MakeVar(lb, ub, integer, "");
          }
        }
      }

      return array;
    }

    public static Variable[,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, double lb, double ub, bool integer, string name) {
      var array = new Variable[rows, cols, dim3];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            var varName = $"{name}[{d1}, {d2}, {d3}]";
            array[d1, d2, d3] = solver.MakeVar(lb, ub, integer, varName);
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub, bool integer) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              array[d1, d2, d3, d4] = solver.MakeVar(lb, ub, integer, "");
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, double lb, double ub, bool integer, string name) {
      var array = new Variable[rows, cols, dim3, dim4];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              var varName = $"{name}[{d1}, {d2}, {d3}, {d4}]";
              array[d1, d2, d3, d4] = solver.MakeVar(lb, ub, integer, varName);
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub, bool integer) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                array[d1, d2, d3, d4, d5] = solver.MakeVar(lb, ub, integer, "");
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, double lb, double ub, bool integer, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}]";
                array[d1, d2, d3, d4, d5] = solver.MakeVar(lb, ub, integer, varName);
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub, bool integer) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeVar(lb, ub, integer, "");
                }
              }
            }
          }
        }
      }

      return array;
    }

    public static Variable[,,,,,] MakeVarArray(this Solver solver,
      int rows, int cols, int dim3, int dim4, int dim5, int dim6, double lb, double ub, bool integer, string name) {
      var array = new Variable[rows, cols, dim3, dim4, dim5, dim6];
      for (var d1 = 0; d1 < rows; d1++) {
        for (var d2 = 0; d2 < cols; d2++) {
          for (var d3 = 0; d3 < cols; d3++) {
            for (var d4 = 0; d4 < cols; d4++) {
              for (var d5 = 0; d5 < cols; d5++) {
                for (var d6 = 0; d6 < cols; d6++) {
                  var varName = $"{name}[{d1}, {d2}, {d3}, {d4}, {d5}, {d6}]";
                  array[d1, d2, d3, d4, d5, d6] = solver.MakeVar(lb, ub, integer, varName);
                }
              }
            }
          }
        }
      }

      return array;
    }
  }
}
