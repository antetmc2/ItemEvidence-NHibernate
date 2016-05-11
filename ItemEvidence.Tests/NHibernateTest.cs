using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ItemEvidence.Model;

namespace ItemEvidence.Tests
{
    public class NHibernateTest
    {
        [Fact]
        public void TestConnection()
        {
            ISessionFactory factory = NHibernateService.BuildSessionFactory();

            Assert.NotNull(factory);

            using (var session = factory.OpenSession())
            {
                var h_stmt = "from Item";
                IQuery query = session.CreateQuery(h_stmt);

                Laptop laptop = new Laptop();
                laptop.Name = "Dell Inspiron 5558";
                laptop.Quantity = 10;
                laptop.Description = "Odličan laptop!";

                Mouse mouse = new Mouse ("Genius miš", "(Y)", 100);

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Save(laptop);
                    session.Save(mouse);
                    trans.Commit();
                }

                foreach(var a in Enum.GetNames(typeof(ItemTypes)))
                {

                }

                IList<Item> itemsList = query.List<Item>();

                //h_stmt = "from Employee";
                //query = session.CreateQuery(h_stmt);

                //using (ITransaction trans = session.BeginTransaction())
                //{
                //    session.Save(emp);
                //    trans.Commit();
                //}

                //IList<Employee> empList = query.List<Employee>();

            }
        }
    }
}
