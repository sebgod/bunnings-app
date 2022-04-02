using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BL
{
    public interface IPlateSolverService
    {
        Task<SubmissionHandle> BlindSolveImageUriAsync(Uri imageUri, string session = default);

        Task<(bool, IList<Uri>)> GetJobsForSubmissionAsync(int subId);
    }
}