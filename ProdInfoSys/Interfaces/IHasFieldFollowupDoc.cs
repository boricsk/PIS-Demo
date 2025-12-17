using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// A UserControlFunctions osztályban használt LINQ-k esetén meg kell adni, hogy mely mezőket
    /// tartalmazza minden adatmodel, ami generikusként jön át a függvénybe. Ezt az intefészt azoknak az adatmodelleknek kell
    /// megvalósítani, amelyekk mennek valamelyik UserControlFuncions-ban lévő metódusba (mivel ezek generikusokat fogadnak majd
    /// </summary>
    public interface IHasFieldFollowupDoc
    {
        int OutputSum { get; }
    }
}
