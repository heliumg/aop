using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCastleAutofacDiff
{
    public interface IController
    {
        List<string> GetUsers();
        string GetUserById(int id);
        Task SaveUser(string name);

        Guid GUID { get; }
    }
}
