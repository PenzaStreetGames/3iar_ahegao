using System.Data;
using Db.Entity;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Db {
    public static class SaveRepository {
        const string DBName = "URI=file:SaveRepository.db";

        const string LevelIdDbFiled = "level_id";
        const string UserIdDbField = "user_id";
        const string FieldSateDbField = "field_state";

        static string GetCreateDbCommand() {
            return $@"
                create table if not exists save
                (
                    {LevelIdDbFiled} bigint,
                    {UserIdDbField} bigint,
                    {FieldSateDbField} text
                );
            ";
        }

        static string GetDeletePreviousSaveCommand(SaveEntity saveEntity) {
            return $@"
                delete from save
                where level_id = {saveEntity.GetLevelId()};
            ";
        }

        static string GetPersistSaveCommand(SaveEntity saveEntity) {
            return $@"
                insert into save
                    (
                         level_id,
                         user_id,
                         field_state
                    )
                values
                    (
                        {saveEntity.GetLevelId()},
                        {saveEntity.GetUserId()},
                        '{saveEntity.GetEncodedFieldState()}'
                    );
            ";
        }

        static string GetSelectSaveCommand(long levelId, long userId) {
            return $@"
                select * from save
                where
                    level_id = {levelId} and
                    user_id = {userId}
            ";
        }

        public static void InitDb() {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetCreateDbCommand();
            command.ExecuteNonQuery();

            Debug.Log($"Db {DBName} initialized");
        }

        public static void PersistSave(SaveEntity saveEntity) {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetDeletePreviousSaveCommand(saveEntity);
            command.ExecuteNonQuery();
            command.CommandText = GetPersistSaveCommand(saveEntity);
            command.ExecuteNonQuery();
        }

        public static SaveEntity GetSave(long levelId = -1, long userId = -1) {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetSelectSaveCommand(levelId, userId);
            using var reader = command.ExecuteReader();
            var result = default(SaveEntity);
            if (reader.Read()) {
                result = SaveParser(reader);
            }
            if (reader.Read()) {
                throw new DataException("Multiple rows returned from query");
            }

            return result;
        }

        static SaveEntity SaveParser(IDataRecord reader) {
            return SaveEntity.MakeSaveFromData(
                levelId: (long)reader[LevelIdDbFiled],
                userId: (long)reader[UserIdDbField],
                fieldState: (string)reader[FieldSateDbField]
            );
        }
    }
}
