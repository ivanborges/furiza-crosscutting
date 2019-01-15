using System.Net.Http;

namespace Furiza.Networking.Abstractions
{
    public interface IHttpClientFactory
    {
        HttpClient Create();

        HttpClient Create(string baseAddress);
    }
}