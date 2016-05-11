using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItemEvidence.Model
{
    public class EmployeeItemsListMap : ClassMap<EmployeeItemsList>
    {
        public EmployeeItemsListMap()
        {
            Table("EmployeeItemsList");
            Id(x => x.Id);
            References(x => x.Item).Column("ItemId").Not.Nullable();
            References(x => x.Employee).Column("EmpId");
            Map(x => x.NumberAssigned).Not.Nullable();
        }
    }
}
