using Microsoft.EntityFrameworkCore.Migrations;

namespace SharedExperinces.WebApi.Migrations
{
	public partial class Migration2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Step 1: Drop the foreign key that references CVR in the Services table
			migrationBuilder.DropForeignKey(
				name: "FK_Services_Providers_CVR",
				table: "Services");


			
			// Step 2: Drop the index on the foreign key column (if any)
			migrationBuilder.DropIndex(
				name: "IX_Services_CVR",
				table: "Services");

			migrationBuilder.DropColumn(
				name: "CVR",
				table: "Services"
				);


			// Step 3: Drop the primary key constraint on CVR in the Providers table
			migrationBuilder.DropPrimaryKey(
				name: "PK_Providers",
				table: "Providers");

			// Step 4: Drop the CVR column in the Providers table
			migrationBuilder.DropColumn(
				name: "CVR",
				table: "Providers");

			// Step 5: Ensure PhoneNumber column has the correct type before setting as primary key
			migrationBuilder.AlterColumn<string>(
				name: "PhoneNumber",
				table: "Providers",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			// Step 6: Set PhoneNumber as the new primary key for the Providers table
			migrationBuilder.AddPrimaryKey(
				name: "PK_Providers",
				table: "Providers",
				column: "PhoneNumber");

			// Step 7: Add PhoneNumber to Services table and create foreign key constraint
			migrationBuilder.AddColumn<string>(
				name: "PhoneNumber",
				table: "Services",
				type: "nvarchar(450)",
				nullable: false,
				defaultValue: "");

			// Step 8: Create an index for PhoneNumber in the Services table
			migrationBuilder.CreateIndex(
				name: "IX_Services_PhoneNumber",
				table: "Services",
				column: "PhoneNumber");

			// Step 9: Add the foreign key relationship between Services and Providers on PhoneNumber
			migrationBuilder.AddForeignKey(
				name: "FK_Services_Providers_PhoneNumber",
				table: "Services",
				column: "PhoneNumber",
				principalTable: "Providers",
				principalColumn: "PhoneNumber",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Rollback changes: Reverse the steps in the Up() method

			// Step 1: Drop the foreign key that references PhoneNumber in the Services table
			migrationBuilder.DropForeignKey(
				name: "FK_Services_Providers_PhoneNumber",
				table: "Services");

			// Step 2: Drop the index on PhoneNumber in Services table
			migrationBuilder.DropIndex(
				name: "IX_Services_PhoneNumber",
				table: "Services");

			// Step 3: Drop the PhoneNumber column from Services table
			migrationBuilder.DropColumn(
				name: "PhoneNumber",
				table: "Services");

			// Step 4: Drop the primary key on PhoneNumber in Providers table
			migrationBuilder.DropPrimaryKey(
				name: "PK_Providers",
				table: "Providers");

			// Step 5: Add the CVR column back to the Providers table
			migrationBuilder.AddColumn<int>(
				name: "CVR",
				table: "Providers",
				type: "int",
				nullable: false,
				defaultValue: 0);

			// Step 6: Set CVR as the primary key again in the Providers table
			migrationBuilder.AddPrimaryKey(
				name: "PK_Providers",
				table: "Providers",
				column: "CVR");

			// Step 7: Add the foreign key on CVR in the Services table again
			migrationBuilder.AddForeignKey(
				name: "FK_Services_Providers_CVR",
				table: "Services",
				column: "CVR",
				principalTable: "Providers",
				principalColumn: "CVR",
				onDelete: ReferentialAction.Cascade);

			// Step 8: Recreate the index for the CVR column in the Services table
			migrationBuilder.CreateIndex(
				name: "IX_Services_CVR",
				table: "Services",
				column: "CVR");
		}
	}
}
