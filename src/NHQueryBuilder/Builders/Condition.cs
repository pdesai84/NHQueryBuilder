using NHQueryBuilder.Enums;
using System.Linq.Expressions;

namespace NHQueryBuilder.Builders
{
    public class Condition<T>
    {
        public Expression<Func<T, object>>? Expression { get; }
        public NHQueryBuilder.Enums.ConditionType ConditionType { get; }
        public object? Values { get; }

        public Condition(Expression<Func<T, object>>? expression, NHQueryBuilder.Enums.ConditionType conditionType, object? values)
        {
            this.Expression = expression;
            this.ConditionType = conditionType;
            this.Values = values;
        }
    }
}
