using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        //private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        //private readonly string _baseAddress;

        public AdvertApiClient(IConfiguration configuration, HttpClient httpClient, IMapper mapper)
        {
            //this._configuration = configuration;
            this._httpClient = httpClient;
            this._mapper = mapper;

            //var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            //_httpClient.BaseAddress = new Uri(createUrl);
            //_httpClient.DefaultRequestHeaders.Add("content-type", "application/json");

            var baseUrl = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertWebRequest model)
        {
            var confirmAdvertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonConfirmAdverModel = JsonConvert.SerializeObject(confirmAdvertModel);

            var response = await _httpClient.PutAsync(new Uri($"{_httpClient.BaseAddress}/Confirm"),
                                    new StringContent(jsonConfirmAdverModel, Encoding.UTF8, "application/json"));

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<CreateAdvertWebResponse> CreateAsync(CreateAdvertWebRequest model)
        {
            var advertApiModel = _mapper.Map<AdvertModel>(model);
            var jsonCreateAdvertModel = JsonConvert.SerializeObject(advertApiModel);

            var response = await _httpClient.PostAsync(new Uri($"{_httpClient.BaseAddress}/Create"), 
                                        new StringContent(jsonCreateAdvertModel, Encoding.UTF8, "application/json"));
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(jsonResponse);
            var advertResponse = _mapper.Map<CreateAdvertWebResponse>(createAdvertResponse); //AutoMapper
            
            return advertResponse;
        }

        public async Task<List<Advertisement>> GetAllAsync()
        {
            var apiCallResponse = await _httpClient.GetAsync(new Uri($"{_httpClient.BaseAddress}/all"));
            var allResponseModels = await apiCallResponse.Content.ReadAsStringAsync();
            var allAdvertModels = JsonConvert.DeserializeObject<List<AdvertModel>>(allResponseModels).ToList();
            
            return allAdvertModels.Select(x => _mapper.Map<Advertisement>(x)).ToList();
        }
    }
}
