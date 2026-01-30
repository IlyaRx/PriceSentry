using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSentry.Application.Common.Exceptions {
    public class RateLimitException : Exception {
        public RateLimitException() : base("The attempt limit has been exceeded") { }

    }
}
