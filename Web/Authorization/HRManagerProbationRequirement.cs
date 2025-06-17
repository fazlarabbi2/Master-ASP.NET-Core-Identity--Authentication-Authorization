using Microsoft.AspNetCore.Authorization;

namespace Web.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }


    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmployementDate"))
                return Task.CompletedTask;
            if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmployementDate")?.Value, out DateTime employementDate))
            {
                var period = DateTime.Now - employementDate;

                if (period.Days > 30 * requirement.ProbationMonths)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
