using System;

namespace ToolBox.Formatting {
  [Serializable()]
  public enum TextFormat {
    // ------------------------------------------------------------------
    /// <summary>
    /// No formatting is applied.
    /// </summary>
    // ------------------------------------------------------------------
    String,

    // ------------------------------------------------------------------
    /// <summary>
    /// Culturally aware currency format.
    /// </summary>
    // ------------------------------------------------------------------
    Currency,

    // ------------------------------------------------------------------
    /// <summary>
    /// Only supports integral numbers. Displays a string using decimal 
    /// digits preceded by a minus sign if negative.
    /// </summary>
    // ------------------------------------------------------------------
    Decimal,

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays numbers in the form ±d.ddddddE±dd where d is a decimal 
    /// digit. 
    /// </summary>
    // ------------------------------------------------------------------
    ScientificNotation,

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a series of decimal digits with a decimal point and 
    /// additional digits.
    /// </summary>
    // ------------------------------------------------------------------
    FixedPoint,

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays either as a fixed-point or scientific notation based 
    /// on the size of the number.
    /// </summary>
    // ------------------------------------------------------------------
    General,

    // ------------------------------------------------------------------
    /// <summary>
    /// Similar to fixed point but uses a separator character (such as ,) 
    /// for groups of digits.
    /// </summary>
    // ------------------------------------------------------------------
    Number,

    // ------------------------------------------------------------------
    /// <summary>
    /// Multiplies the number by 100 and displays with a percent symbol.
    /// </summary>
    // ------------------------------------------------------------------
    Percentage,

    // ------------------------------------------------------------------
    /// <summary>
    /// Formats a floating-point number so that it can be successfully 
    /// converted back to its original value.
    /// </summary>
    // ------------------------------------------------------------------
    RoundTrip,

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays an integral number using the base-16 number system.
    /// </summary>
    // ------------------------------------------------------------------
    Hexadecimal
  }
}
