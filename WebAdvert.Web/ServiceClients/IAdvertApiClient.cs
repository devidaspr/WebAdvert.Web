using AdvertApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<CreateAdvertWebResponse> CreateAsync(CreateAdvertWebRequest model);
        Task<bool> ConfirmAsync(ConfirmAdvertWebRequest model);

        Task<List<Advertisement>> GetAllAsync();
    }
}
