using System;
using System.Collections;
using System.IO;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Our custom "clipboard" for copying diagram elements (or anything else
  /// for that matter) to.
  /// </summary>
  // ----------------------------------------------------------------------
  public static class NetronClipboard {
    // ------------------------------------------------------------------
    /// <summary>
    /// The items on the clipboard.
    /// </summary>
    // ------------------------------------------------------------------
    static ArrayList items = new ArrayList();

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the collection of all items.
    /// </summary>
    // ------------------------------------------------------------------
    public static ArrayList Items {
      get {
        return items;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds the item to the collection.
    /// </summary>
    /// <param name="item">object</param>
    /// <returns>int</returns>
    // ------------------------------------------------------------------
    public static int Add(object item) {
      return items.Add(item);
    }

    public static int AddStream(MemoryStream stream) {
      return items.Add(stream);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Clears the collection.
    /// </summary>
    // ------------------------------------------------------------------
    public static void Clear() {
      items.Clear();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns if we have any items of the type specified.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>bool</returns>
    // ------------------------------------------------------------------
    public static bool ContainsData(Type type) {
      foreach (Object obj in items) {
        if (obj.GetType() == type) {
          return true;
        }
      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all items of the type specified.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    public static ArrayList GetAll(Type type) {
      ArrayList subitems = new ArrayList();
      foreach (Object obj in items) {
        if (obj.GetType() == type) {
          subitems.Add(obj);
        }
      }
      return subitems;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the FIRST item found that has the type specified.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>object</returns>
    // ------------------------------------------------------------------
    public static object Get(Type type) {
      foreach (Object obj in items) {
        if (obj.GetType() == type) {
          return obj;
        }
      }
      return null;
    }

    public static MemoryStream GetMemoryStream() {
      foreach (object obj in items) {
        if (obj is MemoryStream) {
          return obj as MemoryStream;
        }
      }

      return null;
    }
  }
}
