
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Manages the collection of libraries.
  /// </summary>
  // ----------------------------------------------------------------------
  public class LibraryManager {
    // ------------------------------------------------------------------
    /// <summary>
    /// The collection of shapes for this library.
    /// </summary>
    // ------------------------------------------------------------------
    private CollectionBase<Library> myLibraries;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the collection of all libraries.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<Library> Libraries {
      get {
        return myLibraries;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public LibraryManager() {
      myLibraries = new CollectionBase<Library>();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the shape that has the GUID specified.
    /// </summary>
    /// <param name="guid">string</param>
    /// <returns>IShape</returns>
    // ------------------------------------------------------------------
    public IShape CreateNewInstance(string guid) {
      foreach (Library lib in myLibraries) {
        if (lib.ContainsShape(guid)) {
          return lib.CreateNewInstance(guid);
        }
      }
      return null;
    }
  }
}
