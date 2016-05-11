using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SQLite;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using NHibernate.Cfg;

namespace ItemEvidence.Model
{
    public class NHibernateService
    {
        private ISessionFactory sessionFactory;
        private Configuration configuration;

        public void Initialize()
        {
            sessionFactory = BuildSessionFactory();
        }

        public static ISessionFactory BuildSessionFactory()
        {
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("ItemRepositoryDb.db").ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Item>())
                    //.ExposeConfiguration(c => { new SchemaExport(c).Create(false, true); })
                    .ExposeConfiguration(c => { new SchemaUpdate(c).Execute(false, true); })
                    .BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            ISession session = sessionFactory.OpenSession();

            var export = new SchemaExport(configuration);

            export.Execute(true, true, false, session.Connection, null);

            return session;
        }

        public void Dispose()
        {
            if (sessionFactory != null) sessionFactory.Dispose();

            sessionFactory = null;
            configuration = null;
        }
    }
}
