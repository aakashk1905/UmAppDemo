using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
    * The pagination class.
    *
    * This class contains the page number for the next query and the number of records on the page.
    *
    * The class instance is returned when you make a paginated query.
    *
    * @param <T> The generic <T> type.
    */
    [Preserve]
    public class PageResult<T> : BaseModel
    {
        /**
        * The number of records on the current page.
        * 
        * If the value of `PageCount` is smaller than the number of records that you expect to get on each page, the current page is the last page.
        * 
        */
        public int PageCount { get; internal set; }

        /**
        * The data of the generic List<T> type.
        */
        public List<T> Data { get; internal set; }

        [Preserve]
        internal PageResult() { }

        [Preserve]
        internal PageResult(string jsonString, ItemCallback callback = null)
        {
            this.callback = callback;
        }

        [Preserve]
        internal PageResult(JSONObject josnObject, ItemCallback callback = null)
        {
            this.callback = callback;
        }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            PageCount = jsonObject["count"].AsInt;
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
            return null;
        }

        private ItemCallback callback;
        internal delegate T ItemCallback(JSONNode jsonObject);
    }
}
