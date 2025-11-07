using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHQueryBuilder.Enums;

namespace NHQueryBuilder.Builders
{
    internal class ConditionTree
    {
        public string Alias { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public JoinType JoinType { get; set; }
        public ConditionType ConditionType { get; set; }
        public object Value { get; set; }
        public ICriteria Criteria { get; set; }
        public IList<ConditionTree> Children { get; }

        public ConditionTree(string propertyName, Type propertyType, JoinType joinType)
        {
            Children = new List<ConditionTree>();

            PropertyName = propertyName;
            PropertyType = propertyType;
            JoinType = joinType;
            ConditionType = ConditionType.Fetch;
        }

        public ConditionTree Insert(string propertyName, Type propertyType, string alias, JoinType joinType)
        {
            ConditionTree child = new ConditionTree(propertyName, propertyType, joinType);
            child.Criteria = Criteria.CreateCriteria(propertyName, alias, joinType);
            child.Alias = alias;

            Children.Add(child);
            return child;
        }

        public ConditionTree GetNode(string propertyName)
        {
            return Children.FirstOrDefault(x => x.PropertyName == propertyName);
        }

        public bool Exist(string propertyName)
        {
            return Children.Any(x => x.PropertyName == propertyName);
        }

        private static Type TypeConversion(Type propertyType)
        {
            Type pType;
            if (propertyType == typeof(int?))
                pType = typeof(int);
            else if (propertyType == typeof(long?))
                pType = typeof(long);
            else if (propertyType == typeof(DateTime?))
                pType = typeof(DateTime);
            else
                pType = propertyType;

            return pType;
        }

        public static IProjection GetProjection(string propertyName, string alias, ConditionType conditionType)
        {
            IProjection result;
            propertyName = alias + "." + propertyName;

            switch (conditionType)
            {
                case ConditionType.Select:
                    {
                        result = Projections.Property(propertyName);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Condition type '{conditionType}' is not yet implemented.");
                    }
            }

            return result;
        }

        public static ICriterion GetCriteria(string propertyName, Type propertyType, string alias, ConditionType conditionType, object value)
        {
            ICriterion result;
            propertyName = alias + "." + propertyName;

            Type pType = TypeConversion(propertyType);
            switch (conditionType)
            {
                case ConditionType.Equal:
                    {
                        result = Restrictions.Eq(propertyName, Convert.ChangeType(value, pType));
                        break;
                    }
                case ConditionType.NotEqual:
                    {
                        result =
                            Restrictions.Not(
                                Restrictions.Eq(propertyName, Convert.ChangeType(value, pType)));
                        break;
                    }
                case ConditionType.In:
                    {
                        List<object> valList = new List<object>();
                        foreach (object val in (object[])value)
                            valList.Add(Convert.ChangeType(val, pType));

                        result = Restrictions.In(propertyName, valList.ToArray());
                        break;
                    }
                case ConditionType.NotIn:
                    {

                        List<object> valList = new List<object>();
                        foreach (object val in (object[])value)
                            valList.Add(Convert.ChangeType(val, pType));

                        result =
                            Restrictions.Not(
                                Restrictions.In(propertyName, valList.ToArray()));
                        break;
                    }
                case ConditionType.Between:
                    {
                        object lowValue = Convert.ChangeType(((object[])value)[0], pType);
                        object highValue = Convert.ChangeType(((object[])value)[1], pType);

                        result = Restrictions.Between(propertyName, lowValue, highValue);
                        break;
                    }
                case ConditionType.IsNull:
                    {
                        result = Restrictions.IsNull(propertyName);
                        break;
                    }
                case ConditionType.IsNotNull:
                    {
                        result = Restrictions.IsNotNull(propertyName);
                        break;
                    }
                case ConditionType.Like:
                    {
                        result = Restrictions.Like(propertyName, value);
                        break;
                    }
                case ConditionType.NotLike:
                    {
                        result = Restrictions.Not(Restrictions.Like(propertyName, value));
                        break;
                    }
                case ConditionType.GreaterThan:
                    {
                        result = Restrictions.Gt(propertyName, value);
                        break;
                    }
                case ConditionType.GreaterThanOrEqual:
                    {
                        result = Restrictions.Ge(propertyName, value);
                        break;
                    }
                case ConditionType.LessThan:
                    {
                        result = Restrictions.Lt(propertyName, value);
                        break;
                    }
                case ConditionType.LessThanOrEqual:
                    {
                        result = Restrictions.Le(propertyName, value);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Condition type '{conditionType}' is not yet implemented.");
                    }
            }

            return result;
        }
    }
}
