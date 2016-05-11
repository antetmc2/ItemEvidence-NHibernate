using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ItemEvidence.Model
{
    public class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            Table("Item");
            Id(x => x.ItemId).GeneratedBy.Increment();
            Map(x => x.Name).Not.Nullable().Not.LazyLoad();
            Map(x => x.Description).Not.Nullable().Not.LazyLoad();
            Map(x => x.Quantity).Not.Nullable().Not.LazyLoad();
            Map(x => x.Author).Nullable().Not.LazyLoad();
            //DiscriminateSubClassesOnColumn<string>("Type");
        }
    }

    public class BookMap : SubclassMap<Book>
    {
        public BookMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Book");
        }
    }

    public class MonitorMap : SubclassMap<Monitor>
    {
        public MonitorMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Monitor");
        }
    }

    public class KeyboardMap : SubclassMap<Keyboard>
    {
        public KeyboardMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Keyboard");
        }
    }

    public class MouseMap : SubclassMap<Mouse>
    {
        public MouseMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Mouse");
        }
    }

    public class HeadsetMap : SubclassMap<Headset>
    {
        public HeadsetMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Headset");
        }
    }

    public class LaptopMap : SubclassMap<Laptop>
    {
        public LaptopMap()
        {
            Not.LazyLoad();
            //DiscriminatorValue("Laptop");
        }
    }
}
