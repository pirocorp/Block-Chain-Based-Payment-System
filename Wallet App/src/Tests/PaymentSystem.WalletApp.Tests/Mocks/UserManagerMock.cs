namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Identity;

    using Moq;

    public static class UserManagerMock
    {
        public static UserManager<TUser> Get<TUser>(List<TUser> users)
            where TUser : IdentityUser
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<TUser>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            userManager
                .Setup(x => x.DeleteAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => users.Add(x));

            userManager
                .Setup(x => x.UpdateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns<ClaimsPrincipal>(GetId);

            return userManager.Object;
        }

        private static string GetId(ClaimsPrincipal user)
            => user?.Identities
                .FirstOrDefault(i => i.AuthenticationType == "TestAuthentication")
                ?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
    }
}
