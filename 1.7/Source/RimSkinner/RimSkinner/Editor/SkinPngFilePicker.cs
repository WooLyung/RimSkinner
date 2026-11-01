using System.Runtime.InteropServices;

namespace RimSkinner.Editor
{
    public static class SkinPngFilePicker
    {
        public static Task<string?> OpenAsync()
        {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("The PNG file picker currently supports Windows only.");
            var completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

            IntPtr owner = GetActiveWindow();
            Thread thread = new(() =>
            {
                IntPtr buffer = IntPtr.Zero;
                try
                {
                    buffer = Marshal.AllocHGlobal(32768 * sizeof(char));
                    Marshal.WriteInt16(buffer, 0);
                    var dialog = new OpenFileName
                    {
                        Size = Marshal.SizeOf<OpenFileName>(),
                        Owner = owner,
                        Filter = "PNG images (*.png)\0*.png\0\0",
                        FilterIndex = 1,
                        File = buffer,
                        MaxFile = 32768,
                        Title = "Select PNG",
                        DefaultExtension = "png",
                        Flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008 | 0x00000004
                    };
                    string? selected = null;
                    if (GetOpenFileName(ref dialog)) 
                        selected = Marshal.PtrToStringUni(buffer);
                    else
                    {
                        int code = CommDlgExtendedError();
                        if (code != 0) throw new InvalidOperationException($"PNG file dialog failed (0x{code:X}).");
                    }
                    completion.TrySetResult(selected);
                }
                catch (Exception ex) 
                {
                    completion.TrySetException(ex); 
                }
                finally 
                { 
                    if (buffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(buffer); 
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return completion.Task;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OpenFileName
        {
            public int Size;
            public IntPtr Owner;
            public IntPtr Instance;
            public string? Filter;
            public IntPtr CustomFilter;
            public int MaxCustomFilter;
            public int FilterIndex;
            public IntPtr File;
            public int MaxFile;
            public IntPtr FileTitle;
            public int MaxFileTitle;
            public string? InitialDirectory;
            public string? Title;
            public int Flags;
            public short FileOffset;
            public short FileExtension;
            public string? DefaultExtension;
            public IntPtr CustomData;
            public IntPtr Hook;
            public string? TemplateName;
            public IntPtr Reserved;
            public int ReservedSize;
            public int FlagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOpenFileNameW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetOpenFileName(ref OpenFileName dialog);
        [DllImport("comdlg32.dll")]
        private static extern int CommDlgExtendedError();
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
    }
}