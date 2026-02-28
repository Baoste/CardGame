using System;

namespace Game.Domain
{
    public class GameState
    {
        public int Turn;
        public int CurrentPlayerId;
        public int RandomSeed;

        public Random rng = new Random();

        public PlayerState[] players = new PlayerState[2];
        public SkillDeckState skillCardsDeck = new SkillDeckState();
        public PointCardsDeck pointCardsDeck = new PointCardsDeck();
        //public BoardState Board = new BoardState();
    }
}