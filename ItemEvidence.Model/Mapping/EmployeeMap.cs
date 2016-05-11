using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItemEvidence.Model
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employee");
            Id(x => x.EmpId).GeneratedBy.Increment();
            Map(x => x.FirstName).Not.Nullable().Not.LazyLoad();
            Map(x => x.LastName).Not.Nullable().Not.LazyLoad();
            HasMany(x => x.Items).Table("Item").KeyColumn("EmpId").Not.LazyLoad().Inverse().Cascade.All();
        }
    }
}
