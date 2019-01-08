using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Epam.UserAndAwards.Entities;
using Newtonsoft.Json;
using System.Resources;

namespace Epam.UserAndAwards.DataLayer
{
    public class DataStorage
    {
        public void StoreData(User user)
        {
            File.AppendAllText(@"C:\Users\Alexandr\source\repos\UsersAndAwards\Epam.UserAndAwards.DataLayer\UserData.json", JsonConvert.SerializeObject(user, Formatting.Indented) + Environment.NewLine);
        }
    }
}
