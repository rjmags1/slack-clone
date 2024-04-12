using GraphQL;
using GraphQLParser.AST;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL;

namespace Common.Utils;

public static class GraphQLUtils
{
    public static string? GetQueryName(IDictionary<string, object> context)
    {
        return (string?)context["queryName"];
    }

    public static string? GetQuery(IDictionary<string, object> context)
    {
        return (string?)context["query"];
    }

    public static Guid GetSubClaim(IDictionary<string, object> context)
    {
        return (Guid)context["sub"]!;
    }

    public static GraphQLField GetNodeASTFromConnectionAST(
        GraphQLField connectionAst,
        GraphQLDocument document,
        string connectionType,
        string edgeType
    )
    {
        GraphQLField? edgesField = (GraphQLField?)
            connectionAst.SelectionSet!.Selections.FirstOrDefault(
                s =>
                    s.Kind == ASTNodeKind.Field
                    && (s as GraphQLField)!.Name.StringValue == "edges"
            );
        if (edgesField is null)
        {
            var fragmentSpreads = connectionAst.SelectionSet.Selections
                .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
                .Select(
                    s =>
                        (s as GraphQLFragmentSpread)!
                            .FragmentName
                            .Name
                            .StringValue
                );
            var fragDef = (
                (GraphQLFragmentDefinition)
                    document.Definitions
                        .Where(d => d.Kind == ASTNodeKind.FragmentDefinition)
                        .First(
                            fd =>
                                (fd as GraphQLFragmentDefinition)!
                                    .TypeCondition
                                    .Type
                                    .Name
                                    .StringValue == connectionType
                                && fragmentSpreads.Contains(
                                    (fd as GraphQLFragmentDefinition)!
                                        .FragmentName
                                        .Name
                                        .StringValue
                                )
                        )
            );
            //(
            edgesField = (GraphQLField?)
                fragDef.SelectionSet.Selections.First(
                    s =>
                        s.Kind == ASTNodeKind.Field
                        && (s as GraphQLField)!.Name.StringValue == "edges"
                );
        }

        GraphQLField? result = (GraphQLField?)
            edgesField!.SelectionSet!.Selections.FirstOrDefault(
                s =>
                    s.Kind == ASTNodeKind.Field
                    && (s as GraphQLField)!.Name.StringValue == "node"
            );
        if (result is null)
        {
            var fragmentSpreads = edgesField.SelectionSet.Selections
                .Where(s => s.Kind == ASTNodeKind.FragmentSpread)
                .Select(
                    s =>
                        (s as GraphQLFragmentSpread)!
                            .FragmentName
                            .Name
                            .StringValue
                );
            var fragDef = document.Definitions.First(
                d =>
                    d.Kind == ASTNodeKind.FragmentDefinition
                    && fragmentSpreads.Contains(
                        (d as GraphQLFragmentDefinition)!
                            .FragmentName
                            .Name
                            .StringValue
                    )
                    && (d as GraphQLFragmentDefinition)!
                        .TypeCondition
                        .Type
                        .Name
                        .StringValue == edgeType
            );
            result = (GraphQLField)
                (
                    fragDef as GraphQLFragmentDefinition
                )!.SelectionSet.Selections.First(
                    s =>
                        s.Kind == ASTNodeKind.Field
                        && (s as GraphQLField)!.Name.StringValue == "node"
                );
        }

        return result!;
    }
}
