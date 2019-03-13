using System;
using System.Collections.Generic;
using System.Text;

namespace SmsExporter
{
    public interface ISmsReader
    {
        string GetFolderPath();
        int GetCount(int startIndex);
        void CheckPermission();
        IEnumerator<SmsItem> GetEnumerator(int startIndex);
    }
}
