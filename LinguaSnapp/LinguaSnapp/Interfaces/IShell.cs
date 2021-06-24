using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinguaSnapp.Interfaces
{
    interface IShell
    {
        Task LoadEditorShellAsync(int pk);

        Task LoadHomeShellAsync(string route = null);
    }
}
