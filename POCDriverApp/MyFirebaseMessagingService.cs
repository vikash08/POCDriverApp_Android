using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Firebase.Messaging;
using Android.Graphics;
using POCDriverApp.Entity;
using System.Threading;
using POCDriverApp.Utility;
using OperationListEntity = POCDriverApp.Entity.OperationList;

namespace POCDriverApp
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        private readonly List<PushMessage> _pushMessageQueue;
        private readonly object _handlePushMessageLock = new object();



        public  MyFirebaseMessagingService()
        {
            _pushMessageQueue = new List<PushMessage>();
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
           // Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
            // var pushMessage = new Object();
            //pushMessage.User = message.Data["User"];
            //pushMessage.Type = message.Data["Type"];
            //pushMessage.Message = message.Data["Message"];

          //  Toast.MakeText(this, message.GetNotification().Body, ToastLength.Long);
          //  SendNotification(message.GetNotification().Body);
          //  ThreadPool.QueueUserWorkItem(DispatchMessage, message);

        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(ToDoActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.common_full_open_on_phone)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }


        private void DispatchMessage(object mess)
        {
            try
            {


                RemoteMessage message = (RemoteMessage)mess;
                //   var pushMessage  = message as PushMessage;
                PushMessage pushMessage = new PushMessage();
                pushMessage.User = message.Data["User"];
                pushMessage.Type = message.Data["Type"];
                pushMessage.Message = message.Data["Message"];
                if (pushMessage != null)
                {
                    lock (_pushMessageQueue)
                    {
                        _pushMessageQueue.Add(pushMessage);
                    }
                }
                ProcessQueuedMessages();
            }
            catch (Exception ex)
            {
              
            }

        }
        public void ProcessQueuedMessages()
        {
                      
            if (Monitor.TryEnter(_handlePushMessageLock))
            {
                try
                {
                    PushMessage newMessage;
                    while ((newMessage = GetNextMessage()) != null)
                    {
                       

                        switch (newMessage.Type)
                        {
                            case "G30202_OperationList":
                            case "OperationList":
                               
                                OperationListReceived(newMessage.Message);
                                break;

                            case "G30229_DriverAvailabilityResponse": //Alystra
                            case "G30201_DriverAvailability": //Amphora
                               
                                //DriverAvailabilityResponseReceived(newMessage.Message);
                                break;

                            case "G30216_MessageToDriver":
                               
                               // HandleMessageToDriver(newMessage);
                                break;

                            case "G30212_ValidateResortedOpListResponse":
                               
                                //if (OperationListHandler != null)
                                //    OperationListHandler.ValidateResortedOperationlistResponse(newMessage.Message);
                                break;


                            default:
                               
                                break;
                        }
                    }
                }

                catch (Exception ex)
                {
                   
                }
                finally
                {
                    Monitor.Exit(_handlePushMessageLock);
                }
            }
        }

        private void OperationListReceived(string newMessage)
        {
            try
            {
                var s = Serializer.Deserialize<OperationListEntity>(newMessage);
            }
            catch(Exception e)
            {

            }
          
        }

        internal void ViewAcceptOperationList()
        {
            try
            {
                //if (OperationListHandler != null)
                //{
                //    OperationListHandler.ViewAcceptOperationList();
                //}
            }
            catch (Exception ex)
            {
              
            }

        }

        private PushMessage GetNextMessage()
        {
            PushMessage newMessage = null;
            lock (_pushMessageQueue)
            {
                if (_pushMessageQueue.Count > 0)
                {
                    newMessage = _pushMessageQueue[0];
                    _pushMessageQueue.RemoveAt(0);
                }
            }
            return newMessage;
        }



    }
}