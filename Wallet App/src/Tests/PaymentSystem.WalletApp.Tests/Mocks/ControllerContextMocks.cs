namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public static class ControllerContextMocks
    {
        public static ControllerContext FakeTraceIdentifier(string fakeTraceIdentifier)
            => new ()
            {
                HttpContext = new DefaultHttpContext
                {
                    TraceIdentifier = fakeTraceIdentifier,
                },
            };

        public static ControllerContext AnonymousUser()
            => new ()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(),
                },
            };

        public static ControllerContext LoggedInUser(string userId, string username)
            => new ()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                                new Claim(ClaimTypes.NameIdentifier, userId),
                                new Claim(ClaimTypes.Name, username),
                    }, "TestAuthentication")),
                },
            };
    }
}
