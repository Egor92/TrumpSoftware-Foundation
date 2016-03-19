using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TrumpSoftware.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static MemberInfo GetMemberInfo<T, TValue>(this Expression<Func<T, TValue>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member access", "expression");
            return member.Member;
        }

        public static string GetMemberName<T, TValue>(this Expression<Func<T, TValue>> expression)
        {
            return GetMemberInfo(expression).Name;
        }
    }
}
