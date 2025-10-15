; Inno Setup Script for SpellingChecker
; Compile with Inno Setup Compiler 6.x or later

[Setup]
AppName=AI Spelling Checker
AppVersion=1.0.0
AppPublisher=SpellingChecker Team
AppPublisherURL=https://github.com/shinepcs/SpellingChecker
AppSupportURL=https://github.com/shinepcs/SpellingChecker/issues
AppUpdatesURL=https://github.com/shinepcs/SpellingChecker/releases
DefaultDirName={autopf}\SpellingChecker
DefaultGroupName=AI Spelling Checker
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=installer
OutputBaseFilename=SpellingCheckerSetup_v1.0.0
SetupIconFile=SpellingChecker\app.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "Start with Windows"; GroupDescription: "Startup:"

[Files]
Source: "SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "CONFIG.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\AI Spelling Checker"; Filename: "{app}\SpellingChecker.exe"
Name: "{group}\Configuration Guide"; Filename: "{app}\CONFIG.md"
Name: "{group}\{cm:UninstallProgram,AI Spelling Checker}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\AI Spelling Checker"; Filename: "{app}\SpellingChecker.exe"; Tasks: desktopicon
Name: "{userstartup}\AI Spelling Checker"; Filename: "{app}\SpellingChecker.exe"; Tasks: startupicon

[Run]
Filename: "{app}\SpellingChecker.exe"; Description: "{cm:LaunchProgram,AI Spelling Checker}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\SpellingChecker"
