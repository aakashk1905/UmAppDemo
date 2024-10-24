using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public class CursorResult<T> : BaseModel
    {
        /**
	     * The cursor.
	     */
        public string Cursor { get; internal set; }

        /**
        * The data list.
        */
        public List<T> Data { get; internal set; }

        [Preserve]
        internal CursorResult() { }

        [Preserve]
        internal CursorResult(string jsonString, ItemCallback callback = null)
        {
            this.callback = callback;
        }

        [Preserve]
        internal CursorResult(JSONObject jsonObject, ItemCallback callback = null)
        {
            this.callback = callback;
        }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            Cursor = jsonObject["cursor"];
            JSONNode jn = jsonObject["list"];
            if (jn.IsArray)
            {
                JSONArray jsonArray = jn.AsArray;
                Data = new List<T>();
                foreach (var jsonObj in jsonArray)
                {
                    object ret = callback(jsonObj);
                    if (ret != null)
                    {
                        Data.Add((T)ret);
                    }
                }
            }
            callback = null;
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("cursor", Cursor);
            // Note: Data type?
            return jo;
        }

        private ItemCallback callback;
        internal delegate T ItemCallback(JSONNode jsonNode);
    }
}
