using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Capstone.Classes
{
    public class VendingMachine
    {
        //Todo: Private set??? Custom get??? 
        public decimal TotalMoney { get; private set; }
        public bool MachineIsOn { get; private set; } = true;
        public Dictionary<VendingMachineItem, int> StockedItems { get; set; } = new Dictionary<VendingMachineItem, int>();

        private static Dictionary<string, decimal> SalesRep { get; set; } = new Dictionary<string, decimal>();

        private static string Folder = Directory.GetCurrentDirectory();
        private static string FileName = "vendingmachine.csv";
        private static string SalesDataFileName = "salesdata.txt";

        public string FullPath = Path.Combine(Folder, FileName);

        private UI UserInterface { get; set; }
        public VendingMachine(UI ui)
        {
            UserInterface = ui;
            ProcessFile();
            if (!File.Exists(Path.Combine(Folder, SalesDataFileName)))
            {
                CreateSalesDataFile();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            UserInterface.WriteToScreen("Welcome to the Vendo-Matic800 (by UmbrellaCorp)!\n");
        }

        public void ProcessFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(FullPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        string[] lineArray = line.Split("|", StringSplitOptions.RemoveEmptyEntries);
                        decimal price = Convert.ToDecimal(lineArray[2]);

                        VendingMachineItem newItem = new VendingMachineItem(lineArray[0], lineArray[1], price, lineArray[3]);

                        StockedItems.Add(newItem, newItem.Quantity);
                        SalesRep.Add(newItem.Name, newItem.Price);
                    }
                }
            }
            catch (Exception e)
            {
                UserInterface.WriteToScreen(e.Message);
            }
        }
        public void DisplayItems()
        {
            foreach (KeyValuePair<VendingMachineItem, int> item in StockedItems)
            {
                UserInterface.WriteToScreen($"{item.Key.SlotId}) {item.Key.Name} - {item.Key.Type} - ${item.Key.Price.ToString("0.00")} : {item.Value} left!");
            }

            UserInterface.WriteToScreen("\n\n");
        }

        public void MainMenu()
        {
            int input = 0;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;

                input = UserInterface.IntegerInput("(1) Display Vending Machine Items\n(2) Purchase\n(3) Exit");
                if (input < 1 || input > 4)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    UserInterface.WriteToScreen("Please input a valid menu number!\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.WriteToScreen("Please input a valid menu number!\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (input == 1)
            {
                DisplayItems();
            }
            else if (input == 2)
            {
                PurchaseMenu();
            }
            else if (input == 3)
            {
                MachineIsOn = false;
                Console.ForegroundColor = ConsoleColor.Green;
                UserInterface.WriteToScreen("Vendo-Matic800 says goodbye and thank you for your money!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (input == 4)
            {
                GenerateSalesReport();
            }
        }

        public void PurchaseMenu()
        {
            int input = 0;
            try
            {
                input = UserInterface.IntegerInput($"\nCurrent Money Provided: ${TotalMoney.ToString("0.00")}\n(1) Feed Money\n(2) Select Product\n(3) Finish Transaction");
                if (input < 1 || input > 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    UserInterface.WriteToScreen("Please input a valid menu number!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.WriteToScreen("Please input a valid menu number!");
                Console.ForegroundColor = ConsoleColor.White;

            }

            if (input == 1)
            {
                FeedMoney();
            }
            else if (input == 2)
            {
                SelectProduct();
            }
            else if (input == 3)
            {
                FinishTransaction();
            }
            else
            {
                PurchaseMenu();
            }
        }

        public void SelectProduct()
        {
            DisplayItems();

            string entry = UserInterface.StringInput("Please enter code of desired item: ");
            string code = entry.ToUpper();
            bool codeExists = false;
            bool soldOut = false;
            bool notEnoughFunds = false;
            VendingMachineItem currentItem = null;
            foreach (KeyValuePair<VendingMachineItem, int> item in StockedItems)
            {
                if (item.Key.SlotId == code)
                {
                    currentItem = item.Key;
                    codeExists = true;
                    if (StockedItems[currentItem] == 0)
                    {
                        soldOut = true;
                    }
                    else if (item.Key.Price > TotalMoney)
                    {
                        notEnoughFunds = true;
                    }
                }
            }

            if (!codeExists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.WriteToScreen("Item not found, please try again.");
                Console.ForegroundColor = ConsoleColor.White;
                SelectProduct();
            }
            else if (soldOut)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.WriteToScreen("Item sold out, please try again.");
                Console.ForegroundColor = ConsoleColor.White;
                SelectProduct();
            }
            else if (notEnoughFunds)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.WriteToScreen("Not enough funds, please submit more money.");
                Console.ForegroundColor = ConsoleColor.White;
                FeedMoney();
            } else
            {
                HandleItem(currentItem);
            }
        }

        public void HandleItem(VendingMachineItem currentItem)
        {
            AddToAudit($"{currentItem.Name} {currentItem.SlotId}", currentItem.Price * -1);
            UpdateSalesDataFile(currentItem);
            TotalMoney -= currentItem.Price;
            StockedItems[currentItem]--;
            UserInterface.WriteToScreen($"{currentItem.Name}, ${currentItem.Price.ToString("0.00")}, funds left: ${TotalMoney.ToString("0.00")}");
            UserInterface.WriteToScreen(currentItem.Message);
            PurchaseMenu();
        }

        public void FeedMoney()
        {
            string FEEDMONEY = "FEED MONEY:";
            bool isFeeding = true;
            while (isFeeding)
            {
                try
                {
                    int money = UserInterface.IntegerInput("Please insert money in whole dollar amounts, or press 0 to escape.");
                    if (money == 0)
                    {
                        isFeeding = false;
                    }
                    else if (money < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        UserInterface.WriteToScreen("Please input a valid dollar amount (in whole bills)!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        TotalMoney += money;
                        UserInterface.WriteToScreen($"Current Balance: ${TotalMoney.ToString("0.00")}");
                        AddToAudit(FEEDMONEY, money);
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    UserInterface.WriteToScreen("Please input a valid dollar amount (in whole bills)!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            PurchaseMenu();
        }

        public void FinishTransaction()
        {
            string GIVECHANGE = "GIVE CHANGE:";
            decimal change = TotalMoney;
            decimal quarter = .25M;
            decimal dime = .1M;
            decimal nickel = .05M;

            int quarterCount = 0;
            int dimeCount = 0;
            int nickelCount = 0;

            AddToAudit(GIVECHANGE, change * -1);

            if (change > 0)
            {
                while (TotalMoney >= quarter)
                {
                    TotalMoney -= quarter;
                    quarterCount++;
                }
                while (TotalMoney >= dime)
                {
                    TotalMoney -= dime;
                    dimeCount++;
                }
                while (TotalMoney > 0)
                {
                    TotalMoney -= nickel;
                    nickelCount++;
                }

                UserInterface.WriteToScreen($"Your total change ${change.ToString("0.00")} was dispensed as {quarterCount} quarter(s), {dimeCount} dime(s), and {nickelCount} nickel(s).");
                MainMenu();
            }
            else
            {
                UserInterface.WriteToScreen($"There is no change to dispense.");
                MainMenu();
            }


        }

        public void AddToAudit(string transaction, decimal amount)
        {
            string specifiedPath = Path.Combine(Folder, "log.txt");

            try
            {
                using (StreamWriter sw = new StreamWriter(specifiedPath, true))
                {
                    sw.Write(DateTime.Now);
                    sw.Write(" " + transaction + " ");
                    sw.Write(transaction == "FEED MONEY:" ? $"${amount.ToString("0.00")} " : $"${TotalMoney.ToString("0.00")} ");
                    sw.Write(transaction == "FEED MONEY:" ? $"${TotalMoney.ToString("0.00")} " : $"${(TotalMoney + amount).ToString("0.00")} ");
                    sw.WriteLine();
                }
            }
            catch (Exception e)
            {
                UserInterface.WriteToScreen("There was an error writing the audit file: " + e.Message);
            }
        }

        public void CreateSalesDataFile()
        {
            string fullPath = Path.Combine(Folder, SalesDataFileName);

            try
            {
                using (StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    foreach(KeyValuePair<VendingMachineItem, int>currentItem in StockedItems)
                    {
                        sw.WriteLine($"{currentItem.Key.Name}|0");
                    }
                }
            } catch (Exception e)
            {
                UserInterface.WriteToScreen("There was an error writing the sales data file: " + e.Message);
            }
        }

        public void UpdateSalesDataFile(VendingMachineItem item)
        {
            string fullPath = Path.Combine(Folder, SalesDataFileName);
            string outputPath = Path.Combine(Path.GetTempPath(), "tempdata.txt");

            try
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    using (StreamWriter sw = new StreamWriter(outputPath)) {
                        decimal totalSales = 0.00M;
                        while (!sr.EndOfStream)
                        {
                            string currentLine = sr.ReadLine();
                            string[] splitLine = currentLine.Split("|", StringSplitOptions.RemoveEmptyEntries);

                            if (splitLine.Length > 1)
                            {
                                try
                                {
                                    int splitInt = int.Parse(splitLine[1]);
                                    if (item.Name == splitLine[0])
                                    {
                                        splitInt++;
                                    }
                                    sw.WriteLine($"{splitLine[0]}|{splitInt}");

                                    //Trouble?
                                    totalSales += splitInt * SalesRep[splitLine[0]];
                                }
                                catch (Exception e)
                                {
                                    UserInterface.WriteToScreen("Could not parse integer " + splitLine[1] + " " + e.Message);
                                }
                            }
                        }
                        sw.WriteLine($"Gross Sales are ${totalSales.ToString("0.00")}");
                    }
                }

                File.Delete(fullPath);
                File.Copy(outputPath, fullPath);
                File.Delete(outputPath);
            } catch (Exception e)
            {
                UserInterface.WriteToScreen("There was an error reading/writing the file: " + e.Message);
            }
        }

        public void GenerateSalesReport()
        {
            string salesPath = Path.Combine(Folder, $"{DateTime.UtcNow.ToString("yyyyMMddTHHmmss")}SalesReport.txt");
            string inputPath = Path.Combine(Folder, SalesDataFileName);
            File.Copy(inputPath, salesPath);

        }
    }
}
