using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RealEstate.Model;

namespace RealEstate.APi.Infrastructer
{
    public class BaseController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;

        public BaseController(IDistributedCache _distributedCache)
        {
            distributedCache = _distributedCache;
        }
        public RealEstateOwnerViewModel CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
        }

        private RealEstateOwnerViewModel GetCurrentUser()
        {
            var response = new RealEstateOwnerViewModel();

            //if (memoryCache.TryGetValue("LoginUser", out RealEstateOwnerViewModel _loginUser))
            //{
            //    response = _loginUser;
            //}
            var cachedData = distributedCache.GetString("LoginUser");
            if (!string.IsNullOrEmpty(cachedData))
            {
                response = JsonConvert.DeserializeObject<RealEstateOwnerViewModel>(cachedData);
            }

            return response;
        }
    }
}
