using System;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Delegate for the current page changed event.
  /// </summary>
  /// <param name="sender">object</param>
  /// <param name="e">PageEventArgs</param>
  // ----------------------------------------------------------------------
  public delegate void CurrentPageChangedEventHandler(
      object sender,
      PageEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  /// Delegate for the page added event.
  /// </summary>
  /// <param name="sender">object</param>
  /// <param name="e">PageEventArgs</param>
  // ----------------------------------------------------------------------
  public delegate void PageAddedEventHandler(
      object sender,
      PageEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  ///  Delegate for the PaintStyleChanged event.
  /// </summary>
  /// <param name="sender">object</param>
  /// <param name="e">PaintStyleChangedEventArgs</param>
  // ----------------------------------------------------------------------
  public delegate void PaintStyleChangedEventHandler(object sender,
          PaintStyleChangedEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  ///  Delegate for the TextStyleChanged event.
  /// </summary>
  /// <param name="sender">object</param>
  /// <param name="e">TextStyleChangedEventArgs</param>
  // ----------------------------------------------------------------------
  public delegate void TextStyleChangedEventHandler(object sender,
          TextStyleChangedEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  /// Information regarding the addition of a new item to the collection.
  /// </summary>
  // ----------------------------------------------------------------------
  public delegate void CollectionAddInfo<T>(
  CollectionBase<T> collection,
  EntityEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  /// Information regarding the removal of an item from the collection.
  /// </summary>
  // ----------------------------------------------------------------------
  public delegate void CollectionRemoveInfo<T>(
  CollectionBase<T> collection,
  EntityEventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  /// Information regarding the removal/clear of all items from the 
  /// collection.
  /// </summary>
  // ----------------------------------------------------------------------
  public delegate void CollectionClearInfo<T>(
  CollectionBase<T> collection,
  EventArgs e);

  // ----------------------------------------------------------------------
  /// <summary>
  /// The info coming with the show-props event.
  /// </summary>
  // ----------------------------------------------------------------------
  public delegate void PropertiesInfo(object ent);

}
