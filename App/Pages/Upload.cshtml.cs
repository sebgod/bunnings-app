using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using App.Services;
using App.Data;
using App.Models;

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
        public IList<SubmissionDBO> Submissions { get; set; }

        public async Task OnGetAsync()
        {
            var user = User.Identity.Name;

            Submissions = _submissionsDbContext.SubmissionDBOs.Where(p => p.User == user).ToList();
            foreach (var submission in Submissions)
            {
                var jobsPerSubmission = await _apiService.GetJobsForSubmissionAsync(submission.ID);
                // we currently only support one job
                submission.Job = jobsPerSubmission.Jobs.FirstOrDefault();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = User.Identity.Name;

            var submission = await _apiService.UploadImageForBlindPlateSolvingAsync(ImageUri);
            _submissionsDbContext.SubmissionDBOs.Add(
                new SubmissionDBO {
                    ID = submission.Id,
                    User = user,
                    ImageUri = submission.ImageUri
                }
            );
            _submissionsDbContext.SaveChanges();
            return RedirectToPage();
        }
    }
}
