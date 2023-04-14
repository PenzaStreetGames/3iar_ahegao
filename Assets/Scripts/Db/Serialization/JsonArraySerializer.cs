using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Db.Serialization
{
    public static class JsonArraySerializer
    {
        public static string SerializeArray<T>(T[] array)
        {
            return JsonConvert.SerializeObject(array, Formatting.Indented);
        }

        public static string Serialize2DArray<T>(T[,] array)
        {
            var jArray = new JArray();

            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            for (var i = 0; i < rows; i++)
            {
                var rowArray = new JArray();

                for (var j = 0; j < columns; j++)
                {
                    rowArray.Add(JToken.FromObject(array[i, j]));
                }

                jArray.Add(rowArray);
            }

            return jArray.ToString(Formatting.Indented);
        }

        public static T[] DeserializeArray<T>(string json)
        {
            return JsonConvert.DeserializeObject<T[]>(json);
        }

        public static T[,] Deserialize2DArray<T>(string json)
        {
            var jArray = JArray.Parse(json);

            var rows = jArray.Count;
            var columns = jArray[0].Count();

            var array = new T[rows, columns];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    if (jArray[i][j] == null)
                    {
                        array[i, j] = default;
                    }
                    else
                    {
                        array[i, j] = jArray[i][j].ToObject<T>();
                    }
                }
            }

            return array;
        }
    }
}
