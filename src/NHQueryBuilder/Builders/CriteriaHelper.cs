using NHibernate;
using NHibernate.Criterion;

namespace NHQueryBuilder.Builders
{
    internal static class CriteriaHelper
    {
        public static void Skip(this ICriteria criteria, object? value)
        {
            if (value != null)
            {
                int skipRecords = Convert.ToInt32(value);
                criteria.SetFirstResult(skipRecords);
            }
        }

        public static void Take(this ICriteria criteria, object? value)
        {
            if (value != null)
            {
                int takeRecords = Convert.ToInt32(value);
                criteria.SetMaxResults(takeRecords);
            }
        }

        public static void OrderByAscending(this ICriteria criteria, string propertyName)
        {
            criteria.AddOrder(Order.Asc(propertyName));
        }

        public static void OrderByDescending(this ICriteria criteria, string propertyName)
        {
            criteria.AddOrder(Order.Desc(propertyName));
        }

        public static void Select(this ICriteria criteria, string propertyName)
        {
            criteria.SetProjection(Projections.Property(propertyName));
        }
    }
}
