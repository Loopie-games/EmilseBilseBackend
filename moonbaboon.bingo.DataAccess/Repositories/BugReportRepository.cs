using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BugReportRepository : IBugReportRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IDbConnectionFactory _connectionFactory;

        public BugReportRepository(MySqlConnection connection, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = connection.Clone();
        }

        public List<BugReport> FindAll(string adminId)
        {
            var list = new List<BugReport>();

            using var con = _connectionFactory.CreateConnection();
            {
                
                using var command = con.CreateCommand();
                command.CommandText = @"SELECT BugReport.BugReport_Id, BugReport_Title, BugReport_Description, User.*, StarredBugReport_Id
                        FROM BugReport 
                            JOIN User on BugReport_ReportingUserId = User.User_id
                            Left JOIN StarredBugReport ON BugReport.BugReport_Id = StarredBugReport.StarredBugReport_BugReportId AND StarredBugReport_AdminId = @Admin_Id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Admin_Id";
                parameter.Value = adminId;
                command.Parameters.Add(parameter);
                
                con.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
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

        public void RemoveStar(string starId, string? adminId)
        {
            using var con = _connectionFactory.CreateConnection();
            using var command = con.CreateCommand();
            command.CommandText = @"Delete from StarredBugReport where StarredBugReport_AdminId = @adminId AND StarredBugReport_Id = @starId";

            con.Open();
            
            var param1 = command.CreateParameter();
            param1.ParameterName = "@adminId";
            param1.Value = adminId;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@starId";
            param2.Value = starId;
            command.Parameters.Add(param2);

            command.ExecuteNonQuery();
        }
    }
}