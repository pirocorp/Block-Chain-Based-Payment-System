// ReSharper disable VirtualMemberCallInConstructor
namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants.ApplicationUser;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity, IEntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            this.Address = new Address();

            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();

            this.Accounts = new HashSet<Account>();
            this.AccountKeys = new HashSet<AccountKey>();
            this.Activities = new HashSet<Activity>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Beneficiaries = new HashSet<Counterparty>();
            this.CreditCards = new HashSet<CreditCard>();
        }

        [StringLength(FirstNameLength)]
        public string FirstName { get; set; }

        [StringLength(LastNameLength)]
        public string LastName { get; set; }

        [StringLength(ProfilePictureLength)]
        public string ProfilePicture { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<IdentityUserRole<string>> Roles { get; set; }

        public ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public ICollection<Account> Accounts { get; set; }

        public ICollection<AccountKey> AccountKeys { get; set; }

        public ICollection<Activity> Activities { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }

        public ICollection<Counterparty> Beneficiaries { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
