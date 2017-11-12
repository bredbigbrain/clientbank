using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public enum OperationTypes: int
    {
        Transaction,
        Credit
    }

    public abstract class Operation
    {
        public long id { get; protected set; }
        public Client Sender { get; protected set; }
        public Client Recipient { get; protected set; }
        public float value { get; protected set; }
        public OperationTypes type { get; protected set; }
        public int time{ get; protected set; }
    }
}
