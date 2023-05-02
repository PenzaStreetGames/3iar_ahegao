#pragma warning disable CS0618

using System.Data;
using System.IO;
using Db.Entity;
using Db.Serialization;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Db {
    public static class SaveRepository {
        static readonly string DBName;

        const string DBFile = "SaveRepository.bytes";
        const string LevelIdDbFiled = "level_id";
        const string UserIdDbField = "user_id";
        const string FieldSateDbField = "field_state";

        static SaveRepository() {
            DBName = "URI=file:" + GetDatabasePath();
            // InitDb();
        }

        static string GetDatabasePath() {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, DBFile);
#elif UNITY_STANDALONE_WIN
            var filePath = Path.Combine(Application.dataPath, DBFile);
            if (!File.Exists(filePath)) {
                UnpackDatabase(filePath, false);
            }
            return filePath;
#elif UNITY_STANDALONE_OSX
            var filePath = Path.Combine(Application.dataPath, DBFile);
            if (!File.Exists(filePath)) {
                UnpackDatabase(filePath, true);
            }
            return filePath;
#elif UNITY_STANDALONE_LINUX
            var filePath = Path.Combine(Application.dataPath, DBFile);
            if (!File.Exists(filePath)) {
                UnpackDatabase(filePath, true);
            }
            return filePath;
#elif UNITY_ANDROID
            string filePath = Path.Combine(Application.persistentDataPath, DBFile);
            if (!File.Exists(filePath)) {
                UnpackDatabase(filePath, true);
            }
            return filePath;
#endif
        }

        static void UnpackDatabase(string toPath, bool nix) {
            var fromPath = Path.Combine(Application.streamingAssetsPath, DBFile);
            if (nix) {
                fromPath = "file://" + fromPath;
            }

            WWW reader = new WWW(fromPath);
            while (!reader.isDone) { }

            File.WriteAllBytes(toPath, reader.bytes);
        }

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

        private static void InitDb() {
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
