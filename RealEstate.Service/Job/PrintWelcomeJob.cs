using RealEstate.DB.Entities.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Service.Job
{
    public class PrintWelcomeJob : IPrintWelcomeJob
    {
        public void PrintWelcome()
        {
            using (var context = new RealEstateContext())
            {
                var owners = context.RealEstateOwner.Where(u => u.Id>0).ToList();
                foreach (var owner in owners)
                {
                    //sendWelcomeMail(user.Email);
                    Console.WriteLine($"Welcome to the Real Estate, Your Id:{owner.Id} - Mail:{owner.Email}!");
                }
            }
        }

        public void CleanUserTable()
        {
            using (var context = new RealEstateContext())
            {
                var owners = context.RealEstateOwner.Where(u => u.Id>0);
                foreach (var owner in owners)
                    Console.WriteLine($" Removed user: {owner.Email}. ({DateTime.Now})");
                context.RealEstateOwner.RemoveRange(owners);
                context.SaveChanges();
            }
        }
    }
}
