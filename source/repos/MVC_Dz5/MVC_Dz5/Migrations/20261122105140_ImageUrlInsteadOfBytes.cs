using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Dz5.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlInsteadOfBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "BookImages");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "BookImages",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "BookImages",
                newName: "ContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "BookImages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
