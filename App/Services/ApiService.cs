using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace App.Services
{
    // TODO this should be auto-generated with NSwag, but
    // a) that requires more type annotations
    // b) that does not seem to support .NET 5.0 very well (no System.Text.Json)
    // so for now just copy the defintions 😞
    public record PlateSolveSubmissionModel(Uri ImageUri, string Session, int Id);
    public record PlateSolveJobsForSubmissionModel(int Id, IList<Uri> Jobs);

    public interface IApiService
    {
        Task<PlateSolveJobsForSubmissionModel> GetJobsForSubmissionAsync(int subId);

        Task<PlateSolveSubmissionModel> UploadImageForBlindPlateSolvingAsync(Uri imageUri, string session = default);
    }

    internal class ApiService : IApiService
    {
        private ILogger<ApiService> _logger;
        private HttpClient _client;

        public ApiService(HttpClient client, ILogger<ApiService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<PlateSolveJobsForSubmissionModel> GetJobsForSubmissionAsync(int subId)
        {
            var response = await _client.GetAsync($"platesolve/{subId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PlateSolveJobsForSubmissionModel>();
            }
            else
            {
                return new PlateSolveJobsForSubmissionModel(subId, new Uri[0]);
            }
        }

        public async Task<PlateSolveSubmissionModel> UploadImageForBlindPlateSolvingAsync(Uri imageUri, string session = default)
        {
            if (imageUri is null)
            {
                throw new ArgumentNullException(nameof(imageUri));
            }
            var response = await _client.PostAsJsonAsync("platesolve", new { imageUri, session });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PlateSolveSubmissionModel>();
        }
    }
}