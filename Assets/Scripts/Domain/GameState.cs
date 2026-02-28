using System;

namespace Game.Domain
{
    public class GameState
    {
        public int Turn;
        public int CurrentPlayerId;
        public int RandomSeed;

        public Random rng = new Random(12345);

        public PlayerState[] players;
        public SkillDeckState skillCardsDeck = new SkillDeckState();
        public PointCardsDeck pointCardsDeck = new PointCardsDeck();
        //public BoardState Board = new BoardState();

        public GameState()
        {
            players = new PlayerState[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerState();
            }
        }
    }
}