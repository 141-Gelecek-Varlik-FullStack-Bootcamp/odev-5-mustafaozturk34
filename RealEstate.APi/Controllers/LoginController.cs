using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RealEstate.Model;
using RealEstate.Service;
using System;

namespace RealEstate.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly IRealEstateOwnerService realEstateOwnerService;

        public LoginController(IDistributedCache _distributedCache, IRealEstateOwnerService _realEstateOwnerService)
        {
            distributedCache = _distributedCache;
            realEstateOwnerService = _realEstateOwnerService;

        }

        [HttpPost]
        public General<bool> Login([FromBody] LoginModel loginUser)
        {
            General<bool> response = new() { Entity = false };
            General<RealEstateOwnerViewModel> result = realEstateOwnerService.Login(loginUser);

            if (result.IsSuccess)
            {
                var cachedData = distributedCache.GetString("LoginUser");

                //if (!memoryCache.TryGetValue("LoginUser", out RealEstateOwnerViewModel _loginUser))
                //{
                //    memoryCache.Set("LoginUser", result.Entity);
                //}
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10)
                };

                if (string.IsNullOrEmpty(cachedData))
                {
                    distributedCache.SetString("LoginUser", JsonConvert.SerializeObject(result.Entity), cacheOptions);
                }

                response.Entity = true;
                response.IsSuccess = true;

            }
            else
            {
                response.ExceptionMessage = "Lutfen giris yapınız";
            }

            return response;
        }
    }
}

