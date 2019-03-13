using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmsExporter.Droid
{
    public class SmsReader : ISmsReader
    {
        public const string SMS_URI_ALL = "content://sms/";

        public Func<Android.Net.Uri, string[], string, Android.Database.ICursor> GetCursorFunc { get; set; }

        public int GetCount(int startIndex)
        {
            var uri = Android.Net.Uri.Parse(SMS_URI_ALL);
            using (var cursor = GetCursorFunc(uri, null, "_id >= " + startIndex))
                return cursor.Count;
        }

        public class SmsItemEnumerator : IEnumerator<SmsItem>
        {
            private Android.Database.ICursor cursor;
            public SmsItem Current => new SmsItem()
            {
                Id = cursor.GetString(cursor.GetColumnIndex("_id")),
                Address = cursor.GetString(cursor.GetColumnIndex("address")),
                Date = DateUtils.ToDateTime(cursor.GetLong(cursor.GetColumnIndex("date"))),
                ItemType = (SmsItem.SmsItemType)cursor.GetInt(cursor.GetColumnIndex("type")),
                Body = cursor.GetString(cursor.GetColumnIndex("body")),
            };

            object IEnumerator.Current => this.Current;

            public SmsItemEnumerator(SmsReader reader, int startIndex)
            {
                string SMS_URI_ALL = "content://sms/";
                var uri = Android.Net.Uri.Parse(SMS_URI_ALL);
                cursor = reader.GetCursorFunc(
                    uri,
                    new string[] { "_id", "address", "date", "type", "body" },
                    "_id >= " + startIndex);
            }

            public void Dispose()
            {
                cursor.Close();
                cursor.Dispose();
            }

            public bool MoveNext()
            {
                return cursor.MoveToNext();
            }

            public void Reset()
            {
                cursor.MoveToFirst();
            }
        }

        public IEnumerator<SmsItem> GetEnumerator(int startIndex)
        {
            return new SmsItemEnumerator(this, startIndex);
        }

        public void CheckPermission()
        {
            var permissions = new string[] {
                Android.Manifest.Permission.ReadSms,
                Android.Manifest.Permission.WriteExternalStorage
            };
            foreach (var permission in permissions)
            {
                if (Android.App.Application.Context.CheckSelfPermission(permission) == Android.Content.PM.Permission.Denied)
                    throw new System.Security.SecurityException($"没有[{permission}]权限。");
            }
        }

        public string GetFolderPath()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        }
    }
}