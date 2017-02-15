using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCastleAutofacDiff
{
    public interface IIOCContainer
    {
        string Name { get; }

        T Resolve<T>(object args ) where T : class;

        void SetupForTransientTest();
        void SetupForSingletonTest();
    }
}
