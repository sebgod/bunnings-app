using System;
using System.Collections.Generic;

namespace Api.Models
{
    public record PlateSolveModel(Uri ImageUri, string Session = default);

    public record PlateSolveSubmissionModel(Uri ImageUri, string Session, int Id);

    public record PlateSolveJobsForSubmissionModel(int Id, IList<Uri> Jobs);
}