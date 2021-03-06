﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hydrogen.General.Utils;

namespace Nebula.Queue.Implementation
{
    public static class DelayedJobQueueExtentions
    {
        public static async Task Enqueue<TItem>(this IDelayedJobQueue<TItem> delayedQueue, TItem item,
            DateTime processTime) where TItem : IJobStep
        {
            var step = new Tuple<TItem, DateTime>(item, processTime);
            await delayedQueue.EnqueueBatch(step.Yield());
        }

        public static async Task Enqueue<TItem>(this IDelayedJobQueue<TItem> delayedQueue, TItem item,
            TimeSpan delay) where TItem : IJobStep
        {
            var step = new Tuple<TItem, TimeSpan>(item, delay);
            await delayedQueue.EnqueueBatch(step.Yield());
        }

        public static async Task EnqueueBatch<TItem>(this IDelayedJobQueue<TItem> delayedQueue,
            IEnumerable<TItem> items, DateTime processTime) where TItem : IJobStep
        {
            var steps = items.Select(item => new Tuple<TItem, DateTime>(item, processTime)).ToList();

            await delayedQueue.EnqueueBatch(steps);
        }

        public static async Task EnqueueBatch<TItem>(this IDelayedJobQueue<TItem> delayedQueue,
            IEnumerable<TItem> items, TimeSpan delay) where TItem : IJobStep
        {
            var steps = items.Select(item => new Tuple<TItem, TimeSpan>(item, delay)).ToList();

            await delayedQueue.EnqueueBatch(steps);
        }
    }
}