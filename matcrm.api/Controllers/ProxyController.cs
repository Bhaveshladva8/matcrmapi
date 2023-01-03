using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using matcrm.service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    // [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProxyController : Controller
    {
        #region  Service Initialization
        private IMapper _mapper;
        private readonly IProxyService _proxyService;
        private readonly IConfiguration _config;

        #endregion

        public ProxyController(IMapper mapper, IProxyService proxyService, IConfiguration config)
        {
            _mapper = mapper;
            _proxyService = proxyService;
            _config = config;
        }
        [HttpGet("GetProxy")]
        public Task<string> GetProxy()
        {
            // get api and url
            string proxyUrl = "https://testit.weclapp.com/webapp/api/v1/customer?page=1&pageSize=1000";

            return _proxyService.GetProxy(proxyUrl, "7a970695-5f65-4056-ab0e-9c6fd40ad7e6");
        }

        //public Task<string> PostProxy()
        //{
        //    string proxyUrl = "https://testit.weclapp.com/webapp/api/v1/ticket";

        //    return _proxyService.GetProxy("7a970695-5f65-4056-ab0e-9c6fd40ad7e6", proxyUrl);
        //}
        //public Task<string> PutProxy()
        //{
        //    string proxyUrl = "https://testit.weclapp.com/webapp/api/v1/";

        //    return _proxyService.GetProxy("7a970695-5f65-4056-ab0e-9c6fd40ad7e6", proxyUrl);
        //}
    }
}
