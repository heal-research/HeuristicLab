using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;


namespace PersistenceCodeFix {
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingStorableConstructorFix)), Shared]
  public sealed class MissingStorableConstructorFix : CodeFixProvider, IDocumentCodeFixProvider {
    private const string title = "Add storable constructor";

    public sealed override ImmutableArray<string> FixableDiagnosticIds {
      get { return ImmutableArray.Create(MissingStorableConstructorAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider() {
      return SequentialFixAllProvider.Instance;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;
      var baseTypeDecl = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

      context.RegisterCodeFix(
        CodeAction.Create(
          title: title,
          createChangedDocument: c => AddStorableConstructor(context.Document, baseTypeDecl, c),
          equivalenceKey: title),
        diagnostic);
    }

    private static async Task<Document> AddStorableConstructor(Document document, BaseTypeDeclarationSyntax baseTypeDecl, CancellationToken cancellationToken) {
      // create new attribute list
      var attrList = SyntaxFactory.AttributeList();
      attrList = attrList.AddAttributes(
        SyntaxFactory.Attribute(
          SyntaxFactory.IdentifierName("StorableConstructor")));

      // check ctor visibility
      var visibility = SyntaxFactory.Token(
        baseTypeDecl.Modifiers.Any(x => x.IsKind(SyntaxKind.SealedKeyword))
          ? SyntaxKind.PrivateKeyword
          : SyntaxKind.ProtectedKeyword);

      // create ctor parameters
      var paramList = SyntaxFactory.ParameterList();
      paramList = paramList.AddParameters(
        SyntaxFactory.Parameter(
          SyntaxFactory.Identifier("deserializing"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword))));

      // create initializer args
      var initArgs = SyntaxFactory.ArgumentList();
      initArgs = initArgs.AddArguments(
        SyntaxFactory.Argument(
          SyntaxFactory.IdentifierName("deserializing")));

      // create initializer
      var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, initArgs);

      // create new ctor
      var storableCtor = SyntaxFactory.ConstructorDeclaration(baseTypeDecl.Identifier)
        .WithAttributeLists(SyntaxFactory.List(new[] { attrList }))
        .WithModifiers(SyntaxFactory.TokenList(visibility))
        .WithParameterList(paramList)
        .WithInitializer(initializer)
        .WithBody(SyntaxFactory.Block());

      BaseTypeDeclarationSyntax newBaseTypeDecl;

      // add new ctor
      switch (baseTypeDecl.Kind()) {
        case SyntaxKind.ClassDeclaration:
          var classDecl = (ClassDeclarationSyntax)baseTypeDecl;
          var classMembers = classDecl.Members;
          classMembers = InsertIntoMembers(classMembers, storableCtor);
          newBaseTypeDecl = classDecl.WithMembers(classMembers);
          break;
        case SyntaxKind.StructDeclaration:
          var structDecl = (StructDeclarationSyntax)baseTypeDecl;
          var structMembers = structDecl.Members;
          structMembers = InsertIntoMembers(structMembers, storableCtor);
          newBaseTypeDecl = structDecl.WithMembers(structMembers);
          break;
        default: throw new NotSupportedException();
      }

      var root = await document.GetSyntaxRootAsync(cancellationToken);
      var newRoot = root.ReplaceNode(baseTypeDecl, newBaseTypeDecl.WithAdditionalAnnotations(Formatter.Annotation));

      return document.WithSyntaxRoot(newRoot);
    }

    private static SyntaxList<MemberDeclarationSyntax> InsertIntoMembers(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax storableCtor) {
      if (!members.Any()) return members.Add(storableCtor);

      var firstCtor = members.FirstOrDefault(x => x.IsKind(SyntaxKind.ConstructorDeclaration));
      if (firstCtor != null) {
        storableCtor = storableCtor.WithLeadingTrivia(firstCtor.GetLeadingTrivia());
        int index = members.IndexOf(firstCtor);
        members = members.Insert(index++, storableCtor);
        members = members.RemoveAt(index);
        members = members.Insert(index, firstCtor.WithoutLeadingTrivia());
      } else {
        var lastMember = members.Last();
        storableCtor = storableCtor.WithTrailingTrivia(lastMember.GetTrailingTrivia());
        int index = members.IndexOf(lastMember);
        members = members.Insert(index, storableCtor);
        members = members.RemoveAt(index + 1);
        members = members.Insert(index, lastMember.WithoutTrailingTrivia());
      }

      return members;
    }

    public Task<Document> FixDocumentAsync(Document document, SyntaxNode node, CancellationToken cancellationToken) {
      return AddStorableConstructor(document, (BaseTypeDeclarationSyntax)node, cancellationToken);
    }
  }
}
