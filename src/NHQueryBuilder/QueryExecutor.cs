using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHQueryBuilder.Builders;
using System.Data;
using System.Linq.Expressions;

namespace NHQueryBuilder
{
    public static class QueryExecutor
    {
        /// <summary>
        /// Get the first record from the result. If no result found then default will be returned.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="predict">The predicate to filter the entity.</param>
        /// <param name="eagerFetch">If true, eagerly fetches lazy-loaded properties; otherwise, lazy loads them.</param>
        /// <returns>The first of type <typeparamref name="T"/> entity matching the predicate, or null if not found.</returns>
        /// <remarks>This method uses the current NHibernate session to query the database. If eagerFetch is true, all lazy-loaded properties will be fetched immediately, which can improve performance if those properties are needed later.
        /// Note: The session will be evicted as soon as </remarks>
        public static T? Get<T>(this ISession session, Expression<Func<T, bool>> predict, bool eagerFetch = false) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IQueryable<T> query = session.Query<T>();
            if (eagerFetch)
                query = query.FetchLazyProperties();

            return query.FirstOrDefault(predict);
        }

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> from the database based on a predicate and optional fetch properties.
        /// </summary>
        /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
        /// <typeparam name="TResult">The type of the property to fetch (used for fetching related properties).</typeparam>
        /// <param name="predict">A lambda expression that defines the conditions to filter the entity (e.g., p => p.Id == someId).</param>
        /// <param name="fetchProperties">A lambda expression that defines the related properties to fetch (e.g., p => p.RelatedEntity).</param>
        /// <param name="eagerFetch">A boolean flag indicating whether to use lazy fetching for properties (defaults to false).</param>
        /// <returns>The first matching entity or null if no match is found.</returns>
        public static T? Get<T, TResult>(this ISession session, Expression<Func<T, bool>> predict, Expression<Func<T, TResult>> fetchProperties, bool eagerFetch = false) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IQueryable<T> query = session.Query<T>();
            if (eagerFetch)
                query = query.FetchLazyProperties();

            query = query.Where(predict);

            if (fetchProperties != null)
            {
                ExpressionIterator exIterator = new ExpressionIterator(fetchProperties.Body);
                while (exIterator.HasNext())
                {
                    string property = exIterator.Next();
                    query = query.Fetch(PrepareExpression<T>(property));
                }
            }

