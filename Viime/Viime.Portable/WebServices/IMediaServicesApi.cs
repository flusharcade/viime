using System;
using System.Threading.Tasks;

using Refit;

namespace Viime.Portable.WebServices
{
    public interface IMediaServicesApi
    {
        [Post("/api/Channels")]
        Task<ChannelsContract> GetChannels([Header("Authorization")] string authString, 
                                           [Header("x-ms-version")] string apiVersion);

        //[Get("/users/{user}")]
        //IObservable<User> GetChannelById(string user);

        //[Get("/users/{user}")]
        //IObservable<User> CreateChannel(string user);
    }
}
