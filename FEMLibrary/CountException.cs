using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary
{
    public class CountException : Exception
    {
        public CountException() { }
        public CountException(string message) : base(message) { }
        public CountException(string message, Exception inner) : base(message, inner) { }
    } 
}
