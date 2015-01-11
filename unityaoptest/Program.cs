using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Configuration;



namespace unityaoptest
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = Bootstrap();
            var calc = c.Resolve<ICalculator>();

            Console.WriteLine("********start********");

            Console.WriteLine(calc.Add(10, 100));
            Console.WriteLine(calc.Sub(10, 100));

            Console.WriteLine("********stop********");
        }

        /// <summary>
        /// register unity stuff
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer Bootstrap()
        {
            IUnityContainer c = new UnityContainer();
            c.AddNewExtension<Interception>();
            c.RegisterType<ICalculator, Calculator>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            return c;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// attribute for logging
    /// </summary>
    class Log : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogHandler();
        }
    }

    /// <summary>
    /// handler for logging attribute
    /// </summary>
    public class LogHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine("Invoking " + input.MethodBase.Name);
            IMethodReturn result = getNext()(input, getNext);
            Console.WriteLine("Done Invoke");
            return result;
        }

        public int Order { get; set; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// attribute for logging exceptions
    /// </summary>
    class LogException : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogExceptionHandler();
        }
    }

    /// <summary>
    /// handler for logging exceptions attribute
    /// </summary>
    public class LogExceptionHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {

            IMethodReturn result = getNext()(input, getNext);

            if (result.Exception != null)
            {
                Console.WriteLine("Logging exception " + result.Exception.Message);
            }

            return result;
        }

        public int Order { get; set; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    //calculator example stuff


    public interface ICalculator
    {
        double Add(double a, double b);
        double Sub(double a, double b);
    }
    public class Calculator : ICalculator
    {
        [Log]
        public double Add(double a, double b) { return a + b; }

        [Log]
        [LogException]
        public double Sub(double a, double b)
        {
            throw new Exception("fail");
            return a - b;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

}
