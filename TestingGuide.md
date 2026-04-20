# How to Test Triggers Externally

I have provided you with a standalone **Trigger Debugger** tool. This allows you to verify that your PC correctly detects triggers (like WiFi, Battery, or User Idle state) without having to run the full application UI.

## 🚀 How to Run the Debugger

1.  Open your **SmartRoutines** solution in Visual Studio.
2.  In the **Solution Explorer**, right-click on the `SmartRoutines.TriggerDebugger` project.
3.  Select **Debug** > **Start New Instance** (or set it as your Startup Project and press F5).
4.  A black console window will open with a menu.

## 📋 What to Test

### 1. Time Trigger
- **Goal**: Verify if the daily schedule fires at a specific time.
- **How**: Enter a time just a few minutes ahead of your current time (e.g., if it's 14:28, enter `14:30`).
- **Result**: You will see it status change to `MATCHED! (Firing)` when your computer clock reaches that time.

### 2. WiFi Trigger
- **Goal**: Verify if the system can see your specific WiFi network.
- **How**: Enter the exact SSID (name) of your WiFi.
- **Result**: The tool will show your current connected SSID and whether it matches your target.

### 3. Battery Trigger
- **Goal**: Verify if it detects your laptop's power level.
- **How**: Enter a threshold (e.g., `50`).
- **Result**: It will print your current Battery percentage and tell you if it meets the condition.

### 4. Idle Trigger
- **Goal**: Verify if it detects when you stop using your computer.
- **How**: Enter a short timeout in minutes (e.g., `1`).
- **Result**: Stop moving your mouse and typing. After 1 minute of inactivity, the status will change to `ShouldFire: True`. Move the mouse to see it reset to `False`.

---

> [!TIP]
> Use this tool whenever you are unsure if a Routine isn't working because of the UI or because of your Windows settings/environment. If it works in the Debugger, the logic is correct!
