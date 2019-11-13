using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raylib;
using rl = Raylib.Raylib;
using GGPOData;
using CSGGPO;

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
        GGPO csg = new GGPO();

        public unsafe Game()
        {
            int sessionRef;
            GGPOErrorCode testSession = (GGPOErrorCode)csg.Test(&sessionRef);
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