            var rList = query.ToList();
            return rList.FirstOrDefault();
        }

        public static T? Get<T>(this ISession session, FluentConditions<T> conditions) where T : class
        {
            var result = GetList(session, conditions);
            return result.FirstOrDefault();
        }

        private static Expression<Func<T, object>> PrepareExpression<T>(string propertyName)
        {
            // Create the parameter expression (e.g., p => p.Property)
            ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "p");

            // Create the property access expression (e.g., p.Property)
            System.Linq.Expressions.Expression propertyAccess = System.Linq.Expressions.Expression.Property(parameter, propertyName);

            // Create and return the lambda expression (e.g., p => (object)p.Property)
            return System.Linq.Expressions.Expression.Lambda<Func<T, object>>(propertyAccess, parameter);
        }

        /// <summary>
        /// This function will return the first rowTResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eagerFetch"></param>
        /// <returns></returns>
        public static T? Get<T>(this ISession session, bool eagerFetch = false) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IQueryable<T> query = session.Query<T>();
            if (eagerFetch)
                query = query.FetchLazyProperties();

            return query.FirstOrDefault();
        }

        public static IList<T> GetList<T>(this ISession session, bool eagerFetch = false)
        {
            return GetList<T>(session, null, eagerFetch);
        }

        /// <summary>
        /// Retrieves a list of entities of type <typeparamref name="T"/> from the session using LINQ-based filtering.
        /// </summary>
        /// <param name="session">The NHibernate session used to query the database.</param>
        /// <param name="predict">An optional LINQ expression to filter the results.</param>
        /// <param name="eagerFetch">
        /// If <c>true</c>, fetches lazy-loaded properties eagerly using NHibernate's FetchLazyProperties extension.
        /// If <c>false</c>, returns entities with default lazy-loading behavior.
        /// </param>
        /// <returns>A list of entities matching the filter criteria.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
        public static IList<T> GetList<T>(this ISession session, Expression<Func<T, bool>>? predict, bool eagerFetch = false)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IList<T> retList;
            var query = session.Query<T>();
            if (predict != null)
                query = query.Where(predict);

            retList = eagerFetch ? query.FetchLazyProperties().ToList() : query.ToList();
            return retList;
        }

        /// <summary>
        /// Retrieves a list of entities of type <typeparamref name="T"/> using dynamic FluentConditions for filtering and query customization.
        /// </summary>
        /// <param name="session">The NHibernate session used to create the criteria query.</param>
        /// <param name="conditions">
        /// A <see cref="FluentConditions{T}"/> object that defines filtering, ordering, and other query behaviors.
        /// If <c>null</c>, all entities of type <typeparamref name="T"/> are returned.
        /// </param>
        /// <returns>A list of entities matching the specified conditions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
        public static IList<T> GetList<T>(this ISession session, FluentConditions<T> conditions) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IList<T> result;
            ICriteria query = session.CreateCriteria<T>();
            if (conditions == null)
                result = query.List<T>();
            else
                result = conditions.Build(query).List<T>();

            return result;
        }

        /// <summary>
        /// Executes a criteria query with dynamic projections defined in <see cref="FluentConditions{T}"/> and returns the result as a list of scalar values.
        /// </summary>
        /// <param name="session">The NHibernate session used to create the criteria query.</param>
        /// <param name="conditions">
        /// A <see cref="FluentConditions{T}"/> object that defines projections and filtering logic.
        /// Must include at least one <c>Select</c> condition to produce meaningful results.
        /// </param>
        /// <returns>
        /// A list of projected values as <see cref="object"/> instances.
        /// Each item represents a row, and may be a scalar or an <see cref="object[]"/> depending on the number of selected columns.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
        public static IList<TTransform> GetProjectedList<T, TTransform>(this ISession session, FluentConditions<T> conditions) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            conditions.TransformType = typeof(TTransform);

            IList<TTransform> result;
            ICriteria query = session.CreateCriteria<T>();
            if (conditions == null)
                result = query.List<TTransform>();
            else
                result = conditions.Build(query).List<TTransform>();

            return result;
        }

        /// <summary>
        /// Executes a criteria query with dynamic projections defined in <see cref="FluentConditions{T}"/> and returns the result as a list of scalar values.
        /// </summary>
        /// <param name="session">The NHibernate session used to create the criteria query.</param>
        /// <param name="conditions">
        /// A <see cref="FluentConditions{T}"/> object that defines projections and filtering logic.
        /// Must include at least one <c>Select</c> condition to produce meaningful results.
        /// </param>
        /// <returns>
        /// A list of projected values as <see cref="object"/> instances.
        /// Each item represents a row, and may be a scalar or an <see cref="object[]"/> depending on the number of selected columns.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
        public static IList<object> GetProjectedList<T>(this ISession session, FluentConditions<T> conditions) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            IList<object> result;
            ICriteria query = session.CreateCriteria<T>();
            if (conditions == null)
                result = query.List<object>();
            else
                result = conditions.Build(query).List<object>();

            return result;
        }

        public static int GetCount<T>(this ISession session) where T : class
        {
            return GetCount<T>(session, null);
        }

        /// <summary>
        /// Retrieves the total count of entities of type <typeparamref name="T"/> that match the specified <see cref="FluentConditions{T}"/>.
        /// </summary>
        /// <param name="session">The NHibernate session used to create the criteria query.</param>
        /// <param name="conditions">
        /// A <see cref="FluentConditions{T}"/> object that defines filtering logic for the count query.
        /// If <c>null</c>, the count includes all entities of type <typeparamref name="T"/>.
        /// </param>
        /// <returns>The number of entities that satisfy the given conditions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
        public static int GetCount<T>(this ISession session, FluentConditions<T>? conditions) where T : class
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            int result = 0;

            ICriteria query = session.CreateCriteria<T>();
            if (conditions != null)
                query = conditions.Build(query);

            result = query.SetProjection(Projections.Count(Projections.Id())).UniqueResult<int>();
            return result;
        }

        public static T Load<T>(this ISession session, long key) where T : class
        {
            return session.Load<T>(key);
        }

        /// <summary>
        /// Load the object based on the composite key.
        /// </summary>
        /// <typeparam name="T">Type of class to load</typeparam>
        /// <param name="compositeKey">Pass an instance of composite key class. Ensure that the anonymous type with properties match with the properties of the requested class.</param>
        /// <returns></returns>
        public static T Load<T>(this ISession session, object compositeKey) where T : class
        {
            return session.Load<T>(compositeKey);
        }

        /// <summary>
        /// Use this function to execute the script
        /// </summary>
        /// <param name="text">Script content</param>
        public static void ExecuteScript(this ISession session, string text)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            using (var transaction = session.BeginTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = text;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}
