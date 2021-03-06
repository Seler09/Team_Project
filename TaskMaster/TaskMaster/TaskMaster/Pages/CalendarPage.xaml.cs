﻿using System;
using System.Linq;
using TaskMaster.Interfaces;
using TaskMaster.Pages;
using TaskMaster.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskMaster
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage
    {

        public CalendarPage()
        {
            InitializeComponent();
        }

        private async void MainPageItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
        }

        private async void HistoryPageItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new HistoryPage()));
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
            });
            return base.OnBackButtonPressed();
        }

        private async void PlannedPageItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PlannedViewPage()));
        }

        private async void SyncItem_OnClicked(object sender, EventArgs e)
        {
            var send = await SynchronizationService.Instance.SendActivities();
            if (!send)
            {
                await DisplayAlert("Error", "Wystąpił problem z synchronizacją", "Ok");
                return;
            }
            await SynchronizationService.Instance.GetActivities();
            await SynchronizationService.Instance.SendFavorites();
            await SynchronizationService.Instance.GetFavorites();
            await SynchronizationService.Instance.SendPlannedAsync();
            await SynchronizationService.Instance.GetPlanned();
        }

        private async void LogoutItem_OnClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Uwaga",
                "Wszystkie aktywności zostaną automatycznie zakończone. Kontynuować?", "Tak", "Nie");
            if (!result)
            {
                return;
            }
            var activities = await UserService.Instance.GetActivitiesByStatus(StatusType.Start);
            if (activities.Any(activity => activity.TaskId == 0))
            {
                await DisplayAlert("Error", "Nie można wylogować gdyż są nienazwane aktywności", "Ok");
                return;
            }
            var activitiesPause = await UserService.Instance.GetActivitiesByStatus(StatusType.Pause);
            if (activitiesPause.Any(activity => activity.TaskId == 0))
            {
                await DisplayAlert("Error", "Nie można wylogować gdyż są nienazwane aktywności", "Ok");
                return;
            }
            foreach (var activity in activities)
            {
                var part = await UserService.Instance.GetLastActivityPart(activity.ActivityId);
                StopwatchesService.Instance.StopStopwatch(part.PartId);
                activity.Status = StatusType.Stop;
                part.Stop = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                part.Duration = StopwatchesService.Instance.GetStopwatchTime(part.PartId).ToString();
                await UserService.Instance.SaveActivity(activity);
                await UserService.Instance.SavePartOfActivity(part);
            }
            foreach (var activity in activitiesPause)
            {
                activity.Status = StatusType.Stop;
                await UserService.Instance.SaveActivity(activity);
            }
            await UserService.Instance.LogoutUser();
            DependencyService.Get<ILogOutService>().LogOut();
        }

    }
}

