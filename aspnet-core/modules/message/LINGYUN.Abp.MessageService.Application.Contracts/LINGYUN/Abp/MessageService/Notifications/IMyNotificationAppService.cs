﻿using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LINGYUN.Abp.MessageService.Notifications
{
    public interface IMyNotificationAppService : 
        
        IReadOnlyAppService<
            UserNotificationDto,
            long,
            UserNotificationGetByPagedDto
            >,
        IDeleteAppService<long>
    {
        Task MarkReadStateAsync(NotificationMarkReadStateInput input);

        Task<ListResultDto<NotificationGroupDto>> GetAssignableNotifiersAsync();
    }
}
