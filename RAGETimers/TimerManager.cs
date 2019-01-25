using GTANetworkAPI;
using RAGECore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RAGETimers
{
    public class TimerManager : Script, RAGEModule
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; } = "Timer Module";

        public void Load()
        {
            Console.WriteLine("Hello from timer module.... epic victory royale...");
        }

        public void Unload()
        {

        }

        public void Start()
        {

        }

        public class TimedEvent
        {
            public long Id { get; set; }
            public Action TargetAction { get; set; }
            public long TargetTime { get; set; }
            public long Length { get; set; }
            public bool Async { get; set; }
            public bool Loop { get; set; }

            public TimedEvent(long target, long len, Action act, long id, bool loop, bool async)
            {
                TargetTime = target;
                TargetAction = act;
                Length = len;
                Id = id;
                Async = async;
                Loop = loop;
            }
        }

        private static Dictionary<long, TimedEvent> TimedEvents = new Dictionary<long, TimedEvent>();
        private static long TimeEventCount = 0;

        public long ScheduleSyncEvent(long delay, Action act, bool loop = false)
        {
            TimedEvents.Add(TimeEventCount, new TimedEvent(Environment.TickCount + delay, delay, act, TimeEventCount, loop, false));
            return TimeEventCount++;
        }

        public long ScheduleAsyncEvent(long delay, Action act, bool loop = false)
        {
            TimedEvents.Add(TimeEventCount, new TimedEvent(Environment.TickCount + delay, delay, act, TimeEventCount, loop, true));
            return TimeEventCount++;
        }

        public bool CancelQueuedEvent(long id)
        {
            if (TimedEvents.ContainsKey(id))
            {
                TimedEvents.Remove(id);
                return true;
            }
            return false;
        }

        public bool RestartQueuedEvent(long id)
        {
            if (TimedEvents.ContainsKey(id))
            {
                TimedEvents[id].TargetTime = Environment.TickCount + TimedEvents[id].Length;
                return true;
            }
            return false;
        }

        [ServerEvent(Event.Update)]
        public void OnUpdate()
        {
            List<TimedEvent> evnts = TimedEvents.Values.ToList();
            foreach (var action in evnts)
            {
                if (Environment.TickCount > action.TargetTime)
                {
                    if (!action.Loop)
                    {
                        TimedEvents.Remove(action.Id);
                    }
                    else
                        action.TargetTime = Environment.TickCount + action.Length;

                    //if (action.Async)
                        //new Thread(new ThreadStart(action.TargetAction)).Start();
                    //else
                        Core.InvokeElevatedAction(action.TargetAction);
                }
            }
        }
    }
}
