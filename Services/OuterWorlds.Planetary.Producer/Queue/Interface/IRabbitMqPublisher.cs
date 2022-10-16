using System.Collections.Generic;
using System.Threading.Tasks;

namespace Queue.Interface
{
    public interface IRabbitMqPublisher<TMessage>
    {
        Task PublishAsync(TMessage eventMessage, IDictionary<string, object> customHeaders = null);
    }
}
