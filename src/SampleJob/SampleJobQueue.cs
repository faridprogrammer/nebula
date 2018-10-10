﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Queue;

namespace SampleJob
{
    public class SampleJobQueue<TItem> : IJobQueue<SampleJobStep>
    {
        private string _jobId;
        public bool QueueExistenceChecked { get; set; }

        public void Initialize(string jobId = null)
        {
            _jobId = jobId;
        }

        public Task<long> GetQueueLength(string jobId = null)
        {
            throw new NotImplementedException();
        }

        public Task Enqueue(SampleJobStep item, string jobId = null)
        {
            return Task.CompletedTask;
        }

        public Task EnqueueBatch(IEnumerable<SampleJobStep> items, string jobId = null)
        {
            throw new NotImplementedException();
        }
        
        public Task EnsureJobSourceExists(string jobId = null)
        {
            QueueExistenceChecked = true;
            return Task.CompletedTask;
        }

        public Task<bool> Any(string jobId = null)
        {
            throw new NotImplementedException();
        }

        public Task Purge(string jobId = null)
        {
            return Task.CompletedTask;
        }

        public Task<SampleJobStep> GetNext(string jobId = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SampleJobStep>> GetNextBatch(int maxBatchSize, string jobId = null)
        {
            throw new NotImplementedException();
        }

        #region Obsolete members

        public Task EnsureJobQueueExists(string jobId = null)
        {
            return EnsureJobSourceExists(jobId);
        }

        public Task PurgeQueueContents(string jobId = null)
        {
            return Purge(jobId);
        }

        public Task<SampleJobStep> Dequeue(string jobId = null)
        {
            return GetNext(jobId);
        }

        public Task<IEnumerable<SampleJobStep>> DequeueBatch(int maxBatchSize, string jobId = null)
        {
            return GetNextBatch(maxBatchSize, jobId);
        }

        #endregion
    }
}