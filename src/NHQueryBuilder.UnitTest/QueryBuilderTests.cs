using NHibernate;
using NUnit.Framework;
using NHQueryBuilder.Builders;
using NHQueryBuilder.DatabaseSetup.Entities;

namespace NHQueryBuilder.UnitTest
{
    [TestFixture]
    public class QueryBuilderTests : AbstractUnitTest
    {
        [TestCase(TestName = "Equal")]
        public void Equal_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Equal(a => a.Name, "Author 1");
                Author? result = session.Get(conditions);

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo("Author 1"));
            }
        }

        [TestCase(TestName = "NotEqual")]
        public void NotEqual_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.NotEqual(a => a.City, "London");
                var result = session.GetList(conditions);
                
                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.City != "London"), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsIn")]
        public void IsIn_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.IsIn(a => a.Country, ["India", "USA"]);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.Country == "India" || x.Country == "USA"), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsNotIn")]
        public void IsNotIn_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.IsNotIn(a => a.Country, ["India", "USA"]);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.Country != "India" && x.Country != "USA"), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsNull")]
        public void IsNull_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Loan>();
                conditions.IsNull(a => a.ReturnDate);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.ReturnDate == null), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsNotNull")]
        public void IsNotNull_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Loan>();
                conditions.IsNotNull(a => a.ReturnDate);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.ReturnDate != null), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "Between")]
        public void Between_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Between(a => a.BirthDate, new DateTime(1970, 1, 1), new DateTime(1990, 1, 1));
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x =>
                x.BirthDate >= new DateTime(1970, 1, 1) &&
                x.BirthDate <= new DateTime(1990, 1, 1)), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsLike")]
        public void IsLike_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.IsLike(a => a.Name, "%Author%");
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.Name.Contains("Author")), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "IsNotLike")]
        public void IsNotLike_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.IsNotLike(a => a.Name, "%Author%");
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => !x.Name.Contains("Author")), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "GreaterThan")]
        public void GreaterThan_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.GreaterThan(a => a.Key, 5);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.Key > 5), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "GreaterThanOrEqual")]
        public void GreaterThanOrEqual_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.GreaterThanOrEqual(a => a.Key, 5);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.Any(x => x.Key == 5), Is.EqualTo(true));
                Assert.That(result.All(x => x.Key >= 5), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "LessThan")]
        public void LessThan_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.LessThan(a => a.Key, 5);
                var result = session.GetList(conditions);

                //Assert.That(conditions.ConditionList.First().ConditionType, Is.EqualTo(ConditionType.LessThan));
                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.All(x => x.Key < 5), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "LessThanOrEqual")]
        public void LessThanOrEqual_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.LessThanOrEqual(a => a.Key, 5);
                var result = session.GetList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(result.Any(x => x.Key == 5), Is.EqualTo(true));
                Assert.That(result.All(x => x.Key <= 5), Is.EqualTo(true));
            }
        }

        [TestCase(TestName = "Select single column")]
        public void SelectSingleColumn_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Select(a => a.Name);
                var result = session.GetProjectedList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                foreach (object name in result)
                {
                    Assert.That(name, Is.TypeOf<string>());
                    Assert.That(name, Is.Not.Null.And.Not.Empty);
                }
            }
        }

        [TestCase(TestName = "Select multiple columns")]
        public void SelectMultipleColumns_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Select(a => new { a.Key, a.Name, a.Nationality});
                var result = session.GetProjectedList(conditions);

                Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
                foreach (object row in result)
                {
                    object[] columns = (object[]) row;
                    Assert.That(columns, Is.TypeOf<object[]>());
                    Assert.That(columns.Length, Is.EqualTo(3));
                }
            }
        }

        [TestCase(TestName = "Take")]
        public void Take_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Take(10);
                var result = session.GetList(conditions);
            }
        }

        [TestCase(TestName = "Skip")]
        public void Skip_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.Skip(10);
                var result = session.GetList(conditions);
            }
        }

        [TestCase(TestName = "OrderByAscending")]
        public void OrderByAscending_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.OrderByAscending(a => a.Name);
                var result = session.GetList(conditions);

                Assert.That(result.Select(x => x.Name), Is.Ordered.Ascending);
            }
        }

        [TestCase(TestName = "OrderByDescending")]
        public void OrderByDescending_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Author>();
                conditions.OrderByDescending(a => a.Name);
                var result = session.GetList(conditions);

                Assert.That(result.Select(x => x.Name), Is.Ordered.Descending);
            }
        }

        [TestCase(TestName = "Fetch")]
        public void Fetch_Condition()
        {
            using (ISession session = Session.GetSession())
            {
                var conditions = new FluentConditions<Book>();
                conditions.Fetch(b => b.Author);
                var result = session.GetList(conditions);
            }
        }
    }
}
