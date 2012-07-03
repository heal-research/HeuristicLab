using System;

namespace ToolBox.Formatting {
  public class TextFormatter {
    // ------------------------------------------------------------------
    /// <summary>
    /// Returns a string that's formatted using the specified format for
    /// the number specified.
    /// </summary>
    /// <param name="format">TextFormat</param>
    /// <param name="number">double</param>
    /// <param name="significantDigits">int</param>
    /// <returns>string</returns>
    // ------------------------------------------------------------------
    public static string Format(
        TextFormat format,
        double number,
        int significantDigits) {
      return Format(format, number.ToString(), significantDigits);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns a string that's formatted using the specified format for
    /// the text specified.  If a format other than 'String' is specified,
    /// the text is first converted to a double, then the resultant string
    /// that represents the number in the specified format is returned.  If
    /// the conversion from the text to a double fails, no exception is
    /// thrown.  The supplied text is simply returned.
    /// </summary>
    /// <param name="format">TextFormat</param>
    /// <param name="text">string</param>
    /// <param name="significantDigits">int</param>
    /// <returns>string</returns>
    // ------------------------------------------------------------------
    public static string Format(
        TextFormat format,
        string text,
        int significantDigits) {
      double myNumber = double.MaxValue;
      string result = "";

      // Only if the format specified isn't a string,
      // then try to convert the text to a number.  If the
      // conversion fails, just return the text.
      if (format != TextFormat.String) {
        try {
          myNumber = double.Parse(text);
        }
        catch {
          return text;
        }
      }

      // Used for the number of sig digs.  For one sig dig, this
      // will be #.#.  For two sig digs, it'll be #.##.
      string sigDigIdentifier = "#";

      if (significantDigits > 0) {
        sigDigIdentifier = "#.";
        for (int i = 0; i < significantDigits; i++) {
          sigDigIdentifier += "#";
        }
      }
      switch (format) {
        case TextFormat.String:
          result = text;
          break;

        case TextFormat.Currency:
          result = String.Format("{0:c}", myNumber);
          break;

        case TextFormat.Decimal:
          result = String.Format("{0:d}", (int)myNumber);
          break;

        case TextFormat.FixedPoint:
          result = String.Format("{0:f}", myNumber);
          break;

        case TextFormat.General:
          result = String.Format("{0:g}", myNumber);
          break;

        case TextFormat.Hexadecimal:
          result = String.Format("{0:x}", (int)myNumber);
          break;

        case TextFormat.Number:
          //result = String.Format("{0:n}", myNumber);
          result = myNumber.ToString(sigDigIdentifier);
          break;

        case TextFormat.Percentage:
          result = String.Format("{0:p}", myNumber);
          break;

        case TextFormat.RoundTrip:
          result = String.Format("{0:r}", myNumber);
          break;

        case TextFormat.ScientificNotation:
          //result = String.Format("{0:e}", myNumber);
          result = myNumber.ToString(sigDigIdentifier + "E+0");
          break;

        default: result = text;
          break;
      }
      return result;
    }
  }
}
