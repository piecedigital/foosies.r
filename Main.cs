using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raylib;
using rl = Raylib.Raylib;


// callbacks to delegate
// begin_game
// save_game_state
// load_game_state
// log_game_state
// free_buffer
// advance_frame
// on_event

namespace foosies
{
    static class Program
    {
        public static void Main()
        {
            new Game();
        }
    }

    class Game
    {
        string testText = "Hello, world!";

        public delegate bool BeginGameDelegate(string game);
        public bool BeginGameCallback(string game)
        {
            Console.WriteLine("begin");
            return true;
        }

        public unsafe delegate bool SaveGameStateDelegate(byte** buffer, ref int len, ref int checksum, int frame);
        public unsafe bool SaveGameStateCallback(byte** buffer, ref int len, ref int checksum, int frame)
        {
            return true;
        }

        public unsafe delegate bool LoadGameStateDelegate(byte* buffer, int len);
        public unsafe bool LoadGameStateCallback(byte* buffer, int len)
        {
            return true;
        }

        public unsafe delegate bool LogGameStateDelegate(string filename, byte* buffer, int len);
        public unsafe bool LogGameStateCallback(string filename, byte* buffer, int len)
        {
            return true;
        }

        public unsafe delegate void FreeBufferDelegate(void* buffer);
        public unsafe void FreeBufferCallback(void* buffer)
        {
            return;
        }

        public delegate bool AdvanceFrameDelegate(int flags);
        public bool AdvanceFrameCallback(int flags)
        {
            return true;
        }

        public unsafe delegate bool OnEventDelegate(int info);
        public bool OnEventCallback(int info)
        {
            return true;
        }

        public unsafe Game()
        {
            IntPtr sessionRef;
            IntPtr beginGameCallback = Marshal.GetFunctionPointerForDelegate(new BeginGameDelegate(BeginGameCallback));
            IntPtr saveGameStateCallback = Marshal.GetFunctionPointerForDelegate(new SaveGameStateDelegate(SaveGameStateCallback));
            IntPtr loadGameStateCallback = Marshal.GetFunctionPointerForDelegate(new LoadGameStateDelegate(LoadGameStateCallback));
            IntPtr logGameStateCallback = Marshal.GetFunctionPointerForDelegate(new LogGameStateDelegate(LogGameStateCallback));
            IntPtr freeBufferCallback = Marshal.GetFunctionPointerForDelegate(new FreeBufferDelegate(FreeBufferCallback));
            IntPtr advanceFrameCallback = Marshal.GetFunctionPointerForDelegate(new AdvanceFrameDelegate(AdvanceFrameCallback));
            IntPtr onEventCallback = Marshal.GetFunctionPointerForDelegate(new OnEventDelegate(OnEventCallback));

            CSGGPO.GGPOErrorCode testSession = (CSGGPO.GGPOErrorCode)CSGGPO.StartSession(
                out sessionRef,
                beginGameCallback,
                saveGameStateCallback,
                loadGameStateCallback,
                logGameStateCallback,
                freeBufferCallback,
                advanceFrameCallback,
                onEventCallback);

            testText = sessionRef.ToString();

            rl.InitWindow(640, 480, "Hello World");

            while (!rl.WindowShouldClose())
            {
                // InputEvent();
                // UpdateEvent();
                RenderEvent();
            }

            rl.CloseWindow();
        }

        public void RenderEvent()
        {
            rl.BeginDrawing();

            rl.ClearBackground(Color.WHITE);
            rl.DrawText(testText, 12, 12, 20, Color.BLACK);

            rl.EndDrawing();
        }
    }
}
