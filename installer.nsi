;Installer for Spotlight Image Saver
!include MUI2.nsh

Name "Spotlight Image Saver"
ManifestSupportedOS Win10
Icon "SpotlightImageSaver_GUI\files\1480125721_Spotlight.ico"

CRCCheck on

InstallDir "$PROGRAMFILES\SpotlightImageSaver"
RequestExecutionLevel admin
OutFile "SpotlightImageSaver-Install.exe"
Var StartMenuFolder



!insertmacro MUI_PAGE_LICENSE "LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\SpotlightImageSaver" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH



!insertmacro MUI_LANGUAGE "English"

Section "Install"
SetOutPath $INSTDIR
File "SiS_Backend\bin\Release\spotlightsaver.dll"
File "SpotlightImageSaver\bin\Release\spotimgsave.exe"
File "SpotlightImageSaver_GUI\bin\Release\SpotlightImageSaver.exe"
File "LICENSE"
WriteUninstaller "$INSTDIR\uninstall-sis.exe"
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateShortCut "$SMPROGRAMS\Spotlight Image Saver.lnk" "$INSTDIR\SpotlightImageSaver.exe"
!insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section "Uninstall"
	Delete "$INSTDIR\LICENSE"
	Delete "$INSTDIR\spotlightsaver.dll"
	Delete "$INSTDIR\spotimgsave.exe"
	Delete "$INSTDIR\SpotlightImageSaver.exe"
	Delete "$INSTDIR\uninstall-sis.exe"
	!insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
	Delete "$SMPROGRAMS\Spotlight Image Saver.lnk"
	RMDir "$INSTDIR"
SectionEnd