namespace BL
{
    public record PlateSolverHandle(string Session);
    public record SubmissionHandle(string Session, int Id) : PlateSolverHandle(Session);
}