using System.Management;
using System;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace BaiTap_G04
{
    public static class AntiVM
    {
        // PHƯƠNG THỨC 1: Artifact-based (Kiểm tra file đặc trưng của VMWare, VBox, Hyper-V)
        private static bool CheckVMFiles()
        {
            string[] vmFiles = {
                @"C:\Windows\System32\drivers\vmmouse.sys",   // VMWare
                @"C:\Windows\System32\drivers\vmhgfs.sys",    // VMWare
                @"C:\Windows\System32\drivers\VBoxMouse.sys", // VirtualBox
                @"C:\Windows\System32\drivers\VBoxGuest.sys", // VirtualBox
                @"C:\Windows\System32\drivers\vmbus.sys",     // Hyper-V
                @"C:\Windows\System32\drivers\vmusbmouse.sys" // Hyper-V
            };

            // Kiểm tra các file, nếu có file nào tồn tại thì nó đang chạy trong VM
            foreach (string file in vmFiles)
            {
                if (File.Exists(file)) return true;
            }
            return false;
        }

        // PHƯƠNG THỨC 1: Artifact-based (Kiểm tra tiến trình đặc trưng)
        private static bool CheckVMProcesses()
        {
            string[] vmProcesses = {
                "vmtoolsd", "VGAuthService", "vmacthlp", // VMWare
                "VBoxService", "VBoxTray",               // VirtualBox
                "vmwp", "vmcompute"                      // Hyper-V
            };

            // Kiểm tra các tiến trình, nếu có tiến trình nào đang chạy thì nó đang chạy trong VM
            foreach (string procName in vmProcesses)
            {
                if (Process.GetProcessesByName(procName).Length > 0) return true;
            }
            return false;
        }

        // PHƯƠNG THỨC 1: Artifact-based (Kiểm tra MAC Address)
        private static bool CheckMACAddress()
        {
            // Các địa chỉ MAC đặc trưng của các VM
            string[] macPrefixes = {
                "001C14", "005056", "000569", "000C29", // VMWare
                "080027",                               // VirtualBox
                "0003FF", "00155D"                      // Hyper-V
            };

            // Lấy tất cả các Network Interface và kiểm tra địa chỉ MAC
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                // Lấy địa chỉ MAC và kiểm tra xem nó có bắt đầu bằng một trong các prefix đặc trưng không
                string mac = nic.GetPhysicalAddress().ToString();
                foreach (string prefix in macPrefixes)
                {
                    if (mac.StartsWith(prefix)) return true;
                }
            }
            return false;
        }

        // PHƯƠNG THỨC 1: Artifact-based (Kiểm tra Registry Keys)
        private static bool CheckVMRegistry()
        {
            // Các key registry đặc trưng của các VM
            string[] allKeys = {
                @"SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters", // Hyper-V
                @"HARDWARE\ACPI\DSDT\VBOX__",                           // VirtualBox
                @"SOFTWARE\VMware, Inc.\VMware Tools"                   // VMWare
            };

            foreach (string keyPath in allKeys)
            {
                try
                {
                    // Mở key registry, nếu tồn tại thì nó đang chạy trong VM
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                    {
                        if (key != null) return true;
                    }
                }
                catch 
                { 
                
                }
            }
            return false;
        }

        // PHƯƠNG THỨC 2: Behavior-based (Kiểm tra thông tin phần cứng bằng WMI)
        private static bool CheckHardwareManufacturer()
        {
            try
            {
                // Sử dụng WMI để lấy thông tin về nhà sản xuất và model của máy tính
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                {
                    foreach (ManagementBaseObject item in searcher.Get())
                    {
                        // Lấy thông tin của manufacturer và model, chuyển về chữ thường để dễ so sánh
                        string manufacturer = item["Manufacturer"]?.ToString().ToLower();
                        string model = item["Model"]?.ToString().ToLower();

                        // Kiểm tra nếu manufacturer chứa các từ khóa đặc trưng của VM: "vmware", "innotek" (VirtualBox), "microsoft corporation" (Hyper-V)
                        if (manufacturer != null && (manufacturer.Contains("vmware") || manufacturer.Contains("innotek") || manufacturer.Contains("microsoft corporation")))
                        {
                            return true;
                        }
                        // Kiểm tra nếu model chứa các từ khóa đặc trưng của VM: "virtualbox", "vmware", "virtual machine"
                        if (model != null && (model.Contains("virtualbox") || model.Contains("vmware") || model.Contains("virtual machine")))
                        {
                            return true;
                        }
                    }
                }
            }
            catch 
            { 
            
            }
            return false;
        }

        // HÀM TỔNG HỢP KIỂM TRA
        public static bool CheckAll()
        {
            // Tạm thời tắt CheckMACAddress để có thể test được 2 trường hợp antiDebug và antiVM tránh false positive 
            //return CheckVMFiles() || CheckVMProcesses() || CheckMACAddress() || CheckVMRegistry() || CheckHardwareManufacturer();
            return CheckVMFiles() || CheckVMProcesses() || CheckVMRegistry() || CheckHardwareManufacturer();
        }
    }
}