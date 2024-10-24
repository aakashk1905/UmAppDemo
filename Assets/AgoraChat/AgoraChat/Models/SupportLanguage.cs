using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{

    /**
    * Support languages of translation function.
    * 
    */
    [Preserve]
    public class SupportLanguage : BaseModel
    {
        /**
        *  Language code, for example: "zh-Hans" for Chinese Simplified
        */
        public string LanguageCode { get; internal set; }

        /**
        *   Language name, for example: "Chinese Simplified" for Chinese Simplified
        */
        public string LanguageName { get; internal set; }

        /**
        *
        *  Language native name, for example: "中文 (简体)" for Chinese Simplified
        */
        public string LanguageNativeName { get; internal set; }

        [Preserve]
        internal SupportLanguage() { }

        [Preserve]
        internal SupportLanguage(string jsonString) : base(jsonString) { }

        [Preserve]
        internal SupportLanguage(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            LanguageCode = jsonObject["code"];
            LanguageName = jsonObject["name"];
            LanguageNativeName = jsonObject["nativeName"];
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("code", LanguageCode);
            jo.AddWithoutNull("name", LanguageName);
            jo.AddWithoutNull("nativeName", LanguageNativeName);

            return jo;
        }
    }
}
