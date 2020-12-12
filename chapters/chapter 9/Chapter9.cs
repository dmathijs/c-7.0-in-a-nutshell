using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace csharp7.chapters.Chapter9
{
    public class Chapter9 : ChapterModule
    {

        public Chapter9():base("Chapter 9")
        {

        }

        public override void RunExamples(){
            JoinWithSelectMany();
            ExecuteGroupJoin();
        }

        private void JoinWithSelectMany(){
            string[] players = { "Tom", "Jay", "Mary" };

            IEnumerable<string> query = from name1 in players
                                        from name2 in players
                                        where name1.CompareTo(name2) < 0
                                        select name1 + " vs " + name2;

            foreach(var pair in query){
                WriteLine(pair);
            }
        }

        private void ExecuteGroupJoin(){

            var purchase1 = new Purchase(1,0.5, "Tea");
            var purchase2 = new Purchase(1,1.0, "Coffee");
            var purchase3 = new Purchase(2,2.0, "Sugar");

            var customer1 = new Customer(1);
            var customer2 = new Customer(2);
            var customer3 = new Customer(3);

            IEnumerable<Customer> customers = new Customer[] { customer1, customer2, customer3 };
            IEnumerable<Purchase> purchases = new Purchase[] { purchase1, purchase2, purchase3 };

            // Groupjoin -> into after join makes it so that hierarchy can be created.
            var purchaseCollections = from cu in customers
                join pu in purchases on cu.customerId equals pu.customerId
                into customerPurchases
                select new { customer = cu.customerId, customerPurchases = customerPurchases };

            foreach(var purchaseCollection in purchaseCollections){
                foreach(var purchase in purchaseCollection.customerPurchases){
                    WriteLine($"{purchase.name} was purchased by customer with id {purchaseCollection.customer}.");
                }
            }
        }
    }

    public class Customer {

        public int customerId;

        public Customer(int customerId){
            this.customerId = customerId;
        }
    }

    public class Purchase {

        public int customerId;
        public string name;
        
        private double price;

        public Purchase(int customerId, double price, string name){
            this.customerId = customerId;
            this.price = price;
            this.name = name;
        }
    }
}