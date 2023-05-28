using Expressions.Task3.E3SQueryProvider.Models.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "Equals")
            {
                if (node.Object != null && node.Arguments.Count == 1)
                {
                    var argument = node.Arguments[0];
                    if (argument.NodeType == ExpressionType.Constant && ((ConstantExpression)argument).Value == null)
                    {
                        var isNullExpression = Expression.Equal(node.Object, Expression.Constant(null));
                        return Visit(isNullExpression);
                    }
                    else
                    {
                        var equalsExpression = Expression.Equal(node.Object, argument);
                        return Visit(equalsExpression);
                    }
                }

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "StartsWith")
            {
                if (node.Object != null && node.Arguments.Count == 1)
                {
                    var member = node.Object as MemberExpression;
                    var argument = node.Arguments[0];
                    if (argument.NodeType == ExpressionType.Constant)
                    {
                        _resultStringBuilder.Append(member.Member.Name + ":(" + GetValue(node.Arguments[0]) + "*)");
                    }
                }

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "EndsWith")
            {
                if (node.Object != null && node.Arguments.Count == 1)
                {
                    var member = node.Object as MemberExpression;
                    var argument = node.Arguments[0];
                    if (argument.NodeType == ExpressionType.Constant)
                    {
                        _resultStringBuilder.Append(member.Member.Name + ":(*" + GetValue(node.Arguments[0]) + ")");
                    }
                }

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "Contains")
            {
                if (node.Object != null && node.Arguments.Count == 1)
                {
                    var member = node.Object as MemberExpression;
                    var argument = node.Arguments[0];
                    if (argument.NodeType == ExpressionType.Constant)
                    {
                        _resultStringBuilder.Append(member.Member.Name + ":(*" + GetValue(node.Arguments[0]) + "*)");
                    }
                }

                return node;
            }

            return base.VisitMethodCall(node);
        }

        private object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:

                    if (node.Left.NodeType == node.Right.NodeType)
                        throw new NotSupportedException($"Left and Right operands should be different types");

                    Expression start = node.Left, end = node.Right;
                    if (node.Left.NodeType == ExpressionType.Constant)
                    {
                        start = node.Right;
                        end = node.Left;
                    }

                    Visit(start);

                    _resultStringBuilder.Append("(");
                    Visit(end);
                    _resultStringBuilder.Append(")");
                    break;

                case ExpressionType.AndAlso:
                    _resultStringBuilder.Append("{ ");
                    base.Visit(node.Left);
                    _resultStringBuilder.Append(" } AND { ");
                    base.Visit(node.Right);
                    _resultStringBuilder.Append(" }");
                    return node;
                    break;

                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion
    }
}
