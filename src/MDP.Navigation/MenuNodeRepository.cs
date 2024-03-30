using System.Collections.Generic;

namespace MDP.Navigation
{
    public interface MenuNodeRepository
    {
        // Methods
        void SetValue(string menuNodeListKey, List<MenuNode> menuNodeList);

        bool TryGetValue(string menuNodeListKey, out List<MenuNode> menuNodeList);
    }
}