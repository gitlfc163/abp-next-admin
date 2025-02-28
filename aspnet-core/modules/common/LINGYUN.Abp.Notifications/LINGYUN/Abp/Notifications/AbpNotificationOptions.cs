﻿using System;
using Volo.Abp.Collections;

namespace LINGYUN.Abp.Notifications
{
    // TODO: 需要重命名 AbpNotificationsOptions
    public class AbpNotificationOptions
    {
        /// <summary>
        /// 自定义通知集合
        /// </summary>
        public ITypeList<INotificationDefinitionProvider> DefinitionProviders { get; }
        /// <summary>
        /// 发布者集合
        /// </summary>
        public ITypeList<INotificationPublishProvider> PublishProviders { get; }
        /// <summary>
        /// 可以自定义某个通知的格式
        /// </summary>
        public NotificationDataMappingDictionary NotificationDataMappings { get; }
        /// <summary>
        /// 过期时间
        /// 默认60天
        /// </summary>
        public TimeSpan ExpirationTime { get; set; }
        public AbpNotificationOptions()
        {
            PublishProviders = new TypeList<INotificationPublishProvider>();
            DefinitionProviders = new TypeList<INotificationDefinitionProvider>();
            NotificationDataMappings = new NotificationDataMappingDictionary();

            ExpirationTime = TimeSpan.FromDays(60);
        }
    }
}
