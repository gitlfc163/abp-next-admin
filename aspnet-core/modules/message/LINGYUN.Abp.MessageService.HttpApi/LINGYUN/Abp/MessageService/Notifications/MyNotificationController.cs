﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LINGYUN.Abp.MessageService.Notifications
{
    [RemoteService(Name = AbpMessageServiceConsts.RemoteServiceName)]
    [Route("api/my-notifilers")]
    public class MyNotificationController : AbpController, IMyNotificationAppService
    {
        protected IMyNotificationAppService MyNotificationAppService { get; }

        public MyNotificationController(
            IMyNotificationAppService myNotificationAppService)
        {
            MyNotificationAppService = myNotificationAppService;
        }

        [HttpPut]
        [Route("mark-read-state")]
        public async virtual Task MarkReadStateAsync(NotificationMarkReadStateInput input)
        {
            await MyNotificationAppService.MarkReadStateAsync(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public async virtual Task DeleteAsync(long id)
        {
            await MyNotificationAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("assignables")]
        public async virtual Task<ListResultDto<NotificationGroupDto>> GetAssignableNotifiersAsync()
        {
            return await MyNotificationAppService.GetAssignableNotifiersAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async virtual Task<UserNotificationDto> GetAsync(long id)
        {
            return await MyNotificationAppService.GetAsync(id);
        }

        [HttpGet]
        public async virtual Task<PagedResultDto<UserNotificationDto>> GetListAsync(UserNotificationGetByPagedDto input)
        {
            return await MyNotificationAppService.GetListAsync(input);
        }
    }
}
