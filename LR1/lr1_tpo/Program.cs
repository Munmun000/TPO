using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTreeSelectionAlgorithm;

namespace lr1_tpo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var names = new List<string> {
                "Alice", "Bob", "Anna", "Alex", "Andrew",
                "Benjamin", "Carol", "David", "Amanda", "Aaron"
            };
            var aNames = SelectionAlgorithm.Select(names, n => n.StartsWith("A", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine("Names starting with 'A':");
            Console.WriteLine(string.Join(", ", aNames)); // Alice, Anna, Alex, Andrew, Amanda, Aaron
        }
    }
}
