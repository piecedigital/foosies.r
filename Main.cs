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

        public bool BeginGameCallback(string game)
        {
            Console.WriteLine("begin");
            return true;
        }

        public unsafe bool SaveGameStateCallback(byte** buffer, ref int len, ref int checksum, int frame)
        {
            return true;
        }

        public unsafe bool LoadGameStateCallback(byte* buffer, int len)
        {
            return true;
        }

        public unsafe bool LogGameStateCallback(string filename, byte* buffer, int len)
        {
            return true;
        }

        public unsafe void FreeBufferCallback(void* buffer)
        {
            return;
        }

        public bool AdvanceFrameCallback(int flags)
        {
            return true;
        }

        public bool OnEventCallback(int info)
        {
            return true;
        }

        public unsafe Game()
        {
            IntPtr sessionRef;
            CSGGPO.GGPOSessionCallbacks callbacks = new CSGGPO.GGPOSessionCallbacks();
            callbacks.beginGameCallback = new CSGGPO.BeginGameDelegate(callbacks.beginGameCallback);
            callbacks.saveGameStateCallback = new CSGGPO.SaveGameStateDelegate(callbacks.saveGameStateCallback);
            callbacks.loadGameStateCallback = new CSGGPO.LoadGameStateDelegate(callbacks.loadGameStateCallback);
            callbacks.logGameStateCallback = new CSGGPO.LogGameStateDelegate(callbacks.logGameStateCallback);
            callbacks.freeBufferCallback = new CSGGPO.FreeBufferDelegate(callbacks.freeBufferCallback);
            callbacks.advanceFrameCallback = new CSGGPO.AdvanceFrameDelegate(callbacks.advanceFrameCallback);
            callbacks.onEventCallback = new CSGGPO.OnEventDelegate(callbacks.onEventCallback);

            CSGGPO.GGPOErrorCode testSession = (CSGGPO.GGPOErrorCode)CSGGPO.StartSession(
                out sessionRef,
                callbacks);

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
