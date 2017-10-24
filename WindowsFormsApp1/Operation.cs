using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public abstract class Operation
    {
        public long id { get; protected set; }
        public Client sender { get; protected set; }
        public Client recipient { get; protected set; }
        public float value { get; protected set; }
        public string type { get; protected set; }
        public int time{ get; protected set; }
    }
}
