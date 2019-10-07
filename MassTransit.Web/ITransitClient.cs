using System;
using System.Threading.Tasks;

namespace MassTransit.Web
{
    public interface ITransitClient
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessage(string message);
    }
}
