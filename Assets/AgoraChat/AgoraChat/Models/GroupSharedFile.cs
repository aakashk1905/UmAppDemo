using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
    * The shared file information class, which defines how to manage shared files.
    * 
    * For example, you can get information about a group shared file by using {@link IGroupManager#GetGroupFileListFromServer(String, int, int, ValueCallBack)}.
    * 
    */
    [Preserve]
    public class GroupSharedFile : BaseModel
    {
        /**
         * The name of the shared file.
         */
        public string FileName { get; internal set; }

        /**
         * The ID of the shared file.
         */
        public string FileId { get; internal set; }

        /**
         * The user ID of the member who uploads the shared file.
         */
        public string FileOwner { get; internal set; }

        /**
         * The Unix timestamp for updating the shared file. The unit is millisecond.
         */
        public long CreateTime { get; internal set; }

        /**
         * The size of the shared file, in bytes.
         */
        public long FileSize { get; internal set; }

        [Preserve]
        internal GroupSharedFile() { }

        [Preserve]
        internal GroupSharedFile(string jsonString) : base(jsonString) { }

        [Preserve]
        internal GroupSharedFile(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            FileName = jsonObject["name"];
            FileId = jsonObject["fileId"];
            FileOwner = jsonObject["owner"];
            CreateTime = (long)jsonObject["createTime"].AsDouble;
            FileSize = (long)jsonObject["fileSize"].AsDouble;
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("name", FileName);
            jo.AddWithoutNull("fileId", FileId);
            jo.AddWithoutNull("owner", FileOwner);
            jo.AddWithoutNull("createTime", CreateTime);
            jo.AddWithoutNull("fileSize", FileSize);
            return jo;
        }
    }
}
