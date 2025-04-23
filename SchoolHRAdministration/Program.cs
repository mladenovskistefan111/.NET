using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRAdministrationAPI;

namespace SchoolHRAdministration
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal totalSalaries = 0;
            List<IEmployee> employees = new List<IEmployee>();

            SeedData(employees);

            // foreach(IEmployee employee in employees)
            // {
            //     totalSalaries += employee.Salary;
            // }

            // System.Console.WriteLine($"Total annual salaries (including bonus): {totalSalaries}");

            System.Console.WriteLine($"Total annual salaries (including bonus): {employees.Sum(e => e.Salary)}");
        }

        public static void SeedData(List<IEmployee> employees)
        {
            IEmployee teacher1 = new Teacher
            {
                Id = 1, 
                FirstName = "John",
                LastName = "Doe",
                Salary = 3000
            };

            employees.Add(teacher1);

            IEmployee teacher2 = new Teacher
            {
                Id = 1, 
                FirstName = "Johnny",
                LastName = "Sins",
                Salary = 5000
            };

            employees.Add(teacher2);

            IEmployee headOfDepartment = new HeadOfDepartment
            {
                Id = 1, 
                FirstName = "Tony",
                LastName = "Smith",
                Salary = 3000
            };

            employees.Add(headOfDepartment);

            IEmployee deputyHeadMaster = new DeputyHeadMaster
            {
                Id = 1, 
                FirstName = "Peter",
                LastName = "Griffin",
                Salary = 9000
            };

            employees.Add(deputyHeadMaster);

            IEmployee headMaster = new HeadMaster
            {
                Id = 1, 
                FirstName = "Albus",
                LastName = "Dumbledore",
                Salary = 10000
            };

            employees.Add(headMaster);
        }

    }

    public class Teacher : EmployeeBase
    {
        public override decimal Salary { get => base.Salary + (base.Salary * 0.02m);}
    }
    public class HeadOfDepartment : EmployeeBase
    {
        public override decimal Salary { get => base.Salary + (base.Salary * 0.03m);}
    }
    public class DeputyHeadMaster : EmployeeBase
    {
        public override decimal Salary { get => base.Salary + (base.Salary * 0.04m);}
    }
    public class HeadMaster : EmployeeBase
    {
        public override decimal Salary { get => base.Salary + (base.Salary * 0.05m);}
    }

}