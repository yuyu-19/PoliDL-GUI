# PoliDL-GUI

Download: https://github.com/yuyu-19/PoliDL-GUI/releases

Note: A couple of antiviruses on VirusTotal incorrectly flag this as malware. As far as I can tell, this is due to the program extracting the required DLLs in the folder it's running in. You can check the source code yourself and build it to make sure.

### English version below!

Un grande grazie a @sup3rgiu per PoliWebex e PoliDown (su cui si basa l'intera funzionalità di download) e per avermi aiutato con le modifiche necessarie.

PoliWebex: https://github.com/sup3rgiu/PoliWebex
PoliDown: https://github.com/sup3rgiu/PoliDown

## Italiano

Richiede .NET framework v4.6

Questo programma ha due funzionalità principali:
  - Scaricare molteplici lezioni da Webex e Microsoft Stream (Download)
  - Registrare lezioni in locale automaticamente (Locale)
 
### Download
NB: Di default i download avvengono in modalità non-segmentata. La modalità segmentata è più veloce su computer sufficientemente potenti, ma è meno affidabile. È presente una casella per modificare l'impostazione per il download corrente.

Nella modalità non-segmentata se esiste già il file corrispondente ad un video, verrà riscaricato comunque, ondevitare download parziali, con un suffisso (.1, .2, etc). È quindi consigliato, nel caso siano falliti dei download, di reinserire solo i link il cui download è fallito. 

È possibile salvare i link ai video che non sono scaricati cliccando su "Ulteriori informazioni" di fianco al numero di download falliti, e cliccando su "Salva link in un .txt"

Il programma può estrarre link di webex da un paio di tipi di file:
  - .xlsx
  - .docx
  - .html
  - .zip

Puoi selezionare un singolo file o una cartella, o inserire i link manualmente.

Se selezioni una cartella, assicurati di includere solo i tipi di file supportati in essa e nelle sue sottocartelle.



I zip file possono essere "nested", cioè contenere altri zip file al loro interno.

Questo significa che, ad esempio, se un professore avesse una struttura di cartelle (su beep) del genere:

 - Lezioni Online
 	- Lezione 1
 		- .docx/.xlsx file contenente un link
	- Lezione 2
		- .docx/.xlsx file contenente un link
	- Lezione 3
		- .docx/.xlsx file contenente un link
      

Scaricare (cliccando "scarica cartella") e dare al programma il file zip di "Lezioni online" funzionerebbe.

### Locale
Questa funzionalità è un po' complessa:
  1. Prende in input il calendario delle lezioni (servizi online > orario delle lezioni > tasto destro e scarica come solo HTML) come input.
  2. Ti chiederà quindi di selezionare quali professori faranno quale lezione (può essere modificato successivamente)
  3. Una volta fatto, imposterà delle task usando l'utilità di pianificazione di Windows.
  4. Queste task verranno eseguite quando deve iniziare una lezione. Entreranno nella stanza ed inizieranno a registrare la lezione.
  5. Le lezioni saranno salvate nella cartella che hai selezionato in precedenza, che conterrà una sottocartella per ogni corso.
  
 Potrai modificare quale professore fa quale lezione ed eliminare le lezioni rientrando nella modalità locale dopo il setup iniziale.
 
 Puoi anche:
  - Disabilitare solo la prossima registrazione (Nel caso una lezione venga rimandata o cancellata)
  - Aggiungere una registrazione temporanea, che verrà eseguita solo una volta.
  
 Le task verranno cancellate una volta che è terminato il semestre.


## English

Requires .NET framework v4.6

A huge thank you to @sup3rgiu for PoliWebex and PoliDown (on which the entire download functionality is based on) and for helping me make the necessary changes.

PoliWebex: https://github.com/sup3rgiu/PoliWebex
PoliDown: https://github.com/sup3rgiu/PoliDown

This tool has two main functions:
  - Downloading lessons from Webex and Microsoft Stream in bulk   (Download)
  - Automatically recording lessons locally  (Local)

### Download
Note: By default downloads are in unsegmented mode. Segmented mode is faster on sufficiently powerful computers, but is less reliable. There is a checkbox to change this setting for the current download.

In unsegmented mode if the video file already exists, it's downloaded again and a suffix is added. Should some downloads have failed it's therefore suggested that you only redownload the failed ones.

The links to all failed downloads can be saved by clicking on "More information" next to the number of failed downloads, and clicking "Save links to .txt".

The program can extract webex links from a couple of filetypes:
  - .xlsx
  - .docx
  - .html
  - .zip
  
 You can either select a single file or a folder as input, or simply input the links manually. 
 If you select a folder, take care to only include supported filetypes in it and its subfolders.
 
 The zip files can be nested, as in contain other zip files within them.
 
 This means that, for example, if a professor were to have a file structure (on beep) like this:
- Online lessons
	- Lesson 1
		- .docx/.xlsx file containing a link
	- Lesson 2
		- .docx/.xlsx file containing a link
	- Lesson 3
		- .docx/.xlsx file containing a link

Downloading (by clicking "download folder") and giving the program the zip file of "Online lessons" would work.

### Local
This functionality is a bit complex:
  1. It takes the lesson calendar (online services > timetable > right click and download as HTML only) as input.
  2. It will then prompt you to select which professors will be giving which lesson (can be edited later)
  3. Once that's done, it will set up tasks using Windows' task scheduler.
  4. These tasks will run when a lesson is supposed to start, join the room and begin recording.
  5. The lessons will be saved to the folder you will have selected earlier, which will contain a subfolder for each course.
 
 You can edit the professor that is giving a lesson and delete them by entering local mode after the first time setup.
 You are also able to:
  - Disable the recording for the next time (In case a lesson is rescheduled).
  - Add a "one time" recording, which will only run once.
 
 Tasks will be deleted once the semester is over.
