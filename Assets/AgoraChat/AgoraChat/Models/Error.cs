#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
    * The error information class.
    */
    [Preserve]
    public class Error
    {
        /**
         * The error code.
         */
        public int Code { get; internal set; }

        /**
         * The error description.
         */
        public string Desc { get; internal set; }

        [Preserve]
        internal Error(int code, string desc)
        {
            Code = code;
            Desc = desc;
        }
    }
}
