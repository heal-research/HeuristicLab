using System.Collections.Generic;

namespace HeuristicLab.Persistence.Auxiliary {
  /// <summary>
  /// Extension methods for the <see cref="System.String"/> class.
  /// </summary>
  public static class StringExtensions {

    /// <summary>
    /// Enumeration over the substrings when split with a certain delimiter.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>An enumeration over the delimited substrings.</returns>
    public static IEnumerable<string> EnumerateSplit(this string s, char delimiter) {
      int startIdx = 0;
      for (int i = 0; i < s.Length; i++) {
        if (s[i] == delimiter) {
          if (i > startIdx) {
            yield return s.Substring(startIdx, i - startIdx);
          }
          startIdx = i + 1;
        }
      }
      if (startIdx < s.Length)
        yield return s.Substring(startIdx, s.Length - startIdx);
    }

    /// <summary>
    /// Enumeration over the substrings when split with a certain delimiter..
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>An enumerator over the delimited substrings.</returns>
    public static IEnumerator<string> GetSplitEnumerator(this string s, char delimiter) {
      int startIdx = 0;
      for (int i = 0; i < s.Length; i++) {
        if (s[i] == delimiter) {
          if (i > startIdx) {
            yield return s.Substring(startIdx, i - startIdx);
          }
          startIdx = i + 1;
        }
      }
      if (startIdx < s.Length)
        yield return s.Substring(startIdx, s.Length - startIdx);
    }
  }
}
