using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace DevToolsDatabaseLayer
{
    public static class MyLinqExtensions
    {
        /// <summary>
        /// Adds support for the IsLike linq method to nhibernate to mimic sql "LIKE"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsLike(this string source, string pattern)
        {
            pattern = Regex.Escape(pattern);
            pattern = pattern.Replace("%", ".*?").Replace("_", ".");
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

            return Regex.IsMatch(source, pattern);
        }
    }

    public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public MyLinqToHqlGeneratorsRegistry()
            : base()
        {
            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)),
                              new IsLikeGenerator());
        }
    }

    public class IsLikeGenerator : BaseHqlGeneratorForMethod
    {
        public IsLikeGenerator()
        {
            SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)) };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
                                    visitor.Visit(arguments[1]).AsExpression());
        }
    }
}
