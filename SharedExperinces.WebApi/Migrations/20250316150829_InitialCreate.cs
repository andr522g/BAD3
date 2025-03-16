using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedExperinces.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    GuestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.GuestId);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    CVR = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.CVR);
                });

            migrationBuilder.CreateTable(
                name: "SharedExperiences",
                columns: table => new
                {
                    SharedExperienceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedExperiences", x => x.SharedExperienceId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CVR = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_Services_Providers_CVR",
                        column: x => x.CVR,
                        principalTable: "Providers",
                        principalColumn: "CVR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestSharedExperience",
                columns: table => new
                {
                    GuestsGuestId = table.Column<int>(type: "int", nullable: false),
                    SharedExperiencesSharedExperienceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestSharedExperience", x => new { x.GuestsGuestId, x.SharedExperiencesSharedExperienceId });
                    table.ForeignKey(
                        name: "FK_GuestSharedExperience_Guests_GuestsGuestId",
                        column: x => x.GuestsGuestId,
                        principalTable: "Guests",
                        principalColumn: "GuestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuestSharedExperience_SharedExperiences_SharedExperiencesSharedExperienceId",
                        column: x => x.SharedExperiencesSharedExperienceId,
                        principalTable: "SharedExperiences",
                        principalColumn: "SharedExperienceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => new { x.ServiceId, x.GuestCount });
                    table.ForeignKey(
                        name: "FK_Discounts_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    RegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    GuestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.RegistrationId);
                    table.ForeignKey(
                        name: "FK_Registrations_Guests_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Guests",
                        principalColumn: "GuestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSharedExperience",
                columns: table => new
                {
                    ServicesServiceId = table.Column<int>(type: "int", nullable: false),
                    SharedExperiencesSharedExperienceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSharedExperience", x => new { x.ServicesServiceId, x.SharedExperiencesSharedExperienceId });
                    table.ForeignKey(
                        name: "FK_ServiceSharedExperience_Services_ServicesServiceId",
                        column: x => x.ServicesServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceSharedExperience_SharedExperiences_SharedExperiencesSharedExperienceId",
                        column: x => x.SharedExperiencesSharedExperienceId,
                        principalTable: "SharedExperiences",
                        principalColumn: "SharedExperienceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestSharedExperience_SharedExperiencesSharedExperienceId",
                table: "GuestSharedExperience",
                column: "SharedExperiencesSharedExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_GuestId",
                table: "Registrations",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_ServiceId",
                table: "Registrations",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CVR",
                table: "Services",
                column: "CVR");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSharedExperience_SharedExperiencesSharedExperienceId",
                table: "ServiceSharedExperience",
                column: "SharedExperiencesSharedExperienceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "GuestSharedExperience");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "ServiceSharedExperience");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "SharedExperiences");

            migrationBuilder.DropTable(
                name: "Providers");
        }
    }
}
