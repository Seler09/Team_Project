﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaskMaster.BLL.WebApiModels;
using TaskMaster.DAL.DTOModels;
using TaskMaster.DAL.Enum;
using TaskMaster.DAL.Repositories;

namespace TaskMaster.BLL.MobileService
{
    public class PlannedWebApiService
    {
        private readonly UserRepositories _userRepositories = new UserRepositories();
        private readonly GroupWebApiService _groupWebApiService = new GroupWebApiService();
        private readonly TaskRepositories _taskRepositories = new TaskRepositories();
        private readonly ActivityRepositories _activityRepositories = new ActivityRepositories();
        private readonly GroupRepositories _groupRepositories = new GroupRepositories();
        private readonly PartsOfActivityRepositories _partsOfActivityRepositories = new PartsOfActivityRepositories();



        public List<PlannedMobileDto> GetPlanned(string email)
        {
            var user = _userRepositories.Get(email);

            var activityRawList = user.Activities.Where(act => act.State == State.Planned).ToList();

            var returnedList = new List<PlannedMobileDto>();

            foreach (var raw in activityRawList)
            {
                var tmpListOfPatrs = raw.PartsOfActivity.Select(part => new PartsOfActivityMobileDto
                {
                    Start = part.Start.ToString("HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Stop = part.Stop.ToString("HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Duration = part.Duration.ToString("G", CultureInfo.InvariantCulture)
                }).ToList();

                try
                {
                    var tmp = new PlannedMobileDto()
                    {
                        UserEmail = raw.User.Email,
                        Comment = raw.Comment,
                        Guid = raw.Guid,
                        TaskName = raw.Task.Name,
                        Token = null,
                        EditState = raw.EditState,
                        State = raw.State,
                        TaskPart = tmpListOfPatrs.First()
                    };
                    returnedList.Add(tmp);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return returnedList;
        }

        public bool AddPlanned(PlannedMobileDto plannedMobileDto)
        {
            if (plannedMobileDto.State != State.Planned) return false;

            var idAct = 0;
            var tmpTask = _taskRepositories.Get(plannedMobileDto.TaskName);
            if (tmpTask == null)
            {
                tmpTask = new TaskDto()
                {
                    Description = "",
                    Name = plannedMobileDto.TaskName,
                };
                _taskRepositories.Add(tmpTask);
            }
            var tmpActivity = new ActivityDto
            {
                Comment = plannedMobileDto.Comment,
                Guid = plannedMobileDto.Guid,
                State = plannedMobileDto.State,
                EditState = plannedMobileDto.EditState,
                User = _userRepositories.Get(plannedMobileDto.UserEmail),
                Task = _taskRepositories.Get(plannedMobileDto.TaskName),
                Group = _groupRepositories.Get(1)
            };
            try
            {
                idAct = _activityRepositories.Add(tmpActivity);
            }
            catch (Exception e)
            {
                return false;
            }
            
            try
            {
                PartsOfActivityDto tmpPart = new PartsOfActivityDto
                {
                    Start = DateTime.ParseExact(plannedMobileDto.TaskPart.Start, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Stop = DateTime.ParseExact(plannedMobileDto.TaskPart.Start, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Duration = TimeSpan.ParseExact(plannedMobileDto.TaskPart.Duration, "G", CultureInfo.InvariantCulture),
                    Activity = _activityRepositories.Get(idAct)
                };
                _partsOfActivityRepositories.Add(tmpPart);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;

        }

        public bool Delete(PlannedMobileDto plannedMobileDto)
        {
            var toDel = _activityRepositories.Get(plannedMobileDto.UserEmail).First(g => g.Guid.Equals(plannedMobileDto.Guid));
            try
            {
                _activityRepositories.Delete(toDel);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;;
        }



    }
}