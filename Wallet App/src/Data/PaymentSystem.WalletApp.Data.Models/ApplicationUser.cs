// ReSharper disable VirtualMemberCallInConstructor
namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity, IEntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();

            this.Accounts = new HashSet<Account>();
            this.AccountKeys = new HashSet<AccountKey>();
            this.Beneficiaries = new HashSet<Beneficiary>();
        }

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

        public ICollection<Beneficiary> Beneficiaries { get; set; }

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

            builder
                .HasMany(e => e.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(e => e.Beneficiaries)
                .WithOne(a => a.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        }
    }
}
