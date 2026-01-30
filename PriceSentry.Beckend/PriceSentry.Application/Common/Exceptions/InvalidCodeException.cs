using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSentry.Application.Common.Exceptions {
    public class InvalidCodeException : Exception {
        public InvalidCodeException() : base("Invalid code verification") { }

    }
}
