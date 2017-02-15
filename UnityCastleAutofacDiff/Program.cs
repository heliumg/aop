using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Config;
using UnityCastleAutofacDiff.Containers;

namespace UnityCastleAutofacDiff
{
    public class Program
    {
        private const int Max = 100000;
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            IIOCContainer[] containersSingleton = {new CastleContainer(), new UnityIOCContainer() };

            foreach (var cont in containersSingleton)
            {
                TestSingleton(cont);
            }

            IIOCContainer[] containersTtr = { new CastleContainer(), new UnityIOCContainer() };
            List<Task> tasks = new List<Task>();

            foreach (var cont in containersTtr)
            {
                TestTransient(cont);
                TestVirtualFunctionCall(cont);
                TestFunctionCall(cont);
                tasks.Add(Task.Run(()=>TestAsyncFunctionCall(cont)));
            }

            Task.WhenAll(tasks);
            Console.ReadKey();
        }

        private static void TestVirtualFunctionCall(IIOCContainer container)
        {
            var data = new List<string> { "Hermine", "Edward", "Karen", "Ando" };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            //transient
            var obj = container.Resolve<IController>(new { users = data });

            for (var i = 0; i < Max; i++)
            {
                //ordinary method
                obj.GetUsers();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Container: {container.Name}, Virtual Method Call time: {elapsedMs}ms");

        }

        private static void TestFunctionCall(IIOCContainer container)
        {
            var data = new List<string> { "Hermine", "Edward", "Karen", "Ando" };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            //transient
            var obj = container.Resolve<IController>(new { users = data });

            for (var i = 0; i < Max; i++)
            {
                //ordinary method
                obj.GetUserById(1);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Container: {container.Name}, Ordinary Method Call time: {elapsedMs}ms");
        }

        private static async Task TestAsyncFunctionCall(IIOCContainer container)
        {
            var data = new List<string> { "Hermine", "Edward", "Karen", "Ando" };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            //transient
            var obj = container.Resolve<IController>(new { users = data });

            for (var i = 0; i < Max/100; i++)
            {
                //async method
                await obj.SaveUser($"Fredd{i}");
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Container: {container.Name}, Async Method Call time: {elapsedMs}ms");
        }

        private static void TestTransient(IIOCContainer container)
        {
            var data = new List<string> { "Hermine", "Edward", "Karen", "Ando" };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            container.SetupForTransientTest();
            for (var i = 0; i < Max; i++)
            {
                var obj = container.Resolve<IController>(new { users = data });
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Container: {container.Name}, Transient resolve time: {elapsedMs}ms");

        }

        private static void TestSingleton(IIOCContainer container)
        {
            var data = new List<string> { "Hermine", "Edward", "Karen", "Ando"};

            var watch = System.Diagnostics.Stopwatch.StartNew();

            container.SetupForSingletonTest();
            for (var i = 0; i < Max; i++)
            {
                var obj = container.Resolve<IController>(new { users = data });
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Container: {container.Name}, Singleton resolve time: {elapsedMs}ms");
        }
    }
}
