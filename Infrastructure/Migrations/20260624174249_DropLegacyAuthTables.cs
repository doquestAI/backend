using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntraUserId = table.Column<string>(type: "varchar", maxLength: 200, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "varchar", maxLength: 200, nullable: false),
                    SubscriptionStatus = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    PlanId = table.Column<string>(type: "varchar", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEmbedding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EmbeddingProcessingStatus = table.Column<int>(type: "integer", nullable: true),
                    ChunksGenerated = table.Column<int>(type: "integer", nullable: true),
                    EmbeddingCompletedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    EmbeddingErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EmbeddingModel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeletionRequestedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    DeletionRequestedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CloudStorageKey = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContentType = table.Column<int>(type: "integer", nullable: false),
                    SignedUrlExpiration = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    SignedUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ContainerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocalTemporaryPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CloudStorageKey = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "varchar", nullable: true),
                    SignedUrlExpiration = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    SignedUrl = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    ContainerName = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    LocalTemporaryPath = table.Column<string>(type: "varchar", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StripeWebhookEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<string>(type: "varchar", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "varchar", maxLength: 100, nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeWebhookEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    PaymentProvider = table.Column<string>(type: "varchar", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "varchar", nullable: true),
                    Subscription_EndDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Subscription_StartDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    StripeCustomerId = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    StripeSubscriptionId = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    SubscriptionType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSubscriptions_EntraUserId",
                table: "AccountSubscriptions",
                column: "EntraUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountSubscriptions_StripeCustomerId",
                table: "AccountSubscriptions",
                column: "StripeCustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsEmbedding",
                table: "Documents",
                column: "IsEmbedding");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Status",
                table: "Documents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UploadedByUserId",
                table: "Documents",
                column: "UploadedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSubscriptions");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "StripeWebhookEvents");

            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}
