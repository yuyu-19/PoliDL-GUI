#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
#WinActivateForce
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.
SetControlDelay -1
SetTitleMatchMode,2

Gui +LastFound
hWnd := WinExist()
DllCall( "RegisterShellHookWindow", UInt,hWnd )
MsgNum := DllCall( "RegisterWindowMessage", Str,"SHELLHOOK" )
OnMessage( MsgNum, "ShellMessage" )


if  (A_Args.Length() < 2)
{
	Loop,Read,OBS\config\obs-studio\basic\profiles\%1%\WebExLinks.txt
	{
		if (%A_LoopReadLine% = "")
		{
			MsgBox, ERROR: Could not find a matching virtual classroom!
			ExitApp												;We got to the end of the file and found nothing.
		}
		
		DayData := StrSplit(A_LoopReadLine ,",")
		if (DayData[1] != A_WDay )				;Weekday must match
			continue
		TimeData := StrSplit(DayData[2],":")	;Hour must be the same
		if (TimeData[1] != A_Hour)
			continue
		if abs(TimeData[2] - A_Min) > 3		;+-3 minutes from the start time, just to have some leeway
			continue
		
		if (DayData[5] = 0)			;The recording matches but is disabled. Re-enable it and exit the app.
		{
			FileRead, alltext, OBS\config\obs-studio\basic\profiles\%1%\WebExLinks.txt
			StrReplace(alltext,A_LoopReadLine,StrReplace(A_LoopReadLine,DayData[4] & ",0", DayData[4] & ",1"))
			FileDelete, OBS\config\obs-studio\basic\profiles\%1%\WebExLinks.txt
			FileAppend, alltext, OBS\config\obs-studio\basic\profiles\%1%\WebExLinks.txt
			ExitApp
		}
		
		WebExLink := DayData[4]	
		break
	}
}
else
	WebExLink := %2%



FileRead,OBSconfig, OBS\config\obs-studio\global.ini

If InStr(OBSconfig,"Language=it-IT",CaseSensitive := false)
{
	MsgBox,324,Inizio registrazione,Vuoi interrompere l'avvio della registrazione? (Hai 25 secondi prima che parta in automatico), 25
	IfMSgBox Yes
		ExitApp
	else
		{
			if !FileExist("ignorewarning2.txt")
			{
				MsgBox,4,Avviso inizio registrazione,Per favore non toccare nulla finchè il programma non è entrato nella stanza.
				IfMsgBox, No
					FileAppend, ignorewarning, ignorewarning2.txt
				}
		}
}
else
{
	MsgBox,324,Start Recording,Would you like to prevent the recording from starting? (You have 25 seconds before it will start on its own), 25
	IfMSgBox Yes
		ExitApp
	else
		{
			if !FileExist("ignorewarning2.txt")
			{
				MsgBox,4,Recording start warning,Please do not touch anything until the program has joined the room.
				IfMsgBox, No
					FileAppend, ignorewarning, ignorewarning2.txt
				}
		}
}

FoundIDs := []

WinGet, WindowList, List, Cisco Webex Meetings ahk_exe atmgr.exe		;Let's avoid killing existing windows
if (WindowList != 0)
{
	Loop, %WindowList%
	{
		If (HasVal(FoundIDs,WindowList%A_Index%) = 0)
			FoundIDs.Push(WindowList%A_Index%)
	}
}


;Since I rely on image identification to find the buttons, FLUX FUCKS IT UP ROYALLY since it tints the screen. So I have to kill it before doing any image recognition.
Process, Exist, flux.exe
FluxExists := ErrorLevel
If (FluxExists != 0)
	Runwait, taskkill /im flux.exe /f



;This allows the user to easily change the OBS settings - simply change them in the template profile and the settings will be copied to all of them.

FileRead,CurrentTemplate,OBS\config\obs-studio\currenttemplate.ini
FileRead,NewTemplate,OBS\config\obs-studio\basic\profiles\template\basic.ini

