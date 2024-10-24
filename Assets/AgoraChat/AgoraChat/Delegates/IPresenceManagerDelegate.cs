using System.Collections.Generic;

namespace AgoraChat
{
    /**
     *
     * The presence state listener.
     */
    public interface IPresenceManagerDelegate
    {
        /**
         * Occurs when the presence state of a subscribed user changes.
         *
         * @param presences The new presence state of a subscribed user.
         */
        void OnPresenceUpdated(List<Presence> presences);
    }
}
