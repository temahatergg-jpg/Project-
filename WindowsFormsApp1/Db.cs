using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public static class Db
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["TasksDb"].ConnectionString;

        public static bool CheckAdmin(string login, string passwordText)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                const string sql = "SELECT IsAdmin FROM Users " +
                                   "WHERE Login = @login AND PasswordHash = @pass";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@pass", passwordText);

                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        return false;
                    }

                    return Convert.ToBoolean(result);
                }
            }
        }

        public static List<TagItem> GetAllTags()
        {
            var list = new List<TagItem>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                const string sql = "SELECT Id, Name FROM Tags ORDER BY Name";

                using (var cmd = new MySqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tag = new TagItem
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name")
                        };
                        list.Add(tag);
                    }
                }
            }

            return list;
        }

        public static List<ContestItem> GetAllContests()
        {
            var list = new List<ContestItem>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                const string sql = "SELECT Id, NameRu, Year FROM Contests ORDER BY Year, NameRu";

                using (var cmd = new MySqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var contest = new ContestItem
                        {
                            Id = reader.GetInt32("Id"),
                            NameRu = reader.GetString("NameRu"),
                            Year = reader.GetInt16("Year")
                        };
                        list.Add(contest);
                    }
                }
            }

            return list;
        }

        public static List<TaskItem> GetTasksFiltered(int minDifficulty, int maxDifficulty, List<int> tagIds)
        {
            var list = new List<TaskItem>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT DISTINCT t.Id, t.TitleRu, t.ShortStatement, t.ShortIdea, " +
                             "t.PolygonUrl, t.CodeforcesPrepared, t.YandexPrepared, t.Difficulty, t.Note " +
                             "FROM Tasks t ";

                if (tagIds.Count > 0)
                {
                    sql += "JOIN TaskTag tt ON tt.TaskId = t.Id ";
                }

                sql += "WHERE t.Difficulty BETWEEN @min AND @max ";

                if (tagIds.Count > 0)
                {
                    string ids = string.Join(",", tagIds);
                    sql += "AND tt.TagId IN (" + ids + ") ";
                    sql += "GROUP BY t.Id HAVING COUNT(DISTINCT tt.TagId) = @tagCount ";
                }

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@min", minDifficulty);
                    cmd.Parameters.AddWithValue("@max", maxDifficulty);

                    if (tagIds.Count > 0)
                    {
                        cmd.Parameters.AddWithValue("@tagCount", tagIds.Count);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new TaskItem
                            {
                                Id = reader.GetInt32("Id"),
                                TitleRu = reader.GetString("TitleRu"),
                                ShortStatement = reader.GetString("ShortStatement"),
                                ShortIdea = reader.IsDBNull(reader.GetOrdinal("ShortIdea"))
                                    ? string.Empty
                                    : reader.GetString("ShortIdea"),
                                PolygonUrl = reader.IsDBNull(reader.GetOrdinal("PolygonUrl"))
                                    ? string.Empty
                                    : reader.GetString("PolygonUrl"),
                                CodeforcesPrepared = reader.GetBoolean("CodeforcesPrepared"),
                                YandexPrepared = reader.GetBoolean("YandexPrepared"),
                                Difficulty = reader.GetInt32("Difficulty"),
                                Note = reader.IsDBNull(reader.GetOrdinal("Note"))
                                    ? string.Empty
                                    : reader.GetString("Note")
                            };

                            list.Add(task);
                        }
                    }
                }
            }

            return list;
        }

        public static TaskItem GetTaskById(int id)
        {
            TaskItem task = null;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                const string sqlTask = "SELECT Id, TitleRu, ShortStatement, ShortIdea, PolygonUrl, " +
                                       "CodeforcesPrepared, YandexPrepared, Difficulty, Note " +
                                       "FROM Tasks WHERE Id = @id";

                using (var cmd = new MySqlCommand(sqlTask, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            task = new TaskItem
                            {
                                Id = reader.GetInt32("Id"),
                                TitleRu = reader.GetString("TitleRu"),
                                ShortStatement = reader.GetString("ShortStatement"),
                                ShortIdea = reader.IsDBNull(reader.GetOrdinal("ShortIdea"))
                                    ? string.Empty
                                    : reader.GetString("ShortIdea"),
                                PolygonUrl = reader.IsDBNull(reader.GetOrdinal("PolygonUrl"))
                                    ? string.Empty
                                    : reader.GetString("PolygonUrl"),
                                CodeforcesPrepared = reader.GetBoolean("CodeforcesPrepared"),
                                YandexPrepared = reader.GetBoolean("YandexPrepared"),
                                Difficulty = reader.GetInt32("Difficulty"),
                                Note = reader.IsDBNull(reader.GetOrdinal("Note"))
                                    ? string.Empty
                                    : reader.GetString("Note")
                            };
                        }
                    }
                }

                if (task == null)
                {
                    return null;
                }

                task.TagIds = new List<int>();
                const string sqlTags = "SELECT TagId FROM TaskTag WHERE TaskId = @taskId";

                using (var cmdTags = new MySqlCommand(sqlTags, connection))
                {
                    cmdTags.Parameters.AddWithValue("@taskId", id);
                    using (var reader = cmdTags.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            task.TagIds.Add(reader.GetInt32("TagId"));
                        }
                    }
                }

                task.ContestIds = new List<int>();
                const string sqlContests = "SELECT ContestId FROM TaskContest WHERE TaskId = @taskId";

                using (var cmdContests = new MySqlCommand(sqlContests, connection))
                {
                    cmdContests.Parameters.AddWithValue("@taskId", id);
                    using (var reader = cmdContests.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            task.ContestIds.Add(reader.GetInt32("ContestId"));
                        }
                    }
                }
            }

            return task;
        }

        public static int InsertTask(TaskItem task)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                const string sql = "INSERT INTO Tasks " +
                                   "(TitleRu, ShortStatement, ShortIdea, PolygonUrl, " +
                                   "CodeforcesPrepared, YandexPrepared, Difficulty, Note) " +
                                   "VALUES " +
                                   "(@title, @statement, @idea, @url, @cf, @yc, @diff, @note); " +
                                   "SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@title", task.TitleRu);
                    cmd.Parameters.AddWithValue("@statement", task.ShortStatement);
                    cmd.Parameters.AddWithValue("@idea", string.IsNullOrWhiteSpace(task.ShortIdea)
                        ? (object)DBNull.Value
                        : task.ShortIdea);
                    cmd.Parameters.AddWithValue("@url", string.IsNullOrWhiteSpace(task.PolygonUrl)
                        ? (object)DBNull.Value
                        : task.PolygonUrl);
                    cmd.Parameters.AddWithValue("@cf", task.CodeforcesPrepared);
                    cmd.Parameters.AddWithValue("@yc", task.YandexPrepared);
                    cmd.Parameters.AddWithValue("@diff", task.Difficulty);
                    cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(task.Note)
                        ? (object)DBNull.Value
                        : task.Note);

                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    SaveTaskTags(connection, newId, task.TagIds);
                    SaveTaskContests(connection, newId, task.ContestIds);

                    return newId;
                }
            }
        }

        public static void UpdateTask(TaskItem task)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                const string sql = "UPDATE Tasks SET " +
                                   "TitleRu = @title, " +
                                   "ShortStatement = @statement, " +
                                   "ShortIdea = @idea, " +
                                   "PolygonUrl = @url, " +
                                   "CodeforcesPrepared = @cf, " +
                                   "YandexPrepared = @yc, " +
                                   "Difficulty = @diff, " +
                                   "Note = @note " +
                                   "WHERE Id = @id";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@title", task.TitleRu);
                    cmd.Parameters.AddWithValue("@statement", task.ShortStatement);
                    cmd.Parameters.AddWithValue("@idea", string.IsNullOrWhiteSpace(task.ShortIdea)
                        ? (object)DBNull.Value
                        : task.ShortIdea);
                    cmd.Parameters.AddWithValue("@url", string.IsNullOrWhiteSpace(task.PolygonUrl)
                        ? (object)DBNull.Value
                        : task.PolygonUrl);
                    cmd.Parameters.AddWithValue("@cf", task.CodeforcesPrepared);
                    cmd.Parameters.AddWithValue("@yc", task.YandexPrepared);
                    cmd.Parameters.AddWithValue("@diff", task.Difficulty);
                    cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(task.Note)
                        ? (object)DBNull.Value
                        : task.Note);
                    cmd.Parameters.AddWithValue("@id", task.Id);

                    cmd.ExecuteNonQuery();
                }

                DeleteTaskTags(connection, task.Id);
                DeleteTaskContests(connection, task.Id);
                SaveTaskTags(connection, task.Id, task.TagIds);
                SaveTaskContests(connection, task.Id, task.ContestIds);
            }
        }

        public static void DeleteTask(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                const string sql = "DELETE FROM Tasks WHERE Id = @id";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SaveTaskTags(MySqlConnection connection, int taskId, List<int> tagIds)
        {
            if (tagIds == null || tagIds.Count == 0)
            {
                return;
            }

            const string sql = "INSERT INTO TaskTag (TaskId, TagId) VALUES (@taskId, @tagId)";

            foreach (int tagId in tagIds)
            {
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void SaveTaskContests(MySqlConnection connection, int taskId, List<int> contestIds)
        {
            if (contestIds == null || contestIds.Count == 0)
            {
                return;
            }

            const string sql = "INSERT INTO TaskContest (TaskId, ContestId) VALUES (@taskId, @contestId)";

            foreach (int contestId in contestIds)
            {
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@contestId", contestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteTaskTags(MySqlConnection connection, int taskId)
        {
            const string sql = "DELETE FROM TaskTag WHERE TaskId = @taskId";

            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void DeleteTaskContests(MySqlConnection connection, int taskId)
        {
            const string sql = "DELETE FROM TaskContest WHERE TaskId = @taskId";

            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