if (NewTemplate != CurrentTemplate)
{
	;MsgBox, They're different!
	FileRead, OBSconfig, OBS\config\obs-studio\basic\profiles\%1%\basic.ini
	FoundPos := InStr(OBSconfig,"RecFilePath=",CaseSensitive := False)
	;MsgBox, Before edit %FoundPos%
	FoundPos := FoundPos + StrLen("RecFilePath=")
	;MsgBox, After edit %FoundPos%
	
	
	PathLength := InStr(OBSconfig,%1%,CaseSensitive := False, 0)
	;MsgBox, %PathLength% - StrLen(%1%)
	
	PathLength := PathLength + StrLen(%1%)- FoundPos
	
	;MsgBox, Length: %PathLength%
	
	NewPath := SubStr(OBSconfig,FoundPos,PathLength)
	;MsgBox, Path identified: %NewPath%
	NewTemplate := StrReplace(NewTemplate,"[PATH]",NewPath)
	Bodge := A_Args[2]
	NewTemplate := StrReplace(NewTemplate,"template",Bodge)
	NewTemplate := StrReplace(NewTemplate,"[SERVERPORT]",Bodge mod 65534)
	
	;MsgBox, %NewTemplate%
	
	FileDelete,  OBS\config\obs-studio\basic\profiles\%1%\basic.ini
	FileAppend, %NewTemplate%, OBS\config\obs-studio\basic\profiles\%1%\basic.ini
	
	FileDelete, OBS\config\obs-studio\currenttemplate.ini
	FileCopy,OBS\config\obs-studio\basic\profiles\template\basic.ini,OBS\config\obs-studio\currenttemplate.ini	;Update the template
}

MouseMove,0,0	;This is to avoid causing any issues with buttons getting highlighted

Run, WebExLink,,Max

;Let's find a window that wasn't there before
While true
{
	WinGet, WindowList, List, Cisco Webex Meetings ahk_exe atmgr.exe
	if (WindowList != 0)
	{
		Loop, %WindowList%
		{
			If (HasVal(FoundIDs,WindowList%A_Index%) = 0)
			{
				CurrentID := WindowList%A_Index%	;We found the window that was previously not there
				break 2
			}
		}
	}
	Sleep, 1000
}
;Alright moving on
	
Sleep, 10000		;This is to make SURE it loads fully before trying anything

;Focus and maximize the window
WinActivate,ahk_id %CurrentID%				
WinMaximize, ahk_id %CurrentID%

WinGetActiveStats, useless,maxX,maxY,zeroX,zeroY
result1 := 1
result2 := 1

While (result1 != 0 && result2 != 0)		;Basically, wait until either the mute or unmute buttons show up, which means the UI has loaded properly.
{
	Sleep, 800
	ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, warning.png
	if (ErrorLevel = 0)
		{
			MsgBox, Warning found
			ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, blue.png		;The warning sign means that the previous call was interrupted in a weird way
			
			if (ErrorLevel != 0)
				ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, blue2.png	;This means the first one is not present, let's try the "selected" version.
			if (ErrorLevel = 0)
			{
				FoundX := FoundX+15
				FoundY := FoundY+10
				ControlClick,x%FoundX% y%FoundY%,ahk_id %CurrentID%				;We're going to assume the user will want to join again, so we click join again
				Sleep, 6000
			}
		}
	Sleep, 100
	ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, unmutebutton.png
	result1 := ErrorLevel
	Sleep, 100
	ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, mutebutton.png
	result2 := ErrorLevel
	;MsgBox Icons not found %result1% %result2%
}

Sleep, 100
ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, mutebutton.png
if (ErrorLevel = 0)
{
	;MsgBox The icon was found at %FoundX%x%FoundY%.
	ControlClick,x%FoundX% y%FoundY%,ahk_id %CurrentID%
	Sleep, 3000
}
else
	;MsgBox The icon was not found

