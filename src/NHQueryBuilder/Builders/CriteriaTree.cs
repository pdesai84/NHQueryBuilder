using NHibernate;

namespace NHQueryBuilder.Builders
{
    internal class CriteriaTree
    {
        public Type ParentType { get; set; }
        public ICriteria Criteria { get; set; }
        public IList<CriteriaTree> Children { get; set; }

        public CriteriaTree()
        {
            Children = new List<CriteriaTree>();
        }
    }
}
