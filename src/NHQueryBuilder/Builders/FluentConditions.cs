using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq.Expressions;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHQueryBuilder.Enums;
using System.Linq.Expressions;
using System.Reflection;

namespace NHQueryBuilder.Builders
{
    /// <summary>
    /// Provides a fluent API for building NHibernate query conditions for a given entity type.
    /// Supports chaining of various condition types (equality, range, null checks, set membership, ordering, selection, pagination, etc.)
    /// and composes them into NHibernate <see cref="ICriteria"/> queries.
    /// </summary>
    /// <typeparam name="T">The entity type for which the query conditions are being constructed.</typeparam>
    public class FluentConditions<T>
    {
        Alias alias;
        IList<FluentConditions<T>> _conditions;
        private JoinType joinType;
        internal Type TransformType { get; set; } = typeof(object);

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentConditions{T}"/> class with the specified join type.
        /// </summary>
        /// <param name="leftJoin">If true, uses a left outer join; otherwise, uses an inner join.</param>
        public FluentConditions(bool leftJoin)
        {
            alias = new Alias();
            _conditions = new List<FluentConditions<T>>();
            ConditionList = new List<Condition<T>>();
            joinType = leftJoin ? JoinType.LeftOuterJoin : JoinType.InnerJoin;
        }

        /// <summary>
        /// Gets the list of conditions for this instance.
        /// </summary>
        private IList<Condition<T>> ConditionList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentConditions{T}"/> class using an inner join.
        /// </summary>
        public FluentConditions() : this(false) { }

        private ConditionTree _conditionTree = null!;
        /// <summary>
        /// Gets the root <see cref="ConditionTree"/> for this query.
        /// </summary>
        internal ConditionTree Root
        {
            get
            {
                if (_conditionTree == null)
                    _conditionTree = new ConditionTree(typeof(T).Name, typeof(T), joinType);

                return _conditionTree;
            }
        }

        private FluentConditions<T> Insert(Expression<Func<T, object>> expression, ConditionType type)
        {
            return Insert(expression, type, null);
        }

        private FluentConditions<T> Insert(Expression<Func<T, object>>? expression, ConditionType conditionType, object? values)
        {
            FluentConditions<T> conditions;
            if (ConditionList.Count == 0)
            {
                if (conditionType != ConditionType.OrderByAsc && conditionType != ConditionType.OrderByDesc)
                {
                    if (_conditions.Any(a => a.ConditionList.First().ConditionType == ConditionType.OrderByAsc ||
                                             a.ConditionList.First().ConditionType == ConditionType.OrderByDesc))
                        throw new InvalidOperationException("Invalid use of where clause. Cannot add where clause after order by clause.");
                }

                conditions = new FluentConditions<T>();
                _conditions.Add(conditions);
            }
            else
            {
                if (conditionType == ConditionType.OrderByAsc || conditionType == ConditionType.OrderByDesc)
                    throw new InvalidOperationException("Incorrect use of order by clause. Order by clause cannot be used in combination with any other condition.");

                conditions = this;
            }

            conditions.ConditionList.Add(new Condition<T>(expression, conditionType, values));
            return conditions;
        }

        private Type GetPropertyType(Type classType, string propertyName)
        {
            PropertyInfo? info = classType.GetProperty(propertyName);

            if (info == null)
                throw new InvalidOperationException($"Unable to find property with a name: {propertyName} in class {classType}");

            return info.PropertyType;
        }

        /// <summary>
        /// Adds an equality condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> Equal(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.Equal, value);
        }

