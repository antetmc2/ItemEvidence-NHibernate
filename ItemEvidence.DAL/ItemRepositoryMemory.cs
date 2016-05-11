﻿using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemEvidence.Model;

namespace ItemEvidence.DAL
{
    public class ItemRepositoryMemory : Subject, IItemRepository
    {
        private int counter = 1;
        private IList<Item> items = new List<Item>(); //lista svih dodanih proizvoda
        EmployeeRepositoryMemory employeeRepo = new EmployeeRepositoryMemory();
        ISessionFactory factory = NHibernateService.BuildSessionFactory();

        public ItemRepositoryMemory(EmployeeRepositoryMemory e = null)
        {
            employeeRepo = e;

            //Load();

        }

        public void Load()
        {
            IList<Item> itemsList = new List<Item>();
            using (var session = factory.OpenSession())
            {
                //Item item1 = ItemFactory.OrderItem(ItemTypes.Laptop, "Dell Inspiron 5558", "Odličan laptop za faks i programiranje!", 4);
                //Item item2 = ItemFactory.OrderItem(ItemTypes.Book, "Objektno oblikovanje", "Must-have knjiga!", 50, "Zvonimir Vanjak");
                //Item item3 = ItemFactory.OrderItem(ItemTypes.Monitor, "Philips monitor", "24 inča, full HD", 5);
                //Item item4 = ItemFactory.OrderItem(ItemTypes.Mouse, "Genius miš", "Potreban za rad!", 30);
                //Item item5 = ItemFactory.OrderItem(ItemTypes.Keyboard, "Genius bežična tipkovnica", "Veoma potrebna za rad!", 30);
                //Item item6 = ItemFactory.OrderItem(ItemTypes.Headset, "Sony bežične slušalice", "Puno pomaže, izdržljive!", 20);
                //Item item7 = ItemFactory.OrderItem(ItemTypes.Book, "Stranac", "Malo književnosti", 40, "Albert Camus");
                //Item item8 = ItemFactory.OrderItem(ItemTypes.Laptop, "Toshiba Satellite L850-1V6", "Užas od laptopa", 1);

                //using (ITransaction trans = session.BeginTransaction())
                //{
                //    session.Save(item1);
                //    session.Save(item2);
                //    session.Save(item3);
                //    session.Save(item4);
                //    session.Save(item5);
                //    session.Save(item6);
                //    session.Save(item7);
                //    session.Save(item8);
                //    trans.Commit();
                //}

                var str = "from Item";
                IQuery query = session.CreateQuery(str);
                itemsList = query.List<Item>();

                items = itemsList;

            }

            //foreach (Item i in items.ToList())
            //{
            //    foreach (Employee emp in employeeRepo.Employees)
            //    {
            //        try
            //        {
            //            AssignToEmployee(i.ItemId, emp.EmpId, 1);
            //        }
            //        catch { continue; }
            //    }
            //}

        }

        private void NHRefreshList()
        {
            using (var session = factory.OpenSession())
            {
                var str = "from Item";
                IQuery query = session.CreateQuery(str);
                IList<Item> itemsList = query.List<Item>();

                items.Clear();
                foreach (Item i in itemsList) items.Add(i);
            }
        }

        public List<Item> Items
        {
            get
            {
                List<Item> allItems = new List<Item>();
                foreach(var i in items)
                {
                    allItems.Add(i);
                }
                return allItems;
            }
        }

        public List<Employee> ListOfEmployees
        {
            get { return employeeRepo.Employees; }
        }

        public int NumberOfItems
        {
            get { return items.Count; }
        }

        private bool ContainsItem(Item item)
        {
            foreach(var i in items)
            {
                if (item.ItemId == i.ItemId || item.Name.Trim() == i.Name.Trim()) throw new ItemEvidenceException("This item already exists!");
            }
            return false;
        }

        /// <summary>
        /// Dodavanje opreme u skladište
        /// </summary>
        /// <param name="item">Odabrana oprema</param>
        /// <returns>Potvrda o uspješno dodanoj opremi</returns>
        public int Add(Item item)
        {
            if (!items.Contains(item))
            {
                if (!ContainsItem(item))
                {
                    //item.Id = counter;
                    //items.Add(counter, item);
                    //counter++;

                    using (var session = factory.OpenSession())
                    {
                        using (ITransaction trans = session.BeginTransaction())
                        {
                            session.Save(item);
                            trans.Commit();
                        }
                    }
                    NHRefreshList();

                }
                Notify();
                return 1;
            }
            else throw new ItemEvidenceException("This item already exists in the memory!");
        }

