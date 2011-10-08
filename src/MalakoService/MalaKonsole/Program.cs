using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MalaKonsole
{
    class Program
    {
        // [DllImport("malakosnd.dll", EntryPoint = "?PlaySoundA@@YAXXZ")]
        [DllImport("malakosnd.dll")]
        static extern void PlaySound();

        static void Main(string[] args)
        {
            Console.WriteLine("Tocando o som...");
            PlaySound();
        }
    }
}
