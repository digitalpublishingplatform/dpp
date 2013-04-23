using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using Orchard;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Events;
using Orchard.Logging;
using Orchard.Tasks;

namespace DigitalPublishingPlatform
{
    //http://chrisbower.com/2011/08/05/isolating-background-task-transactions/
    [OrchardSuppressDependency("Orchard.Tasks.ISweepGenerator")]    
    public class SweepGenerator : IOrchardShellEvents {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly Timer _timer;   
     
        public SweepGenerator(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;            
            _timer = new Timer();
            //diferents time intervals for debug & realease versions
#if DEBUG
            Interval = TimeSpan.FromMinutes(1); 
#else
            Interval = TimeSpan.FromMinutes(5);
#endif
            _timer.Elapsed += Elapsed;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public TimeSpan Interval {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = value.TotalMilliseconds; }
        }

        public void Activated() {
            lock (_timer) {
                _timer.Start();
            }
        }

        public void Terminating() {
            lock (_timer) {
                _timer.Stop();
            }
        }

        private void Elapsed(object sender, ElapsedEventArgs e) {
            // current implementation disallows re-entrancy
            if (!System.Threading.Monitor.TryEnter(_timer)) {
                return;
            }

            try {
                if (_timer.Enabled) {
                    new System.Threading.Tasks.TaskFactory().StartNew(DoWork);
                }
            }
            catch (Exception ex) {
                Logger.Warning(ex, "Problem in background tasks");
            }
            finally {
                System.Threading.Monitor.Exit(_timer);
            }
        }

        private object _locker = new object();

        public void DoWork() {

            lock (_locker)
            {                
                var visitedTasks = new List<Type>();
                using (var scope = _workContextAccessor.CreateWorkContextScope()) {
                    var transactionManager = scope.Resolve<ITransactionManager>();
                    transactionManager.Demand();
                    var tasks = scope.Resolve<IEnumerable<IEventHandler>>().Where(t => t is IBackgroundTask && t is MediaServiceUpdater); //NOTE: if this is an expensive operation then I need to rethink this.                    
                    foreach (var task in tasks) {
                        try {
                            var type = task.GetType();
                            if (visitedTasks.Contains(type)) continue;
                            ((IBackgroundTask) task).Sweep();
                            visitedTasks.Add(type);                    
                            break;
                        }
                        catch (Exception ex) {
                            // any database changes in this using scope are invalidated
                            transactionManager.Cancel();
                            Logger.Warning(ex, "Problem in background tasks");
                            // pass exception along to actual handler
                        }
                    }                    
                }                
            }
        }

        public void Dispose() {
            _timer.Dispose();
        }
    }
}