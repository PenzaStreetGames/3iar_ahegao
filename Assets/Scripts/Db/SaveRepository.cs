using Db.Entity;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Db {
    public static class SaveRepository {
        const string DBName = "URI=file:SaveRepository.db";

        const string CreateDbCommand = @"
            create table if not exists save
            (
                level_id bigint,
                user_id bigint,
                field_state text
            );
        ";

        static string GetDeletePreviousSaveCommand(Save save) {
            return $@"
                delete from save
                where level_id = {save.GetLevelId()};
            ";
        }

        static string GetPersistSaveCommand(Save save) {
            return $@"
                insert into save
                    (
                         level_id,
                         user_id,
                         field_state
                    )
                values
                    (
                        {save.GetLevelId()},
                        {save.GetUserId()},
                        '{save.GetEncodedFieldState()}'
                    );
            ";
        }

        public static void InitDb() {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = CreateDbCommand;
            command.ExecuteNonQuery();

            Debug.Log($"Db {DBName} initialized");
        }

        public static void PersistSave(Save save) {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetDeletePreviousSaveCommand(save);
            command.ExecuteNonQuery();
            command.CommandText = GetPersistSaveCommand(save);
            command.ExecuteNonQuery();
        }
    }
}
