﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskMaster.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskMaster.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTaskPage : ContentPage
    {
        private Stopwatch _stopwatch;
        private PartsOfActivity _actual;
        private Activities _activity;
        private Tasks _task;
        private DateTime _now = new DateTime();
        public EditTaskPage(ElemList item)
        {
            InitializeComponent();
            Initial(item);
            ActivityName.Text = item.Name;
            ActivityDescription.Text = item.Description;
            TaskName.Text = item.Name;
            TaskDescription.Text = item.Description;
        }

        private async void Initial(ElemList item)
        {
            _activity = await App.Database.GetActivity(item.ActivityId);
            _actual = await App.Database.GetLastActivityPart(_activity.ActivityId);
            _task = new Tasks()
            {
                Name = item.Name,
                Description = item.Description,
                TaskId = item.TaskId
            };
            TaskDates.Text = _actual.Start;
            /*_stopwatch = App.Stopwatches.ElementAt(_actual.PartId - 1);
            TimeSpan t = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);*/
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PauseButton.IsEnabled = _activity.Status == StatusType.Start;
            ResumeButton.IsEnabled = _activity.Status == StatusType.Pause;
            StopButton.IsEnabled = _activity.Status == StatusType.Planned;
        }
        private async void StopButton_OnClicked(object sender, EventArgs e)
        {
            //_stopwatch.Stop();
            //await DisplayAlert("Tytul", _stopwatch.ElapsedMilliseconds.ToString(), "E", "F");
            //long mili = _stopwatch.ElapsedMilliseconds;
            DateTime now = DateTime.Now;
            string date = now.ToString("HH:mm:ss dd/MM/yyyy");
            _actual.Stop = date;
            //_actual.Duration = mili.ToString();
            _activity.Status = StatusType.Stop;
            await App.Database.SaveActivity(_activity);
            await App.Database.SavePartOfTask(_actual);
            await Navigation.PushModalAsync(new MainPage());
        }

        private async void PauseButton_OnClicked(object sender, EventArgs e)
        {
            _activity.Status = StatusType.Pause;
            string date = _now.ToString("HH:mm:ss dd/MM/yyyy");
            _actual.Stop = date;
            //_stopwatch.Stop();
            //_actual.Duration = _stopwatch.ElapsedMilliseconds.ToString(); 
            await App.Database.SaveActivity(_activity);
            await App.Database.SavePartOfTask(_actual);
            UpdateButtons();
        }

        private async void ResumeButton_OnClicked(object sender, EventArgs e)
        {
            _activity.Status = StatusType.Start;
            string date = _now.ToString("HH:mm:ss dd/MM/yyyy");
            var part = new PartsOfActivity()
            {
                ActivityId = _activity.ActivityId,
                Start = date
            };
            //Stopwatch sw = new Stopwatch();
            //App.Stopwatches.Add(sw);
            //App.Stopwatches[idk].Start();
            await App.Database.SavePartOfTask(part);
            _actual = part;
        }

        private void ActivityDescription_OnUnfocused(object sender, FocusEventArgs e)
        {
            TaskDescription.Text = ActivityDescription.Text;
            _task.Description = ActivityDescription.Text;
        }

        private void ActivityName_OnUnfocused(object sender, FocusEventArgs e)
        {
            TaskName.Text = ActivityName.Text;
            _task.Name = ActivityName.Text;
        }

        private async void AcceptButton_OnClicked(object sender, EventArgs e)
        {
            if (_task.TaskId == 0)
            {
                if (await App.Database.GetTask(_task) == null)
                {
                    var result = await App.Database.SaveTask(_task);
                    _activity.TaskId = result;
                    await App.Database.SaveActivity(_activity);
                }

            }
        }
    }
}
