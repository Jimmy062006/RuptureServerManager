# Rupture Server Manager

A Windows Forms–based dedicated server manager for **Star Rupture**, designed to simplify server hosting, configuration, updates, and monitoring using **SteamCMD**.

This tool provides a clean UI for starting, stopping, updating, and automatically maintaining a dedicated server with minimal downtime.

Built versions are here https://github.com/Jimmy062006/RuptureServerManager/releases

---

## ✨ Features

- ✅ Start / Stop Star Rupture dedicated server
- ⚙️ Editable server settings via UI (saved to JSON)
- 🔄 **Automatic update detection** using Steam build IDs
- ⏱ Configurable auto-update timer
- 🧠 Only updates **when a new build is available**
- 🔁 Graceful server restart after updates
- 🔒 UI locking while operations are running
- 📝 Live console output + log file
- 📦 Automatic SteamCMD download & bootstrap

---

## 🧩 How Auto-Update Works

The auto-update system is **non-destructive** and avoids unnecessary restarts:

1. Reads the **local build ID** from:
   ```
   serverfiles/steamapps/appmanifest_<AppID>.acf
   ```
2. Queries the **remote build ID** using SteamCMD:
   ```
   app_info_print <AppID>
   ```
3. Compares local vs remote
4. Only stops & updates the server **if a newer build exists**
5. Restarts the server automatically if it was running

---

## 📁 Folder Structure

```
RuptureServerManager/
│
├─ config/
│  └─ RuptureServerManagerSettings.txt
│
├─ serverfiles/
│  ├─ DSSettings.txt
│  └─ steamapps/
│     └─ appmanifest_<AppID>.acf
│
├─ steamcmd/
│  └─ steamcmd.exe
│
├─ logs/
│  └─ server.txt
│
└─ RuptureServerManager.exe
```

---

## ⚙️ Configuration Files

Two configuration files are maintained intentionally:

### `config/RuptureServerManagerSettings.txt`
- Full UI configuration
- Includes **port** and UI-only options

### `serverfiles/RuptureServerManagerSettings.txt`
- Server-consumable settings only
- **Port excluded** (managed by launcher)

---

## 🚀 Getting Started

### Requirements
- Windows 10 / 11
- .NET 6+ runtime
- Internet access (for SteamCMD)

### Setup
1. Download or build the application
2. Launch `RuptureServerManager.exe`
3. SteamCMD will auto-download if missing
4. Configure server settings
5. Click **Start**

---

## 🔄 Manual Update

Click **Update** in the UI to:
- Stop the server
- Run `steamcmd +app_update`
- Restart automatically

---

## 🧠 Thread Safety & UI Responsiveness

- All long-running tasks are async
- UI is marshaled safely using `BeginInvoke`
- Buttons are disabled during operations
- No blocking calls on the UI thread

---

## 🛠 Development Notes

- Built using **WinForms (.NET)**
- Uses `System.Threading.Timer` for background checks
- No fragile `SynchronizationContext` usage
- SteamCMD output is streamed live to UI

---

## 🧪 Known Limitations

- SteamCMD output parsing relies on current format
- Requires anonymous Steam login
- Windows only (WinForms)

---

## 📌 Roadmap

- 🔔 Discord webhook notifications
- 👥 Player count detection before update
- 🕒 Maintenance windows
- ⚠️ Crash auto-restart
- 🌐 Headless / service mode

---

## 📜 License

Provided as-is for community use.  
Star Rupture is a trademark of its respective owners.  
SteamCMD is © Valve Corporation.

---

## ❤️ Contributing

Issues, pull requests, and suggestions are welcome.