Sleep, 100
ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, stopvideobutton.png
if (ErrorLevel = 0)
{
	;MsgBox The icon was found at %FoundX%x%FoundY%.
	ControlClick,x%FoundX% y%FoundY%,ahk_id %CurrentID%
	Sleep, 3000
}
else
	;MsgBox The icon was not found

Sleep, 100
ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, green.png

While ErrorLevel != 0
{
	ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, green.png
	
	if (ErrorLevel != 0)
		ImageSearch, FoundX,FoundY,zeroX,zeroY,maxX,maxy, green2.png
	
	Sleep, 100
	;MsgBox, Start meeting icon not found wtf
}
FoundX := FoundX+15
FoundY := FoundY+10

;FileAppend, x%FoundX% y%FoundY%,wtf.txt
Sleep, 5000
;Click, %FoundX%,%FoundY%
ControlClick,x%FoundX% y%FoundY%,ahk_id %CurrentID%

WinActivate, ahk_id CurrentID
WinMaximize, ahk_id CurrentID

;Ready. Start OBS.
SetWorkingDir, OBS\bin\64bit
Run, OBS\bin\64bit\obs64.exe --profile %1% --minimize-to-tray --startrecording


Sleep, 5000
;Get the list of running obs64 processes. The latest one should be the one we just started.

If FluxExists != 0	;Restore f.lux if the user was running it, because we're nice.
{
	Run, %USERPROFILE%\AppData\Local\FluxSoftware\Flux\flux.exe
	WinClose, ahk_exe flux.exe
}

;NEED TO WAIT HERE UNTIL THE WINDOW HAS CLOSED.
While true
{
	Sleep, 300000
	if !WinExist(ahk_id %CurrentID%)
		break
}

Run, OBSCommand\OBSCommand.exe /server=127.0.0.1:%1% /stoprecording


ExitApp

ShellMessage( wParam,lParam ) {			;This is just a sub to change the text of the msgbox to say "Yes" and "Never show this again"
  If ( wParam = 1 ) ;  HSHELL_WINDOWCREATED := 1
     {
       WinGetTitle, Title, ahk_id %lParam%
       If  ( Title = "Avviso inizio registrazione" ) 
          {
            ControlSetText, Button1, &Okay   , ahk_id %lParam%
            ControlSetText, Button2, &Non mostrare più, ahk_id %lParam%
          }
		  If  ( Title = "Recording start warning" ) 
          {
            ControlSetText, Button1, &Okay   , ahk_id %lParam%
            ControlSetText, Button2, &Never warn me, ahk_id %lParam%
          }
     }
}

WTSEnumProcesses( Mode := 1 ) { ;        By SKAN,  http://goo.gl/6Zwnwu,  CD:24/Aug/2014 | MD:25/Aug/2014 
  Local tPtr := 0, pPtr := 0, nTTL := 0, LIST := ""

  If not DllCall( "Wtsapi32\WTSEnumerateProcesses", "Ptr",0, "Int",0, "Int",1, "PtrP",pPtr, "PtrP",nTTL )
    Return "", DllCall( "SetLastError", "Int",-1 )        
         
  tPtr := pPtr
  Loop % ( nTTL ) 
    LIST .= ( Mode < 2 ? NumGet( tPtr + 4, "UInt" ) : "" )           ; PID
         .  ( Mode = 1 ? A_Tab : "" )
         .  ( Mode > 0 ? StrGet( NumGet( tPtr + 8 ) ) "`n" : "," )   ; Process name  
  , tPtr += ( A_PtrSize = 4 ? 16 : 24 )                              ; sizeof( WTS_PROCESS_INFO )  
  
  StringTrimRight, LIST, LIST, 1
  DllCall( "Wtsapi32\WTSFreeMemory", "Ptr",pPtr )      

Return LIST, DllCall( "SetLastError", "UInt",nTTL ) 
}