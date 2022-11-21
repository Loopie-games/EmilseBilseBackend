using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BugReportRepository : IBugReportRepository
    {
        private readonly MySqlConnection _connection;

        public BugReportRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<List<BugReport>> FindAll(string adminId)
        {
            var list = new List<BugReport>();

            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command = new(
                    @"SELECT BugReport.BugReport_Id, BugReport_Title, BugReport_Description, User_Id, User_Username, User_Nickname, User_ProfilePicUrl, StarredBugReport_Id
                        FROM BugReport 
                            JOIN User on BugReport_ReportingUserId = User.User_id
                            Left JOIN StarredBugReport ON BugReport.BugReport_Id = StarredBugReport.StarredBugReport_BugReportId AND StarredBugReport_AdminId = @Admin_Id",
                    _connection);
                {
                    command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = adminId;
                }
                await using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(new BugReport(reader));
            }
            return list;
        }

        public async Task<BugReportEntity> Create(BugReportEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO BugReport VALUES (@Id,@ReportingUserId,@Title,@Description);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@ReportingUserId", MySqlDbType.VarChar).Value = entity.UserId;
                    command.Parameters.Add("@Title", MySqlDbType.VarChar).Value = entity.Title;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;
                }
                command.ExecuteNonQuery();
            }
            return entity;
        }

        public async Task<BugReport> ReadById(string id, string adminId)
        {
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT BugReport.BugReport_Id, BugReport_Title, BugReport_Description, User_Id, User_Username, User_Nickname, User_ProfilePicUrl, StarredBugReport_Id
                        FROM BugReport 
                            JOIN User on BugReport_ReportingUserId = User_id
                            Left JOIN StarredBugReport ON BugReport.BugReport_Id = StarredBugReport.StarredBugReport_BugReportId AND StarredBugReport_AdminId = @Admin_Id
                        WHERE BugReport.BugReport_Id = @Id",
                        con);
                {
                    command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = adminId;
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) return new BugReport(reader);
            }

            throw new Exception($"No {nameof(BugReport)} with id: {id}");
        }

        public async Task AddStar(StarredBugReportEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        @"INSERT INTO StarredBugReport (StarredBugReport_Id, StarredBugReport_AdminId, StarredBugReport_BugReportId) 
                        SELECT @Id,@AdminId, @BugReportId 
                        WHERE NOT EXISTS (SELECT * FROM StarredBugReport WHERE StarredBugReport_AdminId = @AdminId AND StarredBugReport_BugReportId = @BugReportId);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = entity.Id;
                    command.Parameters.Add("@AdminId", MySqlDbType.VarChar).Value = entity.AdminId;
                    command.Parameters.Add("@BugReportId", MySqlDbType.VarChar).Value = entity.BugReportId;
                }
                command.ExecuteNonQuery();
            }
        }
    }
}