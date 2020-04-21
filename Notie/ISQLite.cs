using System;
using System.Collections.Generic;
using System.Text;

namespace Notie
{
    public interface ISQLite
    {
        string GetDatabasePath(string filename);
    }
}
