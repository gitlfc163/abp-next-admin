﻿using LINGYUN.Abp.Notifications;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace LINGYUN.Abp.MessageService.Notifications
{
    [Authorize]
    public class MyNotificationAppService : ApplicationService, IMyNotificationAppService
    {
        protected INotificationSender NotificationSender { get; }

        protected INotificationStore NotificationStore { get; }

        protected IUserNotificationRepository UserNotificationRepository { get; }

        protected INotificationDefinitionManager NotificationDefinitionManager { get; }

        public MyNotificationAppService(
            INotificationStore notificationStore,
            INotificationSender notificationSender,
            IUserNotificationRepository userNotificationRepository,
            INotificationDefinitionManager notificationDefinitionManager)
        {
            NotificationStore = notificationStore;
            NotificationSender = notificationSender;
            UserNotificationRepository = userNotificationRepository;
            NotificationDefinitionManager = notificationDefinitionManager;
        }

        public async virtual Task MarkReadStateAsync(NotificationMarkReadStateInput input)
        {
            await NotificationStore.ChangeUserNotificationsReadStateAsync(
                CurrentTenant.Id,
                CurrentUser.GetId(),
                input.IdList,
                input.State);
        }

        public async virtual Task DeleteAsync(long id)
        {
            await NotificationStore
                .DeleteUserNotificationAsync(
                    CurrentTenant.Id,
                    CurrentUser.GetId(),
                    id);
        }

        public virtual Task<ListResultDto<NotificationGroupDto>> GetAssignableNotifiersAsync()
        {
            var groups = new List<NotificationGroupDto>();

            foreach (var group in NotificationDefinitionManager.GetGroups())
            {
                if (!group.AllowSubscriptionToClients)
                {
                    continue;

                }
                var notificationGroup = new NotificationGroupDto
                {
                    Name = group.Name,
                    DisplayName = group.DisplayName.Localize(StringLocalizerFactory)
                };

                foreach (var notification in group.Notifications)
                {
                    if (!notification.AllowSubscriptionToClients)
                    {
                        continue;
                    }

                    var notificationChildren = new NotificationDto
                    {
                        Name = notification.Name,
                        DisplayName = notification.DisplayName.Localize(StringLocalizerFactory),
                        Description = notification.Description.Localize(StringLocalizerFactory),
                        Lifetime = notification.NotificationLifetime,
                        Type = notification.NotificationType
                    };

                    notificationGroup.Notifications.Add(notificationChildren);
                }

                groups.Add(notificationGroup);
            }

            return Task.FromResult(new ListResultDto<NotificationGroupDto>(groups));
        }

        public async virtual Task<UserNotificationDto> GetAsync(long id)
        {
            var notification = await UserNotificationRepository.GetByIdAsync(CurrentUser.GetId(), id);

            return ObjectMapper.Map<UserNotificationInfo, UserNotificationDto>(notification);
        }

        public async virtual Task<PagedResultDto<UserNotificationDto>> GetListAsync(UserNotificationGetByPagedDto input)
        {
            var totalCount = await UserNotificationRepository
                .GetCountAsync(
                    CurrentUser.GetId(),
                    input.Filter,
                    input.ReadState);

            var notifications = await UserNotificationRepository
                .GetListAsync(
                    CurrentUser.GetId(),
                    input.Filter, input.Sorting,
                    input.ReadState, input.SkipCount, input.MaxResultCount);

            return new PagedResultDto<UserNotificationDto>(totalCount,
                ObjectMapper.Map<List<UserNotificationInfo>, List<UserNotificationDto>>(notifications));
        }
    }
}
