using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace formfiller
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            object docname = "";
            var doc = app.Documents.Open(ref docname);
                        
        }
    }
}
