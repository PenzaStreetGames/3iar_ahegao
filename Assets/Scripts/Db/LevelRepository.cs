#pragma warning disable CS0618

using System;
using System.Data;
using System.IO;
using Db.Entity;
using Db.Serialization;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Db {
    public static class LevelRepository {
        static readonly string DBName;

        const string DBFile = "LevelRepository.bytes";
        const string LevelIdDbFiled = "level_id";
        const string LevelDbField = "level";

        static LevelRepository() {
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

        private static void InitDb() {
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
