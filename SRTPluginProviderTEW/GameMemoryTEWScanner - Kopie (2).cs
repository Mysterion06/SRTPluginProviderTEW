using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderTEW.Structs;
using System.Text;
using SRTPluginProviderTEW.Structs.GameStructs;
using System.Collections.Generic;

namespace SRTPluginProviderTEW
{
    internal class GameMemoryTEWScanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 12;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryTEW gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int pointerAddressIGT;
        private int pointerAddressEnemyHP;
        private int pointerAddressHP;
        private int pointerAddressMoney;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerHP { get; set; }
        private MultilevelPointer PointerGameStats { get; set; }
        private MultilevelPointer PointerMoney { get; set; }
        private MultilevelPointer[] PointerEnemyHP { get; set; }

        internal GameMemoryTEWScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryTEW();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));

            //if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
            //    return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                PointerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressHP), 0x68, 0x28);
                PointerGameStats = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressIGT), 0x82040, 0x0);
                PointerMoney = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressMoney), 0x370, 0x650, 0x58);

                gameMemoryValues._enemyHealth = new EnemyHP[MAX_ENTITIES];
                for (int i = 0; i < MAX_ENTITIES; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();

                GenerateEnemyEntries();
            }
        }

        private void SelectPointerAddresses(GameVersion gv)
        {
            if (gv == GameVersion.EvilWithinWW_20220105_1)
            {
                gameMemoryValues._gameInfo = "Latest Release";
                pointerAddressIGT = 0x9ACE548;
                pointerAddressMoney = 0x9ACD1C0;
                pointerAddressHP = 0x09AB3AF0;
                pointerAddressEnemyHP = 0x01D56DE0;
            }
        }

        private unsafe void GenerateEnemyEntries()
        {
            if(PointerEnemyHP == null)
            {
                PointerEnemyHP = new MultilevelPointer[MAX_ENTITIES];
                for (int i = 0; i < MAX_ENTITIES; ++i)
                    PointerEnemyHP[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemyHP), 0x0 + (i * 0x18));
            }
            
        }

        internal void UpdatePointers()
        {
            PointerHP.UpdatePointers();
            PointerGameStats.UpdatePointers();
            PointerMoney.UpdatePointers();

            GenerateEnemyEntries();
            for (int i = 0; i < MAX_ENTITIES; ++i)
                PointerEnemyHP[i].UpdatePointers();
        }

        internal unsafe IGameMemoryTEW Refresh()
        {
            gameMemoryValues._player = PointerHP.Deref<PlayerHP>(0x0);
            gameMemoryValues._stats = PointerGameStats.Deref<GameStats>(0x0);
            gameMemoryValues._greenGel = PointerMoney.DerefInt(0x10);

            GenerateEnemyEntries();
            // Enemy HP
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyHP[i].Address != IntPtr.Zero)
                    {
                        if (i > 0 && PointerEnemyHP[i].DerefFloat(0x8C0) != PointerEnemyHP[i-1].DerefFloat(0x8C0))
                        {
                            if (PointerEnemyHP[i].DerefFloat(0x8C0) != gameMemoryValues._player.MaxHP)
                            {
                                gameMemoryValues.EnemyHealth[i]._currentHP = PointerEnemyHP[i].DerefFloat(0x8C4);
                                gameMemoryValues.EnemyHealth[i]._maximumHP = PointerEnemyHP[i].DerefFloat(0x8C0);
                            }
                        }
                        else if (i == 0)
                        {
                            gameMemoryValues.EnemyHealth[i]._currentHP = PointerEnemyHP[i].DerefFloat(0x8C4);
                            gameMemoryValues.EnemyHealth[i]._maximumHP = PointerEnemyHP[i].DerefFloat(0x8C0);
                        }
                        else
                        {
                            // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                            // This happens when the game removes pointers from the table (map/room change).
                            gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                            gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                        }
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                    }
                }
                catch
                {
                    gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                    gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                }
            }
            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        private unsafe bool SafeReadByteArray(IntPtr address, int size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
            {
                return memoryAccess.TryGetByteArrayAt(address, size, p);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}