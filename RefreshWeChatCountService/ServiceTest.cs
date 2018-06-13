using RefreshWeChatCountService.AppDatas;
using RefreshWeChatCountService.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshWeChatCountService
{
    partial class ServiceTest : ServiceBase
    {
        public ServiceTest()
        {
            InitializeComponent();
        }
        public const int period = 60 * 1000;
        public Timer timer;
        public void StartRefresh()
        {
            timer = new Timer(obj =>
            {
                var cfg = TimerConfigModelContext.GetConfig();

                if (DateTime.Now.Hour == cfg.RefreshMerchantStartHour && DateTime.Now.Minute == cfg.RefreshMerchantStartMinutes)
                {
                    using (AllCountData allCountData = new AllCountData())
                    {
                        allCountData.RefreshMerchants();
                    }
                }

                if (DateTime.Now.Hour == cfg.RefreshWeChatCountStartHour && DateTime.Now.Minute == cfg.RefreshWeChatCountStartMinutes)
                {
                    using (var countData = new WeChatCountData())
                    {
                        countData.RefreshWeChatCountData();
                    }
                }

                //if (DateTime.Now.Hour == cfg.RefreshAllCountStartHour && DateTime.Now.Minute == cfg.RefreshAllCountStartMinutes)
                //{
                //    using (var countData = new WeChatCountData())
                //    {
                //        countData.RefreshCountSumData(cfg);
                //    }
                //}
                //cfg = null;
                //GC.Collect();
            }, null, 0, period);
        }
        protected override void OnStart(string[] args)
        {
            StartRefresh();
        }

        protected override void OnStop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }
    }
}
