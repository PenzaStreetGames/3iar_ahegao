using System.Data;
using Db.Entity;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Db {
    public static class LevelRepository {
        const string DBName = "URI=file:LevelRepository.db";

        const string LevelIdDbFiled = "level_id";
        const string LevelDbField = "level";

        static string GetCreateDbCommand() {
            return $@"
                create table if not exists level
                (
                    {LevelIdDbFiled} bigint,
                    {LevelDbField} text
                );
            ";
        }

        static string GetPersistLevelCommand(LevelEntity level) {
            return $@"
                insert into save
                    (
                         level_id,
                         user_id,
                         field_state
                    )
                values
                    (
                        {level.GetLevelId()},
                        '{level.GetEncodedLevel()}'
                    );
            ";
        }

        static string GetSelectLevelCommand(long levelId) {
            return $@"
                select * from level
                where
                    level_id = {levelId}
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

        public static void PersistLevel(LevelEntity levelEntity) {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetPersistLevelCommand(levelEntity);
            command.ExecuteNonQuery();
        }

        public static LevelEntity GetLevel(long levelId = -1) {
            using var connection = new SqliteConnection(DBName);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = GetSelectLevelCommand(levelId);
            using var reader = command.ExecuteReader();
            var result = default(LevelEntity);
            if (reader.Read()) {
                result = LevelParser(reader);
            }
            if (reader.Read()) {
                throw new DataException("Multiple rows returned from query");
            }

            return result;
        }

        static LevelEntity LevelParser(IDataRecord reader) {
            return LevelEntity.MakeLevelFromData(
                levelId: (long)reader[LevelIdDbFiled],
                level: (string)reader[LevelDbField]
            );
        }
    }
}
