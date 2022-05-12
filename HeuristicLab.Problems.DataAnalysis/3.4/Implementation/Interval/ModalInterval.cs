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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  public class ModalInterval : IEquatable<ModalInterval>{
    [Storable]
    public double LowerBound { get; set; }
    [Storable]
    public double UpperBound { get; set; }
    public double Width => Math.Abs(LowerBound - UpperBound);
    public bool isProper => LowerBound <= UpperBound;

    #region Constructors

    [StorableConstructor]
    protected ModalInterval(StorableConstructorFlag _) { }

    public ModalInterval(double lowerBound, double upperBound) {
      LowerBound = lowerBound;
      UpperBound = upperBound;
    }

    public ModalInterval(double v) : this(v, v) { }

    public static ModalInterval Proper(ModalInterval a) {
      return new ModalInterval(Math.Min(a.LowerBound, a.UpperBound), Math.Max(a.LowerBound, a.UpperBound));
    }

    #endregion

    #region Operations

    public static ModalInterval Add(ModalInterval a, ModalInterval b) {
      return new ModalInterval(a.LowerBound + b.LowerBound, a.UpperBound + b.UpperBound);
    }

    public static ModalInterval Subtract(ModalInterval a, ModalInterval b) {
      return new ModalInterval(a.LowerBound - b.UpperBound, a.UpperBound - b.LowerBound);
    }

    public static ModalInterval Sine(ModalInterval a) {
      if (Math.Abs(a.UpperBound - a.LowerBound) >= Math.PI * 2) return new ModalInterval(-1, 1);

      //divide the interval by PI/2 so that the optima lie at x element of N (0,1,2,3,4,...)
      double Pihalf = Math.PI / 2;
      ModalInterval scaled = ModalInterval.Divide(a, new ModalInterval(Pihalf, Pihalf));
      //move to positive scale
      if (scaled.LowerBound < 0) {
        int periodsToMove = Math.Abs((int)scaled.LowerBound / 4) + 1;
        scaled = ModalInterval.Add(scaled, new ModalInterval(periodsToMove * 4, periodsToMove * 4));
      }

      double scaledLowerBound = scaled.LowerBound % 4.0;
      double scaledUpperBound = scaled.UpperBound % 4.0;
      if (scaledUpperBound < scaledLowerBound) scaledUpperBound += 4.0;
      List<double> sinValues = new List<double>();
      sinValues.Add(Math.Sin(scaledLowerBound * Pihalf));
      sinValues.Add(Math.Sin(scaledUpperBound * Pihalf));

      int startValue = (int)Math.Ceiling(scaledLowerBound);
      while (startValue < scaledUpperBound) {
        sinValues.Add(Math.Sin(startValue * Pihalf));
        startValue += 1;
      }

      return new ModalInterval(sinValues.Min(), sinValues.Max());
    }

    public static ModalInterval Cosine(ModalInterval a) {
      return ModalInterval.Sine(ModalInterval.Add(a, new ModalInterval(Math.PI / 2, Math.PI / 2)));
    }
    public static ModalInterval Tangens(ModalInterval a) {
      return ModalInterval.Divide(ModalInterval.Sine(a), ModalInterval.Cosine(a));
    }
    public static ModalInterval HyperbolicTangent(ModalInterval a) {
      return new ModalInterval(Math.Tanh(a.LowerBound), Math.Tanh(a.UpperBound));
    }

    public static ModalInterval Logarithm(ModalInterval a) {
      return new ModalInterval(Math.Log(a.LowerBound), Math.Log(a.UpperBound));
    }

    public static ModalInterval Exponential(ModalInterval a) {
      return new ModalInterval(Math.Exp(a.LowerBound), Math.Exp(a.UpperBound));
    }

    public static ModalInterval Square(ModalInterval a) {
      return ModalInterval.Power(a, 2);
    }

    public static ModalInterval Cube(ModalInterval a) {
      return ModalInterval.Power(a, 3);
    }

    public static ModalInterval SquareRoot(ModalInterval a) {
      if (a.LowerBound < 0) return new ModalInterval(double.NaN, double.NaN);
      return new ModalInterval(-Math.Sqrt(a.UpperBound), Math.Sqrt(a.UpperBound));
    }

    public static ModalInterval CubicRoot(ModalInterval a) {
      var lower = (a.LowerBound < 0) ? -Math.Pow(-a.LowerBound, 1d / 3d) : Math.Pow(a.LowerBound, 1d / 3d);
      var upper = (a.UpperBound < 0) ? -Math.Pow(-a.UpperBound, 1d / 3d) : Math.Pow(a.UpperBound, 1d / 3d);

      return new ModalInterval(lower, upper);
    }

    public static ModalInterval Power(ModalInterval a, int k) {
      //special treatment for power 0 and 1
      if (k == 0) {
        return new ModalInterval(1);
      }
      if (k == 1) {
        return a;
      }

      //logic for power 
      if (k % 2 != 0) {
        return new ModalInterval(Math.Pow(a.LowerBound, k), Math.Pow(a.UpperBound, k));
      } else if (a.LowerBound >= 0 && a.UpperBound >= 0) {
        return new ModalInterval(Math.Pow(a.LowerBound, k), Math.Pow(a.UpperBound, k));
      } else if (a.LowerBound < 0 && a.UpperBound < 0) {
        return new ModalInterval(Math.Pow(a.UpperBound, k), Math.Pow(a.LowerBound, k));
      } else if (a.LowerBound < 0 && a.UpperBound >= 0) {
        return new ModalInterval(0, Math.Max(Math.Pow(a.LowerBound, k), Math.Pow(a.UpperBound, k)));
      } else if (a.LowerBound >= 0 && a.UpperBound < 0) {
        return new ModalInterval(Math.Max(Math.Pow(a.LowerBound, k), Math.Pow(a.UpperBound, k)), 0);
      }

      //fallback
      return new ModalInterval(double.NaN, double.NaN);
    }

    public static ModalInterval Multiply(ModalInterval a, ModalInterval b) {
      if (a.LowerBound >= 0 && a.UpperBound >= 0) {
        //case1mul
        if (b.LowerBound >= 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.LowerBound * b.LowerBound, a.UpperBound * b.UpperBound);
        } else if (b.LowerBound >= 0 && b.UpperBound < 0) {
          return new ModalInterval(a.LowerBound * b.LowerBound, a.LowerBound * b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.UpperBound * b.LowerBound, a.UpperBound * b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound * b.LowerBound, a.LowerBound * b.UpperBound);
        }
      } else if (a.LowerBound >= 0 && a.UpperBound < 0) {
        //case2mul
        if (b.LowerBound >= 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.LowerBound * b.LowerBound, a.UpperBound * b.LowerBound);
        } else if (b.LowerBound >= 0 && b.UpperBound < 0) {
          return new ModalInterval(Math.Max(a.LowerBound * b.LowerBound, a.UpperBound * b.UpperBound), Math.Min(a.LowerBound * b.UpperBound, a.UpperBound * b.LowerBound));
        } else if (b.LowerBound < 0 && b.UpperBound >= 0) {
          return new ModalInterval(0);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound * b.UpperBound, a.LowerBound * b.LowerBound);
        }
      } else if (a.LowerBound < 0 && a.UpperBound >= 0) {
        //case3mul
        if (b.LowerBound >= 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.LowerBound * b.UpperBound, a.UpperBound * b.UpperBound);
        } else if (b.LowerBound >= 0 && b.UpperBound < 0) {
          return new ModalInterval(0);
        } else if (b.LowerBound < 0 && b.UpperBound >= 0) {
          return new ModalInterval(Math.Min(a.LowerBound * b.UpperBound, a.UpperBound * b.LowerBound), Math.Max(a.LowerBound * b.LowerBound, a.UpperBound * b.UpperBound));
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound * b.LowerBound, a.LowerBound * b.LowerBound);
        }
      } else {
        //case4mul
        if (b.LowerBound >= 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.LowerBound * b.UpperBound, a.UpperBound * b.LowerBound);
        } else if (b.LowerBound >= 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound * b.UpperBound, a.UpperBound * b.LowerBound);
        } else if (b.LowerBound < 0 && b.UpperBound >= 0) {
          return new ModalInterval(a.LowerBound * b.UpperBound, a.LowerBound * b.LowerBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound * b.UpperBound, a.LowerBound * b.LowerBound);
        }
      }

      //fallback
      return new ModalInterval(double.NaN, double.NaN);
    }

    public static ModalInterval Absolute(ModalInterval a) {
      if (a.LowerBound >= 0 && a.UpperBound >= 0) {
        return new ModalInterval(a.LowerBound, a.UpperBound);
      } else if (a.LowerBound < 0 && a.UpperBound < 0) {
        return new ModalInterval(a.UpperBound, a.LowerBound);
      } else if (a.LowerBound < 0 && a.UpperBound >= 0) {
        return new ModalInterval(0, Math.Max(-a.LowerBound, a.UpperBound));
      } else if (a.LowerBound >= 0 && a.UpperBound < 0) {
        return new ModalInterval(Math.Max(a.LowerBound, -a.UpperBound), 0);
      }

      //fallback
      return a;
    }

    public static ModalInterval Divide(ModalInterval a, ModalInterval b) {
      if ((b.LowerBound > 0 && b.UpperBound < 0) ||
          (b.LowerBound < 0 && b.UpperBound > 0) ||
          (b.LowerBound == 0 && b.UpperBound == 0) ||
          IsMember(0, Proper(b))) {
        return new ModalInterval(double.NaN);
      }

      if (a.LowerBound >= 0 && a.UpperBound >= 0) {
        //case1div
        if (b.LowerBound > 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.UpperBound, a.UpperBound / b.LowerBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.UpperBound, a.LowerBound / b.LowerBound);
        } else if (b.LowerBound > 0 && b.UpperBound == 0) {
          return new ModalInterval(double.PositiveInfinity, a.UpperBound / b.LowerBound);
        } else if (b.LowerBound == 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.UpperBound, double.PositiveInfinity);
        } else if (b.LowerBound < 0 && b.UpperBound == 0) {
          return new ModalInterval(double.NegativeInfinity, a.LowerBound / b.LowerBound);
        } else if (b.LowerBound == 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.UpperBound, double.NegativeInfinity);
        }
      } else if (a.LowerBound >= 0 && a.UpperBound < 0) {
        //case2div
        if (b.LowerBound > 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.UpperBound, a.UpperBound / b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.LowerBound, a.LowerBound / b.LowerBound);
        } else if (b.LowerBound > 0 && b.UpperBound == 0) {
          return new ModalInterval(double.PositiveInfinity, double.NegativeInfinity);
        } else if (b.LowerBound == 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.UpperBound, a.UpperBound / b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound == 0) {
          return new ModalInterval(a.UpperBound / b.LowerBound, a.LowerBound / b.LowerBound);
        } else if (b.LowerBound == 0 && b.UpperBound < 0) {
          return new ModalInterval(double.PositiveInfinity, double.NegativeInfinity);
        }
      } else if (a.LowerBound < 0 && a.UpperBound >= 0) {
        //case3div
        if (b.LowerBound > 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.LowerBound, a.UpperBound / b.LowerBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.UpperBound, a.LowerBound / b.UpperBound);
        } else if (b.LowerBound > 0 && b.UpperBound == 0) {
          return new ModalInterval(a.LowerBound / b.LowerBound, a.UpperBound / b.LowerBound);
        } else if (b.LowerBound == 0 && b.UpperBound > 0) {
          return new ModalInterval(double.NegativeInfinity, double.PositiveInfinity);
        } else if (b.LowerBound < 0 && b.UpperBound == 0) {
          return new ModalInterval(double.NegativeInfinity, double.PositiveInfinity);
        } else if (b.LowerBound == 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.UpperBound, a.LowerBound / b.UpperBound);
        }
      } else if (a.LowerBound < 0 && a.UpperBound < 0) {
        //case4div
        if (b.LowerBound > 0 && b.UpperBound > 0) {
          return new ModalInterval(a.LowerBound / b.LowerBound, a.UpperBound / b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound < 0) {
          return new ModalInterval(a.UpperBound / b.LowerBound, a.LowerBound / b.UpperBound);
        } else if (b.LowerBound > 0 && b.UpperBound == 0) {
          return new ModalInterval(a.LowerBound / b.LowerBound, double.NegativeInfinity);
        } else if (b.LowerBound == 0 && b.UpperBound > 0) {
          return new ModalInterval(double.NegativeInfinity, a.UpperBound / b.UpperBound);
        } else if (b.LowerBound < 0 && b.UpperBound == 0) {
          return new ModalInterval(a.UpperBound / b.LowerBound, double.PositiveInfinity);
        } else if (b.LowerBound == 0 && b.UpperBound < 0) {
          return new ModalInterval(double.PositiveInfinity, a.LowerBound / b.UpperBound);
        }
      }

      //fallback
      return new ModalInterval(double.NaN);
    }

    public static ModalInterval AnalyticQuotient(ModalInterval a, ModalInterval b) {
      var dividend = a;
      var divisor = Add(Square(b), new ModalInterval(1.0, 1.0));
      divisor = SquareRoot(divisor);

      var quotient = Divide(dividend, divisor);
      return quotient;
    }

    #endregion

    #region Helpers
    public ModalInterval Improper() {
      return new ModalInterval(Math.Max(LowerBound, UpperBound), Math.Min(LowerBound, UpperBound));
    }

    public static Interval ToInterval(ModalInterval modalInterval) {
      if (modalInterval.isProper) return new Interval(modalInterval.LowerBound, modalInterval.UpperBound);

      return new Interval(modalInterval.UpperBound, modalInterval.LowerBound);
    }

    public static bool IsMember(double value, ModalInterval a) {
      return Math.Min(a.LowerBound, a.UpperBound) <= value && value <= Math.Max(a.LowerBound, a.UpperBound);
    }

    public static ModalInterval Dual(ModalInterval interval) {
      return new ModalInterval(interval.UpperBound, interval.LowerBound);
    }

    public static ModalInterval Mid(ModalInterval interval) {
      var mid = interval.LowerBound + (interval.UpperBound - interval.LowerBound) / 2;
      return new ModalInterval(mid);
    }

    #endregion

    #region Equals, Hash
    public bool Equals(ModalInterval other) {
      if (other == null)
        return false;

      return (UpperBound == other.UpperBound || (double.IsNaN(UpperBound) && double.IsNaN(other.UpperBound)))
        && (LowerBound == other.LowerBound || (double.IsNaN(LowerBound) && double.IsNaN(other.LowerBound)));
    }

    public override bool Equals(object obj) {
      return base.Equals(obj as ModalInterval);
    }

    public override int GetHashCode() {
      return LowerBound.GetHashCode() ^ UpperBound.GetHashCode();
    }

    public static bool operator ==(ModalInterval left, ModalInterval right) {
      if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
      return left.Equals(right);
    }

    public static bool operator !=(ModalInterval left, ModalInterval right) {
      return !(left == right);
    }

    #endregion
  }
}
