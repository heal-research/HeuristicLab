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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HeuristicLab.Common {
  public static class TimeSpanHelper {
    public static bool TryGetFromNaturalFormat(string text, out TimeSpan result) {
      result = TimeSpan.Zero;

      var matches = Regex.Matches(text, @"(\d+)[ \t]*([a-zA-Z]+)");
      if (matches.Count == 0) return false;

      foreach (Match match in matches) {
        var number = match.Groups[1].Value;
        var unit = match.Groups[2].Value;

        double value;
        if (!double.TryParse(number, out value))
          return false;

        switch (unit) {
          case "d":
          case "day":
          case "days": result = result.Add(TimeSpan.FromDays(value)); break;
          case "h":
          case "hour":
          case "hours": result = result.Add(TimeSpan.FromHours(value)); break;
          case "m":
          case "min":
          case "minute":
          case "minutes": result = result.Add(TimeSpan.FromMinutes(value)); break;
          case "s":
          case "sec":
          case "second":
          case "seconds": result = result.Add(TimeSpan.FromSeconds(value)); break;
          default: return false;
        }
      }
      return true;
    }

    public static string FormatNatural(TimeSpan ts, bool abbrevUnit = false, bool composite = false) {
      if (!composite) {
        if (ts.TotalSeconds < 180) return ts.TotalSeconds + (abbrevUnit ? "s" : " seconds");
        if (ts.TotalMinutes < 180) return ts.TotalMinutes + (abbrevUnit ? "min" : " minutes");
        if (ts.TotalHours < 96) return ts.TotalHours + (abbrevUnit ? "h" : " hours");
        return ts.TotalDays + (abbrevUnit ? "d" : " days");
      } else {
        var sb = new StringBuilder();
        if (ts.TotalDays > 0) {
          var absDays = (int)Math.Floor(ts.TotalDays);
          sb.Append(absDays);
          sb.Append(abbrevUnit ? "d" : ((absDays > 1) ? " days" : " day"));
        }
        if (ts.Hours > 0) {
          if (sb.Length > 0) sb.Append(" ");
          sb.Append(ts.Hours);
          sb.Append(abbrevUnit ? "h" : ((ts.Hours > 1) ? " hours" : " hour"));
        }
        if (ts.Minutes > 0) {
          if (sb.Length > 0) sb.Append(" ");
          sb.Append(ts.Minutes);
          sb.Append(abbrevUnit ? "min" : ((ts.Minutes > 1) ? " minutes" : " minute"));
        }
        if (ts.Seconds > 0) {
          if (sb.Length > 0) sb.Append(" ");
          sb.Append(ts.Seconds);
          sb.Append(abbrevUnit ? "s" : ((ts.Minutes > 1) ? " seconds" : " second"));
        }
        return sb.ToString();
      }
    }
  }
}
