namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data.Models;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Users.Profile;

    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Dashboard()
        {
            return this.View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileUser = this.mapper.Map<ProfileUserViewModel>(user);

            profileUser.Address ??= new Address();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(PersonalDetailsUpdateModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = DateTime.ParseExact(model.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var address = new Address()
            {
                Street = model.Street,
                City = model.City,
                StateProvince = model.StateProvince,
                Zip = model.ZipCode,
                Country = model.Country
            };

            user.Address = address;

            await this.userManager.UpdateAsync(user);

            var controller = ControllerHelpers.GetControllerName<UsersController>();

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }
    }
}
