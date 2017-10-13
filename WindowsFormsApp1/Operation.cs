using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public abstract class Operation
    {
        public long id { get; set; }
        public Client sender { get; set; }
        public Client recipient { get; set; }
        public float value { get; set; }
        public string type { get; set; }
        public int time{ get; set; }
    }
}