        /// <summary>
        /// Adds a not-equal condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> NotEqual(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.NotEqual, value);
        }

        /// <summary>
        /// Adds an 'in' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="values">The values to check for inclusion.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsIn(Expression<Func<T, object>> expression, object[] values)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.In, values);
        }

        /// <summary>
        /// Adds an 'in' condition to the query using a list of values.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="values">The list of values to check for inclusion.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsIn(Expression<Func<T, object>> expression, IList<object> values)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.In, values.ToArray());
        }

        /// <summary>
        /// Adds a 'not in' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="values">The values to check for exclusion.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsNotIn(Expression<Func<T, object>> expression, object[] values)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.NotIn, values);
        }

        /// <summary>
        /// Adds a 'not in' condition to the query using a list of values.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="values">The list of values to check for exclusion.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsNotIn(Expression<Func<T, object>> expression, IList<object> values)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.NotIn, values.ToArray());
        }

        /// <summary>
        /// Adds an 'is null' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsNull(Expression<Func<T, object>> expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.IsNull);
        }

        /// <summary>
        /// Adds an 'is not null' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsNotNull(Expression<Func<T, object>> expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.IsNotNull);
        }

        /// <summary>
        /// Adds a 'between' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="lowVal">The lower bound value.</param>
        /// <param name="highVal">The upper bound value.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> Between(Expression<Func<T, object>> expression, object lowVal, object highVal)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            object[] values = new object[2];
            values[0] = lowVal;
            values[1] = highVal;
            return Insert(expression, ConditionType.Between, values);
        }

        /// <summary>
        /// Adds a 'like' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsLike(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.Like, value);
        }

        /// <summary>
        /// Adds a 'not like' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to exclude.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> IsNotLike(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.NotLike, value);
        }

        /// <summary>
        /// Adds a 'greater than' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> GreaterThan(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.GreaterThan, value);
        }

        /// <summary>
        /// Adds a 'greater than or equal' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> GreaterThanOrEqual(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.GreaterThanOrEqual, value);
        }

        /// <summary>
        /// Adds a 'less than' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> LessThan(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.LessThan, value);
        }

        /// <summary>
        /// Adds a 'less than or equal' condition to the query.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public FluentConditions<T> LessThanOrEqual(Expression<Func<T, object>> expression, object value)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            return Insert(expression, ConditionType.LessThanOrEqual, value);
        }

        /// <summary>
        /// Adds a fetch (join) condition to the query.
        /// </summary>
        /// <param name="expression">The property expression to fetch.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public void Fetch(Expression<Func<T, object>> expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            Insert(expression, ConditionType.Fetch);
        }

        /// <summary>
        /// Adds an ascending order by clause to the query.
        /// </summary>
        /// <param name="expression">The property expression to order by.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public void OrderByAscending(Expression<Func<T, object>> expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            Insert(expression, ConditionType.OrderByAsc);
        }

        /// <summary>
        /// Adds a descending order by clause to the query.
        /// </summary>
        /// <param name="expression">The property expression to order by.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
        public void OrderByDescending(Expression<Func<T, object>> expression)
        {
            if (expression == null) 
                throw new ArgumentNullException(nameof(expression));

            Insert(expression, ConditionType.OrderByDesc);
        }

        /// <summary>
        /// Adds a select clause to the query for the specified property or properties.
        /// </summary>
        /// <param name="expressions">The property expression(s) to select.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressions"/> is null.</exception>
        public FluentConditions<T> Select(Expression<Func<T, object>> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions));

            if (expressions.Body is NewExpression newExpr)
            {
                // Handle anonymous type: new { x.Prop1, x.Prop2, ... }
                foreach (var expression in newExpr.Arguments)
                {
                    System.Linq.Expressions.Expression converted = System.Linq.Expressions.Expression.Convert(expression, typeof(object));
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, object>>(converted, expressions.Parameters);
                    ProcessSelectExpression(lambda);
                }
            }
            else
                ProcessSelectExpression(expressions);

            return this;
        }

        private void ProcessSelectExpression(Expression<Func<T, object>> expression)
        {
            ExpressionIterator iterator = new ExpressionIterator(expression.Body.ToString());
            Type parent = typeof(T);
            while (iterator.HasNext())
            {
                string property = iterator.Next();
                Type propType = GetPropertyType(parent, property);

                if (iterator.HasNext())
                    parent = propType;
                else
                {
                    var domainAssembly = typeof(T).Assembly;
                    if (IsNHibernateCollection(propType) || propType.Assembly == domainAssembly)
                    {
                        int count = propType.GenericTypeArguments.Count();
                        if (count > 1)
                            throw new NotSupportedException("This complex type of property is not yet supported for selection.");

                        Insert(expression, ConditionType.Fetch);
                        Insert(expression, ConditionType.Select);
                    }
                    else
                    {
                        Insert(expression, ConditionType.Select);
                    }
                }
            }
        }

        private bool IsNHibernateCollection(Type type)
        {
            return type.IsGenericType && 
                (type.GetGenericTypeDefinition() == typeof(IList<>) ||
                 type.GetGenericTypeDefinition() == typeof(ISet<>));
        }

        /// <summary>
        /// Limits the number of results returned by the query.
        /// </summary>
        /// <param name="take">The maximum number of results to return.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="take"/> is negative.</exception>
        public FluentConditions<T> Take(int take)
        {
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take), "Take value cannot be negative.");

            return Insert(null, ConditionType.Take, take);
        }

        /// <summary>
        /// Skips the specified number of results in the query.
        /// </summary>
        /// <param name="skip">The number of results to skip.</param>
        /// <returns>The updated <see cref="FluentConditions{T}"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="skip"/> is negative.</exception>
        public FluentConditions<T> Skip(int skip)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip), "Skip value cannot be negative.");

            return Insert(null, ConditionType.Skip, skip);
        }

        /// <summary>
        /// Gets the current instance for chaining OR conditions.
        /// </summary>
        public FluentConditions<T> OR => this;

        private ICriteria BuildConditions(ICriteria criteria)
        {
            ProjectionList projections = Projections.ProjectionList();

            foreach (var andCondition in _conditions)
            {
                bool hasCondition = false;
                Junction junction = Restrictions.Disjunction();
                foreach (var orCondition in andCondition.ConditionList)
                {
                    ConditionTree parent = Root;
                    if (orCondition.ConditionType == ConditionType.Skip)
                        criteria.Skip(orCondition.Values);
                    else if (orCondition.ConditionType == ConditionType.Take)
                        criteria.Take(orCondition.Values);
                    else
                    {
                        ExpressionIterator iterator = new ExpressionIterator(orCondition.Expression.Body.ToString());
                        while (iterator.HasNext())
                        {
                            string property = iterator.Next();
                            Type propType = GetPropertyType(parent.PropertyType, property);

                            if (parent.Exist(property))
                            {
                                if (iterator.HasNext())
                                    parent = parent.GetNode(property);
                                else if (orCondition.ConditionType == ConditionType.OrderByAsc)
                                    criteria.OrderByAscending(parent.Alias + "." + property);
                                else if (orCondition.ConditionType == ConditionType.OrderByDesc)
                                    criteria.OrderByDescending(parent.Alias + "." + property);
                                else if (orCondition.ConditionType == ConditionType.Select)
                                {
                                    var domainAssembly = typeof(T).Assembly;
                                    if (IsNHibernateCollection(propType) || propType.Assembly == domainAssembly)
                                    {
                                        parent = parent.GetNode(property);
                                        Type mainClass;

                                        if (propType.IsGenericType)
                                            mainClass = propType.GenericTypeArguments.First();
                                        else 
                                            mainClass = propType;

                                        var properties = mainClass.GetProperties().Where(x => !x.PropertyType.IsGenericType);
                                        foreach (PropertyInfo selectedProperty in properties)
                                            projections.Add(ConditionTree.GetProjection(selectedProperty.Name, parent.Alias, orCondition.ConditionType), 
                                                selectedProperty.Name);
                                    }
                                    else
                                        projections.Add(ConditionTree.GetProjection(property, parent.Alias, orCondition.ConditionType),
                                            property);
                                }
                                else if (orCondition.ConditionType != ConditionType.Fetch)
                                {
                                    hasCondition = true;
                                    junction.Add(ConditionTree.GetCriteria(property, propType, parent.Alias, orCondition.ConditionType, orCondition.Values));
                                }
                            }
                            else
                            {
                                if (iterator.HasNext() || orCondition.ConditionType == ConditionType.Fetch)
                                    parent = parent.Insert(property, propType, alias.Next(), joinType);
                                else
                                {
                                    if (orCondition.ConditionType == ConditionType.OrderByAsc)
                                        criteria.AddOrder(Order.Asc(parent.Alias + "." + property));
                                    else if (orCondition.ConditionType == ConditionType.OrderByDesc)
                                        criteria.AddOrder(Order.Desc(parent.Alias + "." + property));
                                    else if (orCondition.ConditionType == ConditionType.Select)
                                    {
                                        var domainAssembly = typeof(T).Assembly;
                                        if (IsNHibernateCollection(propType) || propType.Assembly == domainAssembly)
                                        {
                                            parent = parent.GetNode(property);
                                            Type mainClass;

                                            if (propType.IsGenericType)
                                                mainClass = propType.GenericTypeArguments.First();
                                            else
                                                mainClass = propType;

                                            var properties = mainClass.GetProperties().Where(x => !x.PropertyType.IsGenericType);
                                            foreach (PropertyInfo selectedProperty in properties)
                                                projections.Add(ConditionTree.GetProjection(selectedProperty.Name, parent.Alias, orCondition.ConditionType),
                                                    selectedProperty.Name);
                                        }
                                        else
                                            projections.Add(ConditionTree.GetProjection(property, parent.Alias, orCondition.ConditionType), 
                                                property);
                                    }
                                    else
                                    {
                                        hasCondition = true;
                                        junction.Add(ConditionTree.GetCriteria(property, propType, parent.Alias, orCondition.ConditionType, orCondition.Values));
                                    }
                                }
                            }
                        }
                    }
                }

                if (hasCondition)
                    criteria.Add(junction);
            }

            if (projections.Length > 0)
            {
                criteria.SetProjection(projections);
                if (TransformType != typeof(object))
                    criteria.SetResultTransformer(Transformers.AliasToBean(TransformType));
            }

            return criteria;
        }

        /// <summary>
        /// Builds the NHibernate <see cref="ICriteria"/> object using the current conditions.
        /// </summary>
        /// <param name="criteria">The base criteria to build upon.</param>
        /// <returns>The built <see cref="ICriteria"/> object.</returns>
        internal ICriteria Build(ICriteria criteria)
        {
            Root.Criteria = criteria;
            Root.Alias = criteria.Alias;

            return BuildConditions(criteria);
        }
    }
}
