using System;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using firebasesample.iOS.Services.FirebaseAuth;
using firebasesample.Services.FirebaseAuth;
using firebasesample.Services.FirebaseDB;
using firebasesample.iOS.Services.FirebaseDB;
using Foundation;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;
using Firebase.Database;
using System.Collections.ObjectModel;
using firebasesample.Models;

[assembly: Dependency(typeof(FirebaseDBService))]
namespace firebasesample.iOS.Services.FirebaseDB
{
    public class FirebaseDBService : IFirebaseDBService
    {
        public static String KEY_MESSAGE = "items";

        
        private FirebaseAuthService authService = new FirebaseAuthService();
        DatabaseReference databaseReference;
        
        public void Connect(){
            databaseReference = Database.DefaultInstance.GetRootReference();
        }
        public void GetMessage(){
          var userId = authService.GetUserId();

           

            var messages = databaseReference.GetChild("items").GetChild(userId);

            nuint handleReference2 = messages.ObserveEvent(DataEventType.Value, (snapshot) => {
                //var folderData = snapshot.GetValue<NSDictionary>();
                // Do magic with the folder data
                NSDictionary folderData = snapshot.GetValue<NSDictionary>();
                var keys = folderData.Keys;
                ObservableCollection<Homework> list = new ObservableCollection<Homework>();
                foreach (var item in folderData)
                {
                    list.Add(new Homework
                    {
                        Key = item.Key.ToString(),
                        HomeWork = item.Value.ToString()
                    });
                }
                MessagingCenter.Send(FirebaseDBService.KEY_MESSAGE, FirebaseDBService.KEY_MESSAGE, list);

            });
        }
        public void SetMessage(String message){
            var userId = authService.GetUserId();
            var messages = databaseReference.GetChild("items").GetChild(userId).Reference;
            var key = messages.GetChildByAutoId().Key;
            messages.GetChild(key).SetValue((NSString)message);
        }
        public String GetMessageKey(){
            return KEY_MESSAGE;
        }

        public void DeleteItem(string key)
        {
            var userId = authService.GetUserId();
            var messages = databaseReference.GetChild("items").GetChild(userId).Reference;
            messages.GetChild(key).RemoveValue();
        }
    }
}
