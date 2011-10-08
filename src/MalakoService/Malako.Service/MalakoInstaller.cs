using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Malako.Service
{
    [RunInstaller(true)]
    public class MalakoInstaller : Installer
    {
        private ServiceInstaller si;
        private ServiceProcessInstaller spi;

        public MalakoInstaller()
        {
            spi = new ServiceProcessInstaller();
            si = new ServiceInstaller();

            spi.Account = ServiceAccount.User;
            si.StartType = ServiceStartMode.Automatic;
            si.ServiceName = "MalakoService";
            
            Installers.Add(si);
            Installers.Add(spi);
        }
    }

}
