using ICSharpCode.CodeConverter.Util.FromRoslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ICSharpCode.CodeConverter.CSharp;
#nullable enable

internal class VisualBasicNullableExpressionsConverterExtension : VisualBasicNullableExpressionsConverter
{
    /// <summary>
    /// The code with this annotation is not nullable (even though the source code that created it is)
    /// </summary>
    private static readonly SyntaxAnnotation IsNotNullableAnnotation = new("CodeConverter.Nullable", false.ToString());

    public VisualBasicNullableExpressionsConverterExtension(SemanticModel semanticModel, HashSet<string> extraUsingDirectives) : base(semanticModel, extraUsingDirectives)
    {
    }

    public override ExpressionSyntax InvokeConversionWhenNotNull(VBSyntax.ExpressionSyntax vbExpr, ExpressionSyntax csExpr, MemberAccessExpressionSyntax conversionMethod, TypeSyntax castType)
    {
        var hasValueCheck = HasValue(ref csExpr);
        var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(csExpr)));
        ExpressionSyntax invocation = SyntaxFactory.InvocationExpression(conversionMethod, arguments);
        invocation = ValidSyntaxFactory.CastExpression(castType, invocation);

        return hasValueCheck.Conditional(invocation, ValidSyntaxFactory.NullExpression).AddParens();
    }

    public override ExpressionSyntax WithBinaryExpressionLogicForNullableTypes(VBSyntax.BinaryExpressionSyntax vbNode, TypeInfo lhsTypeInfo, TypeInfo rhsTypeInfo, BinaryExpressionSyntax csBinExp, ExpressionSyntax lhs, ExpressionSyntax rhs)
    {
        if (!IsSupported(vbNode.Kind()) || 
            !lhsTypeInfo.ConvertedType.IsNullable() ||
            !rhsTypeInfo.ConvertedType.IsNullable()) {
            return csBinExp;
        }
        var isLhsNullable = IsNullable(lhs, lhsTypeInfo);
        var isRhsNullable = IsNullable(rhs, rhsTypeInfo);
        if (!isLhsNullable && !isRhsNullable) return csBinExp.WithAdditionalAnnotations(IsNotNullableAnnotation);

        return WithBinaryExpressionLogicForNullableTypes(vbNode, csBinExp, lhs, rhs, isLhsNullable, isRhsNullable);
    }

    private ExpressionSyntax WithBinaryExpressionLogicForNullableTypes(VBSyntax.BinaryExpressionSyntax vbNode, BinaryExpressionSyntax csBinExp, ExpressionSyntax lhs, ExpressionSyntax rhs, bool isLhsNullable,
        bool isRhsNullable)
    {
        lhs = lhs.AddParens();
        rhs = rhs.AddParens();

        if (vbNode.IsKind(VBasic.SyntaxKind.AndAlsoExpression))
        {
            return ForAndAlsoOperator(lhs, rhs, isLhsNullable, isRhsNullable).AddParens();
        }

        if (vbNode.IsKind(VBasic.SyntaxKind.OrElseExpression))
        {
            return ForOrElseOperator(lhs, rhs, isLhsNullable, isRhsNullable).AddParens();
        }

        return ForRelationalOperators(csBinExp, lhs, rhs, isLhsNullable, isRhsNullable).AddParens();
    }

    private bool IsNullable(ExpressionSyntax csNode, TypeInfo typeInfo)
    {
        return typeInfo.Type.IsNullable() && !csNode.AnyInParens(x => x.HasAnnotation(IsNotNullableAnnotation));
    }

    private static bool IsSupported(VBasic.SyntaxKind op)
    {
        return op switch {
            VBasic.SyntaxKind.EqualsExpression => true,
            VBasic.SyntaxKind.NotEqualsExpression => true,
            VBasic.SyntaxKind.GreaterThanExpression => true,
            VBasic.SyntaxKind.GreaterThanOrEqualExpression => true,
            VBasic.SyntaxKind.LessThanExpression => true,
            VBasic.SyntaxKind.LessThanOrEqualExpression => true,
            VBasic.SyntaxKind.OrElseExpression => true,
            VBasic.SyntaxKind.AndAlsoExpression => true,
            _ => false
        };
    }

    private ExpressionSyntax ForAndAlsoOperator(ExpressionSyntax lhs, ExpressionSyntax rhs,
        bool isLhsNullable, bool isRhsNullable)
    {
        var lhsName = lhs;
        if (isLhsNullable) {
            lhsName = ExtensionMethodCall("ToBool", lhs, null); 
        }

        var rhsName = rhs;
        if (isRhsNullable) {
            rhsName = ExtensionMethodCall("ToBool", rhs, null);
        }

        return lhsName.And(rhsName).WithAdditionalAnnotations(IsNotNullableAnnotation);
    }

    private ExpressionSyntax ForOrElseOperator(ExpressionSyntax lhs, ExpressionSyntax rhs, bool isLhsNullable, bool isRhsNullable)
    {
        var lhsName = lhs;
        if (isLhsNullable)
            lhsName = ExtensionMethodCall("ToBool", lhs, null);

        var rhsName = rhs;
        if (isRhsNullable)
            rhsName = ExtensionMethodCall("ToBool", rhs, null);

        return lhsName.Or(rhsName).WithAdditionalAnnotations(IsNotNullableAnnotation);
    }

    private ExpressionSyntax ForRelationalOperators(BinaryExpressionSyntax csNode, ExpressionSyntax lhs, ExpressionSyntax rhs,
        bool isLhsNullable, bool isRhsNullable)
    {
        if (isLhsNullable || isRhsNullable) {
            string? extMethode;
            switch (csNode.Kind()) {
                case SyntaxKind.EqualsExpression:
                    extMethode = "EqualTo";
                    break;
                case SyntaxKind.GreaterThanExpression:
                    extMethode = "GreaterThan";
                    break;
                case SyntaxKind.LessThanExpression:
                    extMethode = "LesserThan";
                    break;
                case SyntaxKind.NotEqualsExpression:
                    extMethode = "NotEqualTo";
                    break;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    extMethode = "GreaterOrEqualTo";
                    break;
                case SyntaxKind.LessThanOrEqualExpression:
                    extMethode = "LesserOrEqualTo";
                    break;
                default:
                    extMethode = "OperatorNotSupported";
                    break;
            }

            return ExtensionMethodCall(extMethode, lhs, rhs).WithAdditionalAnnotations(IsNotNullableAnnotation);
        }

        return csNode.WithAdditionalAnnotations(IsNotNullableAnnotation);
    }

    private ExpressionSyntax HasValue(ref ExpressionSyntax csName)
    {
        var nullableHasValueExpression = csName.NullableHasValueExpression();
        csName = csName.NullableGetValueExpression();
        return nullableHasValueExpression;
    }

    protected ExpressionSyntax ExtensionMethodCall(string methodName, ExpressionSyntax csName, ExpressionSyntax? arg)
    {
        _extraUsingDirectives.Add("VBtoCSharp.Compatiblity");
        ArgumentListSyntax argLst;
        if (arg is null)
            argLst = SyntaxFactory.ArgumentList();
        else
            argLst = SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(arg)));
        var member = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, csName.AddParens(), ValidSyntaxFactory.IdentifierName(methodName));
        return SyntaxFactory.InvocationExpression(member, argLst);
    }
}