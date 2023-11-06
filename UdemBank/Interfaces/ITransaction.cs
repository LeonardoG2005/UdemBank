using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UdemBank.Enums;

namespace UdemBank
{
    public interface ITransaction
    {
        double Amount { get; }
        TradeType Type { get; }
        DateTimeOffset Date { get; } // Cambiar a DateTimeOffset para incluir hora
        double CurrentBalance { get; } // Saldo actual de cuenta de ahorros
    }
}
