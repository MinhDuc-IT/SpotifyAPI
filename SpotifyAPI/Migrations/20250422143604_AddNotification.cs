using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_SenderUserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceiver_Notification_NotificationId",
                table: "NotificationReceiver");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceiver_Users_ReceiverUserId",
                table: "NotificationReceiver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationReceiver",
                table: "NotificationReceiver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "NotificationReceiver",
                newName: "NotificationReceivers");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationReceiver_ReceiverUserId",
                table: "NotificationReceivers",
                newName: "IX_NotificationReceivers_ReceiverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationReceiver_NotificationId",
                table: "NotificationReceivers",
                newName: "IX_NotificationReceivers_NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderUserId",
                table: "Notifications",
                newName: "IX_Notifications_SenderUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationReceivers",
                table: "NotificationReceivers",
                column: "NotiReceiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceivers_Notifications_NotificationId",
                table: "NotificationReceivers",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceivers_Users_ReceiverUserId",
                table: "NotificationReceivers",
                column: "ReceiverUserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_SenderUserId",
                table: "Notifications",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceivers_Notifications_NotificationId",
                table: "NotificationReceivers");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceivers_Users_ReceiverUserId",
                table: "NotificationReceivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_SenderUserId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationReceivers",
                table: "NotificationReceivers");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "NotificationReceivers",
                newName: "NotificationReceiver");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderUserId",
                table: "Notification",
                newName: "IX_Notification_SenderUserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationReceivers_ReceiverUserId",
                table: "NotificationReceiver",
                newName: "IX_NotificationReceiver_ReceiverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationReceivers_NotificationId",
                table: "NotificationReceiver",
                newName: "IX_NotificationReceiver_NotificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationReceiver",
                table: "NotificationReceiver",
                column: "NotiReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_SenderUserId",
                table: "Notification",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceiver_Notification_NotificationId",
                table: "NotificationReceiver",
                column: "NotificationId",
                principalTable: "Notification",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceiver_Users_ReceiverUserId",
                table: "NotificationReceiver",
                column: "ReceiverUserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
