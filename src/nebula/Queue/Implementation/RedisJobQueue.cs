﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComposerCore.Attributes;
using Nebula.Connection;
using ServiceStack;

namespace Nebula.Queue.Implementation
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class RedisJobQueue<TItem> : IJobQueue<TItem> where TItem : IJobStep
    {
        private string _jobId;

        [ComponentPlug]
        public IRedisConnectionManager RedisManager { get; set; }

        public void Initialize(string jobId = null)
        {
            _jobId = jobId;
        }

        public async Task<long> GetQueueLength()
        {
            return await RedisManager.GetDatabase().ListLengthAsync(GetRedisKey());
        }

        public async Task Enqueue(TItem item)
        {
            await RedisManager.GetDatabase().ListLeftPushAsync(GetRedisKey(), item.ToJson());
        }

        public async Task EnqueueBatch(IEnumerable<TItem> items)
        {
            var redisKey = GetRedisKey();
            var redisDb = RedisManager.GetDatabase();
            var tasks = items.Select(item => redisDb.ListLeftPushAsync(redisKey, item.ToJson()));
            await Task.WhenAll(tasks);
        }

        public Task EnsureJobSourceExists()
        {
            // Redis lists are created upon adding first item, so nothing to do here.
            return Task.CompletedTask;
        }

        public async Task<bool> Any()
        {
            var queueLength = await RedisManager.GetDatabase().ListLengthAsync(GetRedisKey());
            return queueLength > 0;
        }

        public async Task Purge()
        {
            await RedisManager.GetDatabase().KeyDeleteAsync(GetRedisKey());
        }

        public async Task<TItem> GetNext()
        {
            string serialized = await RedisManager.GetDatabase().ListRightPopAsync(GetRedisKey());
            return serialized.FromJson<TItem>();
        }

        public async Task<IEnumerable<TItem>> GetNextBatch(int maxBatchSize)
        {
            if (maxBatchSize < 1 || maxBatchSize > 10000)
                throw new ArgumentException("MaxBatchSize is out of range");

            var redisKey = GetRedisKey();
            var redisDb = RedisManager.GetDatabase();
            var tasks = Enumerable
                .Range(1, maxBatchSize)
                .Select(i => redisDb.ListRightPopAsync(redisKey));

            var results = await Task.WhenAll(tasks);
            return results
                .Select(r => (string) r)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.FromJson<TItem>());
        }

        #region Private helper methods

        private string GetRedisKey()
        {
            return "job_" + (string.IsNullOrEmpty(_jobId) ? typeof(TItem).Name : _jobId);
        }

        #endregion
    }
}