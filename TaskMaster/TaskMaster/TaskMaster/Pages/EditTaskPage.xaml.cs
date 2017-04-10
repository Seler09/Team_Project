﻿using System;
using System.Diagnostics;
using System.Linq;
using TaskMaster.ModelsDto;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskMaster.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTaskPage
    {
        private readonly UserServices _userServices = new UserServices();
        private Stopwatch _stopwatch;
        private PartsOfActivityDto _actual;
        private ActivitiesDto _activity;
        private TasksDto _task;
        private DateTime _now;
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
            _activity = await _userServices.GetActivity(item.ActivityId);
            _actual = await _userServices.GetLastActivityPart(_activity.ActivityId);
            _task = new TasksDto()
            {
                Name = item.Name,
                Description = item.Description,
                TaskId = item.TaskId
            };
            TaskDates.Text = _actual.Start;
            _stopwatch = App.Stopwatches.ElementAt(_actual.PartId - 1);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PauseButton.IsEnabled = _activity.Status == StatusType.Start;
            ResumeButton.IsEnabled = _activity.Status == StatusType.Pause;
            StopButton.IsEnabled = _activity.Status != StatusType.Planned;
        }
        private async void StopButton_OnClicked(object sender, EventArgs e)
        {
            _stopwatch.Stop();
            _now = DateTime.Now;
            string date = _now.ToString("HH:mm:ss dd/MM/yyyy");
            _actual.Stop = date;
            _actual.Duration = _stopwatch.ElapsedMilliseconds.ToString();
            _activity.Status = StatusType.Stop;
            await _userServices.SaveActivity(_activity);
            await _userServices.SavePartOfActivity(_actual);
            await Navigation.PushModalAsync(new MainPage());
        }

        private async void PauseButton_OnClicked(object sender, EventArgs e)
        {
            _activity.Status = StatusType.Pause;
            _now = DateTime.Now;
            string date = _now.ToString("HH:mm:ss dd/MM/yyyy");
            _actual.Stop = date;
            _stopwatch.Stop();
            _actual.Duration = _stopwatch.ElapsedMilliseconds.ToString(); 
            await _userServices.SaveActivity(_activity);
            await _userServices.SavePartOfActivity(_actual);
            UpdateButtons();
        }

        private async void ResumeButton_OnClicked(object sender, EventArgs e)
        {
            _activity.Status = StatusType.Start;
            _now = DateTime.Now;
            string date = _now.ToString("HH:mm:ss dd/MM/yyyy");
            var part = new PartsOfActivityDto()
            {
                ActivityId = _activity.ActivityId,
                Start = date
            };
            await _userServices.SavePartOfActivity(part);
            Stopwatch sw = new Stopwatch();
            App.Stopwatches.Add(sw);
            App.Stopwatches[App.Stopwatches.Count - 1].Start();
            _actual = part;
            await _userServices.SaveActivity(_activity);
            UpdateButtons();
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
                if (await _userServices.GetTask(_task) == null)
                {
                    var result = await _userServices.SaveTask(_task);
                    _activity.TaskId = result;
                    await _userServices.SaveActivity(_activity);
                }

            }
        }
    }
}
