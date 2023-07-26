using System;
namespace MyTravelBuddy.Services;

public interface IShellNavigationService
{
    void AddToShellStack(string viewModel);
    int CheckIfExistsInShellStack(string viewName);
    int GetCurrentIndex(int idx);
}

