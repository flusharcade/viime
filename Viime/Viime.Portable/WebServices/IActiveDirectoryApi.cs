using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;

namespace Viime.Portable.WebServices
{
    public interface IActiveDirectoryApi
    {
        [Post("/oauth2/token")]
        Task<AuthContract> GetToken([Body(BodySerializationMethod.UrlEncoded)] AuthBody auth);

        //[Get("/users/{user}")]
        //IObservable<User> GetChannelById(string user);

        //[Get("/users/{user}")]
        //IObservable<User> CreateChannel(string user);
    }
}