        /// <summary>
        /// Vraćanje opreme iz skladišta
        /// </summary>
        /// <param name="itemID">Identifikacijski broj odabrane opreme</param>
        /// <returns>Potvrda o uspješno dodanoj opremi</returns>
        public int Delete(int itemID)
        {
            using (var session = factory.OpenSession())
            {

                var str = "from Item";
                IQuery query = session.CreateQuery(str);
                IList<Item> itemsList = query.List<Item>();

                var item = session.Get<Item>(itemID);

                if (employeeRepo != null)
                {
                    foreach (var employee in employeeRepo.Employees)
                    {
                        if (employee.ItemExists(item)) throw new ItemEvidenceException("Assigned item cannot be removed until all amounts are removed!");
                    }
                }

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Delete(item);
                    trans.Commit();
                }
                NHRefreshList();
            }
            Notify();
            return 1;
        }

        /// <summary>
        /// Dohvaćanje informacija o odabranoj opremi
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <returns>Dohvaćene informacije o opremi</returns>
        public Item Get(int itemID)
        {
            Item item;
            using (var session = factory.OpenSession())
            {
                item = session.Get<Item>(itemID);
            }
            return item;
        }

        public Employee GetEmployee(int emplId)
        {
            return employeeRepo.Get(emplId);
        }

