using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NX.Internal
{
	static class Property
	{
		public static string Name<TOwner>(Expression<Func<TOwner, object>> property = null)
		{
			if (property == null)
			{
				return null;
			}
			MemberExpression memberExpression;
			var unaryExpression = property.Body as UnaryExpression;
			if (unaryExpression != null)
			{
				memberExpression = (MemberExpression)unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression)property.Body;
			}
			return memberExpression.Member.Name;
		}

		public static string Name(Expression<Func<object>> property = null)
		{
			if (property == null)
			{
				return null;
			}
			MemberExpression memberExpression;
			var unaryExpression = property.Body as UnaryExpression;
			if (unaryExpression != null)
			{
				memberExpression = (MemberExpression)unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression)property.Body;
			}
			return memberExpression.Member.Name;
		}

	}
}