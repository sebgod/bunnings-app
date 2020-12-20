using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BL
{
    internal record LoginResponse(string status, string messeage, string session);

    internal record UrlUploadResponse(string status, int subid, string hash);

    internal class SubmissionStatusResponse
    {
        [JsonPropertyName("processing_started")] public string Started { get; set; }
        [JsonPropertyName("processing_finished")] public string Finished { get; set; }
        [JsonPropertyName("user")] public int User { get; set; }
        [JsonPropertyName("user_images")] public IList<int?> UserImages { get; set; }
        [JsonPropertyName("jobs")] public IList<int?> Jobs { get; set; }
        [JsonPropertyName("job_calibrations")] public IList<IList<int?>> JobCalibrations { get; set; }
    }

    internal class NovaNetPlateSolverService : IPlateSolverService
    {
        private readonly HttpClient _client;
        private readonly IOptions<PlateSolverOptions> _options;

        public NovaNetPlateSolverService(HttpClient client, IOptions<PlateSolverOptions> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private async Task<string> LoginAsync(string session)
        {
            if (session is not null)
            {
                // no easy way to check if session key is expired. This will have to be handled downstream
                return session;
            }
            
            var response = await _client.PostAsync("api/login", EncodeRequestJson(new { apikey = _options.Value.ApiKey }));
            var responseObject = await DecodeResponseJsonAsync<LoginResponse>(response);

            if (responseObject.status != "success")
            {
                throw new InvalidOperationException("failed to obtain a session key");
            }

            return responseObject.session;
        }

        public async Task<SubmissionHandle> BlindSolveImageUriAsync(Uri imageUri, string session = default)
        {
            var authenticatedSession = await LoginAsync(session);

            var response = await _client.PostAsync("api/url_upload", EncodeRequestJson(new {
                session = authenticatedSession,
                url = imageUri.ToString()
            }));

            var responseObject = await DecodeResponseJsonAsync<UrlUploadResponse>(response);
            if (responseObject.status != "success")
            {
                throw new InvalidOperationException("uploading using image uri failed");
            }

            return new SubmissionHandle(authenticatedSession, responseObject.subid);
        }

        public async Task<(bool, IList<int>)> GetJobsForSubmissionAsync(int subId)
        {
            var response = await _client.GetAsync($"api/submissions/{subId}");

            if (!response.IsSuccessStatusCode)
            {
                return (false, default);
            }

            var responseObject = await DecodeResponseJsonAsync<SubmissionStatusResponse>(response);
                
            if (responseObject.Jobs is null || !responseObject.Jobs.Any())
            {
                return (true, new int[0]);
            }
            else
            {
                return (true, responseObject.Jobs.Where(p => p is not null).Cast<int>().ToList());
            }
        }

        // nova.astrometry.net uses a strange format of www-encoded requests that are seralised json strings
        internal static HttpContent EncodeRequestJson(object request)
            => new FormUrlEncodedContent(new []{
                new KeyValuePair<string, string>("request-json", JsonSerializer.Serialize(request))
            });

        internal static async Task<T> DecodeResponseJsonAsync<T>(HttpResponseMessage response)
            => await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync());
    }
}
