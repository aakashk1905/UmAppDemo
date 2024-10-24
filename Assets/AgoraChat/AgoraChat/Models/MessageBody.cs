using AgoraChat.SimpleJSON;
using System.Collections.Generic;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public abstract class IMessageBody : BaseModel
    {
        [Preserve]
        public IMessageBody() { }

        [Preserve]
        internal IMessageBody(string jsonString) : base(jsonString) { }

        [Preserve]
        internal IMessageBody(JSONObject jsonObject) : base(jsonObject) { }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("type", Type.ToInt());

            JSONObject jo_body = new JSONObject();
            jo.AddWithoutNull("body", jo_body);

            return jo;
        }

        internal void FromJsonObjectToIMessageBody(JSONObject jo)
        {
            OperationTime = (long)jo["operationTime"].AsDouble;
            OperatorId = jo["operatorId"];
            OperationCount = (long)jo["operationCount"].AsDouble;
        }

        public MessageBodyType Type;

        /**
         * The UNIX timestamp of the last message modification, in milliseconds (readonly).
         *
         */
        public long OperationTime { get; internal set; }

        /**
         * The user ID of the operator that modified the message last time (readonly).
         *
         */
        public string OperatorId { get; internal set; }

        /**
         * The number of times a message is modified (readonly).
         *
         */
        public long OperationCount { get; internal set; }
    }


    /**
     * The message body.
     * 
     */
    namespace MessageBody
    {
        /**
         * The text message body.
         */
        [Preserve]
        public class TextBody : IMessageBody
        {
            /**
             * The text message content.
             */
            public string Text;

            /**
              * Target languages.
              * 
              */
            public List<string> TargetLanguages;

            /**
              * Target languages and translations.
              * 
              */
            public Dictionary<string, string> Translations;

            /**
             * The text message constructor.
             * 
             * @param text  The text message body.
             */
            [Preserve]
            public TextBody(string text)
            {
                Text = text;
                Type = MessageBodyType.TXT;
            }

            [Preserve]
            internal TextBody()
            {
                Type = MessageBodyType.TXT;
            }

            [Preserve]
            internal TextBody(JSONObject jsonObject) : base(jsonObject)
            {
                Type = MessageBodyType.TXT;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                Text = jo["content"];
                TargetLanguages = List.StringListFromJsonArray(jo["targetLanguages"]);
                Translations = Dictionary.StringDictionaryFromJsonObject(jo["translations"]);
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("content", Text);
                jo_body.AddWithoutNull("targetLanguages", JsonObject.JsonArrayFromStringList(TargetLanguages));
                jo_body.AddWithoutNull("translations", JsonObject.JsonObjectFromDictionary(Translations));

                return jo;
            }
        }

        /**
         * The location message body.
         */
        [Preserve]
        public class LocationBody : IMessageBody
        {
            /**
             * The latitude and longitude.
             */
            public double Latitude, Longitude;

            /**
             * The address.
             */
            public string Address;

            /**
             * The building name.
             */
            public string BuildingName;

            /**
             * The location message body constructor.
             * 
             * @param latitude The latitude.
             * @param longitude The longitude.
             * @param address The address.
             * @param buildName The building name.
             */
            [Preserve]
            public LocationBody(double latitude, double longitude, string address = "", string buildName = "")
            {
                Latitude = latitude;
                Longitude = longitude;
                Address = address;
                BuildingName = buildName;
                Type = MessageBodyType.LOCATION;
            }

            [Preserve]
            internal LocationBody()
            {
                Type = MessageBodyType.LOCATION;
            }

            [Preserve]
            internal LocationBody(string jsonString) : base(jsonString)
            {
                Type = MessageBodyType.LOCATION;
            }

            [Preserve]
            internal LocationBody(JSONObject jsonObject) : base(jsonObject)
            {
                Type = MessageBodyType.LOCATION;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                Latitude = jo["latitude"].AsDouble;
                Longitude = jo["longitude"].AsDouble;
                Address = jo["address"].Value;
                BuildingName = jo["buildingName"].Value;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("latitude", Latitude);
                jo_body.AddWithoutNull("longitude", Longitude);
                jo_body.AddWithoutNull("address", Address ?? "");
                jo_body.AddWithoutNull("buildingName", BuildingName ?? "");

                return jo;
            }
        }

        /**
         * The file message body.
         */
        [Preserve]
        public class FileBody : IMessageBody
        {
            /**
             * The local path of the file.
             */
            public string LocalPath;

            /**
             * The display name of the file.
             */
            public string DisplayName;

            /**
             * The secret for downloading the file.
             */
            public string Secret;

            /**
             * The URL where the file is located on the server.
             */
            public string RemotePath;

            /**
             * The file size in bytes.
             */
            public long FileSize;

            /**
             * The file download status.
             */
            public DownLoadStatus DownStatus = DownLoadStatus.PENDING;

            /**
             * The file message body constructor.
             * 
             * @param localPath     The local path of the file.
             * @param displayName   The display name of the file.
             * @param fileSize      The file size in bytes.
             */
            [Preserve]
            public FileBody(string localPath, string displayName = "", long fileSize = 0)
            {
                LocalPath = localPath;
                DisplayName = displayName;
                FileSize = fileSize;
                Type = MessageBodyType.FILE;
            }

            [Preserve]
            internal FileBody()
            {
                Type = MessageBodyType.FILE;
            }

            [Preserve]
            internal FileBody(string json) : base(json)
            {
                Type = MessageBodyType.FILE;
            }

            [Preserve]
            internal FileBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.FILE;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("localPath", LocalPath ?? "");
                jo_body.AddWithoutNull("displayName", DisplayName ?? "");
                jo_body.AddWithoutNull("fileSize", FileSize);
                jo_body.AddWithoutNull("remotePath", RemotePath ?? "");
                jo_body.AddWithoutNull("secret", Secret ?? "");
                jo_body.AddWithoutNull("fileStatus", DownStatus.ToInt());

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                LocalPath = jo["localPath"];
                FileSize = jo["fileSize"];
                DisplayName = jo["displayName"];
                RemotePath = jo["remotePath"];
                Secret = jo["secret"];
                DownStatus = jo["fileStatus"].AsInt.ToDownLoadStatus();
            }
        }

        /**
         * The image message body.
         */
        [Preserve]
        public class ImageBody : FileBody
        {
            /**
             * The local path of the thumbnail.
             */
            public string ThumbnailLocalPath;

            /**
             * The URL where the thumbnail is located on the server.
             */
            public string ThumbnailRemotePath;

            /**
             * The secret for downloading the thumbnail.
             */
            public string ThumbnailSecret;


            /**
             * The status for downloading the thumbnail.
             */
            public DownLoadStatus ThumbnailDownStatus = DownLoadStatus.PENDING;
            /**
             * The width and height of the image file, in pixels.
             */
            public double Height, Width;

            /**
             * Whether to send the original image.
             * - `true`: Yes. 
             * - (Default) `false`: No. If the image is smaller than 100 KB, the SDK sends the original image and its thumbnail. If the image is equal to or greater than 100 KB, the SDK will compress it before sending the compressed image and the thumbnail of the compressed image.
             */
            public bool Original;


            /**
             * The image message body constructor.
             * 
             * @param localPath     The local path of the image.
             * @param displayName   The display name of the image.
             * @param fileSize      The image size in bytes.
             * @param original      Whether to send the original image.
             * @param width         The image width in pixels.
             * @param Height        The image height in pixels.
             * 
             */
            [Preserve]
            public ImageBody(string localPath, string displayName, long fileSize = 0, bool original = false, double width = 0, double height = 0) : base(localPath, displayName, fileSize)
            {
                Original = original;
                Height = height;
                Width = width;
                Type = MessageBodyType.IMAGE;
            }

            [Preserve]
            internal ImageBody()
            {
                Type = MessageBodyType.IMAGE;
            }

            [Preserve]
            internal ImageBody(string json) : base(json)
            {
                Type = MessageBodyType.IMAGE;
            }

            [Preserve]
            internal ImageBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.IMAGE;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("thumbnailLocalPath", ThumbnailLocalPath ?? "");
                jo_body.AddWithoutNull("thumbnailRemotePath", ThumbnailRemotePath ?? "");
                jo_body.AddWithoutNull("thumbnailSecret", ThumbnailSecret ?? "");
                jo_body.AddWithoutNull("thumbnailStatus", ThumbnailDownStatus.ToInt());
                jo_body.AddWithoutNull("height", Height);
                jo_body.AddWithoutNull("width", Width);
                jo_body.AddWithoutNull("sendOriginalImage", Original);

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                base.FromJsonObject(jo);
                ThumbnailLocalPath = jo["thumbnailLocalPath"].Value;
                ThumbnailRemotePath = jo["thumbnailRemotePath"].Value;
                ThumbnailSecret = jo["thumbnailSecret"].Value;
                ThumbnailDownStatus = jo["thumbnailStatus"].AsInt.ToDownLoadStatus();
                Height = jo["height"].AsDouble;
                Width = jo["width"].AsDouble;
                Original = jo["sendOriginalImage"].AsBool;
            }
        }

        /**
         * The voice message body.
         */

        [Preserve]
        public class VoiceBody : FileBody
        {
            /**
             * The voice duration in seconds.
             */
            public int Duration;
            /**
             * The voice message body constructor.
             * 
             * @param localPath     The local path of the voice message.
             * @param displayName   The display name of the voice message.
             * @param duration      The voice duration in seconds.
             * @param fileSize      The size of the voice file, in bytes.
             * 
             */
            [Preserve]
            public VoiceBody(string localPath, string displayName, int duration, long fileSize = 0) : base(localPath, displayName, fileSize)
            {
                Duration = duration;
                Type = MessageBodyType.VOICE;
            }

            [Preserve]
            internal VoiceBody()
            {
                Type = MessageBodyType.VOICE;
            }

            [Preserve]
            internal VoiceBody(string json) : base(json)
            {
                Type = MessageBodyType.VOICE;
            }

            [Preserve]
            internal VoiceBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.VOICE;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("duration", Duration);

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                base.FromJsonObject(jo);
                Duration = jo["duration"].AsInt;
            }
        }

        /**
         * The video message body.
         */
        [Preserve]
        public class VideoBody : FileBody
        {
            /**
             * The local path of the video thumbnail.
             */
            public string ThumbnaiLocationPath;

            /**
             * The URL of the video thumbnail.
             */
            public string ThumbnailRemotePath;

            /**
             * The secret for downloading the thumbnail.
             */
            public string ThumbnailSecret;


            /**
             * The status for downloading the thumbnail.
             */
            public DownLoadStatus ThumbnailDownStatus = DownLoadStatus.PENDING;
            /**
             * The height and width of the video, in pixels.
             */
            public double Height, Width;

            /**
             * The video duration in seconds.
             */
            public int Duration;

            /**
             * The video message body constructor.
             * 
             * @param localPath     The local path of the video message.
             * @param displayName   The display name of the video file.
             * @param duration      The video duration in seconds.
             * @param fileSize      The size of the video file, in bytes.
             * @param thumbnailLocalPath The local path of the video thumbnail.
             * @param width         The video width in pixels.
             * @param Height        The video height in pixels.
             * 
             */
            [Preserve]
            public VideoBody(string localPath, string displayName, int duration, long fileSize = 0, string thumbnailLocalPath = "", double width = 0, double height = 0) : base(localPath, displayName, fileSize)
            {
                Duration = duration;
                Height = height;
                Width = width;
                ThumbnaiLocationPath = thumbnailLocalPath;
                Type = MessageBodyType.VIDEO;
            }

            [Preserve]
            internal VideoBody()
            {
                Type = MessageBodyType.VIDEO;
            }

            [Preserve]
            internal VideoBody(string json) : base(json)
            {
                Type = MessageBodyType.VIDEO;
            }

            [Preserve]
            internal VideoBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.VIDEO;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("thumbnailRemotePath", ThumbnailRemotePath ?? "");
                jo_body.AddWithoutNull("thumbnailSecret", ThumbnailSecret ?? "");
                jo_body.AddWithoutNull("thumbnailLocalPath", ThumbnaiLocationPath ?? "");
                jo_body.AddWithoutNull("thumbnailStatus", ThumbnailDownStatus.ToInt());
                jo_body.AddWithoutNull("height", Height);
                jo_body.AddWithoutNull("width", Width);
                jo_body.AddWithoutNull("duration", Duration);

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                base.FromJsonObject(jo);
                ThumbnailRemotePath = jo["thumbnailRemotePath"];
                ThumbnailSecret = jo["thumbnailSecret"];
                ThumbnaiLocationPath = jo["thumbnailLocalPath"];
                ThumbnailDownStatus = jo["thumbnailStatus"].AsInt.ToDownLoadStatus();
                Height = jo["height"].AsDouble;
                Width = jo["width"].AsDouble;
                Duration = jo["duration"].AsInt;
            }
        }

        /**
         * The command message body.
         */
        [Preserve]
        public class CmdBody : IMessageBody
        {
            /**
             * The command action.
             */
            public string Action;

            /**
             * Whether this command message is delivered only to online users.
             */
            public bool DeliverOnlineOnly;

            /** 
             * The command message body constructor.
             * 
             * @param action                The command action.
             * @param deliverOnlineOnly     Whether this command message is delivered only to online users.
             *                              - `true`: Yes.
             *                              - (Default) `false`: No. The command message is delivered to users, regardless of their online or offline status.
             * 
             */
            [Preserve]
            public CmdBody(string action, bool deliverOnlineOnly = false)
            {
                Action = action;
                DeliverOnlineOnly = deliverOnlineOnly;
                Type = MessageBodyType.CMD;
            }

            [Preserve]
            internal CmdBody()
            {
                Type = MessageBodyType.CMD;
            }

            [Preserve]
            internal CmdBody(string json) : base(json)
            {
                Type = MessageBodyType.CMD;
            }

            [Preserve]
            internal CmdBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.CMD;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("deliverOnlineOnly", DeliverOnlineOnly);
                jo_body.AddWithoutNull("action", Action ?? "");

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                Action = jo["action"].Value;
                DeliverOnlineOnly = jo["deliverOnlineOnly"].AsBool;
            }

        }

        /**
         * The custom message body.
         */
        [Preserve]
        public class CustomBody : IMessageBody
        {
            /**
             * The custom event.
             */
            public string CustomEvent;

            /**
             * The custom params map.
             */
            public Dictionary<string, string> CustomParams;

            /**
             * The constructor for the custom message body.
             * 
             * @param customEvent      The custom event.
             * @param customParams     The custom params map.
             * 
             */
            [Preserve]
            public CustomBody(string customEvent, Dictionary<string, string> customParams = null)
            {
                CustomEvent = customEvent;
                CustomParams = customParams;
                Type = MessageBodyType.CUSTOM;
            }

            [Preserve]
            internal CustomBody()
            {
                Type = MessageBodyType.CUSTOM;
            }

            [Preserve]
            internal CustomBody(string json) : base(json)
            {
                Type = MessageBodyType.CUSTOM;
            }

            [Preserve]
            internal CustomBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.CUSTOM;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();

                JSONObject jo_body = jo["body"].AsObject;
                jo_body.AddWithoutNull("event", CustomEvent);
                jo_body.AddWithoutNull("params", JsonObject.JsonObjectFromDictionary(CustomParams));

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                CustomEvent = jo["event"].Value;
                CustomParams = Dictionary.StringDictionaryFromJsonObject(jo["params"]);
            }
        }

        /**
         * The body of the combined message.
         */
        [Preserve]
        public class CombineBody : IMessageBody
        {
            /**
             * The title of the combined message.
             */
            public string Title;

            /**
             * The summary of the combined message.
             */
            public string Summary;

            /**
             * The compatible text of the combined message.
             */
            public string CompatibleText;

            /**
             * The list of original messages included in the combined message.
             */
            public List<string> MessageList;

            internal string RemotePath;
            internal string Secret;
            internal string LocalPath;

            /**
             * The constructor of the combined message body.
             *
             * @param title             The title of the combined message.
             * @param summary           The summary of the combined message.
             * @param messageList       The list of IDs of original messages included in the combined message.
             *
             */
            [Preserve]
            public CombineBody(string title, string summary, string compatibleText, List<string> messageList)
            {
                Type = MessageBodyType.COMBINE;

                Title = title;
                Summary = summary;
                CompatibleText = compatibleText;
                MessageList = messageList;
            }

            [Preserve]
            internal CombineBody()
            {
                Type = MessageBodyType.COMBINE;
            }

            [Preserve]
            internal CombineBody(string json) : base(json)
            {
                Type = MessageBodyType.COMBINE;
            }

            [Preserve]
            internal CombineBody(JSONObject jo) : base(jo)
            {
                Type = MessageBodyType.COMBINE;
            }

            internal override JSONObject ToJsonObject()
            {
                JSONObject jo = base.ToJsonObject();
                JSONObject jo_body = jo["body"].AsObject;

                jo_body.AddWithoutNull("remotePath", RemotePath);
                jo_body.AddWithoutNull("secret", Secret);
                jo_body.AddWithoutNull("localPath", LocalPath);

                jo_body.AddWithoutNull("title", Title);
                jo_body.AddWithoutNull("summary", Summary);
                jo_body.AddWithoutNull("compatibleText", CompatibleText);
                jo_body.AddWithoutNull("messageList", JsonObject.JsonArrayFromStringList(MessageList));

                return jo;
            }

            internal override void FromJsonObject(JSONObject jo)
            {
                RemotePath = jo["remotePath"];
                Secret = jo["secret"];
                LocalPath = jo["localPath"];

                Title = jo["title"];
                Summary = jo["summary"];
                CompatibleText = jo["compatibleText"];
                MessageList = List.StringListFromJsonArray(jo["messageList"]);
            }
        }
    }

}


