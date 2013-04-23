using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blackfish.PublicationFramework.Models;
using Blackfish.PublicationFramework.Services;
using Microsoft.WindowsAzure.MediaServices.Client;
using NHibernate;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Tasks.Scheduling;

namespace Blackfish.PublicationFramework
{
    public class ScheduledTaskHandler : IScheduledTaskHandler
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private const string TaskType = "WindowsAzureMediaServiceTask";
        private readonly IScheduledTaskManager _taskManager;
        public ILogger Logger { get; set; }
        
        public ScheduledTaskHandler(IScheduledTaskManager taskManager, IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;

        //, IRepository<EncodedMediaRecord> encodedMediaRepository, IConfig config, ISessionLocator sessionLocato
            _taskManager = taskManager;
            //_encodedMediaRepository = encodedMediaRepository;
            //_config = config;
            //_sessionLocator = sessionLocator;
            Logger = NullLogger.Instance;
            Logger.Debug(DateTime.UtcNow + "> Starting 'ScheduledTaskHandler'");
            try
            {                
                ScheduleNextTask(DateTime.UtcNow.AddSeconds(30));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
            }
        }

        public void Process(ScheduledTaskContext context)
        {
            
            if (context.Task.TaskType != TaskType) return;
            Logger.Debug(DateTime.UtcNow + "> Starting Process");
            try {
                JobsSync();
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
            }
            finally
            {
                var nextTaskDate = DateTime.UtcNow.AddMinutes(5);
                ScheduleNextTask(nextTaskDate);
            }
        }

        private void ScheduleNextTask(DateTime date)
        {
            if (date <= DateTime.UtcNow) {
                return;
            }
            var tasks = _taskManager.GetTasks(TaskType);
            if (tasks == null || !tasks.Any()) {
                _taskManager.CreateTask(TaskType, date, null);
            }
        }

        private object _syncJobLocker = new object();


        public void JobsSync()
        {
            Task.Factory.StartNew(() => BeginJobsSync());
        }

        private bool BeginJobsSync()
        {
            
            lock (_syncJobLocker) {

                using (var workContextScope = _workContextAccessor.CreateWorkContextScope())
                {
                    
                    var encodedMediaRepository = workContextScope.Resolve<IRepository<EncodedMediaRecord>>();
                    var config = workContextScope.Resolve<IConfig>();
                    var cloudMediaContext = new CloudMediaContext(config.MediaServiceAccount, config.MediaServiceKey);

                    var query = encodedMediaRepository.Table.Where(em =>
                                                                                                    em.Status != JobState.Finished.ToString()
                                                                                                    && em.Status != JobState.Error.ToString()
                                                                                                    && em.Status != JobState.Canceled.ToString()
                                                                                                    && em.JobId != null
                                                                                                    && em.JobId != "").ToList();

                    foreach (var encodedMediaRecord in query)
                    {
                        var job = cloudMediaContext.Jobs.Where(j => j.Id == encodedMediaRecord.JobId).ToList().FirstOrDefault();
                        if (job == null)
                        {
                            continue;
                        }
                        if (job.State.ToString() == encodedMediaRecord.Status)
                        {
                            continue;
                        }
                        encodedMediaRecord.Status = job.State.ToString();
                        var outputAsset = job.OutputMediaAssets.FirstOrDefault();
                        if (outputAsset != null)
                        {
                            encodedMediaRecord.AssetId = outputAsset.Id;
                        }

                        if (job.State == JobState.Error)
                        {
                            encodedMediaRecord.Status = job.State.ToString();
                            encodedMediaRecord.JobErrorMessage = "";
                            foreach (var message in job.Tasks.Where(t => t.ErrorDetails.Any()).SelectMany(t => t.ErrorDetails).Select(ed => ed.Message))
                            {
                                encodedMediaRecord.JobErrorMessage += message + "; ";
                            }
                        }
                        encodedMediaRecord.ModifiedUtc = DateTime.UtcNow;
                        encodedMediaRepository.Update(encodedMediaRecord);
                        encodedMediaRepository.Flush();
                    }                                        
                }
                return true;
            }
        }


    }
}