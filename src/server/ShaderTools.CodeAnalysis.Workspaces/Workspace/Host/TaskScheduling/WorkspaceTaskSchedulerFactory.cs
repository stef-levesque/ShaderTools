﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using ShaderTools.CodeAnalysis.Host.Mef;
using System.Composition;

namespace ShaderTools.CodeAnalysis.Host
{
    [ExportWorkspaceService(typeof(IWorkspaceTaskSchedulerFactory))]
    [Shared]
    internal partial class WorkspaceTaskSchedulerFactory : IWorkspaceTaskSchedulerFactory
    {
        public virtual IWorkspaceTaskScheduler CreateBackgroundTaskScheduler()
        {
            return new WorkspaceTaskScheduler(this, TaskScheduler.Default);
        }

        public virtual IWorkspaceTaskScheduler CreateEventingTaskQueue()
        {
            var taskScheduler = (SynchronizationContext.Current != null)
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : TaskScheduler.Default;

            return new WorkspaceTaskQueue(this, taskScheduler);
        }

        protected virtual object BeginAsyncOperation(string taskName)
        {
            // do nothing ... overridden by services layer
            return null;
        }

        protected virtual void CompleteAsyncOperation(object asyncToken, Task task)
        {
            // do nothing ... overridden by services layer
        }
    }
}
