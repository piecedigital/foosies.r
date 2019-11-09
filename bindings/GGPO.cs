/* Physac.cs
*
* Copyright 2019 Chris Dill
*
* Release under zLib License.
* See LICENSE for details.
*/

using System;
using System.Runtime.InteropServices;

namespace GGPO
{
    public enum GGPOErrorCode
    {
        GGPO_OK = 0,
        GGPO_ERRORCODE_SUCCESS = 0,
        GGPO_ERRORCODE_GENERAL_FAILURE = -1,
        GGPO_ERRORCODE_INVALID_SESSION = 1,
        GGPO_ERRORCODE_INVALID_PLAYER_HANDLE = 2,
        GGPO_ERRORCODE_PLAYER_OUT_OF_RANGE = 3,
        GGPO_ERRORCODE_PREDICTION_THRESHOLD = 4,
        GGPO_ERRORCODE_UNSUPPORTED = 5,
        GGPO_ERRORCODE_NOT_SYNCHRONIZED = 6,
        GGPO_ERRORCODE_IN_ROLLBACK = 7,
        GGPO_ERRORCODE_INPUT_DROPPED = 8,
        GGPO_ERRORCODE_PLAYER_DISCONNECTED = 9,
    }

    public enum GGPOPlayerType
    {
        GGPO_PLAYERTYPE_LOCAL,
        GGPO_PLAYERTYPE_REMOTE,
        GGPO_PLAYERTYPE_SPECTATOR,
    }

