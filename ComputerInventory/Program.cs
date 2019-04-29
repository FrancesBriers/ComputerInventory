using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

using ComputerInventory.Data;
using ComputerInventory.Models;

namespace ComputerInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            int result = -1;
            while (result != 9)
            {
                result = MainMenu();
            }
        }

        static int MainMenu()
        {
            int result = -1;
            ConsoleKeyInfo cki;
            bool cont = false;
            do
            {
                Clear();
                WriteHeader("Welcome to Newbie Data Systems");
                WriteHeader("Main Menu");
                WriteLine("\r\nPlease select from the list below for what you would like to do today");
                WriteLine("1. List All Machines in Inventory");
                WriteLine("2. List All Operating Systems");
                WriteLine("3. Data Entry Menu");
                WriteLine("4. Data Modification Menu");
                WriteLine("9. Exit");
                cki = ReadKey();
                try
                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1)
                    {
                        //DisplayAllMachines();
                    }
                    else if (result == 2)
                    {
                        DisplayOperatingSystems();
                    }
                    else if (result == 3)
                    {
                        DataEntryMenu();
                    }
                    else if (result == 4)
                    {
                        DataModificationMenu();
                    }
                    else if (result == 9)
                    {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (FormatException)
                {
                    // a key that wasn't a number
                }

            } while (!cont);

            return result;
        }

        static void DataEntryMenu()
        {
            ConsoleKeyInfo cki;
            int result = -1;
            bool cont = false;
            do
            {
                Clear();
                WriteHeader("Data Entry Menu");
                WriteLine("\r\nPlease select from the list below for what you would like to do today");
                WriteLine("1. Add a New Machine");
                WriteLine("2. Add a New Operating System");
                WriteLine("3. Add a New Warranty Provider");
                WriteLine("9. Exit Menu");
                cki = ReadKey();
                try
                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1)
                    {
                        //AddMachine();
                    }
                    else if (result == 2)
                    {
                        AddOperatingSystem();
                    }
                    else if (result == 3)
                    {
                        //AddNewWarrantyProvider();
                    }
                    else if (result == 9)
                    {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (FormatException)
                {
                    // a key that wasn't a number
                }
            } while (!cont);
        }
        
        static void DataModificationMenu()
        {
            ConsoleKeyInfo cki;
            int result = -1;
            bool cont = false;
            do
            {
                Clear();
                WriteHeader("Data Modification Menu");
                WriteLine("\r\nPlease select from the list below for what you would like to do today");
                WriteLine("1. Delete Operating System");
                WriteLine("2. " +
                          "Modify" +
                          " Operating System");
                WriteLine("3. Delete All Unsupported Operating Systems");
                WriteLine("9. Exit Menu");
                cki = ReadKey();
                try

                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    if (result == 1)
                    {
                        SelectOperatingSystem("Delete");
                    }
                    else if (result == 2)
                    {
                        SelectOperatingSystem("Modify");
                    }
                    else if (result == 3)
                    {
                        DeleteAllUnsupportedOperatingSystems();
                    }
                    else if (result == 9)
                    {
                        // We are exiting so nothing to do
                        cont = true;
                    }
                }
                catch (FormatException)
                {
                    // a key that wasn't a number
                }
            } while (!cont);
        }

        static void WriteHeader(string headerText)
        {
            WriteLine(string.Format("{{0," + ((WindowWidth / 2) +
                                                      headerText.Length / 2) + "}}", headerText));
        }

        static bool ValidateYorN(string entry)
        {
            bool result = entry.ToLower() == "y" || entry.ToLower() == "n";
            return result;
        }

        static bool CheckForExistingOS(string osName)
        {
            bool exists = false;
            using (var context = new MachineContext())
            {
                var os = context.OperatingSys.Where(o => o.Name == osName);
                if (os.Any())
                {
                    exists = true;
                }
            }

            return exists;
        }

        static void AddOperatingSystem()
        {
            Clear();
            ConsoleKeyInfo cki;
            string result;
            bool cont = false;
            OperatingSys os = new OperatingSys();
            string osName = "";
            do
            {
                WriteHeader("Add New Operating System");
                WriteLine("Enter the Name of the Operating System and hit Enter");
                osName = ReadLine();
                if (osName != null && osName.Length >= 4)
                {
                    cont = true;
                }
                else
                {
                    WriteLine("Please enter a valid OS name of at least 4 characters.\r\nPress and key to continue...");
                    ReadKey();
                }
            } while (!cont);

            cont = false;
            os.Name = osName;
            WriteLine("Is the Operating System still supported? [y or n]");

            do
            {
                cki = ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y")
            {
                os.StillSupported = true;
            }
            else
            {
                os.StillSupported = false;
            }

            cont = false;
            do
            {
                Clear();
                WriteLine(
                    $"You entered {os.Name} as the Operating System Name\r\nIs the OS still supported, you entered {os.StillSupported}.\r\nDo you wish to continue? [y or n]");
                cki = ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);

            if (result.ToLower() == "y")
            {
                bool exists = CheckForExistingOS(os.Name);
                if (exists)
                {
                    WriteLine(
                        "\r\nOperating System already exists in the database\r\nPress any key to continue...");
                    ReadKey();
                }
                else
                {
                    using (var context = new MachineContext())
                    {
                        WriteLine("\r\nAttempting to save changes...");
                        context.OperatingSys.Add(os);
                        int i = context.SaveChanges();
                        if (i == 1)
                        {
                            WriteLine("Contents Saved\r\nPress any key to continue...");
                            ReadKey();
                        }
                    }
                }
            }
        }

        static void DisplayOperatingSystems()
        {
            Clear();
            WriteLine("Operating Systems");
            using (var context = new MachineContext())
            {
                foreach (var os in context.OperatingSys.ToList())
                {
                    Write($"Name: {os.Name,-39}\tStill Supported = ");
                    ForegroundColor = os.StillSupported == true ? ConsoleColor.Green : ConsoleColor.Red;
                    WriteLine(os.StillSupported);
                    ForegroundColor = ConsoleColor.Yellow;
                }
            }
            WriteLine("\r\nAny key to continue...");
            ReadKey();
        }

        static OperatingSys GetOperatingSystemById(int id)
        {
            var context = new MachineContext();
            OperatingSys os = context.OperatingSys.FirstOrDefault(i =>
                i.OperatingSysId == id);
            return os;
        }

        static void SelectOperatingSystem(string operation)
        {
            ConsoleKeyInfo cki;
            Clear();
            WriteHeader($"{operation} an Existing Operating System Entry");
            WriteLine($"{"ID",-7}|{"Name",-50}|Still Supported");
            WriteLine("-------------------------------------- -----------");
            using (var context = new MachineContext())
            {
                List<OperatingSys> lOperatingSystems = context.OperatingSys.ToList();
                foreach (OperatingSys os in lOperatingSystems)
                {
                    WriteLine($"{os.OperatingSysId,-7}|{os.Name,-50}|{ os.StillSupported}");
                }
            }
            WriteLine($"\r\nEnter the ID of the record you wish to{ operation}and hit Enter\r\nYou can hit Esc to exit this menu");
            bool cont = false;
            string id = "";
            do
            {
                cki = ReadKey(true);
                if (cki.Key == ConsoleKey.Escape)
                {
                    cont = true;
                    id = "";
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    if (id.Length > 0)
                    {
                        cont = true;
                    }
                    else
                    {
                        WriteLine("Please enter an ID that is at least 1 digit.");
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace)
                {
                    Write("\b \b");
                    try
                    {
                        id = id.Substring(0, id.Length - 1);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // at the 0 position, can't go any further back
                    }
                }
                else
                {
                    if (char.IsNumber(cki.KeyChar))
                    {
                        id += cki.KeyChar.ToString();
                        Write(cki.KeyChar.ToString());
                    }
                }
            } while (!cont);

            int osId = Convert.ToInt32(id);
            if ("Delete" == operation)
            {
                DeleteOperatingSystem(osId);
            }
            else if ("Modify" == operation)
            {
                ModifyOperatingSystem(osId);
            }
        }


        static void DeleteOperatingSystem(int id)
        {
            OperatingSys os = GetOperatingSystemById(id);
            if (os != null)
            {
                WriteLine($"\r\nAre you sure you want to delete {os.Name}?[y or n]");
          
                ConsoleKeyInfo cki;
                string result;
                bool cont;
                do
                {
                    cki = ReadKey(true);
                    result = cki.KeyChar.ToString();
                    cont = ValidateYorN(result);
                } while (!cont);
                if ("y" == result.ToLower())
                {
                    WriteLine("\r\nDeleting record");
                    using (var context = new MachineContext())
                    {
                        context.Remove(os);
                        context.SaveChanges();
                    }
                    WriteLine("Record Deleted");
                    ReadKey();
                }
                else
                {
                    WriteLine("Delete Aborted\r\nHit any key to continue...");
                    ReadKey();
                }
            }
            else
            {
                WriteLine("\r\nOperating System Not Found!");
                ReadKey();
                SelectOperatingSystem("Delete");
            }
        }
        static void DeleteAllUnsupportedOperatingSystems()
        { using (var context = new MachineContext()) {
             var os = (from o in context.OperatingSys where o.StillSupported ==
              false select o);
              WriteLine("\r\nDeleting all Unsupported Operating Systems...");
              context.OperatingSys.RemoveRange(os);
              int i = context.SaveChanges();
              WriteLine($"We have deleted {i} records");
              WriteLine("Hit any key to continue...");
              ReadKey();
            }
        }

        static void ModifyOperatingSystem(int id)
        {
            OperatingSys os = GetOperatingSystemById(id);
            Clear();
            char operation = '0';
            bool cont = false;
            ConsoleKeyInfo cki;
            WriteHeader("Update Operating System");
            if (os != null)
            {
               WriteLine($"\r\nOS Name: {os.Name} Still Supported:{ os.StillSupported}");
               WriteLine("To modify the name press 1\r\n To modify if the OS is Still Supported press 2");
               WriteLine("Hit Esc to exit this menu");

                do
                {
                    cki = ReadKey(true);
                    if (cki.Key == ConsoleKey.Escape)
                        cont = true;
                    else
                    {
                        if (char.IsNumber(cki.KeyChar))
                        {
                            if (cki.KeyChar == '1')
                            {
                                WriteLine("Updated Operating System Name: ");
                                operation = '1';
                                cont = true;
                            }
                            else if (cki.KeyChar == '2')
                            {
                                WriteLine("Update if the OS is Still Supported [y or n]: ");
                                operation = '2';
                                cont = true;
                            }
                        }
                    }
                } while (!cont);
            }
            if (operation == '1')
            {
                string osName;
                cont = false;
                do
                {
                    osName = ReadLine();
                    if (osName.Length >= 4)
                    {
                        cont = true;
                    }
                    else
                    {
                        WriteLine("Please enter a valid OS name of at least 4 characters.\r\nPress and key to continue...");
                       ReadKey();
                    }
                } while (!cont);
                os.Name = osName;
            }
            else if (operation == '2')
            {
                string k;
                do
                {
                    cki = ReadKey(true);
                    k = cki.KeyChar.ToString();
                    cont = ValidateYorN(k);
                } while (!cont);
                if (k == "y")
                {
                    os.StillSupported = true;
                }
                else
                {
                    os.StillSupported = false;
                }
            }
            using (var context = new MachineContext())
            {
                var o = context.OperatingSys.FirstOrDefault(i => i.OperatingSysId ==
                os.OperatingSysId);
                if (o != null)
                {
                    // just making sure
                    o.Name = os.Name;
                    o.StillSupported = os.StillSupported;
                    WriteLine("\r\nUpdating the database...");
                    context.SaveChanges();
                    WriteLine("Done!\r\nHit any key to continue...");
                }
            }
            ReadKey();
        }

    }
}
