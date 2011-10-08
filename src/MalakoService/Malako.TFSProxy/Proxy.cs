using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malako.TFSProxy
{
    public class Proxy : IDisposable
    {
        public bool Inicializado { get; private set; }

        public void Inicializar()
        {
            this.Inicializado = true;
        }

        public Proxy()
        {
            this.Inicializado = false;
        }

        public List<Task> GetTasks()
        {
            Task t = new Task() { Id = 1, DataDigitacao = DateTime.Now};

            List<Task> list = new List<Task>();
            list.Add(t);

            t = new Task() { Id = 2, DataDigitacao = DateTime.Now.AddMinutes(-5) };
            list.Add(t);

            t = new Task() { Id = 3, DataDigitacao = DateTime.Now.AddMinutes(-5.1) };
            list.Add(t);

            return list;
        }

        public void Desligar()
        {
            this.Inicializado = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            Desligar();
        }

        #endregion
    }
}
