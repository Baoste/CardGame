using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteAccountRepository
{
    private readonly string dbPath;

    private string ConnectionString => "URI=file:" + dbPath;

    public SqliteAccountRepository(string databaseFileName)
    {
        dbPath = Path.Combine("/home/ubuntu", databaseFileName);
        Debug.Log("[SqliteAccountRepository] Database path: " + dbPath);
    }

    public void Initialize()
    {
        using (IDbConnection connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS accounts (
                    account_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,

                    chip_count INTEGER NOT NULL DEFAULT 20,
                    chip_color_id INTEGER NOT NULL DEFAULT 0,
                    chip_skin_id INTEGER NOT NULL DEFAULT 0
                );
                ";

                command.ExecuteNonQuery();
            }
        }
    }

    public bool TryCreateAccount(
        string username,
        ChipAppearaceData chipAppearaceData,
        out AccountData accountData,
        out string errorMessage)
    {
        accountData = default;
        errorMessage = null;

        username = NormalizeUsername(username);

        if (!IsValidUsername(username))
        {
            errorMessage = "用户名非法";
            return false;
        }

        if (!IsValidChipAppearance(chipAppearaceData))
        {
            errorMessage = $"筹码皮肤 ID 非法";
            return false;
        }

        try
        {
            using (IDbConnection connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (IDbCommand insertCommand = connection.CreateCommand())
                    {
                        insertCommand.Transaction = transaction;

                        insertCommand.CommandText =
                        @"
                        INSERT INTO accounts (username, chip_count, chip_color_id, chip_skin_id)
                        VALUES (@username, @chip_count, @chip_color_id, @chip_skin_id);
                        ";

                        AddParameter(insertCommand, "@username", username);
                        AddParameter(insertCommand, "@chip_count", 20);
                        AddParameter(insertCommand, "@chip_color_id", chipAppearaceData.ChipColorId);
                        AddParameter(insertCommand, "@chip_skin_id", chipAppearaceData.ChipSkinId);

                        insertCommand.ExecuteNonQuery();
                    }

                    long accountId;

                    using (IDbCommand idCommand = connection.CreateCommand())
                    {
                        idCommand.Transaction = transaction;
                        idCommand.CommandText = "SELECT last_insert_rowid();";

                        object result = idCommand.ExecuteScalar();
                        accountId = Convert.ToInt64(result);
                    }

                    transaction.Commit();

                    accountData = new AccountData(
                        accountId,
                        username,
                        chipCount: 20,
                        new ChipAppearaceData(
                            chipColorId: chipAppearaceData.ChipColorId,
                            chipSkinId: chipAppearaceData.ChipSkinId
                        )
                    );

                    return true;
                }
            }
        }
        catch (SqliteException e)
        {
            // UNIQUE 约束冲突一般就是用户名重复
            if (e.Message.Contains("UNIQUE"))
                errorMessage = "用户名已存在";
            else
                errorMessage = "数据库错误：" + e.Message;

            return false;
        }
        catch (Exception e)
        {
            errorMessage = "服务器错误：" + e.Message;
            return false;
        }
    }

    public bool TryLogin(
        string username,
        out AccountData accountData,
        out string errorMessage)
    {
        accountData = default;
        errorMessage = null;

        username = NormalizeUsername(username);

        if (!IsValidUsername(username))
        {
            errorMessage = "用户名非法";
            return false;
        }

        try
        {
            using (IDbConnection connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                    @"
                    SELECT account_id, username, chip_count, chip_color_id, chip_skin_id
                    FROM accounts
                    WHERE username = @username
                    LIMIT 1;
                    ";

                    AddParameter(command, "@username", username);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            errorMessage = "账号不存在";
                            return false;
                        }

                        accountData = new AccountData(
                            accountId: Convert.ToInt64(reader["account_id"]),
                            username: Convert.ToString(reader["username"]),
                            chipCount: Convert.ToInt32(reader["chip_count"]),
                            chipAppearaceData: new ChipAppearaceData( 
                                Convert.ToInt32(reader["chip_color_id"]),
                                Convert.ToInt32(reader["chip_skin_id"])
                            )
                        );

                        return true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            errorMessage = "服务器错误：" + e.Message;
            return false;
        }
    }

    public bool TryUpdateChipAppearance(
        long accountId,
        ChipAppearaceData chipAppearaceData,
        out AccountData accountData,
        out string errorMessage)
    {
        accountData = default;
        errorMessage = null;

        if (accountId <= 0)
        {
            errorMessage = "账号 ID 非法";
            return false;
        }

        if (!IsValidChipAppearance(chipAppearaceData))
        {
            errorMessage = "筹码外观 ID 非法";
            return false;
        }

        try
        {
            using (IDbConnection connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (IDbCommand updateCommand = connection.CreateCommand())
                    {
                        updateCommand.Transaction = transaction;

                        updateCommand.CommandText =
                        @"
                        UPDATE accounts
                        SET chip_color_id = @chip_color_id,
                            chip_skin_id = @chip_skin_id
                        WHERE account_id = @account_id;
                        ";

                        AddParameter(updateCommand, "@chip_color_id", chipAppearaceData.ChipColorId);
                        AddParameter(updateCommand, "@chip_skin_id", chipAppearaceData.ChipSkinId);
                        AddParameter(updateCommand, "@account_id", accountId);

                        int affectedRows = updateCommand.ExecuteNonQuery();

                        if (affectedRows <= 0)
                        {
                            transaction.Rollback();
                            errorMessage = "账号不存在";
                            return false;
                        }
                    }

                    using (IDbCommand selectCommand = connection.CreateCommand())
                    {
                        selectCommand.Transaction = transaction;

                        selectCommand.CommandText =
                        @"
                        SELECT account_id, username, chip_count, chip_color_id, chip_skin_id
                        FROM accounts
                        WHERE account_id = @account_id
                        LIMIT 1;
                        ";

                        AddParameter(selectCommand, "@account_id", accountId);

                        using (IDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                transaction.Rollback();
                                errorMessage = "账号不存在";
                                return false;
                            }

                            accountData = new AccountData(
                                accountId: Convert.ToInt64(reader["account_id"]),
                                username: Convert.ToString(reader["username"]),
                                chipCount: Convert.ToInt32(reader["chip_count"]),
                                chipAppearaceData: new ChipAppearaceData(
                                    Convert.ToInt32(reader["chip_color_id"]),
                                    Convert.ToInt32(reader["chip_skin_id"])
                                )
                            );
                        }
                    }

                    transaction.Commit();
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            errorMessage = "服务器错误：" + e.Message;
            return false;
        }
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static string NormalizeUsername(string username)
    {
        return string.IsNullOrWhiteSpace(username)
            ? string.Empty
            : username.Trim();
    }

    private static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        if (username.Length < 3 || username.Length > 64)
            return false;

        return true;
    }

    private static bool IsValidChipAppearance(ChipAppearaceData chipAppearaceData)
    {
        return (chipAppearaceData.ChipColorId >= 0 && chipAppearaceData.ChipColorId <= 9999)
            && (chipAppearaceData.ChipSkinId >= 0 && chipAppearaceData.ChipSkinId <= 9999);
    }
}