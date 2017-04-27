﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using TaskMaster.ModelsDto;
using Xamarin.Forms;

namespace TaskMaster.Droid
{
    [Service]
    public class BackgroundStopwatches : Service
    {
        private bool _isWorking = true;
        private readonly UserService _userService = new UserService();
        private readonly List<Stopwatches> _stopwatches = new List<Stopwatches>();
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (App.Stopwatches.Count == 0)
            {
                return StartCommandResult.Sticky;
            }
            GetData();
            Device.StartTimer(TimeSpan.FromMinutes(1), UpdateTimes);
            return StartCommandResult.Sticky;
        }

        private async void GetData()
        {
            if (App.Stopwatches.Count == 0)
            {
                return;
            }
            var list = await _userService.GetActivitiesByStatus(StatusType.Start);
            foreach (var activity in list)
            {
                var part = await _userService.GetLastActivityPart(activity.ActivityId);
                var sw = new Stopwatch();
                var stopwatch = new Stopwatches(sw,part.PartId);
                _stopwatches.Add(stopwatch);
                _stopwatches[_stopwatches.Count -1].Start();
            }
        }

        private bool UpdateTimes()
        {
            foreach (var item in _stopwatches)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var time = item.GetTime();
                    var part = await GetItem(item.GetPartId());
                    part.Duration = time.ToString();
                    SaveItem(part);
                });
            }
            return _isWorking;
        }

        private async Task<PartsOfActivityDto> GetItem(int id)
        {
            var result = await _userService.GetPartsOfActivityById(id);
            return result;
        }
        private async void SaveItem(PartsOfActivityDto item)
        {
            await _userService.SavePartOfActivity(item);
        }

        public override async void OnDestroy()
        {
            foreach (var item in _stopwatches)
            {
                var time = item.GetTime();
                var part = await GetItem(item.GetPartId());
                part.Duration = time.ToString();
                SaveItem(part);
                item.Stop();
            }
            _isWorking = false;
            Console.WriteLine("Wykonało się");
            base.OnDestroy();
        }
    }
}