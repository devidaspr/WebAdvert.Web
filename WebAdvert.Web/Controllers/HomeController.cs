using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvertApiClient _advertApiClient;
        private readonly ILogger<HomeController> _logger;

        public ISearchApiClient SearchApiClient { get; }
        public IMapper Mapper { get; }

        public HomeController(ISearchApiClient searchApiClient, IAdvertApiClient advertApiClient,
                                IMapper mapper, ILogger<HomeController> logger)
        {
            SearchApiClient = searchApiClient;
            this._advertApiClient = advertApiClient;
            Mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            //var test = User.Identity.IsAuthenticated;
            var allAds = await _advertApiClient.GetAllAsync();
            var allViewModels = allAds.Select(x => Mapper.Map<IndexViewModel>(x));

            return View(allViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            var viewModel = new List<SearchViewModel>();
            var searchResult = await SearchApiClient.Search(keyword);
            searchResult.ForEach(advertDoc =>
            {
                var viewModelItem = Mapper.Map<SearchViewModel>(advertDoc);
                viewModel.Add(viewModelItem);
            });

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
