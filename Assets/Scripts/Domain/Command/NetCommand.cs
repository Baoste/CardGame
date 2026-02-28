using System.Collections.Generic;
using System;

namespace Game.Domain
{
    // ===== Command (Client -> Server) =====
    [Serializable]
    public struct NetCommand
    {
        public int index;
        public string type;
        public string jsonData;  // Json
    }
}
