using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Remove_ForeignKey_From_Unnecessary_Table_20220719 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackAccount_Tenants_TenantId",
                table: "BackAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Address_PrimaryAddressId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_CustomerType_CustomerTypeId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Tenants_TenantId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Users_UserId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddress_Address_AddressId",
                table: "CustomerAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddress_Tenants_TenantId",
                table: "CustomerAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBankAccount_BackAccount_BankAccountId",
                table: "CustomerBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBankAccount_Customer_CustomerId",
                table: "CustomerBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBankAccount_Tenants_TenantId",
                table: "CustomerBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContact_Contact_ContactId",
                table: "CustomerContact");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContact_Customer_CustomerId",
                table: "CustomerContact");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContact_Tenants_TenantId",
                table: "CustomerContact");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOnlineAccount_Customer_CustomerId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOnlineAccount_OnlineAccount_OnlineAccountId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOnlineAccount_Tenants_TenantId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTag_Customer_CustomerId",
                table: "CustomerTag");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTag_Tag_TagId",
                table: "CustomerTag");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTopic_Tenants_TenantId",
                table: "CustomerTopic");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTopic_TenantId",
                table: "CustomerTopic");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTag_CustomerId",
                table: "CustomerTag");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTag_TagId",
                table: "CustomerTag");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOnlineAccount_CustomerId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOnlineAccount_OnlineAccountId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOnlineAccount_TenantId",
                table: "CustomerOnlineAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContact_ContactId",
                table: "CustomerContact");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContact_CustomerId",
                table: "CustomerContact");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContact_TenantId",
                table: "CustomerContact");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBankAccount_BankAccountId",
                table: "CustomerBankAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBankAccount_CustomerId",
                table: "CustomerBankAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBankAccount_TenantId",
                table: "CustomerBankAccount");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddress_AddressId",
                table: "CustomerAddress");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddress_CustomerId",
                table: "CustomerAddress");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddress_TenantId",
                table: "CustomerAddress");

            migrationBuilder.DropIndex(
                name: "IX_Contact_CustomerTypeId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_PrimaryAddressId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_TenantId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_UserId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_BackAccount_TenantId",
                table: "BackAccount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerTopic_TenantId",
                table: "CustomerTopic",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTag_CustomerId",
                table: "CustomerTag",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTag_TagId",
                table: "CustomerTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOnlineAccount_CustomerId",
                table: "CustomerOnlineAccount",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOnlineAccount_OnlineAccountId",
                table: "CustomerOnlineAccount",
                column: "OnlineAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOnlineAccount_TenantId",
                table: "CustomerOnlineAccount",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContact_ContactId",
                table: "CustomerContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContact_CustomerId",
                table: "CustomerContact",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContact_TenantId",
                table: "CustomerContact",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBankAccount_BankAccountId",
                table: "CustomerBankAccount",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBankAccount_CustomerId",
                table: "CustomerBankAccount",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBankAccount_TenantId",
                table: "CustomerBankAccount",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_AddressId",
                table: "CustomerAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_TenantId",
                table: "CustomerAddress",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_CustomerTypeId",
                table: "Contact",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_PrimaryAddressId",
                table: "Contact",
                column: "PrimaryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_TenantId",
                table: "Contact",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UserId",
                table: "Contact",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BackAccount_TenantId",
                table: "BackAccount",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackAccount_Tenants_TenantId",
                table: "BackAccount",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Address_PrimaryAddressId",
                table: "Contact",
                column: "PrimaryAddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_CustomerType_CustomerTypeId",
                table: "Contact",
                column: "CustomerTypeId",
                principalTable: "CustomerType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Tenants_TenantId",
                table: "Contact",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Users_UserId",
                table: "Contact",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddress_Address_AddressId",
                table: "CustomerAddress",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId",
                principalSchema: "AppCRM",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddress_Tenants_TenantId",
                table: "CustomerAddress",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBankAccount_BackAccount_BankAccountId",
                table: "CustomerBankAccount",
                column: "BankAccountId",
                principalTable: "BackAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBankAccount_Customer_CustomerId",
                table: "CustomerBankAccount",
                column: "CustomerId",
                principalSchema: "AppCRM",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBankAccount_Tenants_TenantId",
                table: "CustomerBankAccount",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContact_Contact_ContactId",
                table: "CustomerContact",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContact_Customer_CustomerId",
                table: "CustomerContact",
                column: "CustomerId",
                principalSchema: "AppCRM",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContact_Tenants_TenantId",
                table: "CustomerContact",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOnlineAccount_Customer_CustomerId",
                table: "CustomerOnlineAccount",
                column: "CustomerId",
                principalSchema: "AppCRM",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOnlineAccount_OnlineAccount_OnlineAccountId",
                table: "CustomerOnlineAccount",
                column: "OnlineAccountId",
                principalTable: "OnlineAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOnlineAccount_Tenants_TenantId",
                table: "CustomerOnlineAccount",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTag_Customer_CustomerId",
                table: "CustomerTag",
                column: "CustomerId",
                principalSchema: "AppCRM",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTag_Tag_TagId",
                table: "CustomerTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTopic_Tenants_TenantId",
                table: "CustomerTopic",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");
        }
    }
}
