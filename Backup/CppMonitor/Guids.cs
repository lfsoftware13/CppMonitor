// Guids.cs
// MUST match guids.h
using System;

namespace NanjingUniversity.CppMonitor
{
    static class GuidList
    {
        public const string guidCppMonitorPkgString = "6c681f74-3b93-4475-98f8-6c483f42fed8";
        public const string guidCppMonitorCmdSetString = "7012a869-a3c6-4f46-b9b5-17e3e2162b00";

        public static readonly Guid guidCppMonitorCmdSet = new Guid(guidCppMonitorCmdSetString);
    };
}