        /// <summary>
        /// Naručivanje dodatnih komada opreme
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <param name="quantity">Broj naručenih dodatnih komada opreme</param>
        /// <returns>Potvrda o uspješnom naručivanju opreme</returns>
        public int OrderMoreItems(int itemID, int quantity)
        {
            if (quantity > 20) throw new ItemEvidenceException("You cannot order more than 20 same items!");
            //items[itemID].Quantity += quantity;

            var item = Get(itemID);
            using (var session = factory.OpenSession())
            {
                var itemUpd = session.Get<Item>(itemID);
                item.Quantity += quantity;

                itemUpd.Quantity = item.Quantity;

                using(ITransaction trans = session.BeginTransaction())
                {
                    session.Update(itemUpd);
                    trans.Commit();
                    Notify();
                }
            }

            return 1;
        }
        /// <summary>
        /// Vraćanje određenog broja opreme iz skladišta
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <param name="number">Broj komada koji se želi vratiti</param>
        /// <returns>Potvrda o usješno vraćenoj opremi</returns>
        public int ReturnSomeItems(int itemID, int number)
        {
            var item = Get(itemID);

            if(item.Quantity < number) throw new ItemEvidenceException("You cannot return more items than available!");
            using (var session = factory.OpenSession())
            {
                var itemUpd = session.Get<Item>(itemID);
                item.Quantity -= number;

                itemUpd.Quantity = item.Quantity;

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Update(itemUpd);
                    trans.Commit();
                    Notify();
                }
            }
            return 1;
        }

        public int ReturnAllItems(int itemID)
        {
            var item = Get(itemID);
            using (var session = factory.OpenSession())
            {
                var itemUpd = session.Get<Item>(itemID);
                item.Quantity = 0;

                itemUpd.Quantity = item.Quantity;

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Update(itemUpd);
                    trans.Commit();
                    Notify();
                }
            }
            return 1;
        }

        /// <summary>
        /// Uređivanje podataka o opremi
        /// </summary>
        /// <param name="itemID">Identifikacijski broj nepromijenjene opreme</param>
        /// <param name="item">Oprema s kojom treba zamijeniti nepromijenjenu opremu</param>
        /// <returns>Potvrda o uspješnoj izmjeni podataka</returns>
        public int Update(int itemID, Item item)
        {
            var itemFetched = Get(itemID);

            using (var session = factory.OpenSession())
            {

                var str = "from Item";
                IQuery query = session.CreateQuery(str);
                IList<Item> itemsList = query.List<Item>();

                var itemForUpdate = session.Get<Item>(itemID);
                itemForUpdate.Name = item.Name;
                itemForUpdate.Description = item.Description;
                itemForUpdate.Author = item.Author;
                itemForUpdate.Quantity = item.Quantity;

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Update(itemForUpdate);
                    trans.Commit();
                }
                NHRefreshList();

                foreach (var employee in employeeRepo.Employees)
                {
                    if (employee.ItemExists(itemFetched)) employee.UpdateItem(itemFetched, itemForUpdate);
                }
            }

            Notify();
            return 1;
        }

        /// <summary>
        /// Pridruži određeni broj komada opreme zaposleniku
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <param name="emplID">Identifikacijski broj zaposlenika</param>
        /// <param name="quantity">Broj komada opreme</param>
        /// <returns>Potvrda o uspješnom pridruživanju opreme zaposleniku</returns>
        public int AssignToEmployee(int itemID, int emplID, int quantity)
        {
            var employee = employeeRepo.Get(emplID);
            var item = Get(itemID);
            Employee employeeForUpdate;

            if (quantity <= item.Quantity)
            {
                //item.AssignToEmployee(item, employee, quantity);
                //item.Quantity -= quantity;

                //Update(itemID, item);

                using(var session = factory.OpenSession())
                {
                    employeeForUpdate = session.Get<Employee>(emplID);
                    employee = employeeRepo.Get(emplID);

                    item.AssignToEmployee(item, employee, quantity);
                    item.Quantity -= quantity;

                    Update(itemID, item);
                }
                using (var session = factory.OpenSession())
                {
                    employeeForUpdate.Items = employee.Items;

                    using (ITransaction trans = session.BeginTransaction())
                    {
                        foreach (var i in employeeForUpdate.Items)
                        {
                            session.SaveOrUpdate(i);
                        }
                        session.Update(employeeForUpdate);
                        trans.Commit();
                        Notify();
                    }
                }

                //using (var session = factory.OpenSession())
                //{
                //    employeeForUpdate = session.Get<Employee>(emplID);
                //    employee = employeeRepo.Get(emplID);

                //    employeeForUpdate.Items = employee.Items;
                //}
                //employeeRepo.Update(emplID, employeeForUpdate);
                    return 1;
            }
            else throw new ItemEvidenceException("You need to order more items, there is not enough items for assigning!");
        }

        /// <summary>
        /// Oduzmi sve komade neke opreme od zaposlenika
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <param name="emplID">Identifikacijski broj zaposlenika</param>
        /// <returns>Potvrda o uspješnosti oduzimanja komada opreme</returns>
        public int TakeAllFromEmployee(int itemID, int emplID)
        {
            var employee = employeeRepo.Get(emplID);
            var item = Get(itemID);

            Employee employeeForUpdate;
            var quantity = employee.GetItem(item).NumberAssigned;

            using (var session = factory.OpenSession())
            {
                employeeForUpdate = session.Get<Employee>(emplID);
                employee = employeeRepo.Get(emplID);

                employee.TakeAllItems(item);
                item.Quantity += quantity;

                Update(itemID, item);
            }
            using (var session = factory.OpenSession())
            {
                employeeForUpdate.Items = employee.Items;

                using (ITransaction trans = session.BeginTransaction())
                {
                    var str = String.Format("from EmployeeItemsList where EmpId = '{0}' and ItemId = '{1}'", emplID, itemID);
                    IQuery query = session.CreateQuery(str);
                    var empList = query.List<EmployeeItemsList>()[0];
                    //empList.NumberAssigned = 0;

                    session.Delete(empList);
                    //session.Update(employeeForUpdate);
                    trans.Commit();
                    Notify();
                }
            }

            return 1;




        }

        /// <summary>
        /// Oduzmi odrešeni broj komada opreme od zaposlenika
        /// </summary>
        /// <param name="itemID">Identifikacijski broj opreme</param>
        /// <param name="emplID">Identifikacijski broj zaposlenika</param>
        /// <param name="quantity">Broj komada opreme za oduzimanje</param>
        /// <returns>Potvrda o uspješnom oduzimanju opreme</returns>
        public int TakeFromEployee(int itemID, int emplID, int quantity)
        {
            var employee = employeeRepo.Get(emplID);
            var item = Get(itemID);

            Employee employeeForUpdate;

            using (var session = factory.OpenSession())
            {
                employeeForUpdate = session.Get<Employee>(emplID);
                employee = employeeRepo.Get(emplID);

                employee.TakeItems(item, quantity);
                item.Quantity += quantity;

                Update(itemID, item);
            }
            using (var session = factory.OpenSession())
            {
                employeeForUpdate.Items = employee.Items;

                using (ITransaction trans = session.BeginTransaction())
                {
                    var str = String.Format("from EmployeeItemsList where EmpId = '{0}' and ItemId = '{1}'", emplID, itemID);
                    IQuery query = session.CreateQuery(str);
                    var empList = query.List<EmployeeItemsList>()[0];
                    empList.NumberAssigned -= quantity;

                    session.Update(empList);
                    //session.Update(employeeForUpdate);
                    trans.Commit();
                    Notify();
                }
            }

            return 1;
        }

        public List<EmployeeItems> EmployeesPerItem(int itemID)
        {
            var item = Get(itemID);
            EmployeeItemsList pair;
            List<EmployeeItems> employeeItems = new List<EmployeeItems>();

            foreach(var emp in employeeRepo.Employees)
            {

                using (var session = factory.OpenSession())
                {
                    var employee = session.Get<Employee>(emp.EmpId);
                    if (employee.ItemExists(item))
                    {
                        pair = employee.GetItem(item);
                        employeeItems.Add(new EmployeeItems(employee.EmpId, employee.FirstName, employee.LastName, pair.NumberAssigned));
                    }
                }

                //if (emp.ItemExists(item))
                //{
                //    pair = emp.GetItem(item);
                //    employeeItems.Add(new EmployeeItems(emp.EmpId, emp.FirstName, emp.LastName, pair.NumberAssigned));
                //}
            }

            return employeeItems;
        }
    }
}
