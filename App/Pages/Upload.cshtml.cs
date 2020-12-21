using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using App.Services;
using App.Data;

namespace App.Pages
{
    [Authorize]
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly IApiService _apiService;
        private readonly SubmissionsDbContext _submissionsDbContext;

        public UploadModel(ILogger<UploadModel> logger, IApiService apiService, SubmissionsDbContext submissionsDbContext)
        {
            _logger = logger;
            _apiService = apiService;
            _submissionsDbContext = submissionsDbContext;
        }

        [BindProperty]
        public Uri ImageUri { get; set; }

        [BindProperty]
        public IList<Uri> Submissions {get; set;}

        public void OnGet()
        {

        }

        public void OnPost()
        {
            _apiService.UploadImageForBlindPlateSolvingAsync(ImageUri);

            RedirectToPage();
        }
    }
}
