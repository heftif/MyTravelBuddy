using System;
namespace MyTravelBuddy.Services;

public class ShellNavigationService : IShellNavigationService
{
    List<string> ShellStack { get; } = new();

    public ShellNavigationService()
	{
	}

    public void AddToShellStack(string viewName)
    {
        ShellStack.Add(viewName);
    }

    public int CheckIfExistsInShellStack(string viewName)
    {
        return ShellStack.FindIndex(x => x.Contains(viewName));
    }

    public int GetCurrentIndex(int idx)
    {
        var length = ShellStack.Count()-1;

        //pop the other views from the stack
        int i = length;
        while(idx < i)
        {
            ShellStack.RemoveAt(i);
            i--;
        }

        return length;
    }
}

