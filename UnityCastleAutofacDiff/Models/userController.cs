using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCastleAutofacDiff
{
    public class UserController : IController
    {
        private readonly List<string> _users;
        public Guid GUID => Guid.NewGuid();
        public UserController(List<string> users)
        {
            _users = users;
            //Console.WriteLine(GUID);
        }
        public virtual List<string> GetUsers()
        {
            return _users;
        }

        public string GetUserById(int id)
        {
            if (id < 0 || id >= _users.Count)
                return string.Empty;

            return $"User name: {_users[id]}";
        }

        public async Task SaveUser(string name)
        {
            await Task.Delay(100);
            _users.Add(name);
        }
    }
}
