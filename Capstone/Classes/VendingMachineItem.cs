using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Classes
{
    public class VendingMachineItem
    {
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Message { get
            {
                switch(Type)
                {
                    case "Chips":
                        return "Crunch Crunch, Yum!";
                    case "Candy":
                        return "Munch Munch, Yum!";
                    case "Drink":
                        return "Glug Glug, Yum!";
                    case "Gum":
                        return "Chew Chew, Yum!";
                    default:
                        return "Crunch Crunch, Yum!";
                }
            }
        }
        public string SlotId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public VendingMachineItem(string slotId, string name, decimal price, string type, int quantity = 5)
        {
            SlotId = slotId;
            Name = name;
            Price = price;
            Type = type;
            Quantity = quantity;
        }
    }
}
