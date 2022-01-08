using System;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace bc.srv.Classes
{
    public class InMemoryRunner
    {
        public void Do()
        {
            var hash = new Hash(Assembly.GetExecutingAssembly());
            
            var dllAsArray = (byte[]) hash.GetType()
                .GetMethod("GetRawData", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(hash, new object[0]);
            
            var assType = Type.GetType("System.Reflection.Assembly");

            var loadMEthod = assType.GetMethod("Load", new[] {typeof(byte[])});
            var ass = (Assembly) loadMEthod.Invoke(null, new[] {dllAsArray});

            var programType =
                ass.GetTypes()
                    .FirstOrDefault(c =>
                        c.Name == "Program"); // <-- if you don't know the full namespace and when it is unique.
            var method = programType.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { });
        }
    }
}