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
            _connection = connection;
        }

        public async Task<List<BugReport>> FindAll(string adminId)
        {
            var list = new List<BugReport>();

            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT BugReport.BugReport_Id, BugReport_Title, BugReport_Description, U.id AS User_Id, U.username As User_Username, U.nickname As User_Nickname, U.ProfilePicURL as User_ProfilePicUrl, StarredBugReport_Id
                        FROM BugReport 
                            JOIN User U on BugReport_ReportingUserId = U.id
                            Left JOIN StarredBugReport ON BugReport.BugReport_Id = StarredBugReport.BugReport_Id AND StarredBugReport.Admin_Id = @Admin_Id", _connection);
            {
                command.Parameters.Add("@Admin_Id", MySqlDbType.VarChar).Value = adminId;
            }
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new BugReport(reader));

            await _connection.CloseAsync();
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
                        @"SELECT BugReport.BugReport_Id, BugReport_Title, BugReport_Description, U.id AS User_Id, U.username As User_Username, U.nickname As User_Nickname, U.ProfilePicURL as User_ProfilePicUrl, StarredBugReport_Id
                        FROM BugReport 
                            JOIN User U on BugReport_ReportingUserId = U.id
                            Left JOIN StarredBugReport ON BugReport.BugReport_Id = StarredBugReport.BugReport_Id AND StarredBugReport.Admin_Id = @Admin_Id
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
    }
}