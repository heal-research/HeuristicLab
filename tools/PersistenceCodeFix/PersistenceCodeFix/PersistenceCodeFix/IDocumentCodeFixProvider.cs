using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace PersistenceCodeFix {
  public interface IDocumentCodeFixProvider {
    Task<Document> FixDocumentAsync(Document document, SyntaxNode node, CancellationToken cancellationToken);
  }
}
