﻿using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemEvidence.Model;

namespace ItemEvidence.DAL
{
    public class EmployeeRepositoryMemory : Subject, IEmployeeRepository
    {
        private int counter = 1;
        IList<Employee> employees = new List<Employee>(); //lista svih zaposlenika
        ISessionFactory factory = NHibernateService.BuildSessionFactory();

        public EmployeeRepositoryMemory()
        {
        }

        public void Load()
        {
            using (var session = factory.OpenSession())
            {

                //Employee empl1 = new Employee("Ante", "Tomić");
                //Employee empl2 = new Employee("Ana", "Anić");
                //Employee empl3 = new Employee("Pero", "Perić");
                //Employee empl4 = new Employee("Ines", "Inesić");
                //Employee empl5 = new Employee("Krumpiruša", "Šunka");
                //using (ITransaction trans = session.BeginTransaction())
                //{
                //    session.Save(empl1);
                //    session.Save(empl2);
                //    session.Save(empl3);
                //    session.Save(empl4);
                //    session.Save(empl5);
                //    trans.Commit();
                //}
                var str = "from Employee";
                IQuery query = session.CreateQuery(str);
                var empList = query.List<Employee>();

                employees = empList;
            }
        }

        private void NHRefreshList()
        {
            using (var session = factory.OpenSession())
            {
                var str = "from Employee";
                IQuery query = session.CreateQuery(str);
                var empList = query.List<Employee>().OrderBy(x => x.FirstName);

                employees.Clear();
                foreach (Employee emp in empList.ToList())
                {
                    employees.Add(emp);
                }
            }
        }

        public List<Employee> Employees
        {
            get
            {
                List<Employee> allEmployees = new List<Employee>();
                foreach (var e in employees)
                {
                    allEmployees.Add(e);
                }
                return allEmployees;
            }
        }
        
        /// <summary>
        /// Dodavanje novog zaposlenika
        /// </summary>
        /// <param name="employee">Zaposlenik kojeg treba dodati</param>
        /// <returns>Vraća potvrdu o uspješnom dodavanju zaposlenika</returns>
        public int Add(Employee employee)
        {
            if (!employees.Contains(employee))
            {
                //employee.Id = counter;
                //employees.Add(counter, employee);
                //counter++;

                using (var session = factory.OpenSession())
                {
                    using (ITransaction trans = session.BeginTransaction())
                    {
                        session.Save(employee);
                        trans.Commit();
                    }
                }

                NHRefreshList();

                Notify();

                return 1;
            }
            else throw new ItemEvidenceException("This employee already exists!");
        }

        /// <summary>
        /// Brisanje zaposlenika
        /// </summary>
        /// <param name="emplID">Identifikacijski broj zaposlenika kojeg treba izbrisati</param>
        /// <returns>Potvrda o uspješnoj radnji brisanja zaposlenika</returns>
        public int Delete(int emplID)
        {
            Employee empl = Get(emplID);
            if (employees.Contains(empl))
            {
                //Employee emp = Get(emplID);

                using (var session = factory.OpenSession())
                {
                    var emp = session.Get<Employee>(emplID);

                    if (emp.Items.Count > 0) throw new ItemEvidenceException("You cannot delete the employee if he/she has any item assigned!");

                    using (ITransaction trans = session.BeginTransaction())
                    {
                        session.Delete(emp);
                        trans.Commit();
                    }

                    NHRefreshList();
                }


                //employees.Remove(emplID);

                Notify();
                return 1;
            }
            else throw new ItemEvidenceException("Incorrect data!");
        }

        /// <summary>
        /// Dohvaćanje podataka o zaposleniku
        /// </summary>
        /// <param name="emplID">Identifikacijski broj zaposlenika</param>
        /// <returns>Vraća sve spremljene informacije o odabranom zaposleniku</returns>
        public Employee Get(int emplID)
        {
            Employee empl;
            using (var session = factory.OpenSession())
            {
                var str = String.Format("from EmployeeItemsList where EmpId = '{0}'", emplID);
                IQuery query = session.CreateQuery(str);
                IList<EmployeeItemsList> itemsList = query.List<EmployeeItemsList>();
                empl = session.Get<Employee>(emplID);
                empl.Items = itemsList;
                return empl;
            }
        }

        /// <summary>
        /// Uređivanje podataka o zaposleniku
        /// </summary>
        /// <param name="emplID">Identifikacijski broj zaposlenika</param>
        /// <param name="employee">Izmijenjeni podatci zaposlenika koje moramo spremiti</param>
        /// <returns>Potvrdu o uspješno uređenim podatcima o zaposleniku</returns>
        public int Update(int emplID, Employee employee)
        {
            using (var session = factory.OpenSession())
            {
                var e = session.Get<Employee>(emplID);

                e.FirstName = employee.FirstName;
                e.LastName = employee.LastName;

                using (ITransaction trans = session.BeginTransaction())
                {
                    session.Update(e);
                    trans.Commit();
                }
                NHRefreshList();
            }
            //employees[emplID] = employee;
            Notify();
            return 1;
        }
    }
}
