using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Classes
{
    public class UI
    {
        public UI()
        {

        }
        public string StringInput(string prompt)
        {
            Console.WriteLine(prompt);
            string entry = Console.ReadLine();

            return entry;
        }

        public int IntegerInput(string prompt)
        {
            Console.WriteLine(prompt);
            string entry = Console.ReadLine();
            try
            {
                int intEntry = int.Parse(entry);
                return intEntry;
            } catch(Exception e)
            {
                throw new FormatException($"Cannot Parse entry: {entry} into an integer");
            }
            
        }

        public void WriteToScreen(string output)
        {
            Console.WriteLine(output);
        }

        public void ClearScreen()
        {
            Console.Clear();
        }
    }
}
