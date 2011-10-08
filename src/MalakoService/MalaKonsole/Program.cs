using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MalaKonsole
{
    class Program
    {
        // [DllImport("malakosnd.dll", EntryPoint = "?PlaySoundA@@YAXXZ = @ILT+150(?PlaySoundA@@YAXXZ)")]
        // [DllImport("malakosnd.dll", EntryPoint = "MalakoSound")]
        [DllImport("malakoexport.dll", EntryPoint = "?MalakoSound@@YAXXZ")]
        //[DllImport("malakosnd.dll")]
        static extern void MalakoSound();

        static void Main(string[] args)
        {
            Console.WriteLine("Tocando o som...");
            MalakoSound();
        }
    }
}
