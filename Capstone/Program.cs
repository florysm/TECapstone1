using System;
using Capstone.Classes;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            UI ui = new UI();
            VendingMachine vendingMachine = new VendingMachine(ui);

            do
            {
                vendingMachine.MainMenu();
            } while (vendingMachine.MachineIsOn);
        }
    }
}
