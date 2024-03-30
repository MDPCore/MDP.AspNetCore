using System;
using System.Data;

namespace MDP.Navigation.Lab
{
    public class Program
    {
        // Methods
        public static void Run(NavigationContext navigationContext)
        {
            #region Contracts

            if (navigationContext == null) throw new ArgumentException($"{nameof(navigationContext)}=null");

            #endregion

            // MenuNodeList
            var menuNodeList = navigationContext.FindAllMenuNode();
            if (menuNodeList == null) throw new InvalidOperationException($"{nameof(menuNodeList)}=null");

            // Display
            foreach (var menuNode in menuNodeList)
            {
                DisplayMenuNode(menuNode);
            }
        }

        public static void DisplayMenuNode(MenuNode menuNode, int level = 0)
        {
            #region Contracts

            if (menuNode == null) throw new ArgumentException($"{nameof(menuNode)}=null");

            #endregion

            // MenuNode
            Console.WriteLine(new String('-', level * 2) + $"({menuNode.Priority}){menuNode.Name}");

            // MenuNode.Children
            foreach (var child in menuNode.Children)
            {
                DisplayMenuNode(child, level + 1);
            }
        }

        public static void Main(string[] args)
        {
            // Host
            MDP.NetCore.Host.Run<Program>(args);
        }
    }
}
