Add-Type @"
using System.Runtime.InteropServices;
public class SleepBlock {
    [DllImport("kernel32.dll")]
    public static extern uint SetThreadExecutionState(uint esFlags);
}
"@

$ES_CONTINUOUS = [uint32]2147483648   # 0x80000000
$ES_SYSTEM_REQUIRED = [uint32]2

while ($true) {
    [SleepBlock]::SetThreadExecutionState($ES_CONTINUOUS -bor $ES_SYSTEM_REQUIRED)
    Start-Sleep -Seconds 30
}