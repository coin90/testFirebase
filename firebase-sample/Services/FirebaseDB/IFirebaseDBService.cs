using System;
using System.Collections.ObjectModel;
using firebasesample.Models;

namespace firebasesample.Services.FirebaseDB
{
    public interface IFirebaseDBService
    {
        void Connect();
        void GetMessage();
        void SetMessage(String message);
        string GetMessageKey();
        void DeleteItem(string key);
    }
}
