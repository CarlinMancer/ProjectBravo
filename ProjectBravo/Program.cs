using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProjectBravo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ========== Employee Information ========== ");
            List<String[]> records = new List<String[]>();
            if (File.Exists(@"C:\Users\Carlin\source\repos\ProjectBravo\employees.txt"))
            {
                /* Instead of working out how to use a FileStream and rewriting all
                 my code, I'll just load the entire thing into memory. ChunkList
                 is a function I wrote to break the input into a list of fixed-length
                 arrays (5 entries long).*/
                records = ChunkList(File.ReadAllLines
                    (@"C:\Users\Carlin\source\repos\ProjectBravo\employees.txt").ToList());
            }
            bool exiting = false;
            while(exiting == false)
            {
                Console.WriteLine("1. Add an employee");
                Console.WriteLine("2. Insert an employee in a specific position");
                Console.WriteLine("3. Update an employee's information");
                Console.WriteLine("4. Delete an employee's information");
                Console.WriteLine("5. Search for an employee");
                Console.WriteLine("6. Display all employee information");
                Console.WriteLine("or press 'x' to exit");
                Console.WriteLine(""); //get some spacing in the output.

                //added second while loop at last minute to stop every input from triggering
                //a reprint
                bool success = false;
                while (success == false)
                {
                    char selection = Console.ReadKey(true).KeyChar;

                    /* I could use Switch for the below, but I've decided 
                    it makes most sense to use Switch only when I plan to use
                    fallthrough. Otherwise I just have to write a lot of 
                    break statements and it doesn't actually save time.*/
                    if (selection == '1') { success = true; AddInfo(ref records); }
                    else if (selection == '2') { success = true; InsertInfo(ref records); }
                    else if (selection == '3') { success = true; UpdateInfo(ref records); }
                    else if (selection == '4') { success = true; DeleteInfo(ref records); }
                    else if (selection == '5') { success = true; SearchInfo(ref records); }
                    else if (selection == '6') { success = true; DisplayInfo(ref records); }
                    else if (selection == 'x') { success = true; exiting = true; }
                }

                /*save the file. This is probably an inefficient approach,
                 * but things like working with FileStream are beyond me at the moment.*/
                if (records.Count > 0)
                {
                    File.WriteAllLines(@"C:\Users\Carlin\source\repos\ProjectBravo\employees.txt",
                        ConcatAll(records));
                }
                Console.WriteLine(""); //spacing
            }
        }

        static void AddInfo(ref List<String[]> records) 
        { /*Function to add a record to the list. Uses helper function CollectRecord
           to share functionality with InsertInfo. Note that a single record is an
            array of strings, and the 'records' variable is a list of such strings.*/
            String[] record = CollectRecord();
            records.Add(record);
            Console.WriteLine("Record added at index " + records.Count + ". ");
        }
        static void InsertInfo(ref List<String[]> records) 
        {
            Console.WriteLine("Where do you want to insert this record? ");
            /*ValidParse is a function I wrote that both parses an input
             as an integer *and* checks whether it can be an index to 
            a given list, refusing to proceed until it gets successful input.*/
            int index = ValidParse(records);
            //Use same collection method as AddInfo
            String[] record = CollectRecord();
            /* Even though C# is zero-indexed, I want the program to appear
             one-indexed from a user's perspective. So I'm inserting at input-1*/
            records.Insert(index-1, record);
            Console.WriteLine("Record added at index " + index + ". ");
        }
        static String[] CollectRecord()
        {
            //Code for gathering a record.
            //Records are more extensive than first name/last name, as 
            //an attempt to meet SR09. It turns out I didn't understand
            //SR09, but as happened last week I'm continuing with what I
            //was already doing.
            String[] record = new String[5];
            bool validated = false;
            //while loop will repeat until user confirms input accuracy
            while (validated == false)
            {
                Console.WriteLine("What is the employee's first name?");
                record[0] = Console.ReadLine();
                Console.WriteLine("What is the employee's last name?");
                record[1] = Console.ReadLine();
                Console.WriteLine("What is the employee's email address?");
                record[2] = Console.ReadLine();
                Console.WriteLine("What is the employee's mobile number?");
                record[3] = Console.ReadLine();
                Console.WriteLine("What is the employee's home address?");
                record[4] = Console.ReadLine();
                /*Shows the record, using another funcion I wrote, to check accuracy.*/
                Console.WriteLine("You entered the following employee details:");
                DisplayRecord(record);
                Console.WriteLine("Is that correct? (y/n)");
                if(Console.ReadKey(true).KeyChar == 'y')
                    { validated = true; }
            }
            return record;
        }
        static void DisplayRecord(string[] record)
        {
            /* Display format:
            Firstname Lastname
            Email address
            Phone number
            Home address*/
            Console.WriteLine(record[0] + " " + record[1]);
            Console.WriteLine(record[2]);
            Console.WriteLine(record[3]);
            Console.WriteLine(record[4]);
        }
        static int GetIndex(IEnumerable<object> collection, string purpose)
        {
            //Shared functionality between UpdateRecord and DeleteRecord. Prints
            //instructions, then receives a valid index number from the ValidParse
            //function
            Console.WriteLine("Input the index number of the employee record you want to " 
                + purpose + ".");
            Console.WriteLine("(If unsure of index number, use the search function.) ");
            int index = ValidParse(collection);
            return index;
        }
        static int ValidParse(IEnumerable<object> collection)
        {
            //This function continues until it gets something that can be used
            //as an index starting from one for the given collection. The calling
            //function has to convert it into an index starting from zero for C#'s use.
            int index = 0;
            bool validated = false;
            while (validated == false)
            {
                //because we are looking for an index starting from one, we do not need
                //to distinguish a genuinely input 0 from a 0 produced by a TryParse failure.
                int.TryParse(Console.ReadLine(), out index);

                //One more function, which contains the conditions for validating the index
                validated = ValidIndex(index, collection);
                if (validated == false)
                {
                    Console.WriteLine("Please enter a valid index within range. ");
                }
            }
            return index;
        }
        static bool ValidIndex(int index, IEnumerable<object> collection)
        {
            //Core of index validation system; says whether integer is valid index
            //for collection or not.
            //Note that this validates an index starting from one. Other functions
            //will convert into index starting from zero
            if (index < 1) { return false; }
            if (collection.Count() > index) { return false; }
            return true;
        }
        static void UpdateInfo(ref List<String[]> records)
        {
            //Use GetIndex function I wrote to request and validate index from user.
            int i = GetIndex(records, "update");
            String[] record = records[i - 1]; //remember to convert one-index to zero-index

            //Field descriptions for message construction
            String[] fields = {"first name", "last name", "email address", "mobile number",
                "home address"};

            //Loop to check all fields and update as required
            for(short index = 0; index < 5; index++)
            {
                Console.WriteLine("The employee's " + fields[index] + " is recorded as " 
                    + record[index] + ". ");
                Console.WriteLine("Do you want to update this? (y/n)");
                if (Console.ReadKey(true).KeyChar == 'y')
                {
                    Console.WriteLine("Please input the employee's updated " + fields[index]
                        + ". ");
                    record[index] = Console.ReadLine();
                    Console.WriteLine("Updated. ");
                }
            }
            records[i - 1] = record;

            //Print new details.
            Console.WriteLine("Employee details updated. The new details are:");
            DisplayRecord(record);
        }
        static void DeleteInfo(ref List<String[]> records)
        {
            //use the same GetIndex function as in UpdateUser
            int i = GetIndex(records, "delete");
            String[] record = records[i - 1]; //convert one-index to zero-index

            //Confirm employee selection (the user chose by index, and may have made a mistake)
            Console.WriteLine("You have selected the following employee: ");
            DisplayRecord(record);
            Console.WriteLine("Are you sure you want to delete this employee's" +
                " record (y/n)? ");
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                records.RemoveAt(i - 1);
                Console.WriteLine("Deleted.");
            }
        }
        static void SearchInfo(ref List<String[]> records)
        {
            //Give user option for search type
            Console.WriteLine("1. Lookup by index number");
            Console.WriteLine("2. Lookup by first name");
            Console.WriteLine("3. Lookup by last name");
            Console.WriteLine("or hit 'x' to return");
            bool success = false;
            while(success == false)
            {
                char input = Console.ReadKey(true).KeyChar;
                if(input == '1')
                {
                    //lookup by index number. We already have a function to do this
                    //remember user-facing records should be one-indexed.
                    success = true;
                    Console.WriteLine("Type the index you wish to look up. ");
                    int index = ValidParse(records);
                    DisplayRecord(records[index - 1]);
                }
                //2 and 3 should both work the same way.
                //The difference is which fields should be checked.
                if(input == '2')
                {
                    success = true;
                    Console.WriteLine("Type first name (case-sensitive). ");
                    NameSearch(records, 0);
                }
                if(input == '3')
                {
                    success = true;
                    Console.WriteLine("Type last name (case-sensitive). ");
                    NameSearch(records, 1);
                }
                if(input == 'x')
                {
                    success = true;
                }
            }
        }
        static void NameSearch(List<String[]> records, int surname)
        {
            //function for name searching. 'int surname' is which field to check.
            //also handles the output side of a name search
            string inputName = Console.ReadLine();
            Console.WriteLine("");

            /*C# provides FindAll and FindIndex, but not the combination. I want to
             * print indices next to the records, so I needed to write my own function.*/
            List<int> results = FindAllIndices(records, x => x[surname] == inputName);
            if (results.Count == 0)
            {
                Console.WriteLine("We couldn't find any matches for " + inputName);
            }
            else
            {
                Console.WriteLine("We found the following matches:");
                foreach (int record in results)
                {
                    Console.Write("Index number " + (record + 1) + ": ");
                    DisplayRecord(records[record]);
                    Console.WriteLine("");
                }
            }

        }
        static List<int> FindAllIndices(List<String[]> list, Predicate<String[]> match)
        {
            List<int> listIndices = new List<int>();

            /*Unorthodox use of a for loop. I need a list which meets the following 
             * criteria:
             * 1: All matches listed
             * 2: Each matching record only listed once.
             * 3: Stops when all matching records are listed.
             * The following facts about the FindIndex method are relevant:
             * 1: It can take a starting index as an argument.
             * 2: When it is unable to find a match, it returns -1
             * The lastmatch variable represents different things at different times,
             * either the count to start FindIndex at, or a value to add to the output list.*/

            for (int lastmatch = 0; lastmatch >= 0; )
            {
                lastmatch = list.FindIndex(lastmatch, match);
                if(lastmatch >= 0) 
                { 
                    listIndices.Add(lastmatch);
                    lastmatch++; /*This has to be here; if we have a -1 lastmatch,
                                  * we need to keep it for the condition evaluator*/
                }
            }
            return listIndices;
        }
        static void DisplayInfo(ref List<String[]> records)
        {
            Console.WriteLine("Full Employee Roll:");
            //I want to print the indices, so I can't use foreach
            for(int i = 0; i < records.Count; i++)
            {
                Console.WriteLine("");
                Console.Write("Index number " + (i + 1) + ": "); //convert c# zero indexes
                                                                 //to one indexes
                DisplayRecord(records[i]); //my custom function
            }
        }
        static List<string[]> ChunkList(List<string> unsplit)
        {
            //function to split list of strings into list of string arrays
            //by fives, for file input. Uses the Take and Skip methods.
            List<string[]> split = new List<string[]>();
            for(int i = 0; unsplit.Count >  0; i = i + 5)
            {
                split.Add(unsplit.Take<string>(5).ToArray<string>());
                unsplit = unsplit.Skip<string>(5).ToList();
            }
            return split;
        }
        static string[] ConcatAll(List<string[]> records)
        {
            //The inverse of ChunkList above: convert list of arrats into 
            //single array for writing.
            //Don't overwrite the records list itself! Copy it.
            //This isn't ideal resource-wise, but it's all I feel capable
            //of right now.
            List<string[]> writelist = records.ToList();
            while(writelist.Count > 1)
            {
                writelist[0] = Enumerable.Concat(writelist[0], writelist[1]).ToArray();
                writelist.RemoveAt(1);
            }
            return writelist[0];
        }
    }
}
