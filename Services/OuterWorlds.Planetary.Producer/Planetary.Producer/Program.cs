using Planetary.Producer.Infrastructure.IoC;
using SimpleInjector;
using System;
using System.Threading;

namespace Planetary.Producer
{
    static class Program
    {
        private static readonly AutoResetEvent _closingEvent = new AutoResetEvent(false);

        private static void Main(string[] args)
        {
            var container = new Container();

            try
            {
                new Bootstrapper(container);
                var planetaryProducer = new PlanetaryProducer(container);

                planetaryProducer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message: " + ex.Message + "Exception: " + ex);
            }
            finally
            {
                Console.CancelKeyPress += (s, a) =>
                {
                    _closingEvent.Set();
                };
                _closingEvent.WaitOne();
            }
        }
    }
}
