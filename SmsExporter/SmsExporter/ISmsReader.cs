using System;
using System.Collections.Generic;
using System.Text;

namespace SmsExporter
{
    public interface ISmsReader
    {
        string GetFolderPath();
        int GetCount();
        void CheckPermission();
        IEnumerator<SmsItem> GetEnumerator();
    }
}
