using System;
using System.Runtime.InteropServices;

public static class CSGGPO
{
    const string dllName = "CSGGPO";
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

    /*
    * begin_game callback - This callback has been deprecated.  You must
    * implement it, but should ignore the 'game' parameter.
    */
    public delegate bool BeginGameDelegate(byte[] game);

    /*
    * save_game_state - The client should allocate a buffer, copy the
    * entire contents of the current game state into it, and copy the
    * length into the *len parameter.  Optionally, the client can compute
    * a checksum of the data and store it in the *checksum argument.
    */
    public unsafe delegate bool SaveGameStateDelegate(byte** buffer, ref int len, ref int checksum, int frame);

    /*
    * load_game_state - GGPO.net will call this function at the beginning
    * of a rollback.  The buffer and len parameters contain a previously
    * saved state returned from the save_game_state function.  The client
    * should make the current game state match the state contained in the
    * buffer.
    */
    public unsafe delegate bool LoadGameStateDelegate(byte* buffer, int len);

    /*
    * log_game_state - Used in diagnostic testing.  The client should use
    * the ggpo_log function to write the contents of the specified save
    * state in a human readible form.
    */
    public delegate bool LogGameStateDelegate(byte[] filename, byte[] buffer, int len);

    /*
    * free_buffer - Frees a game state allocated in save_game_state.  You
    * should deallocate the memory contained in the buffer.
    */
    public unsafe delegate void FreeBufferDelegate(void* buffer);

    /*
    * advance_frame - Called during a rollback.  You should advance your game
    * state by exactly one frame.  Before each frame, call ggpo_synchronize_input
    * to retrieve the inputs you should use for that frame.  After each frame,
    * you should call ggpo_advance_frame to notify GGPO.net that you're
    * finished.
    *
    * The flags parameter is reserved.  It can safely be ignored at this time.
    */
    public unsafe delegate bool AdvanceFrameDelegate(int flags);

    /*
    * on_event - Notification that something has happened.  See the GGPOEventCode
    * structure above for more information.
    */
    public unsafe delegate bool OnEventDelegate(GGPOEvent* info);

    public struct GGPOSessionCallbacks
    {
        public BeginGameDelegate beginGameCallback;
        public SaveGameStateDelegate saveGameStateCallback;
        public LoadGameStateDelegate loadGameStateCallback;
        public LogGameStateDelegate logGameStateCallback;
        public FreeBufferDelegate freeBufferCallback;
        public AdvanceFrameDelegate advanceFrameCallback;
        public OnEventDelegate onEventCallback;
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

    [DllImport(dllName)]
    private static extern int CGStartSession(
        out IntPtr session,
        IntPtr beginGameCallback,
        IntPtr saveGameStateCallback,
        IntPtr loadGameStateCallback,
        IntPtr logGameStateCallback,
        IntPtr freeBufferCallback,
        IntPtr advanceFrameCallback,
        IntPtr onEventCallback);

    public unsafe static int StartSession(
        out IntPtr session,
        GGPOSessionCallbacks callbacks)
    {
        return CGStartSession(
            out session,
            Marshal.GetFunctionPointerForDelegate(callbacks.beginGameCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.saveGameStateCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.loadGameStateCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.logGameStateCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.freeBufferCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.advanceFrameCallback),
            Marshal.GetFunctionPointerForDelegate(callbacks.onEventCallback));
    }
}
