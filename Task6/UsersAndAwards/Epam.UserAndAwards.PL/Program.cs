using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epam.UserAndAwards.Entities;
using Epam.UserAndAwards.DataLayer;

namespace UsersAndAwards
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter user data to store it in the datafile");
                User user = new User();
                Console.WriteLine("User ID:");
                user.Id = int.Parse(Console.ReadLine());
                Console.WriteLine("Username:");
                user.Name = Console.ReadLine();
                Console.WriteLine("Date of birth:");
                user.DateOfBirth = Console.ReadLine();
                Console.WriteLine("Age:");
                user.Age = Console.ReadLine();

                DataStorage dataStorage = new DataStorage();
                dataStorage.StoreData(user);
            }
        }
    }
}
