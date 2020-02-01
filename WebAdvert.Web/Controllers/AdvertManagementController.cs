using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            this._fileUploader = fileUploader;
            this._advertApiClient = advertApiClient;
            this._mapper = mapper;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                //var id = "11111";
                //must make a call to Advert Api, create Advertisement in the database and return the Id.
                var createAdvertWebRequest = _mapper.Map<CreateAdvertWebRequest>(model);
                createAdvertWebRequest.UserName = User.Identity.Name;

                var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertWebRequest);
                var Id = apiCallResponse.Id;

                var fileName = "";
                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName)
                                    ? Path.GetFileName(imageFile.FileName) : Id;
                    var filePath = $"{Id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please check the logs for details.");
                        }

                        //Call Advert Api and confirm the advertisement.
                        var confirmModel = new ConfirmAdvertWebRequest()
                        {
                            Id = Id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };

                        var canConfirm = await _advertApiClient.ConfirmAsync(confirmModel);
                        if (!canConfirm)
                        {
                            throw new Exception($"Cannot confrim advert of id = {Id}");
                        }

                        return RedirectToAction("index", "home");
                    }
                    catch (Exception ex)
                    {
                        //Call Advert Api and cancel the advertisement.
                        var confirmModel = new ConfirmAdvertWebRequest()
                        {
                            Id = Id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };

                        await _advertApiClient.ConfirmAsync(confirmModel);

                        Console.WriteLine(ex);
                    }
                }

            }

            return View(model);
        }
    }
}
