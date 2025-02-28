﻿using LINGYUN.Abp.MessageService.EntityFrameworkCore;
using LINGYUN.Abp.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace LINGYUN.Abp.MessageService.Notifications
{
    public class EfCoreUserNotificationRepository : EfCoreRepository<IMessageServiceDbContext, UserNotification, long>,
        IUserNotificationRepository, ITransientDependency
    {
        public EfCoreUserNotificationRepository(
            IDbContextProvider<IMessageServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<bool> AnyAsync(
            Guid userId,
            long notificationId,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .AnyAsync(x => x.NotificationId.Equals(notificationId) && x.UserId.Equals(userId),
                    GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserNotificationInfo> GetByIdAsync(
            Guid userId,
            long notificationId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var userNotifilerQuery = dbContext.Set<UserNotification>()
                .Where(x => x.UserId == userId);

            var notificationQuery = dbContext.Set<Notification>();

            var notifilerQuery = from un in userNotifilerQuery
                                 join n in dbContext.Set<Notification>()
                                         on un.NotificationId equals n.NotificationId
                                 where n.NotificationId.Equals(notificationId)
                                 select new UserNotificationInfo
                                 {
                                     Id = n.NotificationId,
                                     TenantId = n.TenantId,
                                     Name = n.NotificationName,
                                     ExtraProperties = n.ExtraProperties,
                                     CreationTime = n.CreationTime,
                                     NotificationTypeName = n.NotificationTypeName,
                                     Severity = n.Severity,
                                     State = un.ReadStatus,
                                     Type = n.Type
                                 };

            return await notifilerQuery
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public async virtual Task<List<UserNotification>> GetListAsync(
            Guid userId,
            IEnumerable<long> notificationIds,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.UserId.Equals(userId) && notificationIds.Contains(x.NotificationId))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<UserNotificationInfo>> GetNotificationsAsync(
            Guid userId,
            NotificationReadState? readState = null,
            int maxResultCount = 10,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var userNotifilerQuery = dbContext.Set<UserNotification>()
                                              .Where(x => x.UserId == userId)
                                              .WhereIf(readState.HasValue, x => x.ReadStatus == readState.Value);

            var notifilerQuery = from un in userNotifilerQuery
                                 join n in dbContext.Set<Notification>()
                                         on un.NotificationId equals n.NotificationId
                                 select new UserNotificationInfo
                                 {
                                     Id = n.NotificationId,
                                     TenantId = n.TenantId,
                                     Name = n.NotificationName,
                                     ExtraProperties = n.ExtraProperties,
                                     CreationTime = n.CreationTime,
                                     NotificationTypeName = n.NotificationTypeName,
                                     Severity = n.Severity,
                                     State = un.ReadStatus,
                                     Type = n.Type
                                 };

            return await notifilerQuery
                .OrderBy(nameof(Notification.CreationTime) + " DESC")
                .Take(maxResultCount)
                .AsNoTracking()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCountAsync(
            Guid userId,
            string filter = "",
            NotificationReadState? readState = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var userNotifilerQuery = dbContext.Set<UserNotification>()
                .Where(x => x.UserId == userId)
                .WhereIf(readState.HasValue, x => x.ReadStatus == readState.Value);

            var notificationQuery = dbContext.Set<Notification>()
                .WhereIf(!filter.IsNullOrWhiteSpace(), nf =>
                    nf.NotificationName.Contains(filter) ||
                    nf.NotificationTypeName.Contains(filter));

            var notifilerQuery = from un in userNotifilerQuery
                                 join n in notificationQuery
                                         on un.NotificationId equals n.NotificationId
                                 select n;

            return await notifilerQuery
                .CountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<UserNotificationInfo>> GetListAsync(
            Guid userId,
            string filter = "",
            string sorting = nameof(Notification.CreationTime),
            NotificationReadState? readState = null,
            int skipCount = 1,
            int maxResultCount = 10,
            CancellationToken cancellationToken = default)
        {
            sorting ??= $"{nameof(Notification.CreationTime)} DESC";
            var dbContext = await GetDbContextAsync();
            var userNotifilerQuery = dbContext.Set<UserNotification>()
                .Where(x => x.UserId == userId)
                .WhereIf(readState.HasValue, x => x.ReadStatus == readState.Value);

            var notificationQuery = dbContext.Set<Notification>()
                .WhereIf(!filter.IsNullOrWhiteSpace(), nf =>
                    nf.NotificationName.Contains(filter) ||
                    nf.NotificationTypeName.Contains(filter));

            var notifilerQuery = from un in userNotifilerQuery
                                 join n in notificationQuery
                                    on un.NotificationId equals n.NotificationId
                                 select new UserNotificationInfo
                                 {
                                     Id = n.NotificationId,
                                     TenantId = n.TenantId,
                                     Name = n.NotificationName,
                                     ExtraProperties = n.ExtraProperties,
                                     CreationTime = n.CreationTime,
                                     NotificationTypeName = n.NotificationTypeName,
                                     Severity = n.Severity,
                                     State = un.ReadStatus,
                                     Type = n.Type
                                 };

            return await notifilerQuery
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .AsNoTracking()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