    public enum GGPOEventCode
    {
        GGPO_EVENTCODE_CONNECTED_TO_PEER = 1000,
        GGPO_EVENTCODE_SYNCHRONIZING_WITH_PEER = 1001,
        GGPO_EVENTCODE_SYNCHRONIZED_WITH_PEER = 1002,
        GGPO_EVENTCODE_RUNNING = 1003,
        GGPO_EVENTCODE_DISCONNECTED_FROM_PEER = 1004,
        GGPO_EVENTCODE_TIMESYNC = 1005,
        GGPO_EVENTCODE_CONNECTION_INTERRUPTED = 1006,
        GGPO_EVENTCODE_CONNECTION_RESUMED = 1007,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GGPOEvent
    {
        GGPOEventCode code;
        public class u
        {
            public struct connected
            {
                /*GGPOPlayerHandle*/
                int player;
            }
            public struct synchronizing
            {
                /*GGPOPlayerHandle*/
                int player;
                int count;
                int total;
            }
            public struct synchronized
            {
                /*GGPOPlayerHandle*/
                int player;
            }
            public struct disconnected
            {
                /*GGPOPlayerHandle*/
                int player;
            }
            public struct timesync
            {
                int frames_ahead;
            }
            public struct connection_interrupted
            {
                /*GGPOPlayerHandle*/
                int player;
                int disconnect_timeout;
            }
            public struct connection_resumed
            {
                /*GGPOPlayerHandle*/
                int player;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GGPOSession
    {
        // ~GGPOSession() { }
        public unsafe delegate GGPOErrorCode DoPoll(int timeout);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode AddPlayer(GGPOPlayer* player, /*GGPOPlayerHandle*/int* handle);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode AddLocalInput(/*GGPOPlayerHandle*/int player, void* values, int size);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode SyncInput(void* values, int size, int* disconnect_flags);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode IncrementFrame();// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode Chat(byte[] text);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode DisconnectPlayer(/*GGPOPlayerHandle*/int handle);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode GetNetworkStats(GGPONetworkStats* stats, /*GGPOPlayerHandle*/int handle);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode Logv(byte[] fmt, params object[] objects);// { return GGPOErrorCode.GGPO_OK; }
        public unsafe delegate GGPOErrorCode SetFrameDelay(/*GGPOPlayerHandle*/int player, int delay);// { return GGPOErrorCode.GGPO_ERRORCODE_UNSUPPORTED; }
        public unsafe delegate GGPOErrorCode SetDisconnectTimeout(int timeout);// { return GGPOErrorCode.GGPO_ERRORCODE_UNSUPPORTED; }
        public unsafe delegate GGPOErrorCode SetDisconnectNotifyStart(int timeout);// { return GGPOErrorCode.GGPO_ERRORCODE_UNSUPPORTED; }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GGPOSessionCallbacks
    {
        /*
            * begin_game callback - This callback has been deprecated.  You must
            * implement it, but should ignore the 'game' parameter.
            */
        public unsafe bool begin_game(byte[] game) { return true; }

        /*
            * save_game_state - The client should allocate a buffer, copy the
            * entire contents of the current game state into it, and copy the
            * length into the *len parameter.  Optionally, the client can compute
            * a checksum of the data and store it in the *checksum argument.
            */
        public unsafe bool save_game_state(byte** buffer, int* len, int* checksum, int frame) { return true; }

        /*
            * load_game_state - GGPO.net will call this function at the beginning
            * of a rollback.  The buffer and len parameters contain a previously
            * saved state returned from the save_game_state function.  The client
            * should make the current game state match the state contained in the
            * buffer.
            */
        public unsafe bool load_game_state(byte* buffer, int len) { return true; }

        /*
            * log_game_state - Used in diagnostic testing.  The client should use
            * the ggpo_log function to write the contents of the specified save
            * state in a human readible form.
            */
        public unsafe bool log_game_state(byte[] filename, byte[] buffer, int len) { return true; }

        /*
            * free_buffer - Frees a game state allocated in save_game_state.  You
            * should deallocate the memory contained in the buffer.
            */
        public unsafe void free_buffer(void* buffer) { return; }

        /*
            * advance_frame - Called during a rollback.  You should advance your game
            * state by exactly one frame.  Before each frame, call ggpo_synchronize_input
            * to retrieve the inputs you should use for that frame.  After each frame,
            * you should call ggpo_advance_frame to notify GGPO.net that you're
            * finished.
            *
            * The flags parameter is reserved.  It can safely be ignored at this time.
            */
        public unsafe bool advance_frame(int flags) { return true; }

        /*
            * on_event - Notification that something has happened.  See the GGPOEventCode
            * structure above for more information.
            */
        public unsafe bool on_event(GGPOEvent* info) { return true; }
    }

    public struct GGPONetworkStats
    {
        public struct network
        {
            int send_queue_len;
            int recv_queue_len;
            int ping;
            int kbps_sent;
        }
        public struct timesync
        {
            int local_frames_behind;
            int remote_frames_behind;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GGPOPlayer
    {
        int size;
        GGPOPlayerType type;
        int player_num;
        public class u
        {
            struct local { };
            struct remote
            {
                string ip_address;
                ushort port;
            }
        }
    }

    public partial class GGPO
    {
        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_start_session(GGPOSession** session, GGPOSessionCallbacks* cb, byte[] game, int num_players, int input_size, int localport);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_add_player(GGPOSession* session, GGPOPlayer* player, /*GGPOPlayerHandle*/int* handle);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_start_synctest(GGPOSession** session, GGPOSessionCallbacks* cb, byte[] game, int num_players, int input_size, int frames);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_close_session(GGPOSession* session);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_set_frame_delay(GGPOSession* session, /*GGPOPlayerHandle*/int player, int frame_delay);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_idle(GGPOSession* session, int timeout);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_add_local_input(GGPOSession* session, /*GGPOPlayerHandle*/int player, void* values, int size);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_synchronize_input(GGPOSession* session, void* values, int size, int* disconnect_flags);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_disconnect_player(GGPOSession* session, /*GGPOPlayerHandle*/int player);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_advance_frame(GGPOSession* session);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_get_network_stats(GGPOSession* session, /*GGPOPlayerHandle*/int player, GGPONetworkStats* sessiontats);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_set_disconnect_timeout(GGPOSession* session, int timeout);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe GGPOErrorCode ggpo_set_disconnect_notify_start(GGPOSession* session, int timeout);

        // [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        // public static extern unsafe void ggpo_log(GGPOSession* session, byte[] fmt, ...);

        [DllImport("ggponet", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void ggpo_logv(GGPOSession* session, byte[] fmt, params object[] objects);
    }
}
