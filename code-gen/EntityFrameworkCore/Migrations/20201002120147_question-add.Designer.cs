// <auto-generated />
using System;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.Migrations
{
    [DbContext(typeof(CodeContext))]
    [Migration("20201002120147_question-add")]
    partial class questionadd
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.SharedDomain.Security.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsCustomer");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Domain.Constants.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArabicName");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("Domain.Constants.LicenceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArabicName");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("LicenceTypes");
                });

            modelBuilder.Entity("Domain.Constants.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArabicName");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("Domain.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Blocked");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("IpAddress");

                    b.Property<string>("LastName");

                    b.Property<int>("NumberOfActivations");

                    b.Property<string>("Phone");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<string>("UserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("UpdatedUserId");

                    b.HasIndex("UserId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Domain.Entities.ProductType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActivatedCount");

                    b.Property<string>("ArabicDescription");

                    b.Property<string>("ArabicHowToActivate");

                    b.Property<string>("ArabicName");

                    b.Property<int?>("BrandId");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<string>("Description");

                    b.Property<int>("GeneratedCount");

                    b.Property<string>("HowToActivate");

                    b.Property<int?>("LicenceTypeId");

                    b.Property<string>("Name");

                    b.Property<int?>("PlatformId");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("LicenceTypeId");

                    b.HasIndex("PlatformId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("Domain.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AnswerContent");

                    b.Property<string>("ArabicAnswerContent");

                    b.Property<string>("ArabicQuestionTitle");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<int>("Order");

                    b.Property<string>("QuestionTitle");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Domain.Entities.RedeemCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ActivationDate");

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedDate");

                    b.Property<string>("CreatedUserId");

                    b.Property<int?>("CustomerId");

                    b.Property<DateTime?>("DeletedDate");

                    b.Property<string>("DeletedUserId");

                    b.Property<int>("Status");

                    b.Property<int?>("TypeId");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUserId");

                    b.Property<bool>("VirtualDeleted");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DeletedUserId");

                    b.HasIndex("TypeId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("RedeemCodes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Domain.Constants.Brand", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Domain.Constants.LicenceType", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Domain.Constants.Platform", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Domain.Entities.Customer", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Domain.Entities.ProductType", b =>
                {
                    b.HasOne("Domain.Constants.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId");

                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Domain.Constants.LicenceType", "LicenceType")
                        .WithMany()
                        .HasForeignKey("LicenceTypeId");

                    b.HasOne("Domain.Constants.Platform", "Platform")
                        .WithMany()
                        .HasForeignKey("PlatformId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Domain.Entities.Question", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Domain.Entities.RedeemCode", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("Domain.Entities.Customer", "Customer")
                        .WithMany("Codes")
                        .HasForeignKey("CustomerId");

                    b.HasOne("Core.SharedDomain.Security.User", "DeletedUser")
                        .WithMany()
                        .HasForeignKey("DeletedUserId");

                    b.HasOne("Domain.Entities.ProductType", "ProductType")
                        .WithMany("Codes")
                        .HasForeignKey("TypeId");

                    b.HasOne("Core.SharedDomain.Security.User", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.SharedDomain.Security.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Core.SharedDomain.Security.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
