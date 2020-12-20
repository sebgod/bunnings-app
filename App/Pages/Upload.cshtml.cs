using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace App.Pages
{
    [Authorize]
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;

        public UploadModel(ILogger<UploadModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
