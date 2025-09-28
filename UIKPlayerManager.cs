using System.Collections.Generic;

namespace UIKit
{
    public interface UIKPlayerManager
    {
        public static UIKPlayerManager instance;


        public List<UIKPlayer> GetPlayers();
    }
} // UIKit namespace
