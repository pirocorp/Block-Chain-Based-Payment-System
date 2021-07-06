﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentSystem.BlockChain.Data;

namespace PaymentSystem.BlockChain.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PaymentSystem.BlockChain.Data.Models.Account", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<string>("PublicKey")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Address");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PaymentSystem.BlockChain.Data.Models.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("PaymentSystem.Common.Data.Models.Block", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("Height")
                        .HasColumnType("bigint");

                    b.HasKey("Hash");

                    b.HasIndex("Height")
                        .IsUnique();

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("PaymentSystem.Common.Data.Models.Transaction", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("BlockHash")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Fee")
                        .HasColumnType("float");

                    b.Property<string>("Recipient")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TimeStamp")
                        .HasColumnType("bigint");

                    b.HasKey("Hash");

                    b.HasIndex("BlockHash");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("PaymentSystem.Common.Data.Models.Block", b =>
                {
                    b.OwnsOne("PaymentSystem.Common.Data.Models.BlockHeader", "BlockHeader", b1 =>
                        {
                            b1.Property<string>("BlockHash")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("Difficulty")
                                .HasColumnType("int");

                            b1.Property<string>("MerkleRoot")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PreviousHash")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<long>("TimeStamp")
                                .HasColumnType("bigint");

                            b1.Property<string>("Validator")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Version")
                                .HasColumnType("int");

                            b1.HasKey("BlockHash");

                            b1.ToTable("Blocks");

                            b1.WithOwner()
                                .HasForeignKey("BlockHash");
                        });

                    b.Navigation("BlockHeader");
                });

            modelBuilder.Entity("PaymentSystem.Common.Data.Models.Transaction", b =>
                {
                    b.HasOne("PaymentSystem.Common.Data.Models.Block", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("BlockHash");

                    b.Navigation("Block");
                });

            modelBuilder.Entity("PaymentSystem.Common.Data.Models.Block", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